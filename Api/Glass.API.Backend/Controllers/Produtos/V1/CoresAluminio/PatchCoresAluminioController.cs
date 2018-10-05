// <copyright file="PatchCoresAluminioController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Produtos.CoresAluminio;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Produtos.CoresAluminio.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresAluminio
{
    /// <summary>
    /// Controller de cores de alumínio.
    /// </summary>
    public partial class CoresAluminioController : BaseController
    {
        /// <summary>
        /// Atualiza uma cor de alumínio.
        /// </summary>
        /// <param name="id">O identificador da cor de alumínio que será alterada.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados na cor de alumínio indicada.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}")]
        [SwaggerResponse(202, "Cor de alumínio alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Cor de alumínio não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarCorAluminio(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var corAluminio = CorAluminioDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    if (corAluminio == null)
                    {
                        return this.NaoEncontrado($"Cor de alumínio {id} não encontrada.");
                    }

                    corAluminio = new ConverterCadastroAtualizacaoParaCorAluminio(dadosParaAlteracao, corAluminio)
                        .ConverterParaCorAluminio();

                    CorAluminioDAO.Instance.Update(sessao, corAluminio);
                    sessao.Commit();

                    return this.Aceito($"Cor de alumínio atualizada com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar cor de alumínio.", ex);
                }
            }
        }
    }
}
