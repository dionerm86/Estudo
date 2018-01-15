using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class FluxoCaixaDAO : BaseDAO<FluxoCaixa, FluxoCaixaDAO>
    {
        //private FluxoCaixaDAO() { }

        private const string TIPO_CONTA_PADRAO = "0,1";

        #region Saldo inicial

        private class SaldoInicial
        {
            public SaldoInicial(DateTime dataSaldoInicial, IEnumerable<FluxoCaixa> movimentacoes)
            {
                DataSaldoInicial = dataSaldoInicial;
                Movimentacoes = movimentacoes;
                Saldo = movimentacoes.OrderByDescending(x => x.NumSeqMov).Select(x => x.SaldoGeral).FirstOrDefault();
                Count = (uint)movimentacoes.Count();
            }

            public DateTime DataSaldoInicial { get; private set; }
            public decimal Saldo { get; private set; }
            public IEnumerable<FluxoCaixa> Movimentacoes { get; private set; }
            public uint Count { get; private set; }
        }

        private decimal SomarSaldoInicial(string sql, string dataFim)
        {
            return ExecuteScalar<decimal>("select sum(if(tipoMov=1, valor, -valor)) from (" +
                sql + ") as temp", new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));
        }

        private SaldoInicial GetSaldoInicial(string dataIni)
        {
            if (String.IsNullOrEmpty(dataIni))
                throw new ArgumentNullException("dataIni");

            DateTime dataSaldoAnterior = Conversoes.ConverteDataNotNull(dataIni).Date.AddDays(-1);
            string dataSaldoAnt = dataSaldoAnterior.ToString("dd/MM/yyyy");

            List<FluxoCaixa> retorno = new List<FluxoCaixa>();

            decimal saldoGeral = 0;
            uint numSeq = 0;

            #region Mov. Banco

            foreach (ContaBanco c in ContaBancoDAO.Instance.GetOrdered(dataSaldoAnt))
            {
                FluxoCaixa banco = new FluxoCaixa()
                {
                    IsTotal = true,
                    Descricao = c.Descricao,
                    NumSeqMov = ++numSeq
                };

                decimal valorBanco = c.SaldoSemCheques;

                banco.TipoMov = valorBanco >= 0 ? 1 : 2;
                banco.Valor = Math.Abs(valorBanco);
                banco.SaldoGeral = (saldoGeral += valorBanco);
                retorno.Add(banco);
            }

            #endregion

            #region Caixa Geral

            FluxoCaixa cxGeral = new FluxoCaixa()
            {
                IsTotal = true,
                Descricao = "SALDO CAIXA GERAL (DINHEIRO)",
                NumSeqMov = ++numSeq
            };

            decimal valorCaixaGeral = SomarSaldoInicial(SqlCaixaGeral(false, true, TIPO_CONTA_PADRAO), dataSaldoAnt);

            cxGeral.TipoMov = valorCaixaGeral >= 0 ? 1 : 2;
            cxGeral.Valor = Math.Abs(valorCaixaGeral);
            cxGeral.SaldoGeral = (saldoGeral += valorCaixaGeral);
            retorno.Add(cxGeral);

            #endregion

            #region Cheques de terceiros

            FluxoCaixa chequesTerc = new FluxoCaixa()
            {
                IsTotal = true,
                Descricao = "SALDO CHEQUES TERCEIROS EM ABERTO",
                Valor = SomarSaldoInicial(SqlChequesTerc(false, true, TIPO_CONTA_PADRAO), dataSaldoAnt),
                TipoMov = 1,
                NumSeqMov = ++numSeq
            };

            chequesTerc.SaldoGeral = (saldoGeral += chequesTerc.Valor);
            retorno.Add(chequesTerc);

            #endregion

            #region Contas a receber

            FluxoCaixa contasReceber = new FluxoCaixa()
            {
                IsTotal = true,
                Descricao = "SALDO CONTAS A RECEBER EM ABERTO",
                Valor = SomarSaldoInicial(SqlContasAReceber(false, true, TIPO_CONTA_PADRAO), dataSaldoAnt),
                TipoMov = 1,
                NumSeqMov = ++numSeq
            };

            contasReceber.SaldoGeral = (saldoGeral += contasReceber.Valor);
            retorno.Add(contasReceber);

            #endregion

            #region Cheques próprios

            FluxoCaixa chequesProp = new FluxoCaixa()
            {
                IsTotal = true,
                Descricao = "SALDO CHEQUES PRÓPRIOS EM ABERTO",
                Valor = Math.Abs(SomarSaldoInicial(SqlChequesProp(false, true, TIPO_CONTA_PADRAO), dataSaldoAnt)),
                TipoMov = 2,
                NumSeqMov = ++numSeq
            };

            chequesProp.SaldoGeral = (saldoGeral -= chequesProp.Valor);
            retorno.Add(chequesProp);

            #endregion

            #region Contas a pagar

            FluxoCaixa contasPagar = new FluxoCaixa()
            {
                IsTotal = true,
                Descricao = "SALDO CONTAS A PAGAR EM ABERTO",
                Valor = Math.Abs(SomarSaldoInicial(SqlContasAPagar(false, true, TIPO_CONTA_PADRAO), dataSaldoAnt)),
                TipoMov = 2,
                NumSeqMov = ++numSeq
            };

            contasPagar.SaldoGeral = (saldoGeral -= contasPagar.Valor);
            retorno.Add(contasPagar);

            #endregion

            #region Parcelas de cartão

            FluxoCaixa parcCartao = new FluxoCaixa()
            {
                IsTotal = true,
                Descricao = "SALDO PARCELAS CARTÃO EM ABERTO",
                Valor = SomarSaldoInicial(SqlParcCartao(false, true, TIPO_CONTA_PADRAO), dataSaldoAnt),
                TipoMov = 1,
                NumSeqMov = ++numSeq
            };

            parcCartao.SaldoGeral = (saldoGeral += parcCartao.Valor);
            retorno.Add(parcCartao);

            #endregion

            return new SaldoInicial(dataSaldoAnterior, retorno);
        }

        #endregion

        #region SQL

        internal string FiltroTipoConta(string alias, string campoContabil, string tipoConta)
        {
            bool semRef = String.IsNullOrEmpty(alias) || String.IsNullOrEmpty(campoContabil);

            return tipoConta == TIPO_CONTA_PADRAO || (tipoConta == "0" && semRef) ? "" :
                String.IsNullOrEmpty(tipoConta) || (tipoConta == "1" && semRef) ? " and false" :
                String.Format(" and {0}.{1}=", alias, campoContabil) + ("," + tipoConta + ",").Contains("1").ToString().ToLower();
        }

        private string SqlMovBanco(bool incluirDataIni, bool incluirDataFim, string tipoConta)
        {
            string sql = @"
                Select mb.dataMov as Data, mb.idConta, Concat('Mov. Banco ', cb.nome, ' Agência: ', cb.agencia, ' Conta: ', 
                    cb.conta, ' - ', pl.Descricao, ' ', Coalesce(Convert(mb.Obs Using utf8), '')) as descricao,
                    Convert(coalesce(" + ClienteDAO.Instance.GetNomeCliente("cli") + ", " + FornecedorDAO.Instance.GetNomeFornecedor("forn") + @") using utf8) as Parceiro, 
                    mb.valorMov as Valor, mb.TipoMov, 0 as prevCustoFixo
                From mov_banco mb
                    Left Join plano_contas pl On (mb.idConta=pl.IdConta) 
                    Left Join cliente cli On (mb.idCliente=cli.id_Cli)
                    Left Join fornecedor forn On (mb.idFornec=forn.idFornec)
                    Left Join conta_banco cb On (mb.idContaBanco=cb.idContaBanco)
                Where 1" + FiltroTipoConta(null, null, tipoConta);

            if (incluirDataIni)
                sql += " and mb.dataMov>=?dataIni";

            if (incluirDataFim)
                sql += " and mb.dataMov<=?dataFim";

            return sql;
        }

        private string SqlCaixaGeral(bool incluirDataIni, bool incluirDataFim, string tipoConta)
        {
            string sqlBase = @"
                Select cg.dataMov as Data, cg.idConta, Concat(pl.Descricao, ' ', Coalesce(Convert(cg.Obs Using utf8), '')) as descricao, 
                    Convert(coalesce(" + ClienteDAO.Instance.GetNomeCliente("cli") + ", " + FornecedorDAO.Instance.GetNomeFornecedor("forn") + @") using utf8) as Parceiro, 
                    cg.valorMov as Valor, cg.TipoMov, 0 as prevCustoFixo
                From caixa_geral cg
                    Left Join plano_contas pl On (cg.idConta=pl.IdConta) 
                    Left Join cliente cli On (cg.idCliente=cli.id_Cli)
                    Left Join fornecedor forn On (cg.idFornec=forn.idFornec)
                Where (cg.idConta in ({0}) {1}) and cg.tipoMov={2}" + FiltroTipoConta(null, null, tipoConta);

            if (incluirDataIni)
                sqlBase += " and cg.dataMov>=?dataIni";

            if (incluirDataFim)
                sqlBase += " and cg.dataMov<=?dataFim";

            string idsContasEstorno = UtilsPlanoConta.GetLstEstornoSaidaByFormaPagto(Glass.Data.Model.Pagto.FormaPagto.Dinheiro, 0);
            
            string sql = String.Format(sqlBase,
                UtilsPlanoConta.GetLstEntradaByFormaPagto(Glass.Data.Model.Pagto.FormaPagto.Dinheiro, 0, false) + "," + idsContasEstorno,
                "Or cg.formaSaida=" + (int)Glass.Data.Model.Pagto.FormaPagto.Dinheiro, 1) + " union all " +
                String.Format(sqlBase, UtilsPlanoConta.GetLstSaidaByFormaPagto(Glass.Data.Model.Pagto.FormaPagto.Dinheiro, 0, 1), 
                "Or cg.formaSaida=" + (int)Glass.Data.Model.Pagto.FormaPagto.Dinheiro, 2);
                        
            //if (!String.IsNullOrEmpty(idsContasEstorno)) 
                //sql += " union all " + String.Format(sqlBase, idsContasEstorno, String.Empty, 1);

            return sql;
        }

        private string SqlChequesTerc(bool incluirDataIni, bool incluirDataFim, string tipoConta)
        {
            string sql = @"
                Select ct.dataVenc as Data, null as idConta, 'Rec. Cheque Terc.' as Descricao, Convert(ct.titular using utf8) as Parceiro, Valor, 1 as TipoMov, 
                    0 as prevCustoFixo 
                From cheques ct
                Where ct.tipo=2 And ct.situacao=" + (int)Cheques.SituacaoCheque.EmAberto + FiltroTipoConta(null, null, tipoConta);

            if (incluirDataIni)
                sql += " and ct.dataVenc>=?dataIni";

            if (incluirDataFim)
                sql += " and ct.dataVenc<=?dataFim";

            return sql;
        }

        private string SqlContasAReceber(bool incluirDataIni, bool incluirDataFim, string tipoConta)
        {
            string sql = @"
                Select cr.dataVec as Data, cr.idConta, Concat(pl.Descricao, ' ', Coalesce(Convert(cr.Obs Using utf8), '')) as descricao, 
                    Convert(cli.Nome using utf8) as Parceiro, cr.valorVec as Valor, 1 as TipoMov, 0 as prevCustoFixo 
                From contas_receber cr 
                    Left Join cliente cli On (cr.IdCliente=cli.id_Cli) 
                    Left Join plano_contas pl On (cr.IdConta=pl.IdConta) 
                Where idAntecipContaRec is null And (cr.recebida=0 Or cr.recebida is null) and (isParcelaCartao is null or isParcelaCartao=false)" + 
                FiltroTipoConta(null, null, tipoConta);

            if (incluirDataIni)
                sql += " and cr.dataVec>=?dataIni";

            if (incluirDataFim)
                sql += " and cr.dataVec<=?dataFim";

            return sql;
        }

        private string SqlChequesProp(bool incluirDataIni, bool incluirDataFim, string tipoConta)
        {
            string sql = @"
                Select cp.dataVenc as Data, null as idConta, 'Pagto. Cheque Próprio' as Descricao, Convert(f.nomeFantasia using utf8) as Parceiro, 
                    cp.Valor, 2 as TipoMov, 0 as prevCustoFixo
                From cheques cp 
                    Left Join pagto_cheque pc On (cp.idCheque=pc.idCheque) Left Join pagto pag On (pc.idPagto=pag.idPagto)
                    Left Join fornecedor f On (pag.idFornec=f.idFornec) 
                Where cp.tipo=1 And cp.situacao=" + (int)Cheques.SituacaoCheque.EmAberto + FiltroTipoConta(null, null, tipoConta);

            if (incluirDataIni)
                sql += " and cp.dataVenc>=?dataIni";

            if (incluirDataFim)
                sql += " and cp.dataVenc<=?dataFim";

            return sql;
        }

        private string SqlContasAPagar(bool incluirDataIni, bool incluirDataFim, string tipoConta)
        {
            string sql = @"
                Select cpg.dataVenc as Data, cpg.idConta, Concat(pl.Descricao, ' ', Coalesce(Convert(cpg.Obs Using utf8), '')) as descricao, 
                    Convert(" + FornecedorDAO.Instance.GetNomeFornecedor("f") + @" using utf8) as Parceiro, (cpg.ValorVenc) as Valor, 2 as TipoMov, 0 as prevCustoFixo
                From contas_pagar cpg
                    Left Join fornecedor f On (cpg.idFornec=f.idFornec)
                    Left Join plano_contas pl On (cpg.IdConta=pl.IdConta)
                Where cpg.paga=0 /*And pl.idGrupo not in (" + UtilsPlanoConta.GetGruposExcluirFluxoSistema + ")*/" + FiltroTipoConta("cpg", "contabil", tipoConta);

            if (incluirDataIni)
                sql += " and cpg.dataVenc>=?dataIni";

            if (incluirDataFim)
                sql += " and cpg.dataVenc<=?dataFim";

            return sql;
        }

        private string SqlParcCartao(bool incluirDataIni, bool incluirDataFim, string tipoConta)
        {
            string sql = @"
                Select cr.dataVec as Data, cr.idConta, Concat('Parcela de cartão de crédito ', Coalesce(Convert(cr.Obs Using utf8), '')) as descricao,
                    Convert(cli.Nome using utf8) as Parceiro, cr.valorVec as Valor, 1 as TipoMov, 0 as prevCustoFixo
                From contas_receber cr 
                    Left Join pedido p On (cr.IdPedido=p.idPedido) Left Join cliente cli On (cr.IdCliente=cli.id_Cli) 
                    Left Join plano_contas pl On (cr.IdConta=pl.IdConta) 
                Where idAntecipContaRec is null And (cr.recebida=0 Or cr.recebida is null) and isParcelaCartao=true
                    And pl.idGrupo not in (" + UtilsPlanoConta.GetGruposExcluirFluxoSistema + ")" + FiltroTipoConta(null, null, tipoConta);

            if (incluirDataIni)
                sql += " and cr.dataVec>=?dataIni";

            if (incluirDataFim)
                sql += " and cr.dataVec<=?dataFim";

            return sql;
        }

        internal string Sql(string paramDataIni, string paramDataFim, string tipoConta)
        {
            string sql = @"
                Select Data, cast(IdConta as signed) as idConta, Descricao, Parceiro, Valor, TipoMov, PrevCustoFixo
                From (
                    " + SqlMovBanco(true, true, tipoConta) + @"
                    union all " + SqlCaixaGeral(true, true, tipoConta) + @"
                    union all " + SqlChequesTerc(true, true, tipoConta) + @"
                    union all " + SqlContasAReceber(true, true, tipoConta) + @"
                    union all " + SqlChequesProp(true, true, tipoConta) + @"
                    union all " + SqlContasAPagar(true, true, tipoConta) + @"
                    union all " + SqlParcCartao(true, true, tipoConta) + @"
                ) as tbl Order By DATE_FORMAT(Data, '%Y-%m-%d') Asc";

            return sql.Replace("?dataIni", paramDataIni).Replace("?dataFim", paramDataFim);
        }

        #endregion

        public FluxoCaixa[] GetForRpt(string dataIni, string dataFim, bool prevCustoFixo, string tipoConta)
        {
            SaldoInicial saldoInicial = null;

            if (String.IsNullOrEmpty(dataFim) || String.IsNullOrEmpty(dataIni))
                return new FluxoCaixa[0];

            GDAParameter[] param = new GDAParameter[] { 
                new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")), 
                new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")) 
            };

            var sql = Sql("?dataIni", "?dataFim", tipoConta);

            var lstFluxoCaixa = objPersistence.LoadData(sql, param).ToList();

            #region Busca os custos fixos se previsão estiver marcada

            if (prevCustoFixo)
            {
                // Data Inicial/Final para a previsão de custos fixos
                DateTime dataIniPrev = DateTime.Parse(dataIni);
                DateTime dataFimPrev = DateTime.Parse(dataFim);

                // Pega a quantidade de meses que serão buscados os custos fixos
                int qtdMeses = (int)FuncoesData.DateDiff(DateInterval.Month, dataIniPrev, dataFimPrev) + 1;
                
                // Contador do mês que serão buscados os custos fixos
                int contMesAtual = 1;

                // Datas utilizadas como parâmetros para a busca de custos fixos
                DateTime dataIniParam = DateTime.Parse(dataIniPrev.ToString("dd/MM/yyyy 00:00"));
                DateTime dataFimParam = DateTime.Parse(dataIniParam.AddMonths(1).ToString("dd/MM/yyyy 00:00"));

                while (contMesAtual <= qtdMeses)
                {
                    // Busca todos os custos fixos ativos do mês que não foram gerados ainda
                    sql = @"Select Str_To_Date(Concat(Least(cf.DiaVenc, Day(Last_Day(Str_To_Date(Concat('01,', Month(?dataIni), ',', Year(?dataIni)), '%d,%m,%Y')))),
                        ',', Month(?dataIni), ',', Year(?dataIni)), '%d,%m,%Y') as Data, cast(cf.idConta as signed) as idConta, 
                        Concat(g.Descricao, ' - ', p.Descricao) as Descricao, Coalesce(f.NomeFantasia, f.RazaoSocial, '') as Parceiro, 
                        cf.valorVenc as Valor, 2 as TipoMov, 1 as prevCustoFixo From custo_fixo cf Left Join fornecedor f On (cf.IdFornec=f.IdFornec) 
                        Left Join plano_contas p On (cf.IdConta=p.IdConta) Left Join grupo_conta g On (p.IdGrupo=g.IdGrupo)
                        Where cf.situacao=" + (int)CustoFixo.SituacaoEnum.Ativo + FiltroTipoConta("cf", "contabil", tipoConta) + @" And cf.idCustoFixo Not In 
                        (select * from (Select Coalesce(cp.idCustoFixo, 0) From contas_pagar cp Where cp.dataVenc>=?dataIni And cp.dataVenc<?dataFim) as temp)";

                    // Se for o primeiro mês, busca custos fixos a partir do dia especificado pelo usuário em dataIni
                    if (contMesAtual == 1)
                        sql += " And cf.diaVenc>=" + dataIniPrev.Day;

                    // Se for o primeiro mês, busca custos fixos até o dia especificado pelo usuário em dataFim
                    if (contMesAtual == qtdMeses)
                        sql += " And cf.diaVenc<=" + dataFimPrev.Day;

                    // Se a dataFimParam for maior que o filtro de dataFim aplicado pelo usuário, utiliza a dataFim aplicada pelo usuário
                    dataFimParam = dataFimParam > dataFimPrev ? dataFimPrev : dataFimParam;

                    lstFluxoCaixa.AddRange(objPersistence.LoadData(sql, new GDAParameter[] { new GDAParameter("?dataIni", dataIniParam), new GDAParameter("?dataFim", dataFimParam) }));

                    // Incrementa em 1 mês a data de verificação de custos fixos
                    dataIniParam = dataIniParam.AddMonths(1);
                    dataFimParam = dataIniParam.AddMonths(1);
                    contMesAtual++;
                }

                // Ordena a lista pelo nome do grupo e pelo nome dos itens
                lstFluxoCaixa.Sort(new Comparison<FluxoCaixa>(delegate(FluxoCaixa x, FluxoCaixa y)
                {
                    int sortOrder = DateTime.Compare(x.Data, y.Data);
                    return sortOrder != 0 ? sortOrder : DateTime.Compare(x.Data, y.Data);
                }));
            }

            #endregion

            if (lstFluxoCaixa.Count > 0)
            {
                DateTime currDate = lstFluxoCaixa[0].Data.Date;
                decimal saldoDiario = 0;

                saldoInicial = GetSaldoInicial(!String.IsNullOrEmpty(dataIni) ? dataIni : 
                    lstFluxoCaixa.Min(x => x.Data).ToString("dd/MM/yyyy"));

                decimal valorSaldoInicial = saldoInicial.Saldo;

                uint numSeq = saldoInicial.Count;
                lstFluxoCaixa.ForEach(x => x.NumSeqMov = ++numSeq);

                // Obtém os saldos diário e geral
                foreach (FluxoCaixa fc in lstFluxoCaixa)
                {
                    if (currDate != fc.Data.Date)
                    {
                        saldoDiario = 0;
                        currDate = fc.Data.Date;
                    }

                    decimal valorParaSaldo = fc.TipoMov == 1 ? fc.Valor : -fc.Valor;

                    // Calcula o saldo diário
                    fc.SaldoDoDia = (saldoDiario += valorParaSaldo);

                    // Calcula o saldo geral
                    fc.SaldoGeral = (valorSaldoInicial += valorParaSaldo);
                }

                // Insere as movimentações de saldo inicial no início da lista
                lstFluxoCaixa.InsertRange(0, saldoInicial.Movimentacoes.OrderBy(x => x.NumSeqMov));

                lstFluxoCaixa[0].Criterio = "Período: " + dataIni + " até " + dataFim;
            }

            return lstFluxoCaixa.ToArray();
        }

        public FluxoCaixa[] GetList(string dataIni, string dataFim, bool prevCustoFixo, string tipoConta)
        {
            FluxoCaixa[] itens = GetForRpt(dataIni, dataFim, prevCustoFixo, tipoConta);

            List<FluxoCaixa> retorno = new List<FluxoCaixa>();

            for (int i = 0; i < itens.Length; i++)
            {
                retorno.Add(itens[i]);

                if (!itens[i].IsTotal && (i == itens.Length - 1 || itens[i].Data.Date != itens[i + 1].Data.Date))
                {
                    FluxoCaixa saldoDia = new FluxoCaixa();
                    saldoDia.IsTotal = true;
                    saldoDia.Descricao = "SALDO DO DIA: " + itens[i].SaldoDoDia.ToString("C");
                    saldoDia.NaoExibirSintetico = true;

                    retorno.Add(saldoDia);
                }
            }

            return retorno.ToArray();
        }
    }
}