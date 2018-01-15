using Glass.Data.Model;
using GDA;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public sealed class PedidoOrdemCargaDAO : BaseDAO<PedidoOrdemCarga, PedidoOrdemCargaDAO>
    {
        //private PedidoOrdemCargaDAO() { }

        #region Busca de Itens

        /// <summary>
        /// Busca pedido da OC
        /// </summary>
        /// <param name="tipoOrdemCarga"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public PedidoOrdemCarga GetElement(GDASession sessao, OrdemCarga.TipoOCEnum tipoOrdemCarga, uint idPedido)
        {
            string sql = @"
                SELECT poc.*
                FROM pedido_ordem_carga poc
                    INNER JOIN ordem_carga oc ON (poc.idOrdemCarga = oc.idOrdemCarga)
                WHERE oc.tipoOrdemCarga=" + (int)tipoOrdemCarga + @"
                    AND poc.idPedido=" + idPedido;

            return objPersistence.LoadOneData(sessao, sql);
        }

        /// <summary>
        /// Verifica se o pedido informado possui ordem de carga
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="tipoOrdemCarga"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiOrdemCarga(GDASession sessao, OrdemCarga.TipoOCEnum tipoOrdemCarga, uint idPedido)
        {
            string sql = @"
                SELECT count(*)
                FROM pedido_ordem_carga poc
                    INNER JOIN ordem_carga oc ON (poc.idOrdemCarga = oc.idOrdemCarga)
                    INNER JOIN pedido p ON (poc.IdPedido = p.IdPedido)
                WHERE !p.OrdemCargaParcial 
                    AND oc.tipoOrdemCarga=" + (int)tipoOrdemCarga + @"
                    AND poc.idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        public int ObterIdOrdemCarga(GDASession sessao, uint idCarregamento, uint idPedido)
        {
            var sql = @"
                SELECT oc.IdOrdemCarga
                FROM pedido_ordem_carga poc
	                INNER JOIN ordem_carga oc ON (poc.IdOrdemCarga = oc.IdOrdemCarga)
                WHERE poc.IdPedido = {0} AND oc.IdCarregamento = {1}";

            return ExecuteScalar<int>(sessao, string.Format(sql, idPedido, idCarregamento));
        }

        /// <summary>
        /// Busca os ids das ocs dos pedidos informados
        /// </summary>
        /// <param name="idsPedidos"></param>
        /// <returns></returns>
        public string GetIdsOCsByPedidos(GDASession sessao, string idsPedidos)
        {
            if (string.IsNullOrEmpty(idsPedidos))
                idsPedidos = "0";

            string sql = @"
                SELECT group_concat(DISTINCT idOrdemCarga)
                FROM pedido_ordem_carga
                WHERE idPedido IN(" + idsPedidos + ")";

            return ExecuteScalar<string>(sessao, sql);
        }

        #endregion

        #region Verifica se um pedido está em alguma ordem de carga

        /// <summary>
        /// Verifica se um pedido esta em alguma ordem de carga
        /// </summary>
        public bool PedidoTemOC(uint idPedido)
        {
            return PedidoTemOC(null, idPedido);
        }

        /// <summary>
        /// Verifica se um pedido esta em alguma ordem de carga
        /// </summary>
        public bool PedidoTemOC(GDASession sessao, uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount("SELECT COUNT(*) FROM pedido_ordem_carga WHERE idPedido=" + idPedido) > 0;
        }

        public bool PedidoTemOC(GDASession session, OrdemCarga.TipoOCEnum tipoOrdemCarga, uint idPedido)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM pedido_ordem_carga poc
                    INNER JOIN ordem_carga oc ON (poc.idOrdemCarga = oc.idOrdemCarga)
                WHERE poc.idPedido = " + idPedido + " AND oc.tipoOrdemCarga=" + (int)tipoOrdemCarga;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Verifica quantos pedidos uma oc possui

        /// <summary>
        /// Verifica quantos pedidos uma oc possui
        /// </summary>
        /// <param name="idOC"></param>
        /// <returns></returns>
        public int ObtemQtdePedidosOC(GDASession sessao, uint idOC)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM pedido_ordem_carga
                WHERE idOrdemCarga=" + idOC;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql);
        }

        /// <summary>
        /// Verifica quantos pedidos uma oc possui
        /// </summary>
        /// <param name="idOC"></param>
        /// <returns></returns>
        public int ObtemQtdeOrdemCarga(GDASession sessao, uint idPedido)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM pedido_ordem_carga
                WHERE idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql);
        }

        /// <summary>
        /// Verifica quantos pedidos uma oc possui
        /// </summary>
        /// <param name="idOC"></param>
        /// <returns></returns>
        public int ObtemQtdePedidosOC(uint idOC)
        {
            return ObtemQtdePedidosOC(null, idOC);
        }

        #endregion

        #region Obter dados

        /// <summary>
        /// Obtém as OCs vinculadas a um pedido.
        /// </summary>
        public List<int> ObterIdsOrdemCargaPeloPedido(GDASession session, int idPedido)
        {
            if (!PedidoTemOC(session, (uint)idPedido))
                return new List<int>();

            return ExecuteMultipleScalar<int>(session, string.Format("SELECT IdOrdemCarga FROM pedido_ordem_carga WHERE IdPedido={0}", idPedido));
        }

        #endregion

        #region Excluir Itens

        /// <summary>
        /// Deleta todos os itens de uma ordem de carga
        /// </summary>
        public void DeleteByOrdemCarga(GDASession session, uint idOrdemCarga)
        {
            VolumeDAO.Instance.DesvincularOrdemCarga(session, idOrdemCarga);

            string sql = "DELETE FROM pedido_ordem_carga WHERE idOrdemCarga=" + idOrdemCarga;
            objPersistence.ExecuteCommand(session, sql);
        }

        /// <summary>
        /// Deleta todos os itens de um pedido
        /// </summary>
        /// <param name="idPedido"></param>
        public void DeleteByPedido(GDASession sessao, uint idPedido, uint idOC)
        {
            VolumeDAO.Instance.DesvincularOrdemCarga(sessao, idOC);

            string sql = "DELETE FROM pedido_ordem_carga WHERE idPedido=" + idPedido + " AND idOrdemCarga=" + idOC;
            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Métodos Sobrescritos

        public override uint Insert(GDASession session, PedidoOrdemCarga objInsert)
        {
            VolumeDAO.Instance.AtualizaIdOrdemCarga(session, objInsert.IdOrdemCarga, objInsert.IdPedido);
            return base.Insert(session, objInsert);
        }

        #endregion
    }
}
