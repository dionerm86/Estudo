using System;
using Glass.Data.Model;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class PedidosCompraDAO : BaseDAO<PedidosCompra, PedidosCompraDAO>
    {
        #region Busca a listagem de pedidos compra

        /// <summary>
        /// Retorna uma lista de registros da tabela pedidos_compra relacionados à compra informada no parâmetro.
        /// </summary>
        /// <param name="idCompra">Id da compra que será usada para buscar os registros.</param>
        /// <returns>Retorna a lista de pedidos compra relacionados ao id da compra informado.</returns>
        public PedidosCompra[] GetByCompra(uint idCompra)
        {
            return objPersistence.LoadData("Select * From pedidos_compra Where idCompra=" + idCompra).ToArray();
        }

        /// <summary>
        /// Retorna uma lista de registros da tabela pedidos_compra relacionados ao pedido informado no parâmetro.
        /// </summary>
        /// <param name="idPedido">Id do pedido que será usado para buscar os registros.</param>
        /// <returns>Retorna a lista de pedidos compra relacionados ao id do pedido informado.</returns>
        public PedidosCompra[] GetByPedido(uint idPedido)
        {
            return objPersistence.LoadData("Select * From pedidos_compra Where idPedido=" + idPedido).ToArray();
        }

        #endregion

        #region Busca os pedidos associados à compra

        /// <summary>
        /// Retorna os pedidos relacionados à compra informada no parâmetro.
        /// </summary>
        /// <param name="idCompra">Id da compra usado para buscar os pedidos.</param>
        /// <returns>Retorna a lista de pedidos associados à compra.</returns>
        public Pedido[] GetPedidosByCompra(uint idCompra)
        {
            var lstPedCompra = objPersistence.LoadData("Select * From pedidos_compra Where idCompra=" + idCompra + " And idPedido Is Not Null");
            var pedidos = String.Empty;

            foreach (var pedido in lstPedCompra)
                pedidos += "," + pedido.IdPedido;

            return PedidoDAO.Instance.GetByString(null, pedidos.Substring(1));
        }

        #endregion

        #region Busca as compras geradas do pedido

        /// <summary>
        /// Método criado para retornar se o pedido possui compra, não cancelada, de produto de beneficiamento gerada ou não.
        /// </summary>
        /// <param name="idPedido">Id do pedido que será usado para buscar as compras geradas.</param>
        /// <returns>Retorna true caso o pedido possua compras de produtos de beneficiamento gerada e false caso contrário.</returns>
        public bool PossuiCompraProdBenefGerada(uint idPedido)
        {
            var sql =
                @"Select Count(*) From pedidos_compra pc
                    Left Join compra c ON (pc.idCompra=c.idCompra And c.situacao Not In (" + (int)Compra.SituacaoEnum.Cancelada + @"))
                Where pc.idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Métodos sobescritos

        /// <summary>
        /// Deleta o registro da tabela pedidos_compra pelo id do pedido e pelo id da compra.
        /// </summary>
        /// <param name="idPedido">Código do pedido.</param>
        /// <param name="idCompra">Código da compra.</param>
        public void DeletarPeloPedidoCompra(uint idPedido, uint idCompra)
        {
            objPersistence.ExecuteCommand("Delete From pedidos_compra Where idCompra=" + idCompra + " And idPedido=" + idPedido);
        }

        /// <summary>
        /// Insere um registro na tabela pedidos_compra.
        /// </summary>
        /// <param name="objInsert">Objeto da classe PedidosCompra a ser inserido no banco de dados.</param>
        public void Insere(PedidosCompra objInsert)
        {
            objPersistence.ExecuteCommand(@"Insert Into pedidos_compra (idCompra, idPedido, produtoBenef)
                Value (" + objInsert.IdCompra + "," + objInsert.IdPedido + "," + objInsert.ProdutoBenef + ");");
        }

        #endregion
    }
}