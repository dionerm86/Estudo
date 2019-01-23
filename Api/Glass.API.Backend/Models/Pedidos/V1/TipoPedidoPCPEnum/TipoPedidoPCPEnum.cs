// <copyright file="TipoPedidoPCPEnum.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.Pedidos.V1.TipoPedidoPCPEnum
{
    /// <summary>
    /// Enumerador com os tipos de pedido PCP.
    /// </summary>
    public enum TipoPedidoPCPEnum
    {
        /// <summary>
        /// Tipo de pedido PCP produção.
        /// </summary>
        [Description("Produção")]
        Producao = 4,

        /// <summary>
        /// Tipo de pedido PCP venda.
        /// </summary>
        [Description("Venda")]
        Venda = 1,
    }
}
