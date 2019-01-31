// <copyright file="GetChapasMateriaPrimaController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.RelDAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.MateriaPrima
{
    /// <summary>
    /// Controller de chapas de matéria prima.
    /// </summary>
    public partial class ChapasMateriaPrimaController : BaseController
    {
        /// <summary>
        /// Recupera a lista de chapas de matéria prima.
        /// </summary>
        /// <param name="filtro">Os filtros para busca das chapas de matéria prima.</param>
        /// <returns>Uma lista JSON com os dados das chapas de matéria prima.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Chapas de matéria prima encontradas.", Type = typeof(IEnumerable<Models.Produtos.V1.MateriaPrima.Lista.ListaDto>))]
        [SwaggerResponse(204, "Chapas de matéria prima não encontradas.")]
        public IHttpActionResult ObterListaChapasMateriaPrima([FromUri] Models.Produtos.V1.MateriaPrima.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Produtos.V1.MateriaPrima.Lista.FiltroDto();

                var validacao = this.ValidarExistenciaEspessuraECorVidro(
                    sessao,
                    filtro.IdCorVidro ?? 0,
                    filtro.Espessura ?? 0);

                if (validacao != null)
                {
                    return validacao;
                }

                var chapasMateriaPrima = PosicaoMateriaPrimaChapaDAO.Instance.GetChapaByCorEsp(
                    (uint)(filtro.IdCorVidro ?? 0),
                    (float)(filtro.Espessura ?? 0),
                    null,
                    null);

                return this.Lista(
                    chapasMateriaPrima
                        .Select(cmp => new Models.Produtos.V1.MateriaPrima.Lista.ListaDto(cmp)));
            }
        }

        /// <summary>
        /// Recupera a lista de chapas disponíveis.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com as chapas disponíveis.</returns>
        [HttpGet]
        [Route("chapasDisponiveis")]
        [SwaggerResponse(200, "Chapas disponíveis encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Produtos.MateriaPrima.ChapasDisponiveis.Lista.ListaDto>))]
        [SwaggerResponse(204, "Chapas disponíveis não encontradas.")]
        [SwaggerResponse(206, "Chapas disponíveis paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Produtos.MateriaPrima.ChapasDisponiveis.Lista.ListaDto>))]
        public IHttpActionResult ObterListaChapasDisponiveis([FromUri]Models.Produtos.MateriaPrima.ChapasDisponiveis.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Produtos.MateriaPrima.ChapasDisponiveis.Lista.FiltroDto();

                var chapasDisponiveis = ChapasDisponiveisDAO.Instance.ObtemChapasDisponiveis(
                    (uint)(filtro.IdFornecedor ?? 0),
                    filtro.NomeFornecedor,
                    filtro.CodigoProduto,
                    filtro.DescricaoProduto,
                    filtro.NumeroNotaFiscal ?? 0,
                    filtro.Lote,
                    filtro.Altura ?? 0,
                    filtro.Largura ?? 0,
                    filtro.IdsCorVidro != null && filtro.IdsCorVidro.Any() ? string.Join(",", filtro.IdsCorVidro) : string.Empty,
                    filtro.Espessura ?? 0,
                    filtro.CodigoEtiqueta,
                    filtro.IdLoja ?? 0,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    chapasDisponiveis.Select(cd => new Models.Produtos.MateriaPrima.ChapasDisponiveis.Lista.ListaDto(cd)),
                    filtro,
                    () => ChapasDisponiveisDAO.Instance.ObtemChapasDisponiveisCount(
                    (uint)(filtro.IdFornecedor ?? 0),
                    filtro.NomeFornecedor,
                    filtro.CodigoProduto,
                    filtro.DescricaoProduto,
                    filtro.NumeroNotaFiscal ?? 0,
                    filtro.Lote,
                    filtro.Altura ?? 0,
                    filtro.Largura ?? 0,
                    filtro.IdsCorVidro != null && filtro.IdsCorVidro.Any() ? string.Join(",", filtro.IdsCorVidro) : string.Empty,
                    filtro.Espessura ?? 0,
                    filtro.CodigoEtiqueta,
                    filtro.IdLoja ?? 0));
            }
        }
    }
}
