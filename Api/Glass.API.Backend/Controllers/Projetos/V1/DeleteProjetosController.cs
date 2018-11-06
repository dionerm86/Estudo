// <copyright file="DeleteProjetosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1
{
    /// <summary>
    /// Controller de projetos.
    /// </summary>
    public partial class ProjetosController : BaseController
    {
        /// <summary>
        /// Exclui um projeto.
        /// </summary>
        /// <param name="id">O identificador do projeto que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Projeto excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Projeto não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirProjeto(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdProjeto(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    ProjetoDAO.Instance.DeleteByPrimaryKey(sessao, id);
                    sessao.Commit();

                    return this.Aceito(string.Format("Projeto {0} excluído com sucesso!", id));
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao excluir o projeto.", e);
                }
            }
        }
    }
}
