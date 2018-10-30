// <copyright file="TipoFastDeliveryEnum.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Enumerador para o filtro de 'fast delivery'.
    /// </summary>
    public enum TipoFastDelivery
    {
        /// <summary>
        /// Apenas peças de pedidos com fast delivery.
        /// </summary>
        [Description("Com Fast Delivery")]
        ComFastDelivery = 1,

        /// <summary>
        /// Apenas peças de pedidos sem fast delivery.
        /// </summary>
        [Description("Sem Fast Delivery")]
        SemFastDelivery,
    }
}
