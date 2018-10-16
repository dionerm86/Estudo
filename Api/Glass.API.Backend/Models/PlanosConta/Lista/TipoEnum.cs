// <copyright file="TipoEnum.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.PlanosConta.Lista
{
    /// <summary>
    /// Enumerador com as opções possíveis de tipo de plano de conta.
    /// </summary>
    public enum Tipo
    {
        /// <summary>
        /// Planos de conta de crédito.
        /// </summary>
        [Description("Crédito")]
        Credito = 1,

        /// <summary>
        /// Planos de conta de débito.
        /// </summary>
        [Description("Débito")]
        Debito = 2,
    }
}
