// <copyright file="TipoPecaExibir.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Enumerador com os tipos possíveis para exibição de peças.
    /// </summary>
    public enum TipoPecaExibir
    {
        /// <summary>
        /// Em produção.
        /// </summary>
        [Description("Em produção")]
        EmProducao,

        /// <summary>
        /// Canceladas (mão-de-obra).
        /// </summary>
        [Description("Canceladas (mão-de-obra)")]
        CanceladasMaoDeObra,

        /// <summary>
        /// Canceladas (venda).
        /// </summary>
        [Description("Canceladas (venda)")]
        CanceladasVenda,
    }
}
