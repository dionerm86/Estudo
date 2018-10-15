// <copyright file="PatchCoresFerragemController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Produtos.CoresFerragem;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Produtos.CoresFerragem.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresFerragem
{
    /// <summary>
    /// Controller de cores de ferragem.
    /// </summary>
    public partial class CoresFerragemController : BaseController
    {
        /// <summary>
        /// Atualiza uma cor de ferragem.
        /// </summary>
        /// <param name="id">O identificador da cor de ferragem que será alterada.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados na cor de ferragem indicada.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}")]
        [SwaggerResponse(202, "Cor de ferragem alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Cor de ferragem não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarCorFerragem(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var corFerragem = CorFerragemDAO.Instance.GetElementByPrimaryKey(sessao, id);

                    if (corFerragem == null)
                    {
                        return this.NaoEncontrado($"Cor de ferragem {id} não encontrada.");
                    }

                    corFerragem = new ConverterCadastroAtualizacaoParaCorFerragem(dadosParaAlteracao, corFerragem)
                        .ConverterParaCorFerragem();

                    CorFerragemDAO.Instance.Update(sessao, corFerragem);
                    sessao.Commit();

                    return this.Aceito($"Cor de ferragem atualizada com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar cor de ferragem.", ex);
                }
            }
        }
    }
}
