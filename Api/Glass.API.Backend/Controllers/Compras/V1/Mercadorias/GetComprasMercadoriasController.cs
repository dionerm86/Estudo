// <copyright file="GetComprasMercadoriasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Compras.V1.Mercadorias.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Compras.V1.Mercadorias
{
    /// <summary>
    /// Controller de compras de mercadorias.
    /// </summary>
    public partial class ComprasMercadoriasController : BaseController
    {
        /// <summary>
        /// Recupera a lista de compras de mercadorias.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das compras de mercadorias.</param>
        /// <returns>Uma lista JSON com os dados das compras de mercadorias.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Compras de mercadoria encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Compras de mercadoria não encontradas.")]
        [SwaggerResponse(206, "Compras de mercadoria paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterListaComprasMercadorias([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var mercadorias = CompraDAO.Instance.GetListPcp(
                    (uint)(filtro.Id ?? 0),
                    (uint)(filtro.IdPedido ?? 0),
                    (uint)(filtro.IdFornecedor ?? 0),
                    filtro.NomeFornecedor,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    mercadorias.Select(cm => new ListaDto(cm)),
                    filtro,
                    () => CompraDAO.Instance.GetCountPcp(
                        (uint)(filtro.Id ?? 0),
                        (uint)(filtro.IdPedido ?? 0),
                        (uint)(filtro.IdFornecedor ?? 0),
                        filtro.NomeFornecedor));
            }
        }
    }
}
