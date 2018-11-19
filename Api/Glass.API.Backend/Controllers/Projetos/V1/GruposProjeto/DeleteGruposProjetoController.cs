// <copyright file="DeleteGruposProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
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
        /// Exclui um grupo de projeto.
        /// </summary>
        /// <param name="id">O identificador do grupo de projeto que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Grupo de projeto excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Grupo de projeto não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirGrupoProjeto(int id)
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

                    GrupoModeloDAO.Instance.DeleteByPrimaryKey(sessao, id);

                    sessao.Commit();

                    return this.Aceito($"Grupo de projeto excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir grupo de projeto.", ex);
                }
            }
        }
    }
}
