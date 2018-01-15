using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.Data.RelDAL
{
    public sealed class RecebimentoDAO : BaseDAO<Recebimento, RecebimentoDAO>
    {
        public IList<Recebimento> GetRecebimentosTipo(string dataIni, string dataFim, uint idLoja, uint usuCad)
        {
            string idsContasValidas = UtilsPlanoConta.ContasAVista() + "," + UtilsPlanoConta.ListaEstornosAVista() + "," +
                UtilsPlanoConta.ContasAPrazo() + "," + UtilsPlanoConta.ListaEstornosAPrazo() + "," +
                UtilsPlanoConta.ContasSinalPedido() + "," + UtilsPlanoConta.ListaEstornosSinalPedido() + "," +
                UtilsPlanoConta.ContasChequeDev() + "," + UtilsPlanoConta.ListaEstornosChequeDev();

            string sqlBoleto = SqlReceb("Boleto", ValidaContas(UtilsPlanoConta.ContasRecebimentoBoleto(), idsContasValidas), ValidaContas(UtilsPlanoConta.ContasEstornoBoleto(), idsContasValidas), idLoja, usuCad, dataIni, dataFim, false);
            string sqlCartao = SqlReceb("Cartão", ValidaContas(UtilsPlanoConta.ContasRecebimentoCartao(), idsContasValidas), ValidaContas(UtilsPlanoConta.ContasEstornoCartao(), idsContasValidas), idLoja, usuCad, dataIni, dataFim, false);
            string sqlCheque = SqlReceb("Cheque", ValidaContas(UtilsPlanoConta.ContasRecebimentoCheque(), idsContasValidas), ValidaContas(UtilsPlanoConta.ContasEstornoCheque(), idsContasValidas), idLoja, usuCad, dataIni, dataFim, false);
            string sqlConstrucard = SqlReceb("Construcard", ValidaContas(UtilsPlanoConta.ContasRecebimentoConstrucard(), idsContasValidas), ValidaContas(UtilsPlanoConta.ContasEstornoConstrucard(), idsContasValidas), idLoja, usuCad, dataIni, dataFim, false);
            string sqlDeposito = SqlReceb("Depósito",
                /* Chamado 22426. */
                UtilsPlanoConta.GetPlanoConta(Data.Helper.UtilsPlanoConta.PlanoContas.DepositoNaoIdentificado) + "," +
                ValidaContas(UtilsPlanoConta.ContasRecebimentoDeposito(), idsContasValidas),
                ValidaContas(UtilsPlanoConta.ContasEstornoDeposito(), idsContasValidas), idLoja, usuCad, dataIni, dataFim, false);
            string sqlDinheiro = SqlReceb("Dinheiro", ValidaContas(UtilsPlanoConta.ContasRecebimentoDinheiro(), idsContasValidas), ValidaContas(UtilsPlanoConta.ContasEstornoDinheiro(), idsContasValidas), idLoja, usuCad, dataIni, dataFim,  false);
            string sqlCredito = SqlReceb("Crédito Utilizado *", ValidaContas(UtilsPlanoConta.ContasRecebimentoCredito(), idsContasValidas), ValidaContas(UtilsPlanoConta.ContasEstornoCredito(), idsContasValidas), idLoja, usuCad, dataIni, dataFim, false);
            string sqlPermuta = SqlReceb("Permuta *", ValidaContas(UtilsPlanoConta.ContasRecebimentoPermuta(), idsContasValidas), ValidaContas(UtilsPlanoConta.ContasEstornoPermuta(), idsContasValidas), idLoja, usuCad, dataIni, dataFim, false);
            string sqlRecChequeDev = SqlReceb("Rec. Cheque Dev. *", null, null, idLoja, usuCad, dataIni, dataFim, true);
            string sqlCreditosAvulso = SqlReceb("Crédito Avulso *", Glass.Data.DAL.PlanoContasDAO.Instance.GetIdsPlanoContas(1), "0", idLoja, usuCad, dataIni, dataFim, false);

            string sqlReceb = @"
                select 2 as grupo, descricao, cast(valor as decimal(12,2)) as valor, 'Período: " + dataIni + (dataFim != "" ? " até " + dataFim : "") + @"' as criterio
                from (
                    " + sqlBoleto + @"
                    union all " + sqlCartao + @"
                    union all " + sqlCheque + @"
                    union all " + sqlConstrucard + @"
                    union all " + sqlDeposito + @"
                    union all " + sqlDinheiro + @"
                    union all " + sqlCredito + @"
                    union all " + sqlPermuta + @"
                    union all " + sqlRecChequeDev + @"
                    union all " + sqlCreditosAvulso + @"
                ) as recebimento_tipo";

            List<GDAParameter> lstParam = new List<GDAParameter>();
            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));
            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            List<Recebimento> retorno = objPersistence.LoadData(sqlReceb, lstParam.ToArray());

            decimal valorTotal = 0;
            foreach (Recebimento r in retorno)
                if (!r.Descricao.ToLower().Contains("crédito") && !r.Descricao.ToLower().Contains("permuta") && !r.Descricao.ToLower().Contains("cheque dev"))
                    valorTotal += r.Valor;

            Recebimento total = new Recebimento();
            total.IsTotal = true;
            total.Descricao = "TOTAL";
            total.Valor = valorTotal;
            retorno.Insert(retorno.Count - 3, total);

            return retorno;
        }

        public Recebimento[] GetRecebimentosDetalhados(string dataIni, string dataFim, uint idLoja, uint usuCad)
        {
            string idsContasValidas = UtilsPlanoConta.ContasAVista() + "," + UtilsPlanoConta.ListaEstornosAVista() + "," +
                UtilsPlanoConta.ContasAPrazo() + "," + UtilsPlanoConta.ListaEstornosAPrazo() + "," +
                UtilsPlanoConta.ContasSinalPedido() + "," + UtilsPlanoConta.ListaEstornosSinalPedido() + "," +
                UtilsPlanoConta.ContasChequeDev() + "," + UtilsPlanoConta.ListaEstornosChequeDev();

            string sqlBoleto = SqlRecebDetalhado("Boleto", ValidaContas(UtilsPlanoConta.ContasRecebimentoBoleto(), idsContasValidas), ValidaContas(UtilsPlanoConta.ContasEstornoBoleto(), idsContasValidas), idLoja, usuCad, dataIni, dataFim);
            string sqlCartao = SqlRecebDetalhado("Cartão", ValidaContas(UtilsPlanoConta.ContasRecebimentoCartao(), idsContasValidas), ValidaContas(UtilsPlanoConta.ContasEstornoCartao(), idsContasValidas), idLoja, usuCad, dataIni, dataFim);
            string sqlCheque = SqlRecebDetalhado("Cheque", ValidaContas(UtilsPlanoConta.ContasRecebimentoCheque(), idsContasValidas), ValidaContas(UtilsPlanoConta.ContasEstornoCheque(), idsContasValidas), idLoja, usuCad, dataIni, dataFim);
            string sqlConstrucard = SqlRecebDetalhado("Construcard", ValidaContas(UtilsPlanoConta.ContasRecebimentoConstrucard(), idsContasValidas), ValidaContas(UtilsPlanoConta.ContasEstornoConstrucard(), idsContasValidas), idLoja, usuCad, dataIni, dataFim);
            string sqlDeposito = SqlRecebDetalhado("Depósito", ValidaContas(UtilsPlanoConta.ContasRecebimentoDeposito(), idsContasValidas), ValidaContas(UtilsPlanoConta.ContasEstornoDeposito(), idsContasValidas), idLoja, usuCad, dataIni, dataFim);
            string sqlDinheiro = SqlRecebDetalhado("Dinheiro", ValidaContas(UtilsPlanoConta.ContasRecebimentoDinheiro(), idsContasValidas), ValidaContas(UtilsPlanoConta.ContasEstornoDinheiro(), idsContasValidas), idLoja, usuCad, dataIni, dataFim);
            
            string sqlReceb = @"
                select 2 as grupo, descricao, cast(valor as decimal(12,2)) as valor, descricaoPlanoConta, nomeCliente, dataMovimentacao,
                idAcerto, idAcertoCheque, idDeposito, idCheque, idCompra, idPedido, idLiberarPedido, idPagto, idObra,
                idAntecipFornec, idTrocaDevolucao, idDevolucaoPagto, idSinal, idSinalCompra, idCreditoFornecedor,
                idAntecipContaRec, idDepositoNaoIdentificado " + @"
                from (
                    " + sqlBoleto + @"
                    union all " + sqlCartao + @"
                    union all " + sqlCheque + @"
                    union all " + sqlConstrucard + @"
                    union all " + sqlDeposito + @"
                    union all " + sqlDinheiro + @"
                ) as recebimento_tipo";

            List<GDAParameter> lstParam = new List<GDAParameter>();
            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));
            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            List<Recebimento> retorno = objPersistence.LoadData(sqlReceb, lstParam.ToArray());

            decimal valorRecebido = 0;
            decimal valorEstornado = 0;
            foreach (Recebimento r in retorno)
            {
                if (r.Valor > 0)
                    valorRecebido = valorRecebido + r.Valor;
                else
                    valorEstornado = valorEstornado + r.Valor;
            }
            
            Recebimento total = new Recebimento();
            total.TotalEstornado = valorEstornado;
            total.TotalRecebido = valorRecebido;
            total.IsTotal = true;
            total.Descricao = "TOTAL";
            total.Valor = valorRecebido + valorEstornado;
            retorno.Add(total);

            return retorno.ToArray();
        }

        #region Métodos privados

        private string ValidaContas(string idsContas, string idsContasValidas)
        {
            List<string> contas = new List<string>(idsContas.Trim(',').Split(','));
            List<string> contasValidas = new List<string>(idsContasValidas.Trim(',').Split(','));

            // Busca apenas as contas que estão na lista "contasValidas"
            contas = contas.FindAll(x => contasValidas.Contains(x));

            return String.Join(",", contas.ToArray());
        }

        private string SqlReceb(string descricao, string idsContas, string idsContasEst, uint idLoja, uint usuCad, string dataIni, string dataFim, bool apenasRecChequeDev)
        {
            bool lojaSelecionada = idLoja > 0 ? true : false;
            bool funcSelecionado = usuCad > 0 ? true : false;

            //var valCxGeral = apenasRecChequeDev ? "cg.valormov" : "cg.valormov + cg.juros";
            /* Chamado 22770. */
            var valCxGeral = "cg.valormov";

            string sqlReceb = @"Select '" + descricao + @"' as descricao, Coalesce(Sum(valCxGeral + valCxDiario + valContaBanco - estCxGeral - 
                estCxDiario - estContaBanco), 0) as valor From (

                Select Sum(" + valCxGeral + @") as valCxGeral, 0 as valCxDiario, 0 as valContaBanco, 0 as estCxGeral, 0 as estCxDiario, 0 as estContaBanco 
                From caixa_geral cg Where cg.tipoMov=1 And cg.idConta Not In (" +
                UtilsPlanoConta.PlanosContaDesconsiderarCxGeral + ")";

            sqlReceb += " And cg.idConta " + (apenasRecChequeDev ? "in" : "not in") + " (" + UtilsPlanoConta.ContasChequeDev() + "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeTrocado) + ")";

            if (!String.IsNullOrEmpty(idsContas))
                sqlReceb += " And cg.IdConta In ($contas) ";

            if (!String.IsNullOrEmpty(dataIni))
                sqlReceb += " and cg.DataMov>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sqlReceb += " and cg.DataMov<=?dataFim";

            if (lojaSelecionada)
                sqlReceb += " and cg.idLoja=" + idLoja;

            if (funcSelecionado)
                sqlReceb += " and cg.usuCad=" + usuCad;

            //var valCxDiario = apenasRecChequeDev ? "cd.valor" : "cd.valor + cd.juros";
            /* Chamado 22770. */
            var valCxDiario = "cd.valor";

            sqlReceb += @"
                union all Select 0 as valCxGeral, Sum(" + valCxDiario + @") as valCxDiario, 0 as valContaBanco, 0 as estCxGeral, 0 as estCxDiario, 0 as estContaBanco 
                From caixa_diario cd Where cd.tipoMov=1 And cd.idConta Not In (" +
                UtilsPlanoConta.PlanosContaDesconsiderarCxGeral + ")";

            sqlReceb += " And cd.idConta " + (apenasRecChequeDev ? "in" : "not in") + " (" + UtilsPlanoConta.ContasChequeDev() + ")";

            if (!String.IsNullOrEmpty(idsContas))
                sqlReceb += " And cd.IdConta In ($contas) ";

            if (!String.IsNullOrEmpty(dataIni))
                sqlReceb += " and cd.DataCad>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sqlReceb += " and cd.DataCad<=?dataFim";

            if (lojaSelecionada)
                sqlReceb += " and cd.idLoja=" + idLoja;

            if (funcSelecionado)
                sqlReceb += " and cd.usuCad=" + usuCad;

            sqlReceb += @"
                union all Select 0 as valCxGeral, 0 as valCxDiario, Sum(mb.valorMov) as valContaBanco, 0 as estCxGeral, 0 as estCxDiario, 0 as estContaBanco 
                From mov_banco mb Where mb.tipoMov=1 ";

            sqlReceb += " And mb.idConta " + (apenasRecChequeDev ? "in" : "not in") + " (" + UtilsPlanoConta.ContasChequeDev() + ")";

            if (!String.IsNullOrEmpty(idsContas))
                sqlReceb += " And mb.IdConta In ($contas) ";

            if (!String.IsNullOrEmpty(dataIni))
                sqlReceb += " and mb.DataMov>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sqlReceb += " and mb.DataMov<=?dataFim";

            if (funcSelecionado)
                sqlReceb += " and mb.usuCad=" + usuCad;

            sqlReceb += @"
                union all Select 0 as valCxGeral, 0 as valCxDiario, 0 as valContaBanco, Sum(" + valCxGeral + @") as estCxGeral, 0 as estCxDiario, 0 as estContaBanco 
                From caixa_geral cg Where cg.tipoMov=2 And cg.idConta Not In (" +
                UtilsPlanoConta.PlanosContaDesconsiderarCxGeral + ")";

            sqlReceb += " And cg.idConta " + (apenasRecChequeDev ? "in" : "not in") + " (" + UtilsPlanoConta.ContasChequeDev() + "," + UtilsPlanoConta.ListaEstornosChequeDev() + ")";

            if (!String.IsNullOrEmpty(idsContas))
                sqlReceb += " And cg.IdConta In ($contasEst) ";

            if (!String.IsNullOrEmpty(dataIni))
                sqlReceb += " and cg.DataMov>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sqlReceb += " and cg.DataMov<=?dataFim";

            if (lojaSelecionada)
                sqlReceb += " and cg.idLoja=" + idLoja;

            if (funcSelecionado)
                sqlReceb += " and cg.usuCad=" + usuCad;

            sqlReceb += @"
                union all Select 0 as valCxGeral, 0 as valCxDiario, 0 as valContaBanco, 0 as estCxGeral, Sum(" + valCxDiario + @") as estCxDiario, 0 as estContaBanco 
                From caixa_diario cd Where cd.tipoMov=2 And cd.idConta Not In (" +
                UtilsPlanoConta.PlanosContaDesconsiderarCxGeral + ")";

            sqlReceb += " And cd.idConta " + (apenasRecChequeDev ? "in" : "not in") + " (" + UtilsPlanoConta.ContasChequeDev() + "," + UtilsPlanoConta.ListaEstornosChequeDev() + ")";

            if (!String.IsNullOrEmpty(idsContas))
                sqlReceb += " And cd.IdConta In ($contasEst) ";

            if (!String.IsNullOrEmpty(dataIni))
                sqlReceb += " and cd.DataCad>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sqlReceb += " and cd.DataCad<=?dataFim";

            if (lojaSelecionada)
                sqlReceb += "and cd.idLoja=" + idLoja + " ";

            if (funcSelecionado)
                sqlReceb += "and cd.usuCad=" + usuCad + " ";

            sqlReceb += @"
                union all Select 0 as valCxGeral, 0 as valCxDiario, 0 as valContaBanco, 0 as estCxGeral, 0 as estCxDiario, Sum(mb.valorMov) as estContaBanco 
                From mov_banco mb Where mb.tipoMov=2 ";

            sqlReceb += " And mb.idConta " + (apenasRecChequeDev ? "in" : "not in") + " (" + UtilsPlanoConta.ContasChequeDev() + "," + UtilsPlanoConta.ListaEstornosChequeDev() + ")";

            if (!String.IsNullOrEmpty(idsContas))
                sqlReceb += " And mb.IdConta In ($contasEst) ";

            if (!String.IsNullOrEmpty(dataIni))
                sqlReceb += " and mb.DataMov>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sqlReceb += " and mb.DataMov<=?dataFim";

            if (funcSelecionado)
                sqlReceb += " and mb.usuCad=" + usuCad;

            sqlReceb += @"
                ) as tbl";

            return sqlReceb.Replace("$contasEst", idsContasEst).Replace("$contas", idsContas);
        }

        private string SqlRecebDetalhado(string descricao, string idsContas, string idsContasEst, uint idLoja, uint usuCad, string dataIni, string dataFim)
        {
            bool lojaSelecionada = idLoja > 0 ? true : false;
            bool funcSelecionado = usuCad > 0 ? true : false;

            //var valCxDiario = "cd.valor + cd.juros";
            /* Chamado 22770. */
            var valCxDiario = "cd.valor";
            //var valCxGeral = "if(cg.idAcertoCheque>0, cg.valorMov, cg.valorMov + cg.juros)";
            /* Chamado 22770. */
            var valCxGeral = "cg.valorMov";

            string sqlReceb = @"Select '" + descricao + @"' as descricao, valCxGeral + valCxDiario + valContaBanco - estCxGeral - 
                estCxDiario - estContaBanco as valor, descr as descricaoPlanoConta, nomeCli as nomeCliente, dataMovimentacao, 
                idAcerto, idAcertoCheque, idDeposito, idCheque, idCompra, idPedido, idLiberarPedido, idPagto, idObra,
                idAntecipFornec, idTrocaDevolucao, idDevolucaoPagto, idSinal, idSinalCompra, idCreditoFornecedor,
                idAntecipContaRec, idDepositoNaoIdentificado From (

                Select " + valCxGeral + @" as valCxGeral, 0 as valCxDiario, 0 as valContaBanco, 0 as estCxGeral, 
                0 as estCxDiario, 0 as estContaBanco, pc.descricao as descr, cli.nome as nomeCLi, cg.datamov as dataMovimentacao,
                cg.idAcerto as idAcerto, cg.idAcertoCheque as idAcertoCheque, cg.idDeposito as idDeposito, cg.idCheque as idCheque, 
                cg.idCompra as idCompra, cg.idPedido as idPedido, cg.idLiberarPedido as idLiberarPedido, cg.idPagto as idPagto, 
                cg.idObra as idObra, cg.idAntecipFornec as idAntecipFornec, cg.idTrocaDevolucao as idTrocaDevolucao, 
                cg.idDevolucaoPagto as idDevolucaoPagto, cg.idSinal as idSinal, cg.idSinalCompra as idSinalCompra, 
                cg.IDCREDFORNEC as idCreditoFornecedor, 0 as idAntecipContaRec, 0 as idDepositoNaoIdentificado 
                From caixa_geral cg inner join plano_contas pc on (cg.idconta = pc.idconta)
                inner join cliente cli on (cli.ID_CLI = cg.IDCLIENTE)
                Where cg.tipoMov=1 And cg.IdConta In ($contas) And cg.idConta Not In (" +
                UtilsPlanoConta.PlanosContaDesconsiderarCxGeral + ")";

            sqlReceb += " And cg.idConta not in (" + UtilsPlanoConta.ContasChequeDev() + "," + UtilsPlanoConta.ListaEstornosChequeDev() + ")";

            if (!String.IsNullOrEmpty(dataIni))
                sqlReceb += " and cg.DataMov>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sqlReceb += " and cg.DataMov<=?dataFim";

            if (lojaSelecionada)
                sqlReceb += " and cg.idLoja=" + idLoja;

            if (funcSelecionado)
                sqlReceb += " and cg.usuCad=" + usuCad;

            sqlReceb += @"
                union all Select 0 as valCxGeral, " + valCxDiario + @" as valCxDiario, 0 as valContaBanco, 0 as estCxGeral, 
                0 as estCxDiario, 0 as estContaBanco, pc.descricao as descr, cli.nome as nomeCLi, cd.datacad as dataMovimentacao,
                cd.idAcerto as idAcerto, 0 as idAcertoCheque, 0 as idDeposito, cd.idCheque as idCheque, 
                0 as idCompra, cd.idPedido as idPedido, cd.idLiberarPedido as idLiberarPedido, 0 as idPagto, 
                cd.idObra as idObra, 0 as idAntecipFornec, cd.idTrocaDevolucao as idTrocaDevolucao, 
                0 as idDevolucaoPagto, cd.idSinal as idSinal, 0 as idSinalCompra, 
                0 as idCreditoFornecedor, 0 as idAntecipContaRec, 0 as idDepositoNaoIdentificado                 
                From caixa_diario cd inner join plano_contas pc on (cd.idconta = pc.idconta) 
                inner join cliente cli on (cli.ID_CLI = cd.IDCLIENTE)
                Where cd.tipoMov=1 And cd.IdConta In ($contas) And cd.idConta Not In (" +
                UtilsPlanoConta.PlanosContaDesconsiderarCxGeral + ")";

            sqlReceb += " And cd.idConta not in (" + UtilsPlanoConta.ContasChequeDev() + "," + UtilsPlanoConta.ListaEstornosChequeDev() + ")";

            if (!String.IsNullOrEmpty(dataIni))
                sqlReceb += " and cd.DataCad>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sqlReceb += " and cd.DataCad<=?dataFim";

            if (lojaSelecionada)
                sqlReceb += " and cd.idLoja=" + idLoja;

            if (funcSelecionado)
                sqlReceb += " and cd.usuCad=" + usuCad;

            sqlReceb += @"
                union all Select 0 as valCxGeral, 0 as valCxDiario, mb.valorMov as valContaBanco, 0 as estCxGeral, 
                0 as estCxDiario, 0 as estContaBanco, pc.descricao as descr, cli.nome as nomeCLi, mb.datamov as dataMovimentacao,
                mb.idAcerto as idAcerto, mb.idAcertoCheque as idAcertoCheque, mb.idDeposito as idDeposito, mb.idCheque as idCheque, 
                0 as idCompra, mb.idPedido as idPedido, mb.idLiberarPedido as idLiberarPedido, mb.idPagto as idPagto, 
                mb.idObra as idObra, mb.idAntecipFornec as idAntecipFornec, mb.idTrocaDevolucao as idTrocaDevolucao, 
                mb.idDevolucaoPagto as idDevolucaoPagto, mb.idSinal as idSinal, mb.idSinalCompra as idSinalCompra, 
                mb.idCreditoFornecedor as idCreditoFornecedor, mb.idAntecipContaRec as idAntecipContaRec, 
                mb.idDepositoNaoIdentificado as idDepositoNaoIdentificado 
                From mov_banco mb inner join plano_contas pc on (mb.idconta = pc.idconta) 
                inner join cliente cli on (cli.ID_CLI = mb.IDCLIENTE)
                Where mb.tipoMov=1 And mb.IdConta In ($contas)";
            
            if (!String.IsNullOrEmpty(dataIni))
                sqlReceb += " and mb.DataMov>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sqlReceb += " and mb.DataMov<=?dataFim";

            if (funcSelecionado)
                sqlReceb += " and mb.usuCad=" + usuCad;

            sqlReceb += @"
                union all Select 0 as valCxGeral, 0 as valCxDiario, 0 as valContaBanco, " + valCxGeral + @" as estCxGeral, 
                0 as estCxDiario, 0 as estContaBanco, pc.descricao as descr, cli.nome as nomeCLi, cg.dataMov as dataMovimentacao,
                cg.idAcerto as idAcerto, cg.idAcertoCheque as idAcertoCheque, cg.idDeposito as idDeposito, cg.idCheque as idCheque, 
                cg.idCompra as idCompra, cg.idPedido as idPedido, cg.idLiberarPedido as idLiberarPedido, cg.idPagto as idPagto, 
                cg.idObra as idObra, cg.idAntecipFornec as idAntecipFornec, cg.idTrocaDevolucao as idTrocaDevolucao, 
                cg.idDevolucaoPagto as idDevolucaoPagto, cg.idSinal as idSinal, cg.idSinalCompra as idSinalCompra, 
                cg.IDCREDFORNEC as idCreditoFornecedor, 0 as idAntecipContaRec, 0 as idDepositoNaoIdentificado 
                From caixa_geral cg inner join plano_contas pc on (cg.idconta = pc.idconta) 
                inner join cliente cli on (cli.ID_CLI = cg.IDCLIENTE)
                Where cg.tipoMov=2 And cg.IdConta In ($contasEst) And cg.idConta Not In (" +
                UtilsPlanoConta.PlanosContaDesconsiderarCxGeral + ")";

            sqlReceb += " And cg.idConta not in (" + UtilsPlanoConta.ContasChequeDev() + "," + UtilsPlanoConta.ListaEstornosChequeDev() + ")";

            if (!String.IsNullOrEmpty(dataIni))
                sqlReceb += " and cg.DataMov>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sqlReceb += " and cg.DataMov<=?dataFim";

            if (lojaSelecionada)
                sqlReceb += " and cg.idLoja=" + idLoja;

            if (funcSelecionado)
                sqlReceb += " and cg.usuCad=" + usuCad;

            sqlReceb += @"
                union all Select 0 as valCxGeral, 0 as valCxDiario, 0 as valContaBanco, 0 as estCxGeral, 
                " + valCxDiario + @" as estCxDiario, 0 as estContaBanco, pc.descricao as descr, cli.nome as nomeCLi, cd.datacad as dataMovimentacao,
                cd.idAcerto as idAcerto, 0 as idAcertoCheque, 0 as idDeposito, cd.idCheque as idCheque, 
                0 as idCompra, cd.idPedido as idPedido, cd.idLiberarPedido as idLiberarPedido, 0 as idPagto, 
                cd.idObra as idObra, 0 as idAntecipFornec, cd.idTrocaDevolucao as idTrocaDevolucao, 
                0 as idDevolucaoPagto, cd.idSinal as idSinal, 0 as idSinalCompra, 
                0 as idCreditoFornecedor, 0 as idAntecipContaRec, 0 as idDepositoNaoIdentificado     
                From caixa_diario cd  inner join plano_contas pc on (cd.idconta = pc.idconta) 
                inner join cliente cli on (cli.ID_CLI = cd.IDCLIENTE)
                Where cd.tipoMov=2 And cd.IdConta In ($contasEst) And cd.idConta Not In (" +
                UtilsPlanoConta.PlanosContaDesconsiderarCxGeral + ")";

            sqlReceb += " And cd.idConta not in (" + UtilsPlanoConta.ContasChequeDev() + "," + UtilsPlanoConta.ListaEstornosChequeDev() + ")";

            if (!String.IsNullOrEmpty(dataIni))
                sqlReceb += " and cd.DataCad>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sqlReceb += " and cd.DataCad<=?dataFim";

            if (lojaSelecionada)
                sqlReceb += "and cd.idLoja=" + idLoja + " ";

            if (funcSelecionado)
                sqlReceb += "and cd.usuCad=" + usuCad + " ";

            sqlReceb += @"
                union all Select 0 as valCxGeral, 0 as valCxDiario, 0 as valContaBanco, 0 as estCxGeral, 
                0 as estCxDiario, mb.valorMov as estContaBanco, pc.descricao as descr, cli.nome as nomeCLi, mb.datamov as dataMovimentacao,
                mb.idAcerto as idAcerto, mb.idAcertoCheque as idAcertoCheque, mb.idDeposito as idDeposito, mb.idCheque as idCheque, 
                0 as idCompra, mb.idPedido as idPedido, mb.idLiberarPedido as idLiberarPedido, mb.idPagto as idPagto, 
                mb.idObra as idObra, mb.idAntecipFornec as idAntecipFornec, mb.idTrocaDevolucao as idTrocaDevolucao, 
                mb.idDevolucaoPagto as idDevolucaoPagto, mb.idSinal as idSinal, mb.idSinalCompra as idSinalCompra, 
                mb.idCreditoFornecedor as idCreditoFornecedor, mb.idAntecipContaRec as idAntecipContaRec, 
                mb.idDepositoNaoIdentificado as idDepositoNaoIdentificado 
                From mov_banco mb inner join plano_contas pc on (mb.idconta = pc.idconta) 
                inner join cliente cli on (cli.ID_CLI = mb.IDCLIENTE)
                Where mb.tipoMov=2 And mb.IdConta In ($contasEst)";

            if (!String.IsNullOrEmpty(dataIni))
                sqlReceb += " and mb.DataCad>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sqlReceb += " and mb.DataCad<=?dataFim";

            if (funcSelecionado)
                sqlReceb += " and mb.usuCad=" + usuCad;

            sqlReceb += @"
                ) as tbl";

            return sqlReceb.Replace("$contasEst", idsContasEst).Replace("$contas", idsContas);
        }

        #endregion

        public RecebimentoImagem[] ObterRecebimentoImagem()
        {
            return new RecebimentoImagem[0];
        }
    }
}
