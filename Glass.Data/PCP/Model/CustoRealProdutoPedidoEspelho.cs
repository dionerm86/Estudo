using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa o valor do custo real do produto do pedido espelho.
    /// </summary>
    public class CustoRealProdutoPedidoEspelho
    {
        #region Propriedades

        /// <summary>
        /// Identificador do pedido.
        /// </summary>
        public int IdPedido { get; set; }

        /// <summary>
        /// Identificador do produdo do pedido espelho.
        /// </summary>
        public int IdProdPedEsp { get; set; }

        /// <summary>
        /// Identificador do produto do pedido.
        /// </summary>
        public int IdProdPed { get; set; }

        /// <summary>
        /// Identificador do produto da nota fiscal associada.
        /// </summary>
        public int? IdProdNf { get; set; }

        /// <summary>
        /// Custo.
        /// </summary>
        public decimal Custo { get; set; }

        /// <summary>
        /// Quantidade.
        /// </summary>
        public int Qtde { get; set; }

        #endregion
    }
}
