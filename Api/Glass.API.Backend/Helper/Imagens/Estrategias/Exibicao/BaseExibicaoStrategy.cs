// <copyright file="BaseExibicaoStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Imagens.V1.Exibicao;
using System;
using System.IO;
using System.Web;

namespace Glass.API.Backend.Helper.Imagens.Estrategias.Exibicao
{
    /// <summary>
    /// Classe base para as estratégias para o controle de exibição de imagem.
    /// </summary>
    internal class BaseExibicaoStrategy
    {
        /// <summary>
        /// Recupera a classe com os dados para exibição de uma imagem.
        /// </summary>
        /// <param name="url">A URL da imagem.</param>
        /// <param name="legenda">A legenda da imagem.</param>
        /// <returns>Os dados da imagem para o controle de exibição.</returns>
        protected DadosImagemDto ObterDadosImagem(string url, string legenda = null)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            return new DadosImagemDto
            {
                UrlImagem = this.ResolverUrlRelativa(url),
                Legenda = legenda,
            };
        }

        private string ResolverUrlRelativa(string url)
        {
            if (url.IndexOf("://") != -1)
            {
                return url;
            }

            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("Contexto HTTP está inválido para a operação.");
            }

            if (url.StartsWith("~"))
            {
                return Path.Combine(
                    HttpContext.Current.Request.ApplicationPath,
                    "..",
                    url.Substring(1).Replace("//", "/"));
            }

            return url;
        }
    }
}
