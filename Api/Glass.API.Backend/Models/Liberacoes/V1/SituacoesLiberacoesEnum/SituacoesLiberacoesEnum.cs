// <copyright file="SituacoesMovimentacoesLiberacoesEnum.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.Liberacoes.V1.SituacoesLiberacoesEnum
{
    /// <summary>
    /// Enumerador com as situações de liberações.
    /// </summary>
    public enum SituacoesLiberacoesEnum
    {
        /// <summary>
        /// Liberado.
        /// </summary>
        [Description("Liberado")]
        Liberado = 1,

        /// <summary>
        /// Cancelado.
        /// </summary>
        [Description("Cancelado")]
        Cancelado = 2,
    }
}
