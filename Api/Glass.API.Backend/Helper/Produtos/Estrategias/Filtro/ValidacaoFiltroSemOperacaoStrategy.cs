// <copyright file="ValidacaoFiltroSemOperacaoStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Produtos.Filtro;
using Glass.Data.Model;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Produtos.Estrategias.Filtro
{
    /// <summary>
    /// Classe para ignorar a validação do filtro de produtos.
    /// </summary>
    internal class ValidacaoFiltroSemOperacaoStrategy : IValidacaoFiltro
    {
        /// <inheritdoc/>
        public IHttpActionResult ValidarAntesBusca(GDASession sessao, string codigoInterno, string dadosAdicionaisValidacao)
        {
            return null;
        }

        /// <inheritdoc/>
        public IHttpActionResult ValidarDepoisBusca(GDASession sessao, string codigoInterno, string dadosAdicionaisValidacao, Produto produto)
        {
            return null;
        }

        /// <inheritdoc/>
        public ProdutoDto ObterProduto(GDASession sessao, string dadosAdicionaisValidacao, Produto produto)
        {
            return new ProdutoDto(sessao, produto);
        }
    }
}
