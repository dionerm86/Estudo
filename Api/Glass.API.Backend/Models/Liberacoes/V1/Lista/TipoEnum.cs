// <copyright file="TipoEnum.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.Liberacoes.V1.Lista
{
    /// <summary>
    /// Enumerador com as situações de liberações.
    /// </summary>
    public enum Tipo
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