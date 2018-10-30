// <copyright file="ExibicaoFactory.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Imagens.Estrategias.Exibicao;
using Glass.API.Backend.Models.Imagens.V1.Exibicao;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Imagens
{
    /// <summary>
    /// Classe para recuperação de estratégias para o controle de exibição de imagens.
    /// </summary>
    internal static class ExibicaoFactory
    {
        /// <summary>
        /// Recupera a instância que trata de um tipo específico de item.
        /// </summary>
        /// <param name="sessao">A transação com o banco de dados.</param>
        /// <param name="apiController">O controller que está sendo executado.</param>
        /// <param name="tipoItem">O tipo de item que contém uma imagem.</param>
        /// <returns>Uma instância para tratamento do item solicitado.</returns>
        public static IExibicao ObterParaControleExibicao(
            GDASession sessao,
            ApiController apiController,
            TipoItem tipoItem)
        {
            switch (tipoItem)
            {
                case TipoItem.Produto:
                    return new ExibicaoProdutoStrategy(sessao, apiController);

                case TipoItem.PecaProducao:
                    return new ExibicaoPecaProducaoStrategy(sessao, apiController);

                case TipoItem.SvgProjeto:
                    return new ExibicaoSvgProjetoStrategy(sessao, apiController);
                case TipoItem.Funcionario:
                    return new ExibicaoFuncionarioStrategy(sessao, apiController);
            }

            return null;
        }
    }
}
