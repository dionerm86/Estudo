using System;

namespace Glass.Data.Model
{
    /// <summary>
    /// Armazena as informações do total diários dos pedidos.
    /// </summary>
    public class PedidoTotalDiario
    {
        #region Propriedades

        /// <summary>
        /// Data.
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Total dos pedido.
        /// </summary>
        public decimal Total { get; set; }

        #endregion
    }
}
