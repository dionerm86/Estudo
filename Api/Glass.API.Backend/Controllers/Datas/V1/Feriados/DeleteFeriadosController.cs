// <copyright file="DeleteFeriadosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Datas.V1.Feriados
{
    /// <summary>
    /// Controller de feriados.
    /// </summary>
    public partial class FeriadosController : BaseController
    {
        /// <summary>
        /// Exclui um feriado.
        /// </summary>
        /// <param name="id">O identificador do feriado que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Feriado excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Feriado não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirFeriado(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdFeriado(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    FeriadoDAO.Instance.DeleteByPrimaryKey(sessao, id);
                    sessao.Commit();

                    return this.Aceito($"Feriado excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir feriado.", ex);
                }
            }
        }
    }
}
