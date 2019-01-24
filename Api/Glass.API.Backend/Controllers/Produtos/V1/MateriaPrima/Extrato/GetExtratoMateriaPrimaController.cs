// <copyright file="GetExtratoMateriaPrimaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Produtos.V1.MateriaPrima.Extrato.Lista;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.MateriaPrima.Extrato
{
    /// <summary>
    /// Controller de extrato de movimentação de matéria prima.
    /// </summary>
    public partial class ExtratoMateriaPrimaController : BaseController
    {
        /// <summary>
        /// Busca os totalizadores para a tela de listagem de extrato de movimentação de chapas de vidro.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os totalizadores para a lista de extrato de movimentação de chapas de vidro.</returns>
        [HttpGet]
        [Route("totalizadores")]
        [SwaggerResponse(200, "Totalizadores do extrato de movimentação de chapa encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Totalizadores do extrato de movimentação de chapa não encontrados.")]
        [SwaggerResponse(206, "Totalizadores do extrato de movimentação de chapa paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterListaTotalizadores([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var totalizadores = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Estoque.Negocios.IMovChapaFluxo>()
                    .ObtemMovChapa(
                        filtro.IdsCorVidro != null && filtro.IdsCorVidro.Any() ? string.Join(",", filtro.IdsCorVidro) : string.Empty,
                        (float)(filtro.Espessura ?? 0),
                        filtro.Altura ?? 0,
                        filtro.Largura ?? 0,
                        filtro.PeriodoMovimentacaoInicio != null ? Convert.ToDateTime(filtro.PeriodoMovimentacaoInicio?.ToShortDateString()) : DateTime.Now,
                        filtro.PeriodoMovimentacaoFim != null ? Convert.ToDateTime(filtro.PeriodoMovimentacaoFim?.ToShortDateString()) : DateTime.Now).ToList();

                return this.ListaPaginada(
                    totalizadores
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(t => new ListaDto(t)),
                    filtro,
                    () => totalizadores.Count);
            }
        }
    }
}