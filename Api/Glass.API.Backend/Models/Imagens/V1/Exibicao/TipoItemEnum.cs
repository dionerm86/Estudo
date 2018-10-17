// <copyright file="TipoItemEnum.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.Imagens.V1.Exibicao
{
    /// <summary>
    /// Enumerador com os tipos de item válidos para o controle de exibição.
    /// </summary>
    public enum TipoItem
    {
        /// <summary>
        /// Dados para exibição de imagens SVG de projetos.
        /// </summary>
        [Description("SVG de Projeto")]
        SvgProjeto,

        /// <summary>
        /// Dados para exibição de imagens de produtos.
        /// </summary>
        [Description("Produto")]
        Produto,

        /// <summary>
        /// Dados para exibição de imagens de peças de produção.
        /// </summary>
        [Description("Peça de produção")]
        PecaProducao,
    }
}