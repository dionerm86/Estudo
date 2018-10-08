// <copyright file="PostSugestaoClienteController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Microsoft.Practices.ServiceLocation;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.V1.SugestoesCliente
{
    /// <summary>
    /// Controller de sugestão de clientes.
    /// </summary>
    public partial class SugestaoClienteController : BaseController
    {
        /// <summary>
        /// Cancela a sugestão do cliente.
        /// </summary>
        /// <param name="id">O identificador do cliente que será ativado/inativado.</param>
        /// <returns>Um status HTTP indicando se o cliente foi ativado/inativado.</returns>
        [HttpPost]
        [Route("{id}/cancelar")]
        [SwaggerResponse(202, "Sugestão de clientes cancelada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação no cancelamento da sugestão de cliente.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Sugestão de cliente não encontrado para o filtro informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult CancelarSugestao(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdSugestao(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    SugestaoClienteDAO.Instance.Cancelar(id);

                    sessao.Commit();

                    return this.Aceito("Sugestão do cliente cancelada com sucesso.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao(ex.Message, ex);
                }
            }
        }
    }
}
