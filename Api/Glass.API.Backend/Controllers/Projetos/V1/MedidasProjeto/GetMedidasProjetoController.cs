// <copyright file="GetMedidasProjetoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Projetos.V1.MedidasProjeto.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Projetos.V1.MedidasProjeto
{
    /// <summary>
    /// Controller de medida de projeto.
    /// </summary>
    public partial class MedidasProjetoController : BaseController
    {
        /// <summary>
        /// Recupera a lista de medidas de projeto.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das medidas de projeto.</param>
        /// <returns>Uma lista JSON com os dados das medidas de projeto.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Medidas de projeto sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Medidas de projeto não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Medidas de projeto paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaMedidasProjeto([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var medidasProjeto = MedidaProjetoDAO.Instance.GetList(
                    filtro.Descricao,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    medidasProjeto.Select(g => new ListaDto(g)),
                    filtro,
                    () => MedidaProjetoDAO.Instance.GetCount(
                        filtro.Descricao));
            }
        }
    }
}
