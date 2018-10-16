// <copyright file="PatchCoresVidroController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Produtos.CoresVidro;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Produtos.V1.CoresVidro.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresVidro
{
    /// <summary>
    /// Controller de cores de vidro.
    /// </summary>
    public partial class CoresVidroController : BaseController
    {
        /// <summary>
        /// Atualiza uma cor de vidro.
        /// </summary>
        /// <param name="id">O identificador da cor de vidro que será alterada.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados na cor de vidro indicada.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}")]
        [SwaggerResponse(202, "Cor de vidro alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Cor de vidro não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarCorVidro(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var corVidro = CorVidroDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    if (corVidro == null)
                    {
                        return this.NaoEncontrado($"Cor de vidro {id} não encontrada.");
                    }

                    corVidro = new ConverterCadastroAtualizacaoParaCorVidro(dadosParaAlteracao, corVidro)
                        .ConverterParaCorVidro();

                    CorVidroDAO.Instance.Update(sessao, corVidro);
                    sessao.Commit();

                    return this.Aceito($"Cor de vidro atualizada com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar cor de vidro.", ex);
                }
            }
        }
    }
}
