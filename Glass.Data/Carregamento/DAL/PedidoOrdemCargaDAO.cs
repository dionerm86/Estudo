using Glass.Data.Model;
using GDA;

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
        public PedidoOrdemCarga GetElement(OrdemCarga.TipoOCEnum tipoOrdemCarga, uint idPedido)
        {
            return GetElement(null, tipoOrdemCarga, idPedido);
        }

        /// <summary>
        /// Busca pedido da OC
        /// </summary>
        /// <param name="tipoOrdemCarga"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public PedidoOrdemCarga GetElement(GDASession sessao, OrdemCarga.TipoOCEnum tipoOrdemCarga, uint idPedido)
        {
            string sql = @"
                SELECT {0}
                FROM pedido_ordem_carga poc
                    INNER JOIN ordem_carga oc ON (poc.idOrdemCarga = oc.idOrdemCarga)
                WHERE oc.tipoOrdemCarga=" + (int)tipoOrdemCarga + @"
                    AND poc.idPedido=" + idPedido;

            if (objPersistence.ExecuteSqlQueryCount(sessao, string.Format(sql, "COUNT(*)")) == 0)
                return null;

            return objPersistence.LoadOneData(sessao, string.Format(sql, "poc.*"));
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
        /// <param name="tipoOrdemCarga"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PedidoTemOC(uint idPedido)
        {
            return PedidoTemOC(null, idPedido);
        }

        /// <summary>
        /// Verifica se um pedido esta em alguma ordem de carga
        /// </summary>
        /// <param name="tipoOrdemCarga"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PedidoTemOC(GDASession sessao, uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount("SELECT COUNT(*) FROM pedido_ordem_carga WHERE idPedido=" + idPedido) > 0;
        }

        public bool PedidoTemOC(OrdemCarga.TipoOCEnum tipoOrdemCarga, uint idPedido)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM pedido_ordem_carga poc
                    INNER JOIN ordem_carga oc ON (poc.idOrdemCarga = oc.idOrdemCarga)
                WHERE poc.idPedido = " + idPedido + " AND oc.tipoOrdemCarga=" + (int)tipoOrdemCarga;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
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
        public int ObtemQtdePedidosOC(uint idOC)
        {
            return ObtemQtdePedidosOC(null, idOC);
        }

        #endregion

        #region Excluir Itens

        /// <summary>
        /// Deleta todos os itens de uma ordem de carga
        /// </summary>
        public void DeleteByOrdemCarga(GDASession session, uint idOrdemCarga)
        {
            string sql = "DELETE FROM pedido_ordem_carga WHERE idOrdemCarga=" + idOrdemCarga;
            objPersistence.ExecuteCommand(session, sql);
        }

        /// <summary>
        /// Deleta todos os itens de um pedido
        /// </summary>
        /// <param name="idPedido"></param>
        public void DeleteByPedido(GDASession sessao, uint idPedido, uint idOC)
        {
            string sql = "DELETE FROM pedido_ordem_carga WHERE idPedido=" + idPedido + " AND idOrdemCarga=" + idOC;
            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Deleta todos os itens de um pedido
        /// </summary>
        /// <param name="idPedido"></param>
        public void DeleteByPedido(uint idPedido, uint idOC)
        {
            DeleteByPedido(null, idPedido, idOC);
        }

        #endregion
    }
}
