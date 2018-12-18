// <copyright file="PostOrdensCargaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;
using WebGlass.Business.OrdemCarga.Fluxo;

namespace Glass.API.Backend.Controllers.Carregamentos.V1.OrdensCarga.Lista
{
    /// <summary>
    /// Controller de ordens de carga.
    /// </summary>
    public partial class OrdensCargaController : BaseController
    {
        /// <summary>
        /// Recupera a permissão para associar pedidos a ordem de carga.
        /// </summary>
        /// <param name="id">O identificador da ordem de carga.</param>
        /// <returns>Um objeto JSON que define a permissão para associar pedidos.</returns>
        [HttpPost]
        [Route("{id:int}/verificarPermissaoAssociarPedidos")]
        [SwaggerResponse(200, "Associação de pedidos autorizada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Ordem de carga não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterPermissaoAssociarPedidos(int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    var validacao = this.ValidarExistenciaIdOrdemCarga(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    var podeAssociarPedidosOrdemCarga = OrdemCargaFluxo.Instance.PodeAdicionarPedido((uint)id);

                    if (!podeAssociarPedidosOrdemCarga)
                    {
                        return this.ErroValidacao($"Associação de pedidos a ordem de carga não autorizada.");
                    }

                    return this.Aceito($"Associação de pedidos a ordem de carga autorizada.");
                }
                catch (Exception ex)
                {
                    return this.ErroValidacao($"Erro ao obter permissão para associar pedidos a ordem de carga.", ex);
                }
            }
        }
    }
}