// <copyright file="TiposOrdemCarga.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.Carregamentos.V1.OrdensCarga.Lista.TiposOrdemCarga
{
    /// <summary>
    /// Enumerador com os tipos de ordem de carga.
    /// </summary>
    public enum TipoMovimentacao
    {
        /// <summary>
        /// Venda.
        /// </summary>
        [Description("Venda")]
        Venda = 1,

        /// <summary>
        /// Transferência.
        /// </summary>
        [Description("Transferencia")]
        Transferencia = 2,
    }
}