using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class OrdemCargaDAO : BaseCadastroDAO<OrdemCarga, OrdemCargaDAO>
    {
        //private OrdemCargaDAO() { }

        #region Busca de Itens

        private string Sql(uint idCarregamento, uint idOC, string idsOCs, uint idPedido, uint idCliente, string nomeCliente, uint idLoja,
            string idsLoja, uint idRota, string idsRota, string dtEntPedidoIni, string dtEntPedidoFin, string situacao, string tipo,
            uint idCliExterno, string nomeCliExterno, string codRotasExternas, bool selecionar)
        {
            string campos = @"oc.*, c.Nome as NomeCliente, c.nomeFantasia as NomeFantasiaCliente, c.Cpf_Cnpj AS CpfCnpjCliente,
                CONCAT(r.codInterno,' - ',r.descricao) as CodRota, l.NomeFantasia as NomeLoja, f.nome as NomeFunc, '$$$' as criterio,
                CONCAT(cid.nomeCidade, ' - ', cid.nomeUF) as CidadeCliente, c.Tel_Cont AS RptTelCont, c.Tel_Cel AS RptTelCel, c.Tel_Res AS RptTelRes";

            string criterio = "";

            string sql = @"
                SELECT " + campos + @"
                FROM ordem_carga oc
                LEFT JOIN cliente c ON (oc.idCliente = c.id_cli) 
                LEFT JOIN cidade cid ON (c.idCidade = cid.idCidade)
                LEFT JOIN rota r ON (oc.idRota = r.idRota)
                LEFT JOIN loja l ON (oc.idLoja = l.idLoja)
                LEFT JOIN funcionario f ON (f.idFunc = oc.usuCad)
                {0}
                WHERE 1";

            bool filtroPedido = false;

            if (idCarregamento > 0)
            {
                sql += " AND oc.idCarregamento=" + idCarregamento;
                criterio += "Carregamento: " + idCarregamento + "     ";
            }

            if (!string.IsNullOrEmpty(idsOCs))
            {
                sql += " AND oc.idOrdemCarga IN(" + idsOCs.TrimEnd(',') + ")";
                criterio += "OCs: " + idsOCs.TrimEnd(',') + "     ";
            }

            if (idOC > 0)
            {
                sql += " AND oc.idOrdemCarga =" + idOC;
                criterio += "OC: " + idOC + "     ";
            }

            if (idCliente > 0)
            {
                sql += " AND oc.idCliente=" + idCliente;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCliente) + "     ";
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " AND oc.idCliente IN(" + ids + ")";
                criterio += "Cliente: " + nomeCliente + "     ";
            }

            if (idLoja > 0)
            {
                sql += " AND oc.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "     ";
            }
            else if (!string.IsNullOrEmpty(idsLoja) && idsLoja != "0")
            {
                sql += " AND oc.idLoja IN(" + idsLoja + ")";
                criterio += "Lojas: " + idsLoja + "     ";
            }

            if (idRota > 0)
            {
                sql += " AND oc.idRota=" + idRota;
            }
            else if (!string.IsNullOrEmpty(idsRota) && idsRota != "0")
            {
                sql += " AND oc.idRota IN(" + idsRota + ")";
            }

            if (!string.IsNullOrEmpty(dtEntPedidoIni))
            {
                sql += " AND p.dataEntrega >=?dtEntPedidoIni";
                filtroPedido = true;
            }

            if (!string.IsNullOrEmpty(dtEntPedidoFin))
            {
                sql += " AND p.dataEntrega <=?dtEntPedidoFin";
                filtroPedido = true;
            }

            if (!string.IsNullOrEmpty(situacao))
            {
                sql += " AND oc.situacao IN(" + situacao + ")";
            }

            if (!string.IsNullOrEmpty(tipo))
            {
                sql += " AND oc.TipoOrdemCarga IN(" + tipo + ")";
            }

            if (idPedido > 0)
            {
                sql += " AND poc.idPedido=" + idPedido;
                filtroPedido = true;
            }

            if (idCliExterno > 0)
            {
                sql += " AND p.IdClienteExterno=" + idCliExterno;
                filtroPedido = true;
            }
            else if (!string.IsNullOrEmpty(nomeCliExterno))
            {
                string ids = ClienteDAO.Instance.ObtemIdsClientesExternos(nomeCliExterno);

                if (!string.IsNullOrEmpty(ids))
                {
                    sql += " AND p.IdClienteExterno IN(" + ids + ")";
                    filtroPedido = true;
                }
            }

            if (!string.IsNullOrEmpty(codRotasExternas))
            {
                var rotas = string.Join(",", codRotasExternas.Split(',').Select(f => "'" + f + "'").ToArray());
                sql += " AND p.RotaExterna IN (" + rotas + ")";
                filtroPedido = true;
            }

            sql += " GROUP BY oc.idOrdemCarga ORDER BY oc.idOrdemCarga desc";

            sql = sql.Replace("$$$", criterio);

            if (filtroPedido)
            {
                string sqlFiltroPedido = @"
                    LEFT JOIN pedido_ordem_carga poc ON (oc.idOrdemCarga = poc.idOrdemCarga)
                    LEFT JOIN pedido p ON (poc.idPedido = p.idPedido)";

                sql = string.Format(sql, sqlFiltroPedido);
            }
            else
            {
                sql = string.Format(sql, "");
            }

            if (!selecionar)
                return "SELECT COUNT(*) FROM (" + sql + ") as tmp";

            return sql;
        }

        /// <summary>
        /// Recupera uma lista de OCs
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="idOC"></param>
        /// <param name="idPedido"></param>
        /// <param name="idCliente"></param>
        /// <param name="nomeCliente"></param>
        /// <param name="idLoja"></param>
        /// <param name="idRota"></param>
        /// <param name="dtEntPedidoIni"></param>
        /// <param name="dtEntPedidoFin"></param>
        /// <param name="situacao"></param>
        /// <param name="tipo"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<OrdemCarga> GetListWithExpression(uint idCarregamento, uint idOC, uint idPedido, uint idCliente, string nomeCliente,
            uint idLoja, uint idRota, string dtEntPedidoIni, string dtEntPedidoFin, string situacao, string tipo, uint idCliExterno, string nomeCliExterno, string idsRotasExternas,
            string sortExpression, int startRow, int pageSize)
        {
            string sql = Sql(idCarregamento, idOC, null, idPedido, idCliente, null, idLoja, null, idRota, null, dtEntPedidoIni, dtEntPedidoFin,
                situacao, tipo, idCliExterno, nomeCliExterno, idsRotasExternas, true);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, GetParams(dtEntPedidoIni, dtEntPedidoFin));
        }

        public int GetListWithExpressionCount(uint idCarregamento, uint idOC, uint idPedido, uint idCliente, string nomeCliente, uint idLoja,
            uint idRota, string dtEntPedidoIni, string dtEntPedidoFin, string situacao, string tipo, uint idCliExterno, string nomeCliExterno, string idsRotasExternas)
        {
            string sql = Sql(idCarregamento, idOC, null, idPedido, idCliente, null, idLoja, null, idRota, null, dtEntPedidoIni, dtEntPedidoFin, situacao,
                tipo, idCliExterno, nomeCliExterno, idsRotasExternas, false);

            return objPersistence.ExecuteSqlQueryCount(sql, GetParams(dtEntPedidoIni, dtEntPedidoFin));
        }

        /// <summary>
        /// Recupera uma lista de OCs para o relatório
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="idOC"></param>
        /// <param name="idPedido"></param>
        /// <param name="idCliente"></param>
        /// <param name="nomeCliente"></param>
        /// <param name="idLoja"></param>
        /// <param name="idRota"></param>
        /// <param name="dtEntPedidoIni"></param>
        /// <param name="dtEntPedidoFin"></param>
        /// <param name="situacao"></param>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public IList<OrdemCarga> GetListForRpt(uint idCarregamento, uint idOC, uint idPedido, uint idCliente, string nomeCliente, uint idLoja,
            uint idRota, string dtEntPedidoIni, string dtEntPedidoFin, string situacao, string tipo, uint idCliExterno, string nomeCliExterno, string idsRotasExternas)
        {
            string sql = Sql(idCarregamento, idOC, null, idPedido, idCliente, null, idLoja, null, idRota, null, dtEntPedidoIni,
                dtEntPedidoFin, situacao, tipo, idCliExterno, nomeCliExterno, idsRotasExternas, true);

            return objPersistence.LoadData(sql, GetParams(dtEntPedidoIni, dtEntPedidoFin)).ToList();
        }

        /// <summary>
        /// Recupera uma lista de OCs
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idLoja"></param>
        /// <param name="idRota"></param>
        /// <param name="dtEntPedidoIni"></param>
        /// <param name="dtEntPedidoFin"></param>
        /// <param name="tipoOC"></param>
        /// <param name="situacao"></param>
        /// <returns></returns>
        public OrdemCarga[] GetList(uint idCliente, uint idLoja, uint idRota, string dtEntPedidoIni, string dtEntPedidoFin, string tipoOC, string situacao)
        {
            string sql = Sql(0, 0, null, 0, idCliente, null, idLoja, null, idRota, null, dtEntPedidoIni, dtEntPedidoFin, situacao, tipoOC, 0, null, null, true);
            return objPersistence.LoadData(sql, GetParams(dtEntPedidoIni, dtEntPedidoFin)).ToArray();
        }

        /// <summary>
        /// Recupera uma lista de OCs para o carregamento
        /// </summary>
        public OrdemCarga[] GetListForCarregamento(uint idCarregamento)
        {
            return objPersistence.LoadData(Sql(idCarregamento, 0, null, 0, 0, null, 0, null, 0, null, null, null, null, null, 0, null, null, true)).ToArray();
        }

        /// <summary>
        /// Recupera uma ordem de carga para o relatório individual
        /// </summary>
        /// <param name="idOC"></param>
        /// <returns></returns>
        public OrdemCarga[] GetForRptInd(uint idOC)
        {
            return objPersistence.LoadData(Sql(0, idOC, null, 0, 0, null, 0, null, 0, null, null, null, null, null, 0, null, null, true)).ToArray();
        }

        /// <summary>
        /// Recupera os ids das OCs para gerar o carregamento.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="nomeCliente"></param>
        /// <param name="idLoja"></param>
        /// <param name="idRota"></param>
        /// <param name="idsOCs"></param>
        /// <returns></returns>
        public string GetIdsOCsParaCarregar(uint idCliente, string nomeCliente, string idLoja, string idRota, string idsOCs)
        {
            var ocs = objPersistence.LoadResult(Sql(0, 0, null, 0, idCliente, nomeCliente, 0, idLoja, 0, idRota, null, null,
                ((int)OrdemCarga.SituacaoOCEnum.Finalizado).ToString(), null, 0, null, null, true), null).Select(f => f.GetUInt32(0)).ToList();

            var lstOCs = new List<string>();

            if (!string.IsNullOrEmpty(idsOCs))
                lstOCs.AddRange(idsOCs.Split(','));

            foreach (uint i in ocs)
                if (!lstOCs.Contains(i.ToString()))
                    lstOCs.Add(i.ToString());

            return lstOCs.Count > 0 ? string.Join(",", lstOCs.ToArray()) : "0";
        }

        /// <summary>
        /// Recupera os ids das ocs para gerar carregamento vindo da tela de geração OC
        /// </summary>
        /// <param name="idsRotas"></param>
        /// <param name="idLoja"></param>
        /// <param name="dtEntPedidoini"></param>
        /// <param name="dtEntPedidoFin"></param>
        /// <returns></returns>
        public string GetIdsOCsForGerarCarregamento(string idsRotas, uint idLoja, string dtEntPedidoIni, string dtEntPedidoFin)
        {
            var ocs = objPersistence.LoadResult(Sql(0, 0, null, 0, 0, null, idLoja, null, 0, idsRotas, dtEntPedidoIni, dtEntPedidoFin,
                ((int)OrdemCarga.SituacaoOCEnum.Finalizado).ToString(), null, 0, null, null, true),
                GetParams(dtEntPedidoIni, dtEntPedidoFin)).Select(f => f.GetUInt32(0)).ToList(); 

            return string.Join(",", ocs.Select(o => o.ToString()).ToArray());
        }

        /// <summary>
        /// Recupera uma lista de OCs para o carregamento
        /// </summary>
        public OrdemCarga[] GetOCsForCarregamento(string idsOCs)
        {
            return GetOCsForCarregamento(null, idsOCs);
        }

        /// <summary>
        /// Recupera uma lista de OCs para o carregamento
        /// </summary>
        public OrdemCarga[] GetOCsForCarregamento(GDASession session, string idsOCs)
        {
            if (string.IsNullOrWhiteSpace(idsOCs))
                return new OrdemCarga[0];

            return objPersistence.LoadData(session, Sql(0, 0, idsOCs, 0, 0, null, 0, null, 0, null, null, null, ((int)OrdemCarga.SituacaoOCEnum.Finalizado).ToString(), null, 0, null, null, true)).ToArray();
        }

        /// <summary>
        /// Recupera uma lista de OCs para o carregamento
        /// </summary>
        public List<OrdemCarga> GetOCsForCarregamento(uint idCarregamento)
        {
            return GetOCsForCarregamento(null, idCarregamento);
        }

        /// <summary>
        /// Recupera uma lista de OCs para o carregamento
        /// </summary>
        public List<OrdemCarga> GetOCsForCarregamento(GDASession sessao, uint idCarregamento)
        {
            return objPersistence.LoadData(sessao, Sql(idCarregamento, 0, null, 0, 0, null, 0, null, 0, null, null, null, null, null, 0, null, null, true)).ToList();
        }

        public GDAParameter[] GetParams(string dtEntPedidoIni, string dtEntPedidoFin)
        {
            List<GDAParameter> parameters = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(dtEntPedidoIni))
                parameters.Add(new GDAParameter("?dtEntPedidoIni", DateTime.Parse(dtEntPedidoIni + " 00:00:00")));

            if (!string.IsNullOrEmpty(dtEntPedidoFin))
                parameters.Add(new GDAParameter("?dtEntPedidoFin", DateTime.Parse(dtEntPedidoFin + " 23:59:59")));

            return parameters.ToArray();
        }

        /// <summary>
        /// Recupera uma lista ocs para a nfe
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<OrdemCarga> GetOCsForNfe(string idsPedidos)
        {
            if (string.IsNullOrEmpty(idsPedidos))
                return null;

            var idsOCs = PedidoOrdemCargaDAO.Instance.GetIdsOCsByPedidos(null, idsPedidos);
            if (String.IsNullOrEmpty(idsOCs))
                return null;

            string sql = Sql(0, 0, idsOCs, 0, 0, null, 0, null, 0, null, null, null, null, null, 0, null, null, true);
            return objPersistence.LoadData(sql).OrderBy(f => f.NomeCliente).ToList();
        }

        /// <summary>
        /// Recupera uma lista de OCs pelo ID do carregamento.
        /// </summary>
        public OrdemCarga[] ObterOrdensCargaPeloCarregamento(GDASession session, int idCarregamento)
        {
            // Retorna um objeto vazio caso o ID do carregamento não seja maior que zero.
            if (idCarregamento <= 0)
                return new OrdemCarga[0];

            // Recupera todas as ordens de carga do carregamento.
            var ordensCarga = objPersistence.LoadData(Sql((uint)idCarregamento, 0, null, 0, 0, null, 0, null, 0, null, null, null, null, null, 0, null, null, true)).ToArray();
            // Recupera todos os pedidos e os totais das ordens de carga recuperadas.
            var pedidosTotaisOrdensCarga = PedidoDAO.Instance.ObterPedidosTotaisOrdensCarga(null, ordensCarga.Select(f => (int)f.IdOrdemCarga)).ToList();

            // Percorre todas as ordens de carga para preencher seus pedidos e totais.
            foreach (var ordemCarga in ordensCarga)
            {
                var pedidosTotaisOrdemCarga = pedidosTotaisOrdensCarga.Where(f => f.IdOrdemCarga == ordemCarga.IdOrdemCarga);

                if (pedidosTotaisOrdemCarga != null)
                    ordemCarga.PedidosTotaisOrdemCarga = pedidosTotaisOrdemCarga.ToList();
            }

            return ordensCarga;
        }

        #endregion

        #region Obtem dados da OC

        /// <summary>
        /// Busca a situação de uma OC
        /// </summary>
        public OrdemCarga.SituacaoOCEnum GetSituacao(GDASession session, uint idOrdemCarga)
        {
            return (OrdemCarga.SituacaoOCEnum)ObtemValorCampo<int>(session, "situacao", "idOrdemCarga=" + idOrdemCarga);
        }

        /// <summary>
        /// Busca o tipo de uma OC
        /// </summary>
        /// <param name="idOrdemCarga"></param>
        /// <returns></returns>
        public OrdemCarga.TipoOCEnum GetTipoOrdemCarga(GDASession sessao, uint idOrdemCarga)
        {
            return (OrdemCarga.TipoOCEnum)ObtemValorCampo<int>(sessao, "tipoOrdemCarga", "idOrdemCarga=" + idOrdemCarga);
        }

        /// <summary>
        /// Busca o tipo de uma OC
        /// </summary>
        /// <param name="idOrdemCarga"></param>
        /// <returns></returns>
        public OrdemCarga.TipoOCEnum GetTipoOrdemCarga(uint idOrdemCarga)
        {
            return GetTipoOrdemCarga(null, idOrdemCarga);
        }

        /// <summary>
        /// Busca o tipo de uma OC
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public OrdemCarga.TipoOCEnum GetTipoOrdemCarga(uint idCarregamento, uint idPedido)
        {
            var sql = @"
                SELECT oc.tipoOrdemCarga
                FROM ordem_carga oc
                    INNER JOIN pedido_ordem_carga poc ON (oc.idOrdemCarga = poc.idOrdemCarga)
                WHERE oc.idCarregamento = " + idCarregamento + " AND poc.idPedido = " + idPedido;

            return (OrdemCarga.TipoOCEnum)ExecuteScalar<int>(sql);
        }

        /// <summary>
        /// Verifica se a oc do pedido no carregamento informado é de transferencia
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idCarregamento"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool TemTransferencia(GDASession session, uint idCarregamento, uint idPedido)
        {
            string sql = @"
                    SELECT count(*)
                    FROM pedido p
                        INNER JOIN pedido_ordem_carga poc ON (p.idPedido = poc.idPedido)
                        INNER JOIN ordem_carga oc ON (poc.idOrdemCarga = oc.idOrdemCarga)
                    WHERE oc.tipoOrdemCarga = " + (int)OrdemCarga.TipoOCEnum.Transferencia + @"
                      AND oc.idCarregamento = " + idCarregamento + @"
                      AND p.idPedido = " + idPedido;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Obtem o cód da rota de uma OC
        /// </summary>
        /// <param name="idOrdemCarga"></param>
        /// <returns></returns>
        public uint GetRota(uint idOrdemCarga)
        {
            return ObtemValorCampo<uint>("IdRota", "idOrdemCarga=" + idOrdemCarga);
        }

        /// <summary>
        /// Obtem o id do carregamento
        /// </summary>
        /// <param name="idOrdemCarga"></param>
        /// <returns></returns>
        public uint? GetIdCarregamento(uint idOrdemCarga)
        {
            return GetIdCarregamento(null, idOrdemCarga);
        }

        /// <summary>
        /// Obtem o id do carregamento
        /// </summary>
        /// <param name="idOrdemCarga"></param>
        /// <returns></returns>
        public uint? GetIdCarregamento(GDASession sessao, uint idOrdemCarga)
        {
            return ObtemValorCampo<uint?>(sessao, "IdCarregamento", "idOrdemCarga=" + idOrdemCarga);
        }

        /// <summary>
        /// Recupera os ids das ocs do carregamento informado.
        /// </summary>
        public List<uint> GetIdsOCsByCarregamento(GDASession sessao, uint idCarregamento)
        {
            string sql = @"
                SELECT DISTINCT(idOrdemCarga)
                FROM ordem_carga
                WHERE idCarregamento=" + idCarregamento;

            return ExecuteMultipleScalar<uint>(sessao, sql);
        }

        /// <summary>
        /// Recupera os ids das ocs do carregamento informado.
        /// </summary>
        /// <param name="idCarregamento"></param>
        /// <returns></returns>
        public List<uint> GetIdsOCsByCarregamento(uint idCarregamento)
        {
            return GetIdsOCsByCarregamento(null, idCarregamento);
        }
 
        /// <summary>
        /// Verifica se o pedido informado pertence à OC.
        /// </summary>
        public bool VerificarPedidoPertenceOC(GDASession sessao, int idOC, int idPedido)
        {
            return ExecuteScalar<bool>(sessao, string.Format("SELECT COUNT(*)>0 FROM pedido_ordem_carga poc WHERE poc.IdPedido={0} AND poc.IdOrdemCarga={1}", idPedido, idOC));
        }

        #endregion

        #region Finalizar OC

        /// <summary>
        /// Finaliza uma OC
        /// </summary>
        /// <param name="idOrdemCarga"></param>
        public void FinalizarOC(GDASession sessao, uint idOrdemCarga)
        {
            string sql = "UPDATE ordem_carga SET situacao=" + (int)OrdemCarga.SituacaoOCEnum.Finalizado + " WHERE idOrdemCarga=" + idOrdemCarga;
            objPersistence.ExecuteCommand(sessao, sql);
            
            // Insere a situação da ordem de carga no log de alterações.
            LogAlteracaoDAO.Instance.LogOrdemCarga(sessao, (int)idOrdemCarga, "Situação: Finalizado");
        }

        #endregion

        #region Vincula OCs ao carregamento

        /// <summary>
        /// Vincula as OC's a um carregamento
        /// </summary>
        public void VinculaOCsCarregamento(GDASession sessao, uint idCarregamento, string idsOCs)
        {
            var idsOrdemCarga = idsOCs.Split(',');

            VerificarExistePedidoOrdemCargaNoCarregamento(
                sessao, 
                idCarregamento, 
                idsOrdemCarga);

            string sql = @"
                UPDATE ordem_carga
                SET idCarregamento=" + idCarregamento + @",
                    situacao=" + (int)OrdemCarga.SituacaoOCEnum.PendenteCarregamento + @"
                WHERE idOrdemCarga IN(" + idsOCs + ")";

            foreach (var idOc in idsOrdemCarga)
            {
                // Insere a situação da ordem de carga no log de alterações.
                LogAlteracaoDAO.Instance.LogOrdemCarga(sessao, idOc.StrParaInt(), "Situação: Pendente Carregamento");

                // Insere o carregamento da ordem de carga no log de alterações.
                LogAlteracaoDAO.Instance.LogOrdemCarga(sessao, idOc.StrParaInt(), string.Format("Adicionada ao carregamento: {0}", idCarregamento));
            }

            objPersistence.ExecuteCommand(sessao, sql);
        }

        private void VerificarExistePedidoOrdemCargaNoCarregamento(GDASession sessao, uint idCarregamento, string[] idsOrdemCarga)
        {
            var idsPedidos = new List<uint>();

            foreach (var idOC in idsOrdemCarga)
            {
                idsPedidos.AddRange(this.GetIdsPedidosOC(
                    sessao,
                    Conversoes.StrParaUint(idOC),
                    OrdemCarga.TipoOCEnum.Venda));
            }

            if (!idsPedidos.Any(f => f > 0))
            {
                return;
            }
                
            var pedidosOrdemCarga = PedidoOrdemCargaDAO.Instance.ObterPedidosOrdemCarga(
                sessao,
                idCarregamento,
                idsPedidos);

            if (!pedidosOrdemCarga.Any(f => f.IdPedidoOrdemCarga > 0))
            {
                return;
            }

            var mensagem = $"Existem pedidos na listagem que já estão vinculados ao carregamento {idCarregamento}:";
            foreach (var pedidoOrdemCarga in pedidosOrdemCarga)
            {
                mensagem += $"\nPedido: {pedidoOrdemCarga.IdPedido}, Ordem de carga: {pedidoOrdemCarga.IdOrdemCarga}";
            }

            throw new Exception(mensagem);
        }

        #endregion

        #region Desvincula OCs ao carregamento

        /// <summary>
        /// Desvincula as OC's a um carregamento
        /// </summary>
        /// <param name="idsOCs"></param>
        public void DesvinculaOCsCarregamento(GDASession sessao, string idsOCs)
        {
            string sql = @"
                UPDATE ordem_carga
                SET idCarregamento=null,
                    situacao=" + (int)OrdemCarga.SituacaoOCEnum.Finalizado + @"
                WHERE idOrdemCarga IN(" + idsOCs + ")";

            foreach (var idOc in idsOCs.Split(','))
                // Insere a situação da ordem de carga no log de alterações.
                LogAlteracaoDAO.Instance.LogOrdemCarga(sessao, idOc.StrParaInt(), "Situação: Finalizado");

            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Força que a transação ocorra fazendo um update no campo transação, dessa forma o roll-back terá o que reverter.
        /// </summary>
        /// <param name="idOC"></param>
        /// <param name="inicio"></param>
        public void ForcarTransacaoOC(GDASession sessao, uint idOc, bool inicio)
        {
            string sql = @"
                UPDATE ordem_carga
                SET TRANSACAO = {0}
                WHERE idOrdemCarga = {1}";

            sql = string.Format(sql, inicio, idOc);

            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Desvincula as OC's a um carregamento
        /// </summary>
        /// <param name="idCarregamento"></param>
        public void DesvinculaOCsCarregamento(GDASession sessao, uint idCarregamento)
        {
            objPersistence.ExecuteCommand(sessao, "UPDATE ordem_carga SET idCarregamento = null, Situacao = "
                + (int)OrdemCarga.SituacaoOCEnum.Finalizado + " WHERE idCarregamento=" + idCarregamento);
        }

        #endregion

        #region Verifica se a OC foi totalmente carregada

        /// <summary>
        /// Verifica se todas as peças e volumes da OC foram carregadas
        /// </summary>
        public void VerificaOCCarregada(GDASession sessao, uint idCarregamento, uint idOC, string etiqueta)
        {
            // Recupera o ID do pedido da etiqueta, para utilizá-lo no log de alteração.
            var idPedido = string.IsNullOrWhiteSpace(etiqueta) ? 0 : ProdutoPedidoProducaoDAO.Instance.ObtemIdPedido(sessao, etiqueta);

            // Verifica se a ordem de carga está carregada.
            var sqlOrdemCargaCarregada = string.Format(@"SELECT COUNT(*)
                FROM item_carregamento ic
                WHERE (ic.Carregado IS NULL OR ic.Carregado = 0) AND ic.IdOrdemCarga={0} AND ic.IdCarregamento={1}", idOC, idCarregamento);
            var ordemCargaCarregada = objPersistence.ExecuteSqlQueryCount(sessao, sqlOrdemCargaCarregada) == 0;

            // Verifica se a ordem de carga é uma ordem de carga parcial.
            var sqlOrdemCargaParcial = string.Format(@"SELECT COUNT(*)
                FROM pedido_ordem_carga poc
                    INNER JOIN pedido p ON (poc.IdPedido = p.IdPedido)
                WHERE p.OrdemCargaParcial AND poc.IdOrdemCarga={0}", idOC);
            var ordemCargaParcial = OrdemCargaConfig.UsarOrdemCargaParcial && objPersistence.ExecuteSqlQueryCount(sessao, sqlOrdemCargaParcial) > 0;

            // Recupera a situação atual da OC.
            var situacao = ordemCargaParcial ? OrdemCarga.SituacaoOCEnum.CarregadoParcialmente : ordemCargaCarregada ? OrdemCarga.SituacaoOCEnum.Carregado : OrdemCarga.SituacaoOCEnum.PendenteCarregamento;
            
            // Insere log de alteração na OC somente se a etiqueta pertencer à ela, caso contrário, somente atualiza a situação.
            if (string.IsNullOrWhiteSpace(etiqueta) || (idOC > 0 && idPedido > 0 && VerificarPedidoPertenceOC(sessao, (int)idOC, (int)idPedido)) ||
                GetSituacao(sessao, idOC) != situacao)
            {
                // Insere a situação da ordem de carga no log de alterações.
                LogAlteracaoDAO.Instance.LogOrdemCarga(sessao, (int)idOC, string.Format("Situação: {0}{1}", situacao == OrdemCarga.SituacaoOCEnum.Carregado ? "Carregado" : "Pendente Carregamento",
                    string.IsNullOrWhiteSpace(etiqueta) ? string.Empty : string.Format(" - Etiqueta lida: {0}", etiqueta)));
            }

            // Atualiza a situação da OC.
            objPersistence.ExecuteCommand(sessao, string.Format("UPDATE ordem_carga SET Situacao={0} WHERE IdOrdemCarga={1}", (int)situacao, idOC));
        }

        #endregion

        #region Verifica se a OC possui alguma peça carregada

        /// <summary>
        /// Verifica se a oc possui algum item carregado
        /// </summary>
        public bool PossuiPecaCarregada(GDASession sessao, uint idOrdemCarga)
        {
            var idsPedidos = string.Join(",", PedidoDAO.Instance.GetIdsPedidosByOCs(sessao, idOrdemCarga.ToString()).Select(f => f.ToString()).ToArray());
            var idCarregamento = OrdemCargaDAO.Instance.GetIdCarregamento(sessao, idOrdemCarga);

            if (idCarregamento == null)
                return false;

            string sql = @"
                SELECT COUNT(*)
                FROM item_carregamento ic
                where ic.carregado=true AND ic.idPedido IN (" + idsPedidos + ") AND ic.idCarregamento=" + idCarregamento;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Retorna os ids dos pedido de uma oc

        /// <summary>
        /// Retorna os ids dos pedido de uma oc
        /// </summary>
        /// <param name="idsPedidos"></param>
        /// <param name="tipoOrdemCarga"></param>
        /// <returns></returns>
        public List<uint> GetIdsPedidosOC(GDASession sessao, uint idOC, OrdemCarga.TipoOCEnum tipoOrdemCarga)
        {
            string sql = @"
                    SELECT poc.idPedido
                    FROM pedido_ordem_carga poc
	                    INNER JOIN ordem_carga oc ON (poc.idOrdemCarga = oc.idOrdemCarga)
                    WHERE oc.tipoOrdemCarga=" + (int)tipoOrdemCarga + @"
	                    AND poc.idPedido IN(SELECT poc.idPedido
						                    FROM pedido_ordem_carga poc
						                    WHERE poc.idOrdemCarga=" + idOC + ")";

            return ExecuteMultipleScalar<uint>(sessao, sql);
        }

        /// <summary>
        /// Retorna os ids dos pedido de uma oc
        /// </summary>
        /// <param name="idsPedidos"></param>
        /// <param name="tipoOrdemCarga"></param>
        /// <returns></returns>
        public List<uint> GetIdsPedidosOC(uint idOC, OrdemCarga.TipoOCEnum tipoOrdemCarga)
        {
            return GetIdsPedidosOC(null, idOC, tipoOrdemCarga);
        }

        #endregion
    }
}
