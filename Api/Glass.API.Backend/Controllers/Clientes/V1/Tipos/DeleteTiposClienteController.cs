// <copyright file="DeleteTiposClienteController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Clientes.V1.Tipos
{
    /// <summary>
    /// Controller de tipos de cliente.
    /// </summary>
    public partial class TiposClienteController : BaseController
    {
        /// <summary>
        /// Exclui um tipo de cliente.
        /// </summary>
        /// <param name="id">O identificador do tipo de cliente que será excluído.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Tipo de cliente excluído.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Tipo de cliente não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirTipo(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdTipoCliente(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Global.Negocios.IClienteFluxo>();

                    var tipoCliente = fluxo.ObtemTipoCliente(id);

                    var resultado = fluxo.ApagarTipoCliente(tipoCliente);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao excluir tipo de cliente. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Tipo de cliente excluído.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir tipo de cliente.", ex);
                }
            }
        }
    }
}
