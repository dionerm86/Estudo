// <copyright file="IExibicao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Imagens.V1.Exibicao;
using System.Web.Http;

namespace Glass.API.Backend.Helper.Imagens.Estrategias.Exibicao
{
    /// <summary>
    /// Interface com os métodos para o controle de exibição de imagens.
    /// </summary>
    internal interface IExibicao
    {
        /// <summary>
        /// Realiza a validação de existência de um item.
        /// </summary>
        /// <param name="id">O identificador do item.</param>
        /// <returns>O erro de validação, se houver.</returns>
        IHttpActionResult ValidarItem(int id);

        /// <summary>
        /// Recupera os dados de uma imagem para o controle de exibição.
        /// </summary>
        /// <param name="id">O identificador do item.</param>
        /// <returns>Os dados de uma imagem, se encontrados.</returns>
        DadosImagemDto RecuperarDados(int id);
    }
}
