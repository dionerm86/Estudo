using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class CaixaGeralDAO : BaseCadastroDAO<CaixaGeral, CaixaGeralDAO>
    {
        //private CaixaGeralDAO() { }

        #region Busca as movimentações do caixa geral no período passado

        /// <summary>
        /// Busca as movimentações do caixa geral no período informado, sobrecarga criada para receber por parâmetro o LoginUsuario,
        /// no caso em que este relatório é chamado pela thread do relatório
        /// </summary>
        /// <param name="idCaixaGeral"></param>
        /// <param name="idFunc"></param>
        /// <param name="dtIni"></param>
        /// <param name="dtFim"></param>
        /// <param name="valorIni"></param>
        /// <param name="valorFim"></param>
        /// <param name="apenasDinheiro"></param>
        /// <param name="tipoMov"></param>
        /// <param name="tipoConta"></param>
        /// <param name="semEstorno"></param>
        /// <param name="idLoja"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public CaixaGeral[] GetMovimentacoes(GDASession sessao, uint idCaixaGeral, uint idFunc, string dtIni, string dtFim, string valorIni, string valorFim,
            bool apenasDinheiro, bool apenasCheque, int tipoMov, int tipoConta, bool semEstorno, uint idLoja, LoginUsuario login)
        {
            string where = String.Empty;
            string criterioRpt = String.Empty;
            decimal saldo = 0;

            if (idCaixaGeral > 0)
            {
                where += " and idCaixaGeral=" + idCaixaGeral;
                criterioRpt += "Cód. Mov.: " + idCaixaGeral + "    ";
            }

            if (!String.IsNullOrEmpty(dtIni))
            {
                where += " And DataMov>=?dtIni";
                criterioRpt += "Movimentações do dia: " + (dtIni.Length == 10 ? DateTime.Parse(dtIni = dtIni.Substring(0, 10)) : DateTime.Parse(dtIni)) + "    ";
            }

            if (!String.IsNullOrEmpty(dtFim))
            {
                where += " And DataMov<=?dtFim";
                criterioRpt = (dtFim != dtIni ? criterioRpt.Trim() + " a " + (dtFim.Length == 10 ? DateTime.Parse(dtFim = dtFim.Substring(0, 10)) : DateTime.Parse(dtFim)) : criterioRpt) + "    ";
            }

            if (idFunc > 0)
            {
                where += " And c.UsuCad=" + idFunc;
                criterioRpt += "Funcionário: " + FuncionarioDAO.Instance.GetNome(idFunc) + "    ";
            }

            if (idLoja > 0)
            {
                where += " AND c.idLoja=" + idLoja;
                criterioRpt += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "   ";
            }

            bool valoresIguais = !String.IsNullOrEmpty(valorIni) && !String.IsNullOrEmpty(valorFim) ?
                float.Parse(valorIni) == float.Parse(valorFim) : true;

            if (!String.IsNullOrEmpty(valorIni))
            {
                where += " and ValorMov>=" + valorIni.Replace(',', '.');
                criterioRpt += "Valor" + (!valoresIguais ? " inicial" : String.Empty) + ": " + float.Parse(valorIni).ToString("C") + "    ";
            }

            if (!String.IsNullOrEmpty(valorFim))
            {
                where += " and ValorMov<=" + valorFim.Replace(',', '.');
                if (!valoresIguais)
                    criterioRpt += "Valor final: " + float.Parse(valorFim).ToString("C") + "    ";
            }

            if (apenasDinheiro && apenasCheque)
            {
                where += " And (c.idConta in (" +
                        UtilsPlanoConta.GetLstEntradaByFormaPagto(Pagto.FormaPagto.Dinheiro, 0, false) + "," +
                        UtilsPlanoConta.GetLstEstornoSaidaByFormaPagto(Pagto.FormaPagto.Dinheiro, 0) + "," +
                        UtilsPlanoConta.GetLstSaidaByFormaPagto(Pagto.FormaPagto.Dinheiro, 0, 1) +
                        UtilsPlanoConta.GetLstEntradaByFormaPagto(Pagto.FormaPagto.ChequeProprio, 0, false) + "," +
                        UtilsPlanoConta.GetLstEntradaByFormaPagto(Pagto.FormaPagto.ChequeTerceiro, 0, false) + "," +
                        UtilsPlanoConta.GetLstEstornoSaidaByFormaPagto(Pagto.FormaPagto.ChequeProprio, 0) + "," +
                        UtilsPlanoConta.GetLstEstornoSaidaByFormaPagto(Pagto.FormaPagto.ChequeTerceiro, 0) + "," +
                        UtilsPlanoConta.GetLstSaidaByFormaPagto(Pagto.FormaPagto.ChequeProprio, 0, 1) + "," +
                        UtilsPlanoConta.GetLstSaidaByFormaPagto(Pagto.FormaPagto.ChequeTerceiro, 0, 1) + "))";
            }
            else if (apenasDinheiro)
            {
                where += " And (formaSaida=" + (int)CaixaDiario.FormaSaidaEnum.Dinheiro + " Or c.idConta in (" +
                    UtilsPlanoConta.GetLstEntradaByFormaPagto(Pagto.FormaPagto.Dinheiro, 0, false) + "," +
                    UtilsPlanoConta.GetLstEstornoSaidaByFormaPagto(Pagto.FormaPagto.Dinheiro, 0) + "," +
                    UtilsPlanoConta.GetLstSaidaByFormaPagto(Pagto.FormaPagto.Dinheiro, 0, 1) + "))";

                criterioRpt += "Apenas movimentações em dinheiro    ";
            }
            else if (apenasCheque)
            {
                where += " And (formaSaida=" + (int)CaixaDiario.FormaSaidaEnum.Cheque + " Or c.idConta in (" +
                    UtilsPlanoConta.GetLstEntradaByFormaPagto(Pagto.FormaPagto.ChequeTerceiro, 0, false) + "," +
                    UtilsPlanoConta.GetLstEstornoSaidaByFormaPagto(Pagto.FormaPagto.ChequeTerceiro, 0) + "," +
                    UtilsPlanoConta.GetLstSaidaByFormaPagto(Pagto.FormaPagto.ChequeTerceiro, 0, 1) + "))";

                criterioRpt += "Apenas movimentações em cheque    ";
            }

            if (semEstorno)
            {
                string campoSemEstornoBase = @"concat(coalesce({0}idPedido,''), ',', coalesce({0}idAcerto,''), ',',
                    coalesce({0}idSinal,''), ',', coalesce({0}idCompra,''), ',', coalesce({0}idContaPg,''), ',',
                    coalesce({0}idContaR,''), ',', coalesce({0}idCheque,''), ',', coalesce({0}idPagto,''), ',',
                    coalesce({0}idLiberarPedido,''), ',', coalesce({0}idObra,''), ',',
                    coalesce({0}idAcertoCheque,''))";

                string busca = @"select c.idCaixaGeral
                    from caixa_geral c
                        inner join (
                            select dataMov, valorMov, " + String.Format(campoSemEstornoBase, String.Empty) + @" as buscaEstorno
                            from caixa_geral
                            where tipoMov=2 and idConta in (" + UtilsPlanoConta.ListaEstornosAPrazo() + "," +
                                UtilsPlanoConta.ListaEstornosAVista() + "," + UtilsPlanoConta.ListaEstornosChequeDev() + "," +
                                UtilsPlanoConta.ListaEstornosSinalPedido() + "," + FinanceiroConfig.PlanoContaEstornoJurosReceb + @")
                        ) e on (c.valorMov=e.valorMov and " + String.Format(campoSemEstornoBase, "c.") + @"=e.buscaEstorno)
                    where 1";

                if (!String.IsNullOrEmpty(dtIni))
                    busca += " and c.dataMov>=?dtIni";

                if (!String.IsNullOrEmpty(dtFim))
                    busca += " and c.dataMov<=?dtFim";

                var ids = objPersistence.LoadResult(busca, GetParams(dtIni, dtFim)).Select(f => f.GetUInt32(0))
                       .ToList().ToArray();
                string itens = String.Join(",", Array.ConvertAll<uint, string>(ids, new Converter<uint, string>(
                    delegate(uint x)
                    {
                        return x.ToString();
                    }
                )));

                if (!String.IsNullOrEmpty(itens))
                    where += @" and c.idCaixaGeral not in (" + itens + ")";

                where += @" and c.tipoMov=1";
                criterioRpt += "Movimentações de Entrada sem Estorno    ";
            }

            if (tipoMov > 0)
            {
                where += " And c.tipoMov=" + tipoMov;
                criterioRpt += "Apenas movimentações de " + (tipoMov == 1 ? "entrada" : "saída") + "    ";
            }

            if (tipoConta > 0)
            {
                where += (tipoConta == 1 ? " And cp.paga = 1 And cp.contabil = 1" : " And coalesce(cp.contabil, 0) = 0");
                criterioRpt += tipoConta == 1 ? "Tipos de contas: Contábeis    " : "Tipos de contas: Não Contábeis    ";
            }

            // Se o funcionário logado for financeiro, retorna apenas as movimentações feitas pelo mesmo e de estorno
            if (Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) && !Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento) && !FinanceiroConfig.CaixaGeral.CxGeralSaldoTotal)
            {
                where += " And (c.UsuCad In (select idFunc from config_funcao_func where idFuncaoMenu=" + Config.ObterIdFuncaoMenu(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) +
                    ") Or c.idConta In (" + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoDinheiro) +
                    ", " + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoCartao) +
                    ", " + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoCheque) +
                    ", " + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoDeposito) + "))";
            }

            if (!string.IsNullOrEmpty(criterioRpt))
                criterioRpt = "'" + criterioRpt + "' as Criterio, ";

            string sql = @"
                Select c.*," + criterioRpt + @" coalesce(forn.RazaoSocial, forn.NomeFantasia) as nomeFornecedor,
                    f.Nome as DescrUsuCad, p.Descricao as DescrPlanoConta, g.Descricao as DescrGrupoConta, l.NomeFantasia as NomeLoja,
                    cli.Nome as NomeCliente, " + ContasReceberDAO.Instance.SqlCampoDescricaoContaContabil("cr") + @" as descricaoContaReceberContabil
                From caixa_geral c
                    Left Join funcionario f On (c.UsuCad=f.IdFunc)
                    Left Join cliente cli On (cli.Id_Cli=c.IdCliente)
                    Left Join fornecedor forn On (c.idFornec=forn.idFornec)
                    Left Join loja l On (c.IdLoja=l.IdLoja) "
                    + (tipoConta > 0 ? " Left Join contas_pagar cp On (c.IdPagto = cp.IdPagto)" : "") +
                    @"Left Join plano_contas p On (c.IdConta=p.IdConta)
                    Left Join grupo_conta g On (p.IdGrupo=g.IdGrupo)
                    Left Join contas_receber cr on (c.idContaR=cr.idContaR)
                Where 1 " + where + (tipoConta > 0 ? " group by c.idCaixaGeral" : "") +
                " Order By IdCaixaGeral Asc";

            List<CaixaGeral> lstCaixa = objPersistence.LoadData(sessao, sql, GetParams(dtIni, dtFim));

            if (lstCaixa.Count == 0)
                return new CaixaGeral[0];

            uint idContaTransf = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfCaixaGeral);
            uint idContaPagtoDinheiro = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoDinheiro);

            decimal saldoInicialApenasDinheiro = 0;

            //Se utilizar o filtro de apenas dinheiro busca o saldo inicial desde o inicio do sistema
            if (apenasDinheiro)
                saldoInicialApenasDinheiro = GetSaldoByFormaPagto(Pagto.FormaPagto.Dinheiro, 0, "01/01/1990", lstCaixa[0].DataMov.AddDays(-1).ToString("dd/MM/yyyy"), 1, idLoja);

            // Se o funcionário logado for financeiro, ou se houver filtro por funcionário ou apenas dinheiro, calcula o saldo de
            // cada operação realizada por ele e estornos, uma vez que o que está salvo no BD considera todas
            // as movimentações do caixa geral
            if ((Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) && !Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento)) || idFunc > 0 || idLoja > 0 ||
                apenasDinheiro)
            {
                for (int i = 0; i < lstCaixa.Count; i++)
                {
                    if (i == 0)
                    {
                        // O idloja não deve ser passado para buscar a movimentação anterior, pois o que se deseja saber é se a movimentação alterou o saldo ou não
                        CaixaGeral movAnterior = BuscaMovAnterior(lstCaixa[0].IdCaixaGeral, 0);

                        if (movAnterior != null && movAnterior.Saldo == lstCaixa[0].Saldo)
                            lstCaixa[0].Saldo = 0;
                        else
                        {
                            lstCaixa[0].Saldo = lstCaixa[0].ValorMov * (lstCaixa[0].TipoMov == 1 ? 1 : -1);

                            if (apenasDinheiro)
                                lstCaixa[0].Saldo = saldoInicialApenasDinheiro + lstCaixa[0].Saldo;

                            saldo = lstCaixa[0].Saldo;
                        }

                        continue;
                    }

                    // Só altera se o saldo tiver sido alterado
                    if (lstCaixa[i].Saldo != ObtemSaldoMovAnterior(lstCaixa[i].IdCaixaGeral, 0))
                    {
                        if (lstCaixa[i].TipoMov == 1)
                            saldo += lstCaixa[i].ValorMov;
                        else
                            saldo -= lstCaixa[i].ValorMov;
                    }

                    lstCaixa[i].Saldo = saldo;
                }
            }

            // Busca o fornecedor caso seja pagamento
            foreach (CaixaGeral c in lstCaixa)
                if (c.IdPagto != null)
                    c.DescrPlanoConta += " " + c.NomeFornecedor;

            // Inclui mov. SALDO ANTERIOR
            for (int i = 0; i < lstCaixa.Count; i++)
            {
                if (i == 0 || lstCaixa[i].DataMov.ToString("dd/MM/yyyy") != lstCaixa[i - 1].DataMov.ToString("dd/MM/yyyy"))
                {
                    CaixaGeral mov = new CaixaGeral();
                    mov.Obs = "SALDO ANTERIOR";
                    mov.Saldo =
                        apenasDinheiro && i == 0 ? saldoInicialApenasDinheiro :
                        i > 0 ? lstCaixa[i - 1].Saldo :
                        (i == 0 && (ObtemSaldoMovAnterior(lstCaixa[0].IdCaixaGeral, idLoja) == lstCaixa[0].Saldo || lstCaixa[0].Saldo == 0)) ? lstCaixa[0].Saldo :
                        lstCaixa[0].Saldo - (lstCaixa[0].ValorMov * ((decimal)lstCaixa[0].TipoMov == 1 ? 1 : -1));


                    mov.Criterio = lstCaixa[0].Criterio;
                    lstCaixa.Insert(i, mov);
                    i++;
                }
            }

            // Inclui mov. SALDO ATUAL
            if (lstCaixa.Count > 0)
            {
                CaixaGeral mov = new CaixaGeral();
                mov.Obs = "SALDO ATUAL";
                mov.Saldo = lstCaixa[lstCaixa.Count - 1].Saldo;
                lstCaixa.Add(mov);
            }

            return lstCaixa.ToArray();
        }

        public CaixaGeral ObterTotalizadores(GDASession sessao, uint idFunc, string dtIni, string dtFim, bool apenasDinheiro, bool semEstorno, uint idLoja, LoginUsuario login)
        {
            var caixaGeral = new CaixaGeral();

            // Usado apenas se a empresa não possuir caixa diário
            caixaGeral.TotalDinheiro = !FinanceiroConfig.CaixaGeral.CxGeralTotalCumulativo ? 0 : GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Dinheiro, idFunc, null, null, !semEstorno ? 1 : 4, idLoja);
            caixaGeral.SaldoDinheiro = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Dinheiro, idFunc, dtIni, dtFim, !semEstorno ? 1 : 4, idLoja);
            caixaGeral.TotalSaidaDinheiro = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Dinheiro, idFunc, dtIni, dtFim, !semEstorno ? 3 : 5, idLoja);
            caixaGeral.TotalEntradaDinheiro = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Dinheiro, idFunc, dtIni, dtFim, 2, idLoja);

            caixaGeral.TotalCheque = !FinanceiroConfig.CaixaGeral.CxGeralTotalCumulativo ? 0 : ChequesDAO.Instance.GetTotalTerc(idLoja);
            caixaGeral.TotalChequeDevolvido = !FinanceiroConfig.CaixaGeral.CxGeralTotalCumulativo ? 0 : ChequesDAO.Instance.GetTotalTerc(false, true, ChequesDAO.TipoReapresentados.IgnorarReapresentados, idLoja);
            caixaGeral.TotalChequeReapresentado = !FinanceiroConfig.CaixaGeral.CxGeralTotalCumulativo ? 0 : ChequesDAO.Instance.GetTotalTerc(true, true, ChequesDAO.TipoReapresentados.ApenasReapresentados, idLoja);
            caixaGeral.TotalChequeTerc = !FinanceiroConfig.CaixaGeral.CxGeralTotalCumulativo ? 0 : ChequesDAO.Instance.GetTotalTercVenc(idLoja);
            caixaGeral.SaldoCheque = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.ChequeProprio, idFunc, dtIni, dtFim, !semEstorno ? 1 : 4, idLoja);
            caixaGeral.TotalSaidaCheque = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.ChequeProprio, idFunc, dtIni, dtFim, !semEstorno ? 3 : 5, idLoja);
            caixaGeral.TotalEntradaCheque = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.ChequeProprio, idFunc, dtIni, dtFim, 2, idLoja);

            if (!apenasDinheiro)
            {
                caixaGeral.SaldoCartao = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Cartao, idFunc, dtIni, dtFim, !semEstorno ? 1 : 4, idLoja);
                caixaGeral.TotalSaidaCartao = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Cartao, idFunc, dtIni, dtFim, !semEstorno ? 3 : 5, idLoja);
                caixaGeral.TotalEntradaCartao = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Cartao, idFunc, dtIni, dtFim, 2, idLoja);

                caixaGeral.SaldoConstrucard = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Construcard, idFunc, dtIni, dtFim, !semEstorno ? 1 : 4, idLoja);
                caixaGeral.TotalSaidaConstrucard = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Construcard, idFunc, dtIni, dtFim, !semEstorno ? 3 : 5, idLoja);

                caixaGeral.SaldoPermuta = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Permuta, idFunc, dtIni, dtFim, !semEstorno ? 1 : 4, idLoja);
                caixaGeral.TotalSaidaPermuta = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Permuta, idFunc, dtIni, dtFim, !semEstorno ? 3 : 5, idLoja);

                caixaGeral.TotalSaidaDeposito = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Deposito, idFunc, dtIni, dtFim, !semEstorno ? 3 : 5, idLoja);
                caixaGeral.TotalEntradaDeposito = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Deposito, idFunc, dtIni, dtFim, 2, idLoja);
                caixaGeral.SaldoDeposito = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Deposito, idFunc, dtIni, dtFim, !semEstorno ? 1 : 4, idLoja);

                caixaGeral.TotalSaidaBoleto = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Boleto, idFunc, dtIni, dtFim, !semEstorno ? 3 : 5, idLoja);
                caixaGeral.TotalEntradaBoleto = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Boleto, idFunc, dtIni, dtFim, 2, idLoja);
                caixaGeral.SaldoBoleto = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Boleto, idFunc, dtIni, dtFim, !semEstorno ? 1 : 4, idLoja);
            }

            caixaGeral.TotalCreditoRecebido = GetCreditoRecebido(sessao, idFunc, dtIni, dtFim, idLoja);
            caixaGeral.TotalCreditoGerado = GetCreditoGerado(sessao, idFunc, dtIni, dtFim, idLoja);
            caixaGeral.TotalEntradaConstrucard = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Construcard, idFunc, dtIni, dtFim, 2, idLoja);
            caixaGeral.TotalEntradaPermuta = GetSaldoByFormaPagto(sessao, Pagto.FormaPagto.Permuta, idFunc, dtIni, dtFim, 2, idLoja);
            caixaGeral.ContasReceberGeradas = ContasReceberDAO.Instance.GetTotalGeradasPeriodo(idFunc, dtIni, dtFim, idLoja);

            if (FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
            {
                caixaGeral.TotalContasRecebidasContabeis = ContasReceberDAO.Instance
                    .GetTotalRecebidasPeriodo(dtIni, dtFim, ((int)ContasReceber.TipoContaEnum.Contabil).ToString(), idLoja);

                caixaGeral.TotalContasRecebidasNaoContabeis = ContasReceberDAO.Instance
                    .GetTotalRecebidasPeriodo(dtIni, dtFim, ((int)ContasReceber.TipoContaEnum.NaoContabil).ToString(), idLoja);
            }

            // Irá mostrar total de dinheiro, cheque e cheques de terc em aberto se a empresa tiver caixa único
            caixaGeral.MostrarTotalGeral = FinanceiroConfig.CaixaGeral.CxGeralTotalCumulativo
                && Config.PossuiPermissao((int)login.CodUser, Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento)
                && Config.PossuiPermissao((int)login.CodUser, Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento);

            return caixaGeral;
        }

        private GDAParameter[] GetParams(string dtIni, string dtFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dtIni))
                lstParam.Add(new GDAParameter("?dtIni", (dtIni.Length == 10 ? DateTime.Parse(dtIni = dtIni + " 00:00") : DateTime.Parse(dtIni))));

            if (!String.IsNullOrEmpty(dtFim))
                lstParam.Add(new GDAParameter("?dtFim", (dtFim.Length == 10 ? DateTime.Parse(dtFim = dtFim + " 23:59:59") : DateTime.Parse(dtFim))));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Retorna movimentação

        /// <summary>
        /// Retorna uma única movimentação
        /// </summary>
        /// <param name="idCxGeral"></param>
        /// <returns></returns>
        public CaixaGeral GetMovimentacao(uint idCxGeral)
        {
            string sql = @"
                Select c.*, f.Nome as DescrUsuCad, p.Descricao as DescrPlanoConta, l.NomeFantasia as NomeLoja
                From caixa_geral c
                    Left Join funcionario f On c.UsuCad=f.IdFunc
                    Left Join loja l On f.IdLoja=l.IdLoja
                    Left Join plano_contas p On c.IdConta=p.IdConta
                Where c.idCaixaGeral=" + idCxGeral;

            List<CaixaGeral> lstMov = objPersistence.LoadData(sql);

            return lstMov.Count > 0 ? lstMov[0] : null;
        }

        #endregion

        #region Movimenta Caixa

        /// <summary>
        /// Movimentação geral
        /// </summary>
        public uint MovCxGeral(uint? idLoja, uint? idCliente, uint? idFornec, uint idConta, int tipoMov, int formaSaida, decimal valorMov,
            decimal juros, string numAutConstrucard, bool mudarSaldo, string obs, DateTime? dataMovBanco)
        {
            return MovCxGeral(null, idLoja, idCliente, idFornec, idConta, tipoMov, formaSaida, valorMov, juros, numAutConstrucard,
                mudarSaldo, obs, dataMovBanco);
        }

        /// <summary>
        /// Movimentação geral
        /// </summary>
        public uint MovCxGeral(GDASession sessao, uint? idLoja, uint? idCliente, uint? idFornec, uint idConta, int tipoMov, int formaSaida, decimal valorMov, decimal juros, string numAutConstrucard,
            bool mudarSaldo, string obs, DateTime? dataMovBanco, int? idCaixaDiario = null)
        {
            return MovCxGeral(sessao, idLoja, idCliente, idFornec, idConta, tipoMov, formaSaida, valorMov, juros, numAutConstrucard, mudarSaldo, obs, dataMovBanco, false, null, idCaixaDiario);
        }

        /// <summary>
        /// Movimentação geral
        /// </summary>
        public uint MovCxGeral(GDASession sessao, uint? idLoja, uint? idCliente, uint? idFornec, uint idConta, int tipoMov, int formaSaida, decimal valorMov, decimal juros, string numAutConstrucard,
            bool mudarSaldo, string obs, DateTime? dataMovBanco, bool lancManual, int? contadorDataUnica, int? idCaixaDiario = null)
        {
            return MovimentaCaixa(sessao, null, idLoja, idCliente, idFornec, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, idConta, tipoMov, formaSaida,
                valorMov, juros, numAutConstrucard, mudarSaldo, null, null, obs, lancManual, dataMovBanco, contadorDataUnica, idCaixaDiario);
        }

        /// <summary>
        /// Movimentação proveniente de pedido
        /// </summary>
        public uint MovCxPedido(GDASession sessao, uint idPedido, uint? idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros,
            string numAutConstrucard, bool mudarSaldo, DateTime? dataMovBanco, string obs)
        {
            return MovCxPedido(sessao, idPedido, idCliente, idConta, tipoMov, valorMov, juros, numAutConstrucard, mudarSaldo, dataMovBanco, obs, null);
        }

        /// <summary>
        /// Movimentação proveniente de pedido
        /// </summary>
        public uint MovCxPedido(GDASession sessao, uint idPedido, uint? idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros,
            string numAutConstrucard, bool mudarSaldo, DateTime? dataMovBanco, string obs, int? contadorDataUnica)
        {
            return MovimentaCaixa(sessao, null, null, idCliente, null, idPedido, null, null, null, null, null, null, null, null, null, null, null, null, null, null, idConta,
                tipoMov, 0, valorMov, juros, numAutConstrucard, mudarSaldo, null, null, obs, false, dataMovBanco, contadorDataUnica);
        }

        /// <summary>
        /// Movimentação proveniente de sinal
        /// </summary>
        public uint MovCxSinal(GDASession sessao, uint idSinal, uint? idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros,
            string numAutConstrucard, bool mudarSaldo, DateTime? dataMovBanco, string obs)
        {
            return MovCxSinal(sessao, idSinal, idCliente, idConta, tipoMov, valorMov, juros, numAutConstrucard, mudarSaldo, dataMovBanco, obs, null);
        }

        /// <summary>
        /// Movimentação proveniente de sinal
        /// </summary>
        public uint MovCxSinal(GDASession sessao, uint idSinal, uint? idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros,
            string numAutConstrucard, bool mudarSaldo, DateTime? dataMovBanco, string obs, int? contadorDataUnica)
        {
            return MovimentaCaixa(sessao, null, null, idCliente, null, null, idSinal, null, null, null, null, null, null, null, null, null, null, null, null, null, idConta,
                tipoMov, 0, valorMov, juros, numAutConstrucard, mudarSaldo, null, null, obs, false, dataMovBanco, contadorDataUnica);
        }

        /// <summary>
        /// Movimentação proveniente de pedido
        /// </summary>
        public uint MovCxPedido(GDASession session, uint idPedido, uint? idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros, string numAutConstrucard, string obs, bool mudarSaldo, DateTime? dataMovBanco)
        {
            return MovimentaCaixa(session, null, null, idCliente, null, idPedido, null, null, null, null, null, null, null, null, null, null, null, null, null, null, idConta,
                tipoMov, 0, valorMov, juros, numAutConstrucard, mudarSaldo, null, null, obs, false, dataMovBanco);
        }

        /// <summary>
        /// Movimentação proveniente de acerto
        /// </summary>
        public uint MovCxAcerto(GDASession sessao, uint idAcerto, uint? idCliente, uint idConta, int tipoMov, decimal valorMov,
            decimal juros, string numAutConstrucard, int formaSaida, bool mudarSaldo, DateTime? dataMovBanco, string obs)
        {
            return MovCxAcerto(sessao, idAcerto, idCliente, idConta, tipoMov, valorMov, juros, numAutConstrucard, formaSaida, mudarSaldo, dataMovBanco, obs, null);
        }

        /// <summary>
        /// Movimentação proveniente de acerto
        /// </summary>
        public uint MovCxAcerto(GDASession sessao, uint idAcerto, uint? idCliente, uint idConta, int tipoMov, decimal valorMov,
            decimal juros, string numAutConstrucard, int formaSaida, bool mudarSaldo, DateTime? dataMovBanco, string obs, int? contadorDataUnica)
        {
            return MovimentaCaixa(sessao, null, null, idCliente, null, null, null, null, null, idAcerto, null, null, null, null, null, null, null, null, null, null, idConta,
                tipoMov, formaSaida, valorMov, juros, numAutConstrucard, mudarSaldo, null, null, obs, false, dataMovBanco, contadorDataUnica);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Movimentação proveniente de conta a receber
        /// </summary>
        public uint MovCxContaRec(uint? idPedido, uint? idLiberarPedido, uint idContaR, uint? idCliente, uint idConta,
            int tipoMov, decimal valorMov, decimal juros, string numAutConstrucard, int formaSaida, bool mudarSaldo, DateTime? dataMovBanco, string obs)
        {
            return MovCxContaRec(null, idPedido, idLiberarPedido, idContaR, idCliente, idConta, tipoMov, valorMov,
                juros, numAutConstrucard, formaSaida, mudarSaldo, dataMovBanco, obs);
        }

        /// <summary>
        /// Movimentação proveniente de conta a receber
        /// </summary>
        public uint MovCxContaRec(GDASession sessao, uint? idPedido, uint? idLiberarPedido, uint idContaR, uint? idCliente, uint idConta,
            int tipoMov, decimal valorMov, decimal juros, string numAutConstrucard, int formaSaida, bool mudarSaldo, DateTime? dataMovBanco, string obs)
        {
            return MovCxContaRec(sessao, idPedido, idLiberarPedido, idContaR, idCliente, idConta, tipoMov, valorMov, juros, numAutConstrucard, formaSaida, mudarSaldo, dataMovBanco, obs, null);
        }

        /// <summary>
        /// Movimentação proveniente de conta a receber
        /// </summary>
        public uint MovCxContaRec(GDASession sessao, uint? idPedido, uint? idLiberarPedido, uint idContaR, uint? idCliente, uint idConta,
            int tipoMov, decimal valorMov, decimal juros, string numAutConstrucard, int formaSaida, bool mudarSaldo, DateTime? dataMovBanco, string obs, int? contadorDataUnica)
        {
            return MovimentaCaixa(sessao, null, null, idCliente, null, idPedido, null, null, null, null, null, idContaR, null, null, idLiberarPedido, null, null, null,
                null, null, idConta, tipoMov, formaSaida, valorMov, juros, numAutConstrucard, mudarSaldo, null, null, obs, false, dataMovBanco, contadorDataUnica);
        }

        /// <summary>
        /// Movimentação proveniente de pagamento
        /// </summary>
        public uint MovCxPagto(GDASession sessao, uint idPagto, uint? idContaPg, uint? idFornec, uint idConta, int tipoMov, decimal valorMov,
            decimal juros, string numAutConstrucard, string obs, int formaSaida, bool mudarSaldo, DateTime? dataMovBanco)
        {
            return MovCxPagto(sessao, idPagto, null, idContaPg, idFornec, idConta, tipoMov, valorMov, juros,
                numAutConstrucard, obs, formaSaida, mudarSaldo, dataMovBanco);
        }

        /// <summary>
        /// Movimentação proveniente de pagamento
        /// </summary>
        public uint MovCxPagto(GDASession sessao, uint idPagto, int? idAcertoCheque, uint? idContaPg, uint? idFornec, uint idConta, int tipoMov, decimal valorMov,
            decimal juros, string numAutConstrucard, string obs, int formaSaida, bool mudarSaldo, DateTime? dataMovBanco)
        {
            return MovimentaCaixa(sessao, null, null, null, idFornec, null, null, null, null, null, idPagto, null, idContaPg, null, null, null, null, null, null, null, idConta,
                tipoMov, formaSaida, valorMov, juros, numAutConstrucard, mudarSaldo, (uint?)idAcertoCheque, null, obs, false, dataMovBanco);
        }

        /// <summary>
        /// Movimentação proviniente do pagamento de sinal da compra
        /// </summary>
        public uint MovCxSinalCompra(GDASession session, uint idSinalCompra, uint? idContaPg, uint? idFornec, uint idConta, int tipoMov,
            decimal valorMov, string obs, int formaSaida, bool mudarSaldo, DateTime? dataMovBanco)
        {
            return MovimentaCaixa(session, null, null, null, idFornec, null, null, idSinalCompra, null, null, null, null, idContaPg, null, null, null,
                null, null, null, null, idConta, tipoMov, formaSaida, valorMov, 0, null, mudarSaldo, null, null, obs, false, dataMovBanco);
        }

        /// <summary>
        /// Movimentação proviniente do pagamento de antecipação fornecedor
        /// </summary>
        /// <param name="idSinalCompra"></param>
        /// <returns></returns>
        public uint MovCxAntecipFornec(GDASession sessao, uint idAntecipFornec, uint? idFornec, uint idConta, int tipoMov,
            decimal valorMov, string obs, int formaSaida, bool mudarSaldo, DateTime? dataMovBanco)
        {
            return MovimentaCaixa(sessao, null, null, null, idFornec, null, null, null, null, null, null, null, null, null, null, null,
                idAntecipFornec, null, null, null, idConta, tipoMov, formaSaida, valorMov, 0, null, mudarSaldo, null, null, obs, false, dataMovBanco);
        }

        /// <summary>
        /// Movimentação proveniente de cheque
        /// </summary>
        public uint MovCxCheque(uint idCheque, uint? idCliente, uint? idFornec, uint idConta, int formaSaida, int tipoMov,
            decimal valorMov, decimal juros, string numAutConstrucard, bool mudarSaldo, DateTime? dataMovBanco)
        {
            return MovimentaCaixa(null, null, idCliente, idFornec, null, null, null, null, null, null, null, null, idCheque, null, null, null, null, null, null, idConta,
                tipoMov, formaSaida, valorMov, juros, numAutConstrucard, mudarSaldo, null, null, null, false, dataMovBanco);
        }

        /// <summary>
        /// Movimentação proveniente de cheque
        /// </summary>
        public uint MovCxCheque(GDASession sessao, uint idCheque, uint? idDeposito, uint? idAcertoCheque, uint? idCliente, uint? idFornec, uint idConta, int tipoMov, decimal valorMov, decimal juros, string numAutConstrucard, bool mudarSaldo, string obs, DateTime? dataMovBanco)
        {
            return MovimentaCaixa(sessao, null, null, idCliente, idFornec, null, null, null, null, null, null, null, null, idCheque, null, null, null, null, idDeposito,
                null, idConta, tipoMov, 2, valorMov, juros, numAutConstrucard, mudarSaldo, idAcertoCheque, null, obs, false, dataMovBanco);
        }

        /// <summary>
        /// Movimentação de depósito
        /// </summary>
        public uint MovCxDeposito(uint? idDeposito, uint? idLoja, uint? idCliente, uint? idFornec, uint idConta,
            int tipoMov, int formaSaida, decimal valorMov, decimal juros, string numAutConstrucard, bool mudarSaldo,
            string obs, DateTime? dataMovBanco)
        {
            return MovCxDeposito(null, idDeposito, idLoja, idCliente, idFornec, idConta, tipoMov, formaSaida, valorMov,
                juros, numAutConstrucard, mudarSaldo, obs, dataMovBanco);
        }

        /// <summary>
        /// Movimentação de depósito
        /// </summary>
        public uint MovCxDeposito(GDASession sessao, uint? idDeposito, uint? idLoja, uint? idCliente, uint? idFornec,
            uint idConta, int tipoMov, int formaSaida, decimal valorMov, decimal juros, string numAutConstrucard,
            bool mudarSaldo, string obs, DateTime? dataMovBanco)
        {
            return MovimentaCaixa(sessao, null, idLoja, idCliente, idFornec, null, null, null, null, null, null, null, null,
                null, null, null, null, null, idDeposito, null, idConta, tipoMov, formaSaida, valorMov, juros,
                numAutConstrucard, mudarSaldo, null, null, obs, false, dataMovBanco);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        public uint MovCxAcertoCheque(GDASession sessao, uint idAcertoCheque, uint? idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros,
            string numAutConstrucard, int formaSaida, bool mudarSaldo, DateTime? dataMovBanco, string obs)
        {
            return MovCxAcertoCheque(sessao, idAcertoCheque, idCliente, idConta, tipoMov, valorMov, juros,
                numAutConstrucard, formaSaida, mudarSaldo, dataMovBanco, obs, null);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        public uint MovCxAcertoCheque(GDASession sessao, uint idAcertoCheque, uint? idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros,
            string numAutConstrucard, int formaSaida, bool mudarSaldo, DateTime? dataMovBanco, string obs, int? contadorDataUnica)
        {
            return MovimentaCaixa(sessao, null, null, idCliente, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, idConta, tipoMov, formaSaida, valorMov, juros, numAutConstrucard, mudarSaldo, idAcertoCheque, null, obs, false, dataMovBanco,
                contadorDataUnica);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        public uint MovCxAcertoCheque(GDASession sessao, uint idAcertoCheque, uint? idCheque, uint? idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros,
            string numAutConstrucard, int formaSaida, bool mudarSaldo, DateTime? dataMovBanco, string obs)
        {
            return MovCxAcertoCheque(sessao, idAcertoCheque, idCheque, idCliente, idConta, tipoMov, valorMov, juros,
                numAutConstrucard, formaSaida, mudarSaldo, dataMovBanco, obs, null);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        public uint MovCxAcertoCheque(GDASession sessao, uint idAcertoCheque, uint? idCheque, uint? idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros,
            string numAutConstrucard, int formaSaida, bool mudarSaldo, DateTime? dataMovBanco, string obs, int? contadorDataUnica)
        {
            return MovimentaCaixa(sessao, null, null, idCliente, null, null, null, null, null, null, null, null, null, idCheque, null, null, null, null,
                null, null, idConta, tipoMov, formaSaida, valorMov, juros, numAutConstrucard, mudarSaldo, idAcertoCheque, null, obs, false, dataMovBanco, contadorDataUnica);
        }

        /// <summary>
        /// Movimentação proveniente de cheque
        /// </summary>
        public uint MovCxLiberarPedido(GDASession sessao, uint idLiberarPedido, uint? idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros,
            string numAutConstrucard, bool mudarSaldo, DateTime? dataMovBanco, string obs)
        {
            return MovCxLiberarPedido(sessao, idLiberarPedido, idCliente, idConta, tipoMov, valorMov, juros, numAutConstrucard, mudarSaldo, dataMovBanco, obs, null);
        }

        /// <summary>
        /// Movimentação proveniente de cheque
        /// </summary>
        public uint MovCxLiberarPedido(GDASession sessao, uint idLiberarPedido, uint? idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros,
            string numAutConstrucard, bool mudarSaldo, DateTime? dataMovBanco, string obs, int? contadorDataUnica)
        {
            return MovimentaCaixa(sessao, null, null, idCliente, null, null, null, null, null, null, null, null, null, null, idLiberarPedido, null, null, null, null, null,
                idConta, tipoMov, 0, valorMov, juros, numAutConstrucard, mudarSaldo, null, null, obs, false, dataMovBanco, contadorDataUnica);
        }

        /// <summary>
        /// Movimentação proveniente de obra.
        /// </summary>
        public uint MovCxObra(GDASession sessao, uint idObra, uint? idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros,
            string numAutConstrucard, bool mudarSaldo, DateTime? dataMovBanco, string obs)
        {
            return MovCxObra(sessao, idObra, idCliente, idConta, tipoMov, valorMov, juros, numAutConstrucard, mudarSaldo, dataMovBanco, obs, null);
        }

        /// <summary>
        /// Movimentação proveniente de obra.
        /// </summary>
        public uint MovCxObra(GDASession sessao, uint idObra, uint? idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros,
            string numAutConstrucard, bool mudarSaldo, DateTime? dataMovBanco, string obs, int? contadorDataUnica)
        {
            return MovimentaCaixa(sessao, null, null, idCliente, null, null, null, null, null, null, null, null, null, null, null, idObra, null, null, null, null, idConta,
                tipoMov, 0, valorMov, juros, numAutConstrucard, mudarSaldo, null, null, obs, false, dataMovBanco, contadorDataUnica);
        }

        /// <summary>
        /// Movimentação proveniente de troca/devolução.
        /// </summary>
        public uint MovCxTrocaDev(GDASession sessao, uint idTrocaDevolucao, uint? idPedido, uint idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros,
            string numAutConstrucard, bool mudarSaldo, DateTime? dataMovBanco, string obs)
        {
            return MovCxTrocaDev(sessao, idTrocaDevolucao, idPedido, idCliente, idConta, tipoMov, valorMov, juros, numAutConstrucard, mudarSaldo, dataMovBanco, obs, null);
        }

        /// <summary>
        /// Movimentação proveniente de troca/devolução.
        /// </summary>
        public uint MovCxTrocaDev(GDASession sessao, uint idTrocaDevolucao, uint? idPedido, uint idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros,
            string numAutConstrucard, bool mudarSaldo, DateTime? dataMovBanco, string obs, int? contadorDataUnica)
        {
            return MovimentaCaixa(sessao, null, null, idCliente, null, idPedido, null, null, null, null, null, null, null, null, null, null, null, idTrocaDevolucao, null, null,
                idConta, tipoMov, 0, valorMov, juros, numAutConstrucard, mudarSaldo, null, null, obs, false, dataMovBanco, contadorDataUnica);
        }

        /// <summary>
        /// Movimentação proveniente de conta bancária.
        /// </summary>
        public uint MovCxContaBanco(uint idContaBanco, uint idConta, int tipoMov, decimal valorMov, decimal juros, string obs, bool mudarSaldo, DateTime? dataMovBanco)
        {
            return MovCxContaBanco(null, idContaBanco, idConta, tipoMov, valorMov, juros, obs, mudarSaldo, dataMovBanco);
        }

        /// <summary>
        /// Movimentação proveniente de conta bancária.
        /// </summary>
        public uint MovCxContaBanco(GDASession session, uint idContaBanco, uint idConta, int tipoMov, decimal valorMov, decimal juros, string obs, bool mudarSaldo, DateTime? dataMovBanco)
        {
            return MovimentaCaixa(session, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, idContaBanco, idConta, tipoMov, 0, valorMov, juros, null, mudarSaldo, null, null, obs, false, dataMovBanco);
        }

        /// <summary>
        /// Movimentação manual de crédito
        /// </summary>
        public uint MovCxCredito(uint idConta, int tipoMov, int formaSaida, decimal valorMov, bool mudarSaldo, string obs, DateTime? dataMovBanco)
        {
            return MovimentaCaixa(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, idConta, tipoMov, formaSaida, valorMov, 0, null, mudarSaldo, null, null, obs, true, dataMovBanco);
        }

        /// <summary>
        /// Movimentação manual de débito
        /// </summary>
        public uint MovCxDebito(GDASession session, uint? idFornec, uint? idCheque, uint idConta, int formaSaida, decimal valorMov, string obs)
        {
            return MovimentaCaixa(session, null, null, null, idFornec, null, null, null, null, null, null, null, null, idCheque, null, null, null, null, null, null, idConta, 2, formaSaida,
                valorMov, 0, null, true, null, null, obs, true, null);
        }

        /// <summary>
        /// Movimentação de vale de funcionário.
        /// </summary>
        public uint MovCxMovFunc(GDASession sessao, uint? idCliente, uint? idPedido, uint? idLiberarPedido, uint idConta, int tipoMov, decimal valorMov, decimal juros,
            string numAutConstrucard, DateTime? dataMovBanco, string obs, int? contadorDataUnica)
        {
            return MovimentaCaixa(sessao, null, null, idCliente, null, idPedido, null, null, null, null, null, null, null, null, idLiberarPedido, null, null,
                null, null, null, idConta, tipoMov, 0, valorMov, 0, null, true, null, null, obs, false, dataMovBanco, contadorDataUnica);
        }

        public uint MovCxDevolucaoPagto(GDASession sessao, uint idDevolucaoPagto, uint idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros,
            string numAutConstrucard, bool mudarSaldo, DateTime? dataMovBanco, string obs)
        {
            return MovCxDevolucaoPagto(sessao, idDevolucaoPagto, idCliente, idConta, tipoMov, valorMov, juros, numAutConstrucard, mudarSaldo, dataMovBanco, obs, null);
        }

        public uint MovCxDevolucaoPagto(GDASession sessao, uint idDevolucaoPagto, uint idCliente, uint idConta, int tipoMov, decimal valorMov, decimal juros,
            string numAutConstrucard, bool mudarSaldo, DateTime? dataMovBanco, string obs, int? contadorDataUnica)
        {
            return MovimentaCaixa(sessao, null, null, idCliente, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                idConta, tipoMov, 0, valorMov, juros, numAutConstrucard, mudarSaldo, null, idDevolucaoPagto, obs, false, dataMovBanco, contadorDataUnica);
        }

        public uint MovCxCreditoFornecedor(GDASession session, uint idCredFornec, uint? idFornec, uint idConta, int tipoMov, decimal valorMov,
            int formaSaida, bool mudarSaldo, DateTime? dataMovBanco, int? contadorDataUnica)
        {
            return MovimentaCaixa(session, idCredFornec, null, null, idFornec, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, idConta,
                tipoMov, formaSaida, valorMov, 0, null, mudarSaldo, null, null, null, false, dataMovBanco, contadorDataUnica);
        }

        public uint MovCxCartaoNaoIdentificado(GDASession sessao, uint idCartaoNaoIdentificado, uint idConta, int tipoMov, decimal valorMov,
            decimal juros, bool mudarSaldo, DateTime? dataMovBanco, string obs)
        {
            return MovimentaCaixa(sessao, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                idConta, tipoMov, 0, valorMov, juros, null, mudarSaldo, null, null, idCartaoNaoIdentificado, obs, false, dataMovBanco, null);
        }

        /// <summary>
        /// Gera movimentação no caixa geral
        /// </summary>
        private uint MovimentaCaixa(uint? idCredFornec, uint? idLoja, uint? idCliente, uint? idFornec, uint? idPedido, uint? idSinal, uint? idSinalCompra, uint? idCompra, uint? idAcerto,
            uint? idPagto, uint? idContaR, uint? idContaPg, uint? idCheque, uint? idLiberarPedido, uint? idObra, uint? idAntecipFornec, uint? idTrocaDevolucao,
            uint? idDeposito, uint? idContaBanco, uint idConta, int tipoMov, int formaSaida, decimal valorMov, decimal juros, string numAutConstrucard, bool mudarSaldo,
            uint? idAcertoCheque, uint? idDevolucaoPagto, string obs, bool lancManual, DateTime? dataMovBanco)
        {
            return MovimentaCaixa(null, idCredFornec, idLoja, idCliente, idFornec, idPedido, idSinal, idSinalCompra, idCompra, idAcerto,
            idPagto, idContaR, idContaPg, idCheque, idLiberarPedido, idObra, idAntecipFornec, idTrocaDevolucao,
            idDeposito, idContaBanco, idConta, tipoMov, formaSaida, valorMov, juros, numAutConstrucard, mudarSaldo,
            idAcertoCheque, idDevolucaoPagto, obs, lancManual, dataMovBanco);
        }

        /// <summary>
        /// Gera movimentação no caixa geral
        /// </summary>
        private uint MovimentaCaixa(GDASession sessao, uint? idCredFornec, uint? idLoja, uint? idCliente, uint? idFornec, uint? idPedido, uint? idSinal, uint? idSinalCompra, uint? idCompra, uint? idAcerto,
            uint? idPagto, uint? idContaR, uint? idContaPg, uint? idCheque, uint? idLiberarPedido, uint? idObra, uint? idAntecipFornec, uint? idTrocaDevolucao,
            uint? idDeposito, uint? idContaBanco, uint idConta, int tipoMov, int formaSaida, decimal valorMov, decimal juros, string numAutConstrucard, bool mudarSaldo,
            uint? idAcertoCheque, uint? idDevolucaoPagto, string obs, bool lancManual, DateTime? dataMovBanco)
        {
            return MovimentaCaixa(sessao, idCredFornec, idLoja, idCliente, idFornec, idPedido, idSinal, idSinalCompra, idCompra, idAcerto,
                idPagto, idContaR, idContaPg, idCheque, idLiberarPedido, idObra, idAntecipFornec, idTrocaDevolucao, idDeposito, idContaBanco,
                idConta, tipoMov, formaSaida, valorMov, juros, numAutConstrucard, mudarSaldo, idAcertoCheque, idDevolucaoPagto, obs, lancManual,
                dataMovBanco, null);
        }

        /// <summary>
        /// Gera movimentação no caixa geral
        /// </summary>
        private uint MovimentaCaixa(GDASession sessao, uint? idCredFornec, uint? idLoja, uint? idCliente, uint? idFornec, uint? idPedido, uint? idSinal, uint? idSinalCompra, uint? idCompra, uint? idAcerto,
            uint? idPagto, uint? idContaR, uint? idContaPg, uint? idCheque, uint? idLiberarPedido, uint? idObra, uint? idAntecipFornec, uint? idTrocaDevolucao,
            uint? idDeposito, uint? idContaBanco, uint idConta, int tipoMov, int formaSaida, decimal valorMov, decimal juros, string numAutConstrucard, bool mudarSaldo,
            uint? idAcertoCheque, uint? idDevolucaoPagto, string obs, bool lancManual, DateTime? dataMovBanco, int? contadorDataUnica, int? idCaixaDiario = null)
        {
            return MovimentaCaixa(sessao, idCredFornec, idLoja, idCliente, idFornec, idPedido, idSinal, idSinalCompra, idCompra, idAcerto,
                idPagto, idContaR, idContaPg, idCheque, idLiberarPedido, idObra, idAntecipFornec, idTrocaDevolucao, idDeposito,
                idContaBanco, idConta, tipoMov, formaSaida, valorMov, juros, numAutConstrucard, mudarSaldo, idAcertoCheque,
                idDevolucaoPagto, null, obs, lancManual, dataMovBanco, contadorDataUnica, idCaixaDiario);
        }

        /// <summary>
        /// Gera movimentação no caixa geral
        /// </summary>
        private uint MovimentaCaixa(GDASession sessao, uint? idCredFornec, uint? idLoja, uint? idCliente, uint? idFornec, uint? idPedido, uint? idSinal, uint? idSinalCompra, uint? idCompra, uint? idAcerto,
            uint? idPagto, uint? idContaR, uint? idContaPg, uint? idCheque, uint? idLiberarPedido, uint? idObra, uint? idAntecipFornec, uint? idTrocaDevolucao,
            uint? idDeposito, uint? idContaBanco, uint idConta, int tipoMov, int formaSaida, decimal valorMov, decimal juros, string numAutConstrucard, bool mudarSaldo,
            uint? idAcertoCheque, uint? idDevolucaoPagto, uint? idCartaoNaoIdentificado, string obs, bool lancManual, DateTime? dataMovBanco, int? contadorDataUnica, int? idCaixaDiario = null)
        {
            if (idConta == 0)
                throw new Exception("O plano de contas não pode ser 0.");

            // Se for utilização de crédito, não gera movimentação no caixa
            if (new List<uint>(UtilsPlanoConta.GetLstCredito(3)).Contains(idConta))
                mudarSaldo = false;

            if (idCliente == 0) idCliente = null;
            if (idFornec == 0) idFornec = null;
            if (idCredFornec == 0) idCredFornec = null;

            CaixaGeral caixaGeral = new CaixaGeral();
            caixaGeral.IdLoja = idLoja;
            caixaGeral.IdCliente = idCliente;
            caixaGeral.IdFornec = idFornec;
            caixaGeral.IdPedido = idPedido;
            caixaGeral.IdSinal = idSinal;
            caixaGeral.IdSinalCompra = idSinalCompra;
            caixaGeral.IdCompra = idCompra;
            caixaGeral.IdAcerto = idAcerto;
            caixaGeral.IdPagto = idPagto;
            caixaGeral.IdContaR = idContaR;
            caixaGeral.IdContaPg = idContaPg;
            caixaGeral.IdCheque = idCheque;
            caixaGeral.IdLiberarPedido = idLiberarPedido;
            caixaGeral.IdObra = idObra;
            caixaGeral.IdAntecipFornec = idAntecipFornec;
            caixaGeral.IdAcertoCheque = idAcertoCheque;
            caixaGeral.IdTrocaDevolucao = idTrocaDevolucao;
            caixaGeral.IdContaBanco = idContaBanco;
            caixaGeral.IdDevolucaoPagto = idDevolucaoPagto;
            caixaGeral.IdCartaoNaoIdentificado = idCartaoNaoIdentificado;
            caixaGeral.IdConta = idConta;
            caixaGeral.TipoMov = tipoMov;
            caixaGeral.DataMov = DateTime.Now;
            caixaGeral.DataMovBanco = dataMovBanco;
            caixaGeral.IdCaixaDiario = idCaixaDiario;
            caixaGeral.NumAutConstrucard = numAutConstrucard;
            caixaGeral.Saldo = mudarSaldo ? (tipoMov == 1 ? GetSaldo(sessao) + valorMov : GetSaldo(sessao) - valorMov) : GetSaldo(sessao);
            caixaGeral.ValorMov = valorMov;
            caixaGeral.Juros = juros;
            caixaGeral.Obs = obs != null && obs.Length > 500 ? obs.Substring(0, 500) : obs;
            caixaGeral.LancManual = lancManual;
            caixaGeral.IdCreditoFornecedor = idCredFornec;
            caixaGeral.Usucad = UserInfo.GetUserInfo.CodUser;
            caixaGeral.MudarSaldo = mudarSaldo;

            if (formaSaida > 0)
                caixaGeral.FormaSaida = formaSaida;

            var idCaixaGeral = base.Insert(sessao, caixaGeral);

            /* Chamado 33659. */
            if (contadorDataUnica.HasValue)
                objPersistence.ExecuteCommand(sessao,
                    string.Format("UPDATE caixa_geral SET DataUnica=CONCAT(DataUnica, '_', {0}) WHERE IdCaixaGeral={1};",
                        contadorDataUnica, idCaixaGeral));

            return idCaixaGeral;
        }

        /// <summary>
        /// Remove as movimentações do caixa geral com base no Identificador do Caixa Diário
        /// </summary>
        /// <param name="sessao">Sessão do GDA.</param>
        /// <param name="idCaixaDiario">Identificador da movimentação do caixa diário.</param>
        public void RemoverMovimentacoesFechamanto(GDASession sessao, int idCaixaDiario)
        {
            var sql = $@"
                SELECT IdCaixaGeral
                FROM caixa_geral
                WHERE IdCaixaDiario = {idCaixaDiario}";

            try
            {
                if (CaixaDiarioDAO.Instance.ObterValorMov(sessao, idCaixaDiario) > 0)
                {
                    var idsCaixaGeralExcluir = this.ExecuteMultipleScalar<int>(sessao, sql);

                    if (!idsCaixaGeralExcluir.Any(c => c > 0))
                    {
                        throw new Exception("Não foram transferidos valores para o Caixa Geral com este fechamento.");
                    }

                    sql = $@"
                        SELECT (SELECT Saldo FROM caixa_geral WHERE IdCaixaGeral <
		                    ALL (SELECT IdCaixaGeral FROM caixa_geral
		            	    WHERE IdCaixaGeral IN ({string.Join(",", idsCaixaGeralExcluir)})) ORDER BY IdCaixaGeral DESC LIMIT 1)-(
                            SELECT Saldo
		            	    FROM caixa_geral
                            WHERE IdCaixaGeral IN ({string.Join(",", idsCaixaGeralExcluir)}) ORDER BY IdCaixaGeral DESC LIMIT 1)";

                    var valorMov = this.ExecuteScalar<float>(sessao, sql);

                    sql = $@"
                        UPDATE caixa_geral
                        SET Saldo = Saldo - {valorMov}
                        WHERE IdCaixaGeral > {idsCaixaGeralExcluir.OrderByDescending(item => item).First()}";

                    this.objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?valor", valorMov));

                    this.DeleteByPKs(sessao, idsCaixaGeralExcluir);
                }
            }
            catch
            {
                throw new Exception("Falha ao ajustar os valores do caixa geral.");
            }
        }

        /// <summary>
        /// Associa um cartão não identificado ao caixa geral
        /// </summary>
        public void AssociarCaixaGeralIdCartaoNaoIdentificado(GDASession sessao, uint idCxGeral, uint idCartaoNaoIdentificado)
        {
            objPersistence.ExecuteCommand(sessao,
                    string.Format("UPDATE caixa_geral SET IdCartaoNaoIdentificado={0} WHERE IdCaixaGeral={1};", idCartaoNaoIdentificado, idCxGeral));
        }

        #endregion

        #region Retorna o saldo

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Recupera o saldo do caixa geral
        /// </summary>
        /// <returns></returns>
        public decimal GetSaldo()
        {
            var sessao = new GDASession();
            sessao = null;
            return GetSaldo(sessao);
        }

        /// <summary>
        /// Recupera o saldo do caixa geral
        /// </summary>
        /// <returns></returns>
        public decimal GetSaldo(GDASession sessao)
        {
            return GetSaldo(sessao, null);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Recupera o saldo do caixa geral do dia passado
        /// </summary>
        /// <param name="dataSaldo"></param>
        /// <returns></returns>
        public decimal GetSaldo(DateTime? dataSaldo)
        {
            return GetSaldo(null, dataSaldo);
        }

        /// <summary>
        /// Recupera o saldo do caixa geral do dia passado
        /// </summary>
        /// <param name="dataSaldo"></param>
        /// <returns></returns>
        public decimal GetSaldo(GDASession sessao, DateTime? dataSaldo)
        {
            string sql = "Select idCaixaGeral From caixa_geral";

            GDAParameter[] lstParam = null;

            if (dataSaldo != null)
            {
                sql += " Where dataMov<=?dataMov";
                lstParam = new GDAParameter[] { new GDAParameter("?dataMov", DateTime.Parse(dataSaldo.Value.ToString("dd/MM/yyyy 23:59:59"))) };
            }

            sql += " Order By IdCaixaGeral {0} limit 1";

            uint idCaixaGeralUlt = ExecuteScalar<uint>(sessao, String.Format(sql, "Desc"), lstParam);
            uint idCaixaGeralPrim = ExecuteScalar<uint>(sessao, String.Format(sql, "Asc"), lstParam);

            return ObtemValorCampo<decimal>(sessao, "saldo", "idCaixaGeral=" + Math.Max(idCaixaGeralPrim, idCaixaGeralUlt));
        }

        /// <summary>
        /// Recupera o saldo de lançamentos avulsos do caixa geral pelo período informado.
        /// </summary>
        /// <param name="dataIni">Data de inicio da busca de lançamentos.</param>
        /// <param name="dataFim">Data fim da busca de lançamentos.</param>
        /// <param name="idFornec">Identificador do fornecedor a ser filtrado.</param>
        /// <param name="idLoja">Identificador da loja a ser filtrada.</param>
        /// <param name="planoConta">Descrição do plano de conta a ser filtrado.</param>
        /// <returns>Saldo de lançamentos avulsos do caixa geral.</returns>
        public decimal GetSaldoLancAvulsos(DateTime? dataIni, DateTime? dataFim, uint? idFornec, int? idLoja, string planoConta)
        {
            if ((dataIni == null) &&
                (dataFim == null) &&
                (idFornec.GetValueOrDefault() == 0) &&
                (idLoja.GetValueOrDefault() == 0) &&
                string.IsNullOrWhiteSpace(planoConta))
            {
                return 0;
            }

            var joinsPlanoConta = !string.IsNullOrEmpty(planoConta) ?
                @"Left Join plano_contas pl On (cg.IdConta=pl.IdConta)
                Left Join grupo_conta g On (pl.IdGrupo = g.IdGrupo)
                Left Join categoria_conta cc On(g.idCategoriaConta = cc.idCategoriaConta)" :
                string.Empty;

            string sql = $@"
                Select Sum(valormov) from caixa_geral cg
                {joinsPlanoConta}
                where cg.lancmanual = 1 and cg.tipoMov = 2 ";
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (dataIni != null)
            {
                sql += "and cg.datamov >= ?dataIni ";
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni.Value.ToString("dd/MM/yyyy 00:00:00"))));
            }
            if (dataFim != null)
            {
                sql += "and cg.datamov <= ?dataFim ";
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim.Value.ToString("dd/MM/yyyy 23:59:59"))));
            }
            if (idFornec > 0)
            {
                sql += " AND cg.idfornec = ?idFornec";
                lstParam.Add(new GDAParameter("?idFornec", idFornec));
            }
            if (idLoja > 0)
            {
                sql += " AND cg.IdLoja = ?idLoja";
                lstParam.Add(new GDAParameter("?idLoja", idLoja));
            }
            if (!string.IsNullOrEmpty(planoConta))
            {
                sql += " and (pl.descricao like ?planoConta or g.descricao like ?planoConta or cc.descricao like ?planoConta)";
                lstParam.Add(new GDAParameter("?planoConta", planoConta));
            }

            return ExecuteScalar<uint>(sql, lstParam.ToArray());
        }

        #endregion

        #region Retorna o saldo/entrada/saída por forma de pagamento e período

        /// <summary>
        /// Retorna o saldo/entrada/saída por forma de pagamento e período
        /// </summary>
        public decimal GetSaldoByFormaPagto(Pagto.FormaPagto formaPagto, uint idFunc, string dataIni, string dataFim, int tipo, uint idLoja)
        {
            return GetSaldoByFormaPagto(null, formaPagto, idFunc, dataIni, dataFim, tipo, idLoja);
        }

        /// <summary>
        /// Retorna o saldo/entrada/saída por forma de pagamento e período
        /// </summary>
        public decimal GetSaldoByFormaPagto(GDASession session, Pagto.FormaPagto formaPagto, uint idFunc, string dataIni, string dataFim, int tipo, uint idLoja)
        {
            string idsContaEstornoSaida = UtilsPlanoConta.GetLstEstornoSaidaByFormaPagto(formaPagto, 0);

            string sqlEntrada = "Coalesce((Select Sum(valorMov) From caixa_geral Where tipomov=1 and (idConta in(" +
                UtilsPlanoConta.GetLstEntradaByFormaPagto(formaPagto, 0, false) +
                (formaPagto == Pagto.FormaPagto.Dinheiro ? "," + idsContaEstornoSaida : "") + ") " +
                (formaPagto == Pagto.FormaPagto.Dinheiro ? " Or formaSaida=" + (int)Pagto.FormaPagto.Dinheiro :
                formaPagto == Pagto.FormaPagto.ChequeProprio ? " Or (idConta not in (" + UtilsPlanoConta.GetLstEstornoSaidaByFormaPagto(formaPagto, 0) + ") and formaSaida=" + (int)Pagto.FormaPagto.ChequeProprio + ")" : String.Empty) +
                ")&dtIni&&dtFim&&func&&idLoja&), 0)";

            string sqlSaida = "Coalesce((Select Sum(valorMov) From caixa_geral Where tipomov=2 and (idConta in (" +
                UtilsPlanoConta.GetLstSaidaByFormaPagto(formaPagto, 0, 1) + ") " +
                (formaPagto == Pagto.FormaPagto.Dinheiro ? " Or formaSaida=" + (int)Pagto.FormaPagto.Dinheiro :
                formaPagto == Pagto.FormaPagto.ChequeProprio ? " Or formaSaida=" + (int)Pagto.FormaPagto.ChequeProprio : String.Empty) +
                ")&dtIni&&dtFim&&func&&idLoja&), 0)";

            string sqlEstornoSaida = !String.IsNullOrEmpty(idsContaEstornoSaida) && formaPagto != Pagto.FormaPagto.Dinheiro ?
                "Coalesce((Select Sum(valorMov) From caixa_geral Where tipomov=1 and (idConta in(" +
                UtilsPlanoConta.GetLstEstornoSaidaByFormaPagto(formaPagto, 0) + ") " +
                ")&dtIni&&dtFim&&func&&idLoja&), 0)" : "0";

            string sql =
                tipo == 1 ? "Select " + sqlEntrada + "-ABS(" + sqlSaida + "-" + sqlEstornoSaida + ") as Total" :
                tipo == 2 ? "Select " + sqlEntrada + " as Total" :
                tipo == 3 ? "Select ABS(" + sqlSaida + "-" + sqlEstornoSaida + ") as Total" :
                tipo == 4 ? "Select " + sqlEntrada + " as Total" :
                tipo == 5 ? "Select 0 as Total" :
                String.Empty;

            sql = sql.Replace("&dtIni&", !String.IsNullOrEmpty(dataIni) ? " And dataMov>=?dtIni" : String.Empty);
            sql = sql.Replace("&dtFim&", !String.IsNullOrEmpty(dataFim) ? " And dataMov<=?dtFim" : String.Empty);

            sql = sql.Replace("&func&", idFunc > 0 ? " And usuCad=" + idFunc : String.Empty);

            sql = sql.Replace("&idLoja&", idLoja > 0 ? " And idLoja=" + idLoja : String.Empty);

            return ExecuteScalar<decimal>(session, sql, GetParams(dataIni, dataFim));
        }

        #endregion

        #region Retorna os valores de crédito gerado/recebido por período

        private decimal GetCreditoPeriodo(GDASession sessao, string idsContas, string idsContasEstorno, uint idFunc, string dtIni, string dtFim, uint idLoja)
        {
            string sqlEntrada = "Coalesce((Select Sum(valorMov + Coalesce(juros, 0)) From caixa_geral Where tipomov=1 and idConta in(" +
                idsContas + ") &dtIni&&dtFim&&func&&idLoja&), 0)";

            string sqlSaida = "Coalesce((Select Sum(valorMov + Coalesce(juros, 0)) From caixa_geral Where tipomov=2 and idConta in(" +
                idsContasEstorno + ") &dtIni&&dtFim&&func&&idLoja&), 0)";

            string sql = "Select " + sqlEntrada + "-" + sqlSaida + " as Total";

            sql = sql.Replace("&dtIni&", !String.IsNullOrEmpty(dtIni) ? " And dataMov>=?dtIni" : String.Empty);
            sql = sql.Replace("&dtFim&", !String.IsNullOrEmpty(dtFim) ? " And dataMov<=?dtFim" : String.Empty);
            sql = sql.Replace("&func&", idFunc > 0 ? " And usuCad=" + idFunc : String.Empty);
            sql = sql.Replace("&idLoja&", idLoja > 0 ? " And idLoja=" + idLoja : String.Empty);

            return ExecuteScalar<decimal>(sessao, sql, GetParams(dtIni, dtFim));
        }

        /// <summary>
        /// Retorna o valor de crédito recebido em um período.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="dtIni"></param>
        /// <param name="dtFim"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal GetCreditoRecebido(GDASession sessao, uint idFunc, string dtIni, string dtFim, uint idLoja)
        {
            string idsContas = UtilsPlanoConta.ContasCredito(3);
            string idsContasEstorno = UtilsPlanoConta.ContasCredito(4);
            return GetCreditoPeriodo(sessao, idsContas, idsContasEstorno, idFunc, dtIni, dtFim, idLoja);
        }

        /// <summary>
        /// Retorna o valor de crédito gerado em um período.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="dtIni"></param>
        /// <param name="dtFim"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal GetCreditoGerado(GDASession sessao, uint idFunc, string dtIni, string dtFim, uint idLoja)
        {
            string idsContas = UtilsPlanoConta.ContasCredito(2);
            string idsContasEstorno = UtilsPlanoConta.ContasCredito(5);
            return GetCreditoPeriodo(sessao, idsContas, idsContasEstorno, idFunc, dtIni, dtFim, idLoja);
        }

        #endregion

        #region Estorna compra

        /// <summary>
        /// Busca a soma de todos os valores que a compra tenha gerado no caixa geral
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public decimal GetEstornoCompra(uint idCompra)
        {
            string sql = "Select Round(Sum(ValorMov), 2) From caixa_geral Where tipoMov=2 And idContaPg is null And idCompra=" + idCompra;

            return ExecuteScalar<decimal>(sql);
        }

        /// <summary>
        /// Estorna conta paga no caixa geral
        /// </summary>
        /// <param name="idContaPaga"></param>
        public void EstornaCompra(uint idCompra)
        {
            CaixaGeral caixa = new CaixaGeral();
            caixa.DataMov = DateTime.Now;
            caixa.IdConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoPagtoDinheiro);
            caixa.IdCompra = idCompra;
            caixa.ValorMov = GetEstornoCompra(idCompra);
            caixa.Saldo = GetSaldo() + caixa.ValorMov;
            caixa.TipoMov = 1;

            Insert(caixa);
        }

        #endregion

        #region Estorna conta a pagar

        /// <summary>
        /// Estorna conta paga no caixa geral
        /// </summary>
        /// <param name="idContaPaga"></param>
        public uint EstornaContaPaga(uint idContaPaga, decimal valor)
        {
            CaixaGeral caixa = new CaixaGeral();
            caixa.DataMov = DateTime.Now;
            caixa.IdConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoPagtoDinheiro);
            caixa.IdContaPg = idContaPaga;
            caixa.Saldo = GetSaldo() + valor;
            caixa.TipoMov = 1;
            caixa.ValorMov = valor;

            return Insert(caixa);
        }

        #endregion

        #region Busca o sinal recebido pelo caixa geral referente a um pedido

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Busca o sinal recebido pelo caixa geral referente ao pedido passado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public CaixaGeral GetPedidoSinal(uint idPedido)
        {
            return GetPedidoSinal(null, idPedido);
        }

        /// <summary>
        /// Busca o sinal recebido pelo caixa geral referente ao pedido passado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public CaixaGeral GetPedidoSinal(GDASession sessao, uint idPedido)
        {
            string sql = @"
                Select c.*, f.Nome as DescrUsuCad
                From caixa_geral c
                    Left Join funcionario f On c.UsuCad=f.IdFunc
                Where idSinal in (Select idSinal From pedido Where idPedido=" + idPedido + @")
                    And idConta In (" + UtilsPlanoConta.ContasSinalPedido() + ")";

            List<CaixaGeral> lst = objPersistence.LoadData(sessao, sql).ToList();

            return lst.Count > 0 ? lst[0] : null;
        }

        #endregion

        #region Busca movimentações relacionadas a um pedido à vista

        /// <summary>
        /// Busca movimentações relacionadas a um pedido à vista
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public CaixaGeral[] GetByPedidoAVista(uint idPedido)
        {
            return GetByPedidoAVista(null, idPedido);
        }

        /// <summary>
        /// Busca movimentações relacionadas a um pedido à vista
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public CaixaGeral[] GetByPedidoAVista(GDASession session, uint idPedido)
        {
            string sql = @"
                Select * From caixa_geral
                Where idConta In (" + UtilsPlanoConta.ContasAVista() + "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado) + @")
                    And idPedido=" + idPedido + @" And idTrocaDevolucao is null
                Order By idCaixaGeral Desc";

            return objPersistence.LoadData(session, sql).ToArray();
        }

        #endregion

        #region Busca movimentações relacionadas a um sinal de pedido

        /// <summary>
        /// Busca movimentações relacionadas a um sinal de pedido
        /// </summary>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public CaixaGeral[] GetBySinal(uint idSinal)
        {
            return GetBySinal(null, idSinal);
        }

        /// <summary>
        /// Busca movimentações relacionadas a um sinal de pedido
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public CaixaGeral[] GetBySinal(GDASession sessao, uint idSinal)
        {
            string sql = @"
                Select * From caixa_geral
                Where idSinal=" + idSinal + @"
                    And idConta in (" + UtilsPlanoConta.ContasSinalPedido() + "," + UtilsPlanoConta.ResumoDiarioContasCreditoGerado() + @")
                Order By idCaixaGeral Desc";

            return objPersistence.LoadData(sessao, sql).ToArray();
        }

        #endregion

        #region Busca movimentações relacionadas a uma liberação

        /// <summary>
        /// Busca movimentações relacionadas a um pedido à vista
        /// </summary>
        /// <param name="idLiberacao"></param>
        /// <param name="tipo">1-A Vista, 2-Sinal</param>
        /// <returns></returns>
        public CaixaGeral[] GetByLiberacao(uint idLiberacao, int tipo)
        {
            return GetByLiberacao(null, idLiberacao, tipo);
        }

        /// <summary>
        /// Busca movimentações relacionadas a um pedido à vista
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idLiberacao"></param>
        /// <param name="tipo">1-A Vista, 2-Sinal</param>
        /// <returns></returns>
        public CaixaGeral[] GetByLiberacao(GDASession session, uint idLiberacao, int tipo)
        {
            string idConta =
                tipo == 1 ? UtilsPlanoConta.ContasAVista() + "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado) :
                tipo == 2 ? UtilsPlanoConta.ContasSinalPedido() + "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoEntradaGerado) : String.Empty;

            string sql = @"
                Select * From caixa_geral
                Where idConta In (" + idConta + @")
                    And idLiberarPedido=" + idLiberacao + @"
                Order By idCaixaGeral Desc";

            return objPersistence.LoadData(session, sql).ToArray();
        }

        #endregion

        #region Busca movimentações relacionadas a um acerto

        /// <summary>
        /// Retorna movimentações relacionadas ao acerto passado
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public CaixaGeral[] GetByAcerto(GDASession sessao, uint idAcerto)
        {
            var sql = string.Format(@"SELECT * FROM caixa_geral
                WHERE IdAcerto={0} OR (IdAcerto IS NULL AND IdContaR IN (SELECT IdContaR FROM contas_receber WHERE IdAcerto={0})) ORDER BY IdCaixaGeral DESC", idAcerto);

            return objPersistence.LoadData(sessao, sql).ToArray();
        }

        #endregion

        #region Busca movimentações relacionadas à uma conta recebida

        /// <summary>
        /// Busca movimentações relacionadas à uma conta recebida
        /// </summary>
        /// <param name="idContaR"></param>
        /// <returns></returns>
        public CaixaGeral[] GetByContaRec(GDASession sessao, uint idContaR)
        {
            string sql = "Select * From caixa_geral where idContaR=" + idContaR + " Order By idCaixaGeral desc";

            return objPersistence.LoadData(sessao, sql).ToArray();
        }

        public CaixaGeral[] GetByLiberacao(GDASession session, uint idLiberarPedido)
        {
            string sql = "Select * From caixa_geral where idLiberarPedido=" + idLiberarPedido + " Order By idCaixaGeral desc";

            return objPersistence.LoadData(session, sql).ToArray();
        }

        #endregion

        #region Retorna movimentações relacionadas à um cheque

        /// <summary>
        /// Retorna movimentações relacionadas à uma conta recebida
        /// </summary>
        /// <param name="idCheque"></param>
        /// <returns></returns>
        public CaixaGeral[] GetByCheque(uint idCheque)
        {
            string sql = "Select * From caixa_geral where idCheque=" + idCheque + " Order By idCaixaGeral desc";

            return objPersistence.LoadData(sql).ToArray();
        }

        #endregion

        #region Retorna movimentações relacionadas à um pagamento

        /// <summary>
        /// Retorna movimentações relacionadas à um pagamento
        /// </summary>
        public CaixaGeral[] GetByPagto(uint idPagto)
        {
            return GetByPagto(null, idPagto);
        }

        /// <summary>
        /// Retorna movimentações relacionadas à um pagamento
        /// </summary>
        public CaixaGeral[] GetByPagto(GDASession session, uint idPagto)
        {
            string sql = "Select * From caixa_geral where idPagto=" + idPagto + " Order By idCaixaGeral desc";

            return objPersistence.LoadData(session, sql).ToArray();
        }

        #endregion

        #region Retorna movimentações relacionadas à um sinal de compra

        /// <summary>
        /// Retorna movimentações relacionadas à um sinal de compra
        /// </summary>
        public CaixaGeral[] GetBySinalCompra(GDASession session, uint idSinalCompra)
        {
            string sql = "Select * From caixa_geral where idSinalCompra=" + idSinalCompra + " Order By idCaixaGeral desc";

            return objPersistence.LoadData(session, sql).ToArray();
        }

        #endregion

        #region Retorna movimentações relacionadas à uma antecipação de pagamento de fornecedor

        /// <summary>
        /// Retorna movimentações relacionadas à uma antecipação de fornecedor
        /// </summary>
        public CaixaGeral[] GetByAntecipFornec(GDASession session, uint idAntecipFornec)
        {
            string sql = "Select * From caixa_geral where idAntecipFornec=" + idAntecipFornec + " Order By idCaixaGeral desc";

            return objPersistence.LoadData(session, sql).ToArray();
        }

        #endregion

        #region Busca movimentações relacionadas a um acerto de cheques

        /// <summary>
        /// Retorna movimentações relacionadas ao acerto de cheques passado
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public CaixaGeral[] GetByAcertoCheque(uint idAcertoCheque)
        {
            return GetByAcertoCheque(null, idAcertoCheque);
        }

        /// <summary>
        /// Retorna movimentações relacionadas ao acerto de cheques passado
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public CaixaGeral[] GetByAcertoCheque(GDASession session, uint idAcertoCheque)
        {
            string sql = "Select * From caixa_geral where idAcertoCheque=" + idAcertoCheque + " Order By idCaixaGeral Desc";

            return objPersistence.LoadData(session, sql).ToArray();
        }

        #endregion

        #region Busca movimentações relacionadas a uma obra

        /// <summary>
        /// Retorna movimentações relacionadas a uma obra
        /// </summary>
        /// <param name="idObra"></param>
        /// <returns></returns>
        public CaixaGeral[] GetByObra(uint idObra)
        {
            return GetByObra(null, idObra);
        }

        /// <summary>
        /// Retorna movimentações relacionadas a uma obra
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idObra"></param>
        /// <returns></returns>
        public CaixaGeral[] GetByObra(GDASession sessao, uint idObra)
        {
            string sql = "Select * From caixa_geral where idObra=" + idObra + " And idConta In (" + UtilsPlanoConta.ContasAVista() +
                "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoObraGerado) +
                "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecObraCredito) + ") Order By idCaixaGeral Desc";

            return objPersistence.LoadData(sessao, sql).ToArray();
        }

        #endregion

        #region Busca movimentações relacionadas a uma troca/devolução

        /// <summary>
        /// Retorna movimentações relacionadas a uma troca/devolução
        /// </summary>
        /// <param name="idTrocaDevolucao"></param>
        /// <returns></returns>
        public CaixaGeral[] GetByTrocaDevolucao(uint idTrocaDevolucao)
        {
            return GetByTrocaDevolucao(null, idTrocaDevolucao);
        }

        /// <summary>
        /// Retorna movimentações relacionadas a uma troca/devolução
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idTrocaDevolucao"></param>
        /// <returns></returns>
        public CaixaGeral[] GetByTrocaDevolucao(GDASession session, uint idTrocaDevolucao)
        {
            string sql = "Select * From caixa_geral where idTrocaDevolucao=" + idTrocaDevolucao + " And idConta In (" + UtilsPlanoConta.ContasAVista() +
                "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado) + ") Order By idCaixaGeral Desc";

            return objPersistence.LoadData(session, sql).ToArray();
        }

        #endregion

        #region Busca movimentações relacionadas a uma devolução de pagamento

        /// <summary>
        /// Retorna movimentações relacionadas a uma devolução de pagamento
        /// </summary>
        /// <param name="idDevolucaoPagto"></param>
        /// <returns></returns>
        public CaixaGeral[] GetByDevolucaoPagto(uint idDevolucaoPagto)
        {
            return GetByDevolucaoPagto(null, idDevolucaoPagto);
        }

        /// <summary>
        /// Retorna movimentações relacionadas a uma devolução de pagamento
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idDevolucaoPagto"></param>
        /// <returns></returns>
        public CaixaGeral[] GetByDevolucaoPagto(GDASession session, uint idDevolucaoPagto)
        {
            string sql = "Select * From caixa_geral where idDevolucaoPagto=" + idDevolucaoPagto + " And idConta In (" + UtilsPlanoConta.ContasDevolucaoPagto() +
                "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado) + ") Order By idCaixaGeral Desc";

            return objPersistence.LoadData(session, sql).ToArray();
        }

        #endregion

        #region Busca movimentação anterior

        /// <summary>
        /// Busca movimentação anterior
        /// </summary>
        /// <param name="idCaixaGeral"></param>
        /// <returns></returns>
        public CaixaGeral BuscaMovAnterior(uint idCaixaGeral, uint idLoja)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM caixa_geral
                WHERE idCaixaGeral={0}";

            if (idLoja > 0)
                sql += " AND idLoja=" + idLoja;

            idCaixaGeral = idCaixaGeral - 1;

            while (idCaixaGeral > 0 && objPersistence.ExecuteSqlQueryCount(string.Format(sql, idCaixaGeral)) == 0)
                idCaixaGeral--;

            if (idCaixaGeral == 0)
                return null;

            return GetElementByPrimaryKey(idCaixaGeral);
        }

        /// <summary>
        /// Busca o saldo da movimentação anterior
        /// </summary>
        /// <param name="idCaixaGeral"></param>
        /// <returns></returns>
        public decimal ObtemSaldoMovAnterior(uint idCaixaGeral, uint idLoja)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM caixa_geral
                WHERE idCaixaGeral={0}";

            if (idLoja > 0)
                sql += " AND idLoja=" + idLoja;

            idCaixaGeral = idCaixaGeral - 1;

            while (idCaixaGeral > 0 && objPersistence.ExecuteSqlQueryCount(string.Format(sql, idCaixaGeral)) == 0)
                idCaixaGeral--;

            if (idCaixaGeral == 0)
                return 0;

            return ObtemValorCampo<decimal>("saldo", "idCaixaGeral=" + idCaixaGeral);
        }

        #endregion

        #region Busca crédito utilizado em um pedido

        /// <summary>
        /// Busca crédito utilizado em um pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public decimal GetCreditoPedido(uint idPedido, bool aVista, bool sinal, bool recPrazo)
        {
            string idConta = String.Empty;

            if (aVista)
                idConta += UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EntradaCredito) + ",";

            if (sinal)
                idConta += UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EntradaCredito) + ",";

            if (recPrazo)
                idConta += UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecPrazoCredito) + ",";

            idConta = idConta.Trim(',');

            string sql = "Select Round(Sum(ValorMov), 2) From caixa_geral Where idConta In (" + idConta + ") And idPedido=" + idPedido;

            return ExecuteScalar<decimal>(sql);
        }

        #endregion

        #region Busca crédito utilizado em uma liberação de pedido

        /// <summary>
        /// Busca crédito utilizado em uma liberação de pedido
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public decimal GetCreditoLiberarPedido(uint idLiberarPedido)
        {
            return GetCreditoLiberarPedido(idLiberarPedido, false);
        }

        /// <summary>
        /// Busca crédito utilizado em uma liberação de pedido
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public decimal GetCreditoLiberarPedido(uint idLiberarPedido, bool apenasEntrada)
        {
            string idConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EntradaCredito) + (!apenasEntrada ? "," +
                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecPrazoCredito) + "," + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.VistaCredito) : String.Empty);

            string sql = "Select Round(Sum(ValorMov), 2) From caixa_geral Where idConta In (" + idConta + ") And idLiberarPedido=" + idLiberarPedido;

            return ExecuteScalar<decimal>(sql);
        }

        #endregion

        #region Busca crédito gerado em um pedido

        /// <summary>
        /// Busca crédito gerado em um pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public decimal GetCreditoGeradoPedido(uint idPedido, bool aVista, bool sinal, bool recPrazo)
        {
            string idConta = String.Empty;

            if (aVista)
                idConta += UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado) + ",";

            if (sinal)
                idConta += UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoEntradaGerado) + ",";

            if (recPrazo)
                idConta += UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoRecPrazoGerado) + ",";

            idConta = idConta.Trim(',');

            string sql = "Select Round(Sum(ValorMov), 2) From caixa_geral Where idConta In (" + idConta + ") And idPedido=" + idPedido;

            return ExecuteScalar<decimal>(sql);
        }

        #endregion

        #region Busca crédito gerado em uma liberação de pedido

        /// <summary>
        /// Busca crédito gerado em uma liberação de pedido
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public decimal GetCreditoGeradoLiberarPedido(uint idLiberarPedido)
        {
            string idConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado).ToString();

            string sql = "Select Round(Sum(ValorMov), 2) From caixa_geral Where idConta In (" + idConta + ") And idLiberarPedido=" + idLiberarPedido;

            return ExecuteScalar<decimal>(sql);
        }

        #endregion

        #region Busca crédito utilizado em um pagto

        /// <summary>
        /// Busca crédito utilizado em um pagto
        /// </summary>
        /// <param name="idPagto"></param>
        /// <returns></returns>
        public decimal GetCreditoPagto(uint idPagto)
        {
            string sql = "Select Round(Sum(ValorMov), 2) From caixa_geral Where idConta=" +
                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PagtoCreditoFornecedor) + " And idPagto=" + idPagto;

            return ExecuteScalar<decimal>(sql);
        }

        #endregion

        #region Busca crédito gerado em um pagto

        /// <summary>
        /// Busca crédito gerado em um pagto
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public Single GetCreditoGeradoPagto(uint idPagto)
        {
            string sql = "Select Coalesce(Round(Sum(ValorMov), 2), 0) From caixa_geral Where idConta=" +
                (uint)UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoCompraGerado) + " And idPagto=" + idPagto;

            return ExecuteScalar<float>(sql);
        }

        #endregion

        #region Verifica se a conta passada foi paga

        /// <summary>
        /// Verifica se a conta passada foi paga no caixa geral
        /// </summary>
        /// <param name="idContaPg"></param>
        /// <returns></returns>
        public bool ExisteContaPaga(uint idContaPg)
        {
            string sql = "Select Count(*) From caixa_geral Where TipoMov=2 And IdContaPg=" + idContaPg;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Verifica se a conta paga passada já foi estornada

        /// <summary>
        /// Verifica se a conta passada foi paga no caixa geral
        /// </summary>
        /// <param name="idContaPg"></param>
        /// <returns></returns>
        public bool ExisteEstornoConta(uint idContaPg)
        {
            string sql = "Select Count(*) From caixa_geral Where TipoMov=1 And IdContaPg=" + idContaPg;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Exclui movimentações por PKs

        /// <summary>
        /// Método que deleta movimentações com base em uma listagem de Identificadores de movimentações.
        /// </summary>
        /// <param name="sessao">Sessão do GDA.</param>
        /// <param name="idsCaixaDiario">Listagem de Identificadores de movimentações.</param>
        public void DeleteByPKs(GDASession sessao, List<int> idsCaixaDiario)
        {
            if (!idsCaixaDiario.Any(c => c > 0))
            {
                return;
            }

            string sql = $"DELETE FROM caixa_geral WHERE IdCaixaGeral IN ({string.Join(",", idsCaixaDiario)})";

            this.objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Recupera as movimentações por cliente

        /// <summary>
        /// Retorna as movimentações por cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="planosConta">Os planos de conta usados como filtro. Pode ser omitido para não filtrar pelos planos de conta.</param>
        /// <returns></returns>
        public CaixaGeral[] GetByCliente(uint idCliente, DateTime inicio, DateTime fim, params uint[] planosConta)
        {
            string sql = @"select * from caixa_geral where idCliente=?idCli and DataCad>=?inicio and DataCad<=?fim";

            if (planosConta.Length > 0)
            {
                sql += " and idConta in (";

                foreach (uint p in planosConta)
                    sql += "," + p;

                sql = sql.Replace("(,", "(") + ")";
            }

            return objPersistence.LoadData(sql + " order by DataCad desc", new GDAParameter("?idCli", idCliente),
                new GDAParameter("?inicio", DateTime.Parse(inicio.ToShortDateString() + " 00:00")), new GDAParameter("?fim", DateTime.Parse(fim.ToShortDateString() + " 23:59"))).ToArray();
        }

        #endregion

        #region Recupera as movimentações por fornecedor

        /// <summary>
        /// Retorna as movimentações por fornecedor.
        /// </summary>
        /// <param name="idFornec"></param>
        /// <param name="planosConta">Os planos de conta usados como filtro. Pode ser omitido para não filtrar pelos planos de conta.</param>
        /// <returns></returns>
        public CaixaGeral[] GetByFornecedor(uint idFornec, DateTime inicio, DateTime fim, params uint[] planosConta)
        {
            string sql = @"select * from caixa_geral where idFornec=?idFornec and DataCad>=?inicio and DataCad <=?fim";

            if (planosConta.Length > 0)
            {
                sql += " and idConta in (";

                foreach (uint p in planosConta)
                    sql += "," + p;

                sql = sql.Replace("(,", "(") + ")";
            }

            return objPersistence.LoadData(sql + " order by DataCad desc", new GDAParameter("?idFornec", idFornec),
                new GDAParameter("?inicio", DateTime.Parse(inicio.ToShortDateString() + " 00:00")), new GDAParameter("?fim", DateTime.Parse(fim.ToShortDateString() + " 23:59"))).ToArray();
        }

        #endregion

        #region Existe movimentação por pedido?

        /// <summary>
        /// Existe movimentação por pedido?
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool ExistsByPedido(uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount("select count(*) from caixa_geral where idPedido=" + idPedido +
                " and idConta not in (" + UtilsPlanoConta.ListaEstornosAPrazo() + "," + UtilsPlanoConta.ListaEstornosAVista() + "," +
                UtilsPlanoConta.ListaEstornosSinalPedido() + ")") > 0;
        }

        #endregion

        #region Retirada caixa geral

        public void RetirarValorCaixaGeral(int? idFornec, int idConta, int? idCheque, decimal valor, int formaSaida, string obs)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    // Busca o saldo do caixa geral da forma de pagamento selecionada
                    var saldo = GetSaldoByFormaPagto(transaction, formaSaida == 1 ? Pagto.FormaPagto.Dinheiro : Pagto.FormaPagto.ChequeProprio, 0, null, null, 1, 0);

                    // Verifica se o caixa possui saldo para realizar esta retirada
                    if (saldo - valor < 0 && formaSaida == (int)CaixaGeral.FormaSaidaEnum.Dinheiro)
                        throw new Exception(string.Format("Não há saldo suficiente em {0} para realizar esta retirada.", formaSaida == 1 ? "dinheiro" : "cheque"));

                    MovCxDebito(transaction, (uint?)idFornec, formaSaida == 2 ? (uint?)idCheque : null, (uint)idConta, formaSaida, valor, obs);

                    if (idCheque > 0)
                        ChequesDAO.Instance.UpdateSituacao(null, (uint)idCheque, Cheques.SituacaoCheque.Compensado);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        #endregion

        #region Cancela movimentação

        /// <summary>
        /// Cancela movimentação.
        /// </summary>
        /// <param name="mov"></param>
        public void CancelaMovimentacao(CaixaGeral mov, string motivo)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    if (!mov.LancManual ||
                        mov.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioDinheiro) ||
                        mov.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfDeCxDiarioCheque))
                        throw new Exception("Essa movimentação foi gerada pelo sistema. Só é possível cancelar movimentações manuais.");

                    // Atualiza o saldo
                    objPersistence.ExecuteCommand(transaction, string.Format("UPDATE caixa_geral SET Saldo=Saldo {0} ?valor WHERE IdCaixaGeral>{1}",
                        mov.TipoMov == 1 ? "-" : "+", mov.IdCaixaGeral), new GDAParameter("?valor", mov.ValorMov));

                    if (mov.IdCheque > 0)
                        ChequesDAO.Instance.UpdateSituacao(transaction, mov.IdCheque.Value, Cheques.SituacaoCheque.EmAberto);

                    DeleteByPrimaryKey(transaction, mov.IdCaixaGeral);
                    LogCancelamentoDAO.Instance.LogCaixaGeral(transaction, mov, motivo, true);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        #endregion

        #region Busca movimentações por data

        /// <summary>
        /// Obtém as movimentações do caixa geral pela sua data de cadastro.
        /// Método utilizado principalmente para ajustar a tabela pagto_contas_receber.
        /// </summary>
        public IList<CaixaGeral> ObterMovimentacoesPorData(string dataInicio, string dataFim)
        {
            var parametros = new List<GDAParameter>();

            var sql = "SELECT * FROM caixa_geral cg WHERE 1 ";

            if (!string.IsNullOrEmpty(dataInicio))
            {
                sql += " AND cg.DataCad >= ?dataIni";
                parametros.Add(new GDAParameter("?dataIni", DateTime.Parse(dataInicio + " 00:00:00")));
            }

            if (!string.IsNullOrEmpty(dataFim))
            {
                sql += " AND cg.DataCad <= ?dataFim";
                parametros.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));
            }

            return objPersistence.LoadData(sql, parametros.ToArray()).ToList();
        }

        #endregion

        #region Transferência para conta bancária

        /// <summary>
        /// Cancela movimentação.
        /// </summary>
        public void TransferenciaParaContaBancaria(int idContaBanco, decimal valorMov, DateTime dataMov, string obs)
        {
            using (var transaction = new GDA.GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    Instance.MovCxContaBanco(transaction, (uint)idContaBanco,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfCxGeralParaContaBancariaDinheiro),
                        2, valorMov, 0, obs, true, null);

                    // Credita valor da conta bancária
                    ContaBancoDAO.Instance.MovContaSaida(transaction, (uint)idContaBanco,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfCxGeralParaContaBancariaDinheiro), (int)UserInfo.GetUserInfo.IdLoja,
                        1, valorMov, dataMov, obs);

                    if (transaction != null)
                    {
                        transaction.Commit();
                        transaction.Close();
                    }
                }
                catch
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                        transaction.Close();
                    }


                    throw;
                }
            }
        }

        #endregion

        /// <summary>
        /// recupera movimentações a partir do cartão não identificado
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCartaoNaoIdentificado"></param>
        /// <returns></returns>
        public IList<CaixaGeral> ObterMovimentacoesPorCartaoNaoIdentificado(GDASession sessao, int idCartaoNaoIdentificado)
        {
            string sql = @"Select * From caixa_geral where idPedido=null and idLiberarPedido=null and idAcerto=null and idContaR=null and idObra=null and idSinal=null and
                              idTrocaDevolucao=null and idDevolucaoPagto=null and idAcertoCheque=null and IdCartaoNaoIdentificado=" + idCartaoNaoIdentificado;

            return objPersistence.LoadData(sessao, sql).ToList();
        }
    }
}
