// <copyright file="DeleteCoresAluminioController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.CoresAluminio
{
    /// <summary>
    /// Controller de cores de alumínio.
    /// </summary>
    public partial class CoresAluminioController : BaseController
    {
        /// <summary>
        /// Exclui uma cor de alumínio.
        /// </summary>
        /// <param name="id">O identificador da cor de alumínio que será excluída.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Cor de alumínio excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Cor de alumínio não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirCorAluminio(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdCorAluminio(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    CorAluminioDAO.Instance.DeleteByPrimaryKey(sessao, id);
                    sessao.Commit();

                    return this.Aceito($"Cor de alumínio excluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir cor de alumínio.", ex);
                }
            }
        }
    }
}
