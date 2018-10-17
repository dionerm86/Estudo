// <copyright file="TipoPedidoEnum.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Enumerador para o tipo de pedido.
    /// </summary>
    public enum TipoPedido
    {
        /// <summary>
        /// Apenas pedidos de venda.
        /// </summary>
        [Description("Venda")]
        Venda = 1,

        /// <summary>
        /// Apenas pedidos de produção.
        /// </summary>
        [Description("Produção")]
        Producao,

        /// <summary>
        /// Apenas pedidos de mão-de-obra.
        /// </summary>
        [Description("Mão-de-obra")]
        MaoDeObra,
    }
}
