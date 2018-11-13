// <copyright file="GetOrdensCargaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Carregamentos.V1.OrdensCarga
{
    /// <summary>
    /// Controller de ordens de carga.
    /// </summary>
    public partial class OrdensCargaController : BaseController
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
        public IHttpActionResult ObterListaCarregamentos([FromUri] IdDto idCarregamento)
        {
            using (var sessao = new GDATransaction())
            {
                if (idCarregamento == null || idCarregamento.Id == null)
                {
                    return this.ErroValidacao("Carregamento não informado.");
                }

                var carregamentos = OrdemCargaDAO.Instance.ObterOrdensCargaPeloCarregamento(null, idCarregamento.Id.Value);

                return this.Lista(carregamentos);
            }
        }
    }
}
