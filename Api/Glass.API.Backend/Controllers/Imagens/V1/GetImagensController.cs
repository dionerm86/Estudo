// <copyright file="GetImagensController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Imagens;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Imagens.V1.Exibicao;
using Swashbuckle.Swagger.Annotations;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Imagens.V1
{
    /// <summary>
    /// Controller para recuperação de dados de imagens.
    /// </summary>
    public partial class ImagensController : BaseController
    {
        /// <summary>
        /// Recupera os dados de uma imagem para o controle de exibição.
        /// </summary>
        /// <param name="idItem">O identificador do item que contém a imagem.</param>
        /// <param name="tipoItem">O tipo do item que contém a imagem.</param>
        /// <returns>Um JSON com os dados da imagem encontrados.</returns>
        [HttpGet]
        [Route("exibicao/{idItem}/{tipoItem}")]
        [SwaggerResponse(200, "Dados da imagem encontrados.", Type = typeof(DadosImagemDto))]
        [SwaggerResponse(204, "Dados da imagem não encontrados.")]
        [SwaggerResponse(400, "Dados do item inválidos.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Item não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterDadosExibicao(int idItem, TipoItem? tipoItem)
        {
            var validacao = this.ValidarDadosItem(idItem, tipoItem);

            if (validacao != null)
            {
                return validacao;
            }

            using (var sessao = new GDATransaction())
            {
                var estrategia = ExibicaoFactory.ObterParaControleExibicao(sessao, this, tipoItem.Value);

                if (estrategia == null)
                {
                    return this.ErroValidacao($"Tipo de item '{tipoItem.Value}' inválido.");
                }

                validacao = estrategia.ValidarItem(idItem);

                if (validacao != null)
                {
                    return validacao;
                }

                var dadosItem = estrategia.RecuperarDados(idItem);

                return dadosItem != null
                    ? this.Item(dadosItem) as IHttpActionResult
                    : this.SemConteudo();
            }
        }
    }
}
