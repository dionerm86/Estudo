// <copyright file="GetPosicaoMateriaPrimaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.Produtos.V1.MateriaPrima.Posicao.Lista;
using Glass.Data.Model;
using Glass.Data.RelDAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.MateriaPrima.Posicao
{
    /// <summary>
    /// Controller de posição de matéria prima.
    /// </summary>
    public partial class PosicaoMateriaPrimaController : BaseController
    {
        /// <summary>
        /// Recupera a lista de posições de matéria prima.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das posições de matéria prima.</param>
        /// <returns>Uma lista JSON com os dados das posições de matéria prima.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Posições de matéria prima encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Posições de matéria prima não encontradas.")]
        [SwaggerResponse(206, "Posições de matéria prima paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterListaPosicoesMateriaPrima([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var posicoesMateriaPrima = PosicaoMateriaPrimaDAO.Instance.GetPosMateriaPrima(
                    filtro.IdsRota != null && filtro.IdsRota.Any() ? string.Join(",", filtro.IdsRota) : string.Empty,
                    filtro.TiposPedido.ToString(),
                    filtro.SituacoesPedido.ToString(),
                    filtro.PeriodoEntregaPedidoInicio?.ToShortDateString(),
                    filtro.PeriodoEntregaPedidoFim?.ToShortDateString(),
                    filtro.IdsCorVidro != null && filtro.IdsCorVidro.Any() ? string.Join(",", filtro.IdsCorVidro) : string.Empty,
                    (float)(filtro.Espessura ?? 0),
                    filtro.BuscarApenasEstoqueDisponivelNegativo.GetValueOrDefault(false));

                return this.ListaPaginada(
                    posicoesMateriaPrima
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(pmp => new ListaDto(pmp)),
                    filtro,
                    () => posicoesMateriaPrima.Count);
            }
        }
    }
}
