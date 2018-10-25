// <copyright file="DeleteCondutoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Condutores.V1
{
    /// <summary>
    /// Controller de Condutores.
    /// </summary>
    public partial class CondutoresController : BaseController
    {
        /// <summary>
        /// Exclui um condutor.
        /// </summary>
        /// <param name="id">O identificador do condutor que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Condutor excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Condutor não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirCondutor(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdCondutor(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    CondutoresDAO.Instance.DeleteByPrimaryKey(sessao, id);
                    sessao.Commit();

                    return this.Aceito($"Condutor {id} excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir condutor {id}.", ex);
                }
            }
        }
    }
}
