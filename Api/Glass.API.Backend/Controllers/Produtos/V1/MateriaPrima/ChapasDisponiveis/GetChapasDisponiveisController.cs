// <copyright file="GetChapasDisponiveisController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Produtos.MateriaPrima.ChapasDisponiveis.V1.Lista;
using Glass.Data.RelDAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.MateriaPrima.ChapasDisponiveis
{
    /// <summary>
    /// Controller de chapas disponíveis.
    /// </summary>
    public partial class ChapasDisponiveisController : BaseController
    {
        /// <summary>
        /// Recupera a lista de chapas disponíveis.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com as chapas disponíveis.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Chapas disponíveis encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Chapas disponíveis não encontradas.")]
        [SwaggerResponse(206, "Chapas disponíveis paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterListaChapasDisponiveis([FromUri]FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var chapasDisponiveis = ChapasDisponiveisDAO.Instance.ObtemChapasDisponiveis(
                    (uint)(filtro.IdFornecedor ?? 0),
                    filtro.NomeFornecedor,
                    filtro.CodigoProduto,
                    filtro.DescricaoProduto,
                    (int)(filtro.NumeroNotaFiscal ?? 0),
                    filtro.Lote,
                    (int)(filtro.Altura ?? 0),
                    (int)(filtro.Largura ?? 0),
                    filtro.IdsCorVidro,
                    (int)(filtro.Espessura ?? 0),
                    filtro.CodigoEtiqueta,
                    (int)(filtro.IdLoja ?? 0),
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    chapasDisponiveis.Select(cd => new ListaDto(cd)),
                    filtro,
                    () => ChapasDisponiveisDAO.Instance.ObtemChapasDisponiveisCount(
                    (uint)(filtro.IdFornecedor ?? 0),
                    filtro.NomeFornecedor,
                    filtro.CodigoProduto,
                    filtro.DescricaoProduto,
                    (int)(filtro.NumeroNotaFiscal ?? 0),
                    filtro.Lote,
                    (int)(filtro.Altura ?? 0),
                    (int)(filtro.Largura ?? 0),
                    filtro.IdsCorVidro,
                    (int)(filtro.Espessura ?? 0),
                    filtro.CodigoEtiqueta,
                    (int)(filtro.IdLoja ?? 0)));
            }
        }
    }
}
