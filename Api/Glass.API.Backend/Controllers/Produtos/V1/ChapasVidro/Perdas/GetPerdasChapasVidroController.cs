// <copyright file="GetPerdasChapasVidroController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Produtos.V1.ChapasVidro.Perdas.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Produtos.V1.ChapasVidro.Perdas
{
    /// <summary>
    /// Controller de produtos.
    /// </summary>
    public partial class PerdasChapasVidroController : BaseController
    {
        /// <summary>
        /// Recupera a lista de produtos.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os dados dos produtos.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Perdas de chapa de vidro encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Perdas de chapa de vidro não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Perdas de chapa de vidro encontradas paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaPerdasChapasVidro([FromUri]FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Produtos.V1.Lista.FiltroDto();

                var perdasChapasVidro = PerdaChapaVidroDAO.Instance.GetListPerdaChapaVidro(
                    filtro.Id,
                    filtro.id
                    );

                ((Colosoft.Collections.IVirtualList)produtos).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)produtos).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    produtos
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(c => new Models.Produtos.V1.Lista.ListaDto(c)),
                    filtro,
                    () => produtos.Count);
            }
        }
    }
}