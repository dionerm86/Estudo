// <copyright file="PostClientesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Clientes.V1
{
    /// <summary>
    /// Controller de clientes.
    /// </summary>
    public partial class ClientesController : BaseController
    {
        /// <summary>
        /// Ativa/inativa o cliente.
        /// </summary>
        /// <param name="id">O identificador do cliente que será ativado/inativado.</param>
        /// <returns>Um status HTTP indicando se o cliente foi ativado/inativado.</returns>
        [HttpPost]
        [Route("{id}/alterarSituacao")]
        [SwaggerResponse(202, "Situação do cliente alterada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação na alteração da situação do cliente.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Cliente não encontrado para o filtro informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult AlterarSituacao(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdCliente(sessao, id);

                if (validacao != null)
                {
                    return validacao;
                }

                try
                {
                    sessao.BeginTransaction();

                    ClienteDAO.Instance.AlteraSituacao(sessao, (uint)id);

                    sessao.Commit();

                    return this.Aceito("Situação do cliente alterada com sucesso.");
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
