// <copyright file="DeleteTabelasDescontoAcrescimoClienteController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.TabelasDescontoAcrescimoCliente.V1
{
    /// <summary>
    /// Controller de tabelas de desconto/acréscimo de cliente.
    /// </summary>
    public partial class TabelasDescontoAcrescimoClienteController : BaseController
    {
        /// <summary>
        /// Exclui uma tabela de desconto/acréscimo de cliente.
        /// </summary>
        /// <param name="id">O identificador da tabela de desconto/acréscimo de cliente que será excluída.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Tabela de desconto/acréscimo de cliente excluída.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Tabela de desconto/acréscimo de cliente não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ExcluirTabelasDescontoAcrescimoCliente(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdTabelaDescontoAcrescimoCliente(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Global.Negocios.IClienteFluxo>();

                    var tabela = fluxo.ObtemTabelaDescontoAcrescimoCliente(id);

                    var resultado = fluxo.ApagarTabelaDescontoAcrescimo(tabela);

                    if (!resultado)
                    {
                        return this.ErroValidacao($"Falha ao excluir tabela de desconto/acréscimo de cliente. {resultado.Message.Format()}");
                    }

                    return this.Aceito($"Tabela de desconto/acréscimo de cliente excluída.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao excluir tabela de desconto/acréscimo.", ex);
                }
            }
        }
    }
}
