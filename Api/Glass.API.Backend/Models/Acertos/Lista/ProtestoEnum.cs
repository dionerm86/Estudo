// <copyright file="ProtestoEnum.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.Acertos.Lista
{
    /// <summary>
    /// Enumerador com as opções possíveis para a busca de acertos com contas protestadas.
    /// </summary>
    public enum Protesto
    {
        /// <summary>
        /// Inclui no resultado acertos com contas protestadas.
        /// </summary>
        [Description("Incluir contas em jurídico/cartório")]
        IncluirContaProtestada,

        /// <summary>
        /// Busca apenas acertos com contas protestadas.
        /// </summary>
        [Description("Somente contas em jurídico/cartório")]
        ApenasComContaProtestada,

        /// <summary>
        /// Busca apenas acertos sem contas protestadas.
        /// </summary>
        [Description("Não incluir contas em jurídico/cartório")]
        ApenasSemContaProtestada,
    }
}
