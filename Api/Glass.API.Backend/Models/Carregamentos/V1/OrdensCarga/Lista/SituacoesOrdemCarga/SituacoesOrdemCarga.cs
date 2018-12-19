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
        /// Finalizado.
        /// </summary>
        [Description("Finalizado")]
        SituacaoOrdemCargaFinalizada = 1,

        /// <summary>
        /// Pendente carregamento.
        /// </summary>
        [Description("Pendente Carregamento")]
        SituacaoOrdemCargaPendenteCarregamento = 2,

        /// <summary>
        /// Carregado.
        /// </summary>
        [Description("Carregado")]
        SituacaoOrdemCargaCarregada = 3,

        /// <summary>
        /// Carregado parcialmente.
        /// </summary>
        [Description("Carregado Parcialmente")]
        SituacaoOrdemCargaCarregadaParcialmente = 4,
    }
}