// <copyright file="GetOrdensCargaComCarregamentoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Carregamentos.V1.OrdensCarga;
using Glass.API.Backend.Helper.Respostas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Carregamentos.V1.OrdensCarga
{
    /// <summary>
    /// Controller de ordens de carga.
    /// </summary>
    public partial class OrdensCargaComCarregamentoController : BaseController
    {
        /// <summary>
        /// Recupera a lista de ordens de carga.
        /// </summary>
        /// <param name="idCarregamento">O carregamento usado para carregar as ordens de carga.</param>
        /// <returns>Uma lista JSON com os dados das ordens de carga.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Ordens de carga sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Carregamentos.V1.OrdensCarga.Carregamento.ListaDto>))]
        [SwaggerResponse(204, "Ordens de carga não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Ordens de carga paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Carregamentos.V1.OrdensCarga.Carregamento.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Carregamento não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaOrdensCarga(int idCarregamento)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarExistenciaIdCarregamento(sessao, idCarregamento);

                if (validacao != null)
                {
                    return validacao;
                }

                var ordensCarga = OrdemCargaDAO.Instance.ObterOrdensCargaPeloCarregamento(sessao, idCarregamento);

                return this.Lista(ordensCarga.Select(o => new Models.Carregamentos.V1.OrdensCarga.Carregamento.ListaDto(o)));
            }
        }
    }
}
