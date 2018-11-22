// <copyright file="DeleteOrdensCargaComCarregamentoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Carregamentos.V1.OrdensCarga
{
    /// <summary>
    /// Controller de ordens de carga.
    /// </summary>
    public partial class OrdensCargaComCarregamentoController : BaseController
    {
        /// <summary>
        /// Atualiza dados da ordem de carga.
        /// </summary>
        /// <param name="idCarregamento">O identificador do carregamento.</param>
        /// <param name="id">O identificador da ordem de carga que será desassociada.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpDelete]
        [Route("{id}")]
        [SwaggerResponse(202, "Ordem de carga desassociada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Carrgamento ou Ordem de carga não encontrados para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult DesassociarDoCarregamento(int idCarregamento, int id)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdCarregamento(sessao, id)
                        ?? this.ValidarExistenciaIdOrdemCarga(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    WebGlass.Business.OrdemCarga.Fluxo.OrdemCargaFluxo.Instance.RetiraOcCarregamento(sessao, (uint)idCarregamento, (uint)id);

                    sessao.Commit();

                    return this.Aceito($"Ordem de carga desassociada.");
                }
                catch (Exception ex)
                {
                    sessao.Rollback();
                    return this.ErroValidacao($"Erro ao desassociar ordem de carga.", ex);
                }
            }
        }
    }
}
