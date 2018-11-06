// <copyright file="DeleteMedidasProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.MedidasProjeto
{
    /// <summary>
    /// Controller de medidas de projeto.
    /// </summary>
    public partial class MedidasProjetoController : BaseController
    {
        /// <summary>
        /// Exclui uma medida de projeto.
        /// </summary>
        /// <param name="id">O identificador da medida de projeto que será excluída.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Medida de projeto excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Medida de projeto não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirMedidaProjeto(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdMedidaProjeto(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    MedidaProjetoDAO.Instance.DeleteByPrimaryKey(sessao, (uint)id);

                    sessao.Commit();

                    return this.Aceito($"Medida de projeto excluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir medida de projeto.", ex);
                }
            }
        }
    }
}
