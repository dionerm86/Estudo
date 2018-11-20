// <copyright file="PatchOrdensCargaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Carregamentos.V1.OrdensCarga
{
    /// <summary>
    /// Controller de ordens de carga.
    /// </summary>
    public partial class OrdensCargaController : BaseController
    {
        /// <summary>
        /// Atualiza dados da ordem de carga.
        /// </summary>
        /// <param name="id">O identificador da ordem de carga que será desassociada.</param>
        /// <param name="idCarregamento">O identificador do carregamento.</param>
        /// <returns>O status HTTP que representa o resultado da operação.</returns>
        [HttpPatch]
        [Route("{id}/desassociar")]
        [SwaggerResponse(202, "Ordem de carga desassociada.", Type = typeof(MensagemDto))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Ordem de carga não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult DesassociarDoCarregamento(int id, [FromBody] IdDto idCarregamento)
        {
            using (var sessao = new GDATransaction())
            {
                try
                {
                    sessao.BeginTransaction();

                    var validacao = this.ValidarExistenciaIdOrdemCarga(sessao, id);

                    if (validacao != null)
                    {
                        return validacao;
                    }

                    if (idCarregamento == null || idCarregamento.Id == null)
                    {
                        return this.ErroValidacao("O carregamento é obrigatório.");
                    }

                    WebGlass.Business.OrdemCarga.Fluxo.OrdemCargaFluxo.Instance.RetiraOcCarregamento(sessao, (uint)idCarregamento.Id.Value, (uint)id);

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
