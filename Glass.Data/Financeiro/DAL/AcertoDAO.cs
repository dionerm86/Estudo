using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class AcertoDAO : BaseDAO<Acerto, AcertoDAO>
    {
        //private AcertoDAO() { }

        #region Busca acertos do cliente

        private string SqlList(int numAcerto, uint idPedido, uint idLiberarPedido, uint idCli, string dataIni, string dataFim,
            uint idFormaPagto, uint idTipoBoleto, int numNotaFiscal, int protestadas, bool selecionar, out bool temFiltro)
        {
            temFiltro = false;
            string criterio = String.Empty;

            string campos = selecionar ? @"a.*, (select cast(sum(valorPagto) as decimal(12,2)) from pagto_acerto where idAcerto=a.idAcerto) as TotalAcerto, 
                cli.Nome as NomeCliente, f.Nome as Funcionario, '$$$' as criterio" : "count(distinct a.idAcerto)";

            string sql = @"
                Select " + campos + @" From acerto a
                    LEFT JOIN contas_receber cr ON (a.IdAcerto=cr.IdAcerto)
                    Left Join cliente cli on (a.Id_Cli=cli.Id_Cli) 
                    Left Join funcionario f On (a.UsuCad=f.IdFunc)
                    LEFT JOIN pagto_acerto pa ON (a.IdAcerto=pa.IdAcerto)
                Where 1";

            if (numAcerto > 0)
            {
                sql += " And a.IdAcerto=" + numAcerto;
                criterio += "Acerto: " + numAcerto + "    ";
                temFiltro = true;
            }

            if (idPedido > 0)
            {
                if (!PedidoConfig.LiberarPedido)
                    sql += " and a.idAcerto in (select idAcerto from contas_receber where idPedido=" + idPedido + ")";
                else
                    sql += @" and a.idAcerto in (select idAcerto from contas_receber where idLiberarPedido In 
                        (Select idLiberarPedido From produtos_liberar_pedido Where idPedido=" + idPedido + "))";

                criterio += "Pedido: " + idPedido + "    ";
                temFiltro = true;
            }

            if (idLiberarPedido > 0)
            {
                sql += " and a.idAcerto in (select idAcerto from contas_receber where idLiberarPedido=" + idLiberarPedido + ")";
                criterio += "Liberação: " + idLiberarPedido + "    ";
                temFiltro = true;
            }

            if (idCli > 0)
            {
                sql += " And a.Id_Cli=" + idCli;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCli) + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " And a.DataCad>=?dataIni";
                criterio += "Data Início: " + dataIni + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " And a.DataCad<=?dataFim";
                criterio += "Data Fim: " + dataFim + "    ";
                temFiltro = true;
            }

            if (idFormaPagto > 0)
            {
                /*if (idFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto)
                {
                    if (idTipoBoleto > 0)
                        sql += " And a.idAcerto In (Select idAcerto from contas_receber Where idConta=" + UtilsPlanoConta.GetPlanoRecebTipoBoleto(idTipoBoleto) + ")";
                    else
                        sql += " And a.idAcerto In (Select idAcerto from contas_receber Where idConta In (" + UtilsPlanoConta.ContasTodosTiposBoleto() + "))";
                }
                else
                    sql += " And a.idAcerto In (Select idAcerto from contas_receber Where idConta in (" + UtilsPlanoConta.ContasTodasPorTipo((Glass.Data.Model.Pagto.FormaPagto)idFormaPagto) + "))";*/
                /* Chamado 17897. */
                sql += " AND pa.IdFormaPagto=" + idFormaPagto;

                criterio += "Forma Pagto: " + FormaPagtoDAO.Instance.GetDescricao(idFormaPagto) + "    ";
                temFiltro = true;
            }

            if (numNotaFiscal > 0)
            {
                sql += @" AND EXISTS (
                                        SELECT nf.IdNf
                                        FROM nota_fiscal nf
                                            LEFT JOIN pedidos_nota_fiscal pnf ON(nf.IdNf = pnf.IdNf)
                                            LEFT JOIN liberarpedido lp ON(pnf.IdLiberarPedido = lp.IdLiberarPedido)
                                            LEFT JOIN contas_receber cr ON(lp.IdLiberarPedido = cr.IdLiberarPedido)
                                        WHERE cr.IdAcerto = a.IdAcerto AND nf.NumeroNfe = " + numNotaFiscal + @"
                                    )";

                criterio += "Nota Fiscal: " + numNotaFiscal + "    ";
                temFiltro = true;
            }

            if (protestadas == 1)
            {
                sql += " AND (COALESCE(cr.Juridico, 0) = 1)";
                criterio += "Somente contas em jurídico/cartório  ";
                temFiltro = true;
            }
            else if (protestadas == 2)
            {
                sql += " AND (COALESCE(cr.Juridico, 0) = 0)";
                criterio += "Não incluir contas em jurídico/cartório  ";
                temFiltro = true;
            }

            sql += " GROUP BY a.IdAcerto";

            return sql.Replace("$$$", criterio);
        }

        private void PreencheIdRefJuros(ref IList<Acerto> acertos)
        {
            PreencheIdRefJuros(null, ref acertos);
        }

        private void PreencheIdRefJuros(GDASession session, ref IList<Acerto> acertos)
        {
            if (acertos.Count == 0)
                return;

            // Chamado 18545: É necessário ter um "limit 1" para que caso o acerto tenha sido pago com mais de uma forma de pagto, busque apenas a primeira.
            /* Chamado 24598.
             * O limit faz com que o valor de juros fique incorreto caso seja utilizada mais de uma forma de pagamento, pois, este valor é dividido
             * igualitariamente para cada forma de pagamento. Ou seja, devem ser somados o valor de juros de todas movimentações de entrada do acerto. */
            /* Chamado 25489.
             * O correto é buscar o valor a movimentação de juros, com base no id da conta da movimentação. */
            var sql = @"
                SELECT CAST(CONCAT(IdAcerto, ';', COALESCE(SUM(Juros), 0)) AS CHAR) AS Dados
                FROM (
                    SELECT cd.IdAcerto, cd.Valor AS Juros
                    FROM caixa_diario cd
                    WHERE cd.IdAcerto IN ({0}) AND cd.IdConta IN ({1},{2},{3})
 
                    UNION ALL SELECT cg.IdAcerto, cg.ValorMov AS Juros
                    FROM caixa_geral cg
                    WHERE cg.IdAcerto IN ({0}) AND cg.IdConta IN ({1},{2},{3})
                ) AS caixa
                GROUP BY IdAcerto";
                
            var idsAcerto = string.Empty;
            foreach (var acerto in acertos)
                idsAcerto += acerto.IdAcerto + ",";

            var retornoCaixaGeralDiario = GetValoresCampo(session,
                string.Format(sql, idsAcerto.TrimEnd(','),
                    FinanceiroConfig.PlanoContaJurosReceb,
                    FinanceiroConfig.PlanoContaJurosCartao,
                    (int)UtilsPlanoConta.PlanoContas.JurosVendaConstrucard), "dados");

            var idsAcertoJuros = new Dictionary<uint, decimal>();
            foreach (var idAcertoJuros in retornoCaixaGeralDiario.Split(','))
            {
                if (string.IsNullOrEmpty(idAcertoJuros))
                    continue;

                var dados = idAcertoJuros.Split(';');
                idsAcertoJuros.Add(dados[0].StrParaUint(), dados[1].StrParaDecimal());
            }

            if (!PedidoConfig.LiberarPedido)
                foreach (var acerto in acertos)
                {
                    // Na listagem do acerto não podem ser exibidas todas referências.
                    acerto.IdPedido = ContasReceberDAO.Instance.ObtemIdsPedido(session, acerto.IdAcerto, true);
                    if (idsAcertoJuros.ContainsKey(acerto.IdAcerto))
                        acerto.Juros = idsAcertoJuros[acerto.IdAcerto];
                }
            else
                foreach (var acerto in acertos)
                {
                    // Na listagem do acerto não podem ser exibidas todas referências.
                    acerto.IdLiberarPedido = ContasReceberDAO.Instance.ObtemIdsLiberarPedido(session, acerto.IdAcerto, true);
                    if (idsAcertoJuros.ContainsKey(acerto.IdAcerto))
                        acerto.Juros = idsAcertoJuros[acerto.IdAcerto];
                }
        }

        /// <summary>
        /// Retorna todos os acertos do cliente passado
        /// </summary>
        /// <param name="idCli"></param>
        /// <returns></returns>
        public IList<Acerto> GetByCliRpt(uint idCli)
        {
            bool temFiltro;
            var a = LoadDataWithSortExpression(SqlList(0, 0, 0, idCli, null, null, 0, 0, 0, 0, true, out temFiltro) + " Order By a.idAcerto desc", "", 0, 0, null);

            PreencheIdRefJuros(ref a);
            return a;
        }

        public IList<Acerto> GetByCliList(int numAcerto, uint idPedido, uint idLiberarPedido, uint idCli, string dataIni, string dataFim, 
            uint idFormaPagto, uint idTipoBoleto, int numNotaFiscal, int protestadas, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "a.idAcerto desc" : sortExpression;

            bool temFiltro;
            var dados = LoadDataWithSortExpression(SqlList(numAcerto, idPedido, idLiberarPedido, idCli, dataIni, dataFim, idFormaPagto,
                idTipoBoleto, numNotaFiscal, protestadas, true, out temFiltro), sort, startRow, pageSize, temFiltro, GetParam(dataIni, dataFim));

            PreencheIdRefJuros(ref dados);
            return dados;
        }

        public IList<Acerto> GetListRpt(uint idAcerto, uint idPedido, uint idLiberarPedido,
            uint idCli, string dataIni, string dataFim, uint idFormaPagto, int numNotaFiscal)
        {
            bool temFiltro;
            var dados = LoadDataWithSortExpression(SqlList((int)idAcerto, idPedido, idLiberarPedido, idCli, dataIni, dataFim, idFormaPagto,
                0, numNotaFiscal, 0, true, out temFiltro), "", 0, 0, GetParam(dataIni, dataFim));

            PreencheIdRefJuros(ref dados);
            return dados;
        }

        public int GetByCliListCount(int numAcerto, uint idPedido, uint idLiberarPedido, uint idCli, string dataIni, string dataFim, 
            uint idFormaPagto, uint idTipoBoleto, int numNotaFiscal, int protestadas)
        {
            bool temFiltro;
            return GetCountWithInfoPaging(SqlList(numAcerto, idPedido, idLiberarPedido, idCli, dataIni, dataFim,
                idFormaPagto, idTipoBoleto, numNotaFiscal, protestadas, true, out temFiltro), temFiltro, null, GetParam(dataIni, dataFim));
        }

        public int GetCount(int numAcerto, uint idCli, string dataIni, string dataFim)
        {
            bool temFiltro;
            return objPersistence.ExecuteSqlQueryCount(SqlList(numAcerto, 0, 0, idCli, dataIni, dataFim, 0, 0, 0, 0, false, out temFiltro), 
                GetParam(dataIni, dataFim));
        }

        private GDAParameter[] GetParam(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca detalhes do acerto

        /// <summary>
        /// Retorna dados do Acerto
        /// </summary>
        public Acerto GetAcertoDetails(uint idAcerto)
        {
            return GetAcertoDetails(null, idAcerto);
        }

        /// <summary>
        /// Retorna dados do Acerto
        /// </summary>
        public Acerto GetAcertoDetails(GDASession session, uint idAcerto)
        {
            string formaPagto = string.Empty;
            var totalParcelas = Glass.Data.DAL.ContasReceberDAO.Instance.ObterNumParcMaxAcerto(session, idAcerto);

            // Busca movimentações relacionadas a este acerto e agrupadas pela forma de pagto
            foreach (var pa in PagtoAcertoDAO.Instance.GetByAcerto(session, idAcerto))
            {
                formaPagto += string.Format("{0} {2}: {1}", pa.DescrFormaPagto, pa.ValorPagto.ToString("C"),
                    (pa.IdFormaPagto == 5 && totalParcelas > 0 ? totalParcelas + " parcela(s)" : ""));
                var idContaBanco = pa.IdContaBanco;

                /* Chamado 56306. */
                if (pa.IdFormaPagto == (int)Pagto.FormaPagto.Deposito && idContaBanco.GetValueOrDefault() == 0)
                    idContaBanco = ExecuteScalar<uint?>(session, string.Format("SELECT IdContaBanco FROM deposito_nao_identificado WHERE IdAcerto={0} AND ValorMov=?valor;", idAcerto),
                        new GDAParameter("?valor", pa.ValorPagto));
                
                if (idContaBanco > 0)
                    formaPagto += string.Format(" ({0})", ContaBancoDAO.Instance.GetDescricao(session, idContaBanco.Value));

                formaPagto += "\n";
            }

            // Retorna o acerto, apenas um registro deverá ser retornado
            bool temFiltro;
            var lst = LoadDataWithSortExpression(session, SqlList((int)idAcerto, 0, 0, 0, null, null, 0, 0, 0, 0, true, out temFiltro), "", 0, 0, null);

            PreencheIdRefJuros(session, ref lst);

            if (lst.Count > 0)
            {
                // Se for renegociação, busca a forma de pagamento da parcela renegociada
                if (string.IsNullOrEmpty(formaPagto))
                {
                    var lstContaRec = ContasReceberDAO.Instance.GetRenegByAcerto(session, idAcerto, false);

                    if (lstContaRec.Count > 0 && lstContaRec[0].IdFormaPagto > 0)
                    {
                        formaPagto += PagtoDAO.Instance.GetDescrFormaPagto(lstContaRec[0].IdFormaPagto.Value) +
                            (lstContaRec[0].IdFormaPagto.Value == 5 && totalParcelas > 0 ? " " + totalParcelas + " parcela(s)" : "");
                    }
                }

                lst[0].FormaPagto = formaPagto.TrimEnd('\n');
                return lst[0];
            }
            else
                return null;
        }

        /// <summary>
        /// Retorna todos os pedidos do acerto
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public IList<Pedido> GetPedidosInAcerto(uint idAcerto)
        {
            PersistenceObject<Pedido> obj = new PersistenceObject<Pedido>(GDA.GDASettings.GetProviderConfiguration("WebGlass"));

            string sql = @"
                Select p.*, c.Nome as NomeCliente, f.Nome as NomeFunc, l.NomeFantasia as nomeLoja 
                From pedido p Left Join cliente c On (p.idCli=c.id_Cli) 
                    Left Join funcionario f On (p.IdFunc=f.IdFunc) 
                    Left Join loja l On (p.IdLoja = l.IdLoja) 
                Where p.idPedido In (
                    Select c.IdPedido From contas_receber c where c.idAcerto=" + idAcerto + ")";

            return obj.LoadData(sql).ToList();
        }

        /// <summary>
        /// Retorna todos os ids dos pedidos do acerto.
        /// </summary>
        public string ObterIdsPedido(GDASession session, int idAcerto)
        {
            var sql =
                string.Format(
                    @"SELECT CAST(GROUP_CONCAT(IdPedido) AS CHAR)
                    FROM pedido p
                    WHERE p.IdPedido IN
                        (SELECT c.IdPedido FROM contas_receber c WHERE c.IdAcerto={0})", idAcerto);

            object ids = objPersistence.ExecuteScalar(session, sql);

            return ids != null ? ids.ToString() : string.Empty;
        }

        /// <summary>
        /// Retorna todos os ids das liberações de pedido do acerto.
        /// </summary>
        public string ObterIdsLiberarPedido(GDASession session, int idAcerto)
        {
            var sql =
                string.Format(
                    @"SELECT CAST(GROUP_CONCAT(IdLiberarPedido) AS CHAR)
                    FROM liberarpedido
                    WHERE IdLiberarPedido IN
                        (SELECT c.IdLiberarPedido FROM contas_receber c WHERE c.IdAcerto={0})", idAcerto);

            object ids = objPersistence.ExecuteScalar(session, sql);

            return ids != null ? ids.ToString() : string.Empty;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém o id do cliente
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public uint ObtemIdCliente(uint idAcerto)
        {
            return ObtemIdCliente(null, idAcerto);
        }

        /// <summary>
        /// Obtém o id do cliente
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public uint ObtemIdCliente(GDASession sessao, uint idAcerto)
        {
            string sql = "Select id_Cli From acerto Where idAcerto=" + idAcerto;
            return ExecuteScalar<uint>(sessao, sql);
        }

        public Acerto.SituacaoEnum ObterSituacao(GDASession session, int idAcerto)
        {
            return ObtemValorCampo<Acerto.SituacaoEnum>(session, "Situacao", string.Format("IdAcerto={0}", idAcerto));
        }

        #endregion

        #region Verifica se o pedido possui algum acerto

        /// <summary>
        /// Verifica se o pedido possui algum acerto
        /// </summary>
        public bool ExisteAcerto(uint idPedido)
        {
            return ExisteAcerto(null, idPedido);
        }

        /// <summary>
        /// Verifica se o pedido possui algum acerto
        /// </summary>
        public bool ExisteAcerto(GDASession session, uint idPedido)
        {
            string sql = "Select Count(*) From contas_receber Where idPedido=" + idPedido + " And idAcerto is not null";

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Atualiza taxa de antecipação

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Atualizar a taxa de antecipação da empresa
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <param name="txAntecip"></param>
        public void AtualizaTaxaAntecip(uint idAcerto, decimal txAntecip)
        {
            AtualizaTaxaAntecip(null, idAcerto, txAntecip);
        }

        /// <summary>
        /// Atualizar a taxa de antecipação da empresa
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <param name="txAntecip"></param>
        public void AtualizaTaxaAntecip(GDASession sessao, uint idAcerto, decimal txAntecip)
        {
            objPersistence.ExecuteCommand(sessao, "update acerto set taxaAntecip=?tx where idAcerto=" + idAcerto,
                new GDAParameter("?tx", txAntecip));
        }

        #endregion

        #region Atualiza Número de Autorização Construcard

        /// <summary>
        /// Atualizar o Número de Autorização Construcard
        /// </summary>
        public void AtualizaNumAutConstrucard(GDASession sessao, uint idAcerto, string numAutConstrucard)
        {
            objPersistence.ExecuteCommand(sessao, "update acerto set numAutConstrucard=?num where idAcerto=" + idAcerto,
                new GDAParameter("?num", numAutConstrucard));
        }

        #endregion

        #region Busca os IDs das contas recebidas no acerto

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Busca os IDs das contas recebidas no acerto.
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public string GetIdsContasR(uint idAcerto)
        {
            return GetIdsContasR(null, idAcerto);
        }

        /// <summary>
        /// Busca os IDs das contas recebidas no acerto.
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public string GetIdsContasR(GDASession sessao, uint idAcerto)
        {
            string sql = "select cast(group_concat(idContaR) as char) from contas_receber where idAcerto=" + idAcerto;
            return ExecuteScalar<string>(sessao, sql);
        }

        #endregion
    }
}
