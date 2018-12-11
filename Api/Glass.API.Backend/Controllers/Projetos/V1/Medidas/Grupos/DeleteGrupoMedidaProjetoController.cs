// <copyright file="DeleteGrupoMedidaProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.Medidas.Grupos
{
    /// <summary>
    /// Controller de Grupos de medida de projeto.
    /// </summary>
    public partial class GrupoMedidaProjetoController : BaseController
    {
        /// <summary>
        /// Exclui um grupo de medida de projeto.
        /// </summary>
        /// <param name="id">O identificador do grupo de medida de projeto que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Grupo de medida de projeto excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Grupo de medida de projeto não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirGrupoMedidaProjeto(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdGrupoMedidaProjeto(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    GrupoMedidaProjetoDAO.Instance.DeleteByPrimaryKey(sessao, id);
                    sessao.Commit();

                    return this.Aceito(string.Format("Grupo de medida de projeto {0} excluído com sucesso!", id));
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao excluir o grupo de medida de projeto.", e);
                }
            }
        }
    }
}