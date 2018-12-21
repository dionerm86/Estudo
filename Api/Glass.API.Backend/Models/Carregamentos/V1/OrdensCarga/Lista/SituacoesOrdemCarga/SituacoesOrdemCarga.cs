// <copyright file="SituacoesOrdemCarga.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.Carregamentos.V1.OrdensCarga.Lista.SituacoesOrdemCarga
{
    /// <summary>
    /// Enumerador com as situações de ordem de carga.
    /// </summary>
    public enum SituacoesOrdemCarga
    {
        /// <summary>
        /// Finalizada.
        /// </summary>
        [Description("Finalizada")]
        Finalizada = 1,

        /// <summary>
        /// Pendente carregamento.
        /// </summary>
        [Description("Pendente Carregamento")]
        PendenteCarregamento = 2,

        /// <summary>
        /// Carregada.
        /// </summary>
        [Description("Carregada")]
        Carregada = 3,

        /// <summary>
        /// Carregada parcialmente.
        /// </summary>
        [Description("Carregada Parcialmente")]
        CarregadaParcialmente = 4,
    }
}