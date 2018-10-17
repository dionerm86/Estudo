// <copyright file="TipoProdutoComposicaoEnum.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Enumerador com os tipos para busca de produtos de composição.
    /// </summary>
    public enum TipoProdutoComposicao
    {
        /// <summary>
        /// Buscar todos os tipos de produtos.
        /// </summary>
        [Description("Incluir produtos de composição")]
        IncluirProdutosComposicao,

        /// <summary>
        /// Buscar apenas produtos de composição.
        /// </summary>
        [Description("Somente produtos de composição")]
        SomenteProdutosComposicao,

        /// <summary>
        /// Buscar apenas produtos que não são composição.
        /// </summary>
        [Description("Não incluir produtos de composição")]
        NaoIncluirProdutosComposicao,
    }
}
