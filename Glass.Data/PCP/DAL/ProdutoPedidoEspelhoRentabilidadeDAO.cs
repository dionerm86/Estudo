using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.DAL
{
    /// <summary>
    /// Representa o objeto de acesso a dados para a rentabilidade do produto do pedido.
    /// </summary>
    public class ProdutoPedidoEspelhoRentabilidadeDAO : BaseDAO<ProdutoPedidoEspelhoRentabilidade, ProdutoPedidoEspelhoRentabilidadeDAO>
    {
        #region Métodos Públicos

        /// <summary>
        /// Recupera os registros de rentabilidade pelo produtos do pedido.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        public IList<ProdutoPedidoEspelhoRentabilidade> ObterPorProdutoPedido(GDA.GDASession sessao, uint idProdPed)
        {
            return objPersistence.LoadData(sessao, "SELECT * FROM produto_pedido_espelho_rentabilidade WHERE IdProdPed=?id",
                new GDA.GDAParameter("?id", idProdPed))
                .Select(f =>
                {
                    f.ExistsInStorage = true;
                    return f;
                }).ToList();
        }

        /// <summary>
        /// Apaga os registros associados com o produto do pedido.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdPed"></param>
        public void ApagarPorProdutoPedido(GDA.GDASession sessao, uint idProdPed)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM produto_pedido_espelho_rentabilidade WHERE IdProdPed=?idProdPed", new GDA.GDAParameter("?idProdPed", idProdPed));
        }

        #endregion
    }
}
