// <copyright file="PatchGruposProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Projetos.GruposProjeto;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Projetos.V1.GruposProjeto.CadastroAtualizacao;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.GruposProjeto
{
    /// <summary>
    /// Controller de grupos de projeto.
    /// </summary>
    public partial class GruposProjetoController : BaseController
    {
        /// <summary>
        /// Atualiza um grupo de projeto.
        /// </summary>
        /// <param name="id">O identificador do grupo de projeto que será alterado.</param>
        /// <param name="dadosParaAlteracao">Os novos dados que serão alterados no grupo de projeto indicado.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Grupos de projeto alterado.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Grupos de projeto não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AtualizarGrupoProjeto(int id, [FromBody] CadastroAtualizacaoDto dadosParaAlteracao)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdGrupoProjeto(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var grupoProjetoAtual = GrupoModeloDAO.Instance.GetElementByPrimaryKey(id);

                    grupoProjetoAtual = new ConverterCadastroAtualizacaoParaGrupoProjeto(dadosParaAlteracao, grupoProjetoAtual)
                        .ConverterParaGrupoProjeto();

                    GrupoModeloDAO.Instance.Update(sessao, grupoProjetoAtual);

                    sessao.Commit();

                    return this.Aceito($"Grupos de projeto atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao atualizar grupo de projeto.", ex);
                }
            }
        }
    }
}
