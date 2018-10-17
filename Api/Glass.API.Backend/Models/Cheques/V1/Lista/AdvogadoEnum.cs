// <copyright file="AdvogadoEnum.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.Cheques.V1.Lista
{
    /// <summary>
    /// Enumerador com as opções possíveis para a busca de cheques com advogados.
    /// </summary>
    public enum Advogado
    {
        /// <summary>
        /// Apenas cheques com advogados.
        /// </summary>
        [Description("Apenas cheques com advogados")]
        ChequesComAdvogados = 1,

        /// <summary>
        /// Apenas cheques que não estão com advogados.
        /// </summary>
        [Description("Apenas cheques que não estão com advogados")]
        ChequesQueNaoEstaoComAdvogados = 2,
    }
}
