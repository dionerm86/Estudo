// <copyright file="DeleteContabilistasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Contabilistas.V1
{
    /// <summary>
    /// Controller de contabilistas.
    /// </summary>
    public partial class ContabilistasController : BaseController
    {
        /// <summary>
        /// Exclui um contabilista.
        /// </summary>
        /// <param name="id">O identificador do contabilista que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponse(202, "Contabilista excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Contabilista não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirContabilista(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdContabilista(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    ContabilistaDAO.Instance.DeleteByPrimaryKey(sessao, id);

                    sessao.Commit();

                    return this.Aceito($"Contabilista excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir contabilista.", ex);
                }
            }
        }
    }
}
