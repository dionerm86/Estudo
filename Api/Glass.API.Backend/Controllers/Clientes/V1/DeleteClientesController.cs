// <copyright file="DeleteClientesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
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
        /// Exclui um cliente, se possível.
        /// </summary>
        /// <param name="id">O identificador do cliente que será excluído.</param>
        /// <returns>Um status HTTP indicando se o cliente foi excluído.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Cliente excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de valor ou formato do campo id ou de validação na exclusão do cliente.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Cliente não encontrado para o filtro informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirCliente(int id)
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
                    var clienteFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Glass.Global.Negocios.IClienteFluxo>();

                    var cliente = clienteFluxo.ObtemCliente(id);

                    var resultado = clienteFluxo.ApagarCliente(cliente);

                    return resultado ? this.Aceito(string.Format("Cliente {0} excluído com sucesso!", id)) :
                        (IHttpActionResult)this.ErroValidacao($"Erro ao excluir o cliente. {resultado.Message.Format()}");
                }
                catch (Exception e)
                {
                    sessao.Rollback();
                    return this.ErroValidacao("Erro ao excluir o cliente.", e);
                }
            }
        }
    }
}
