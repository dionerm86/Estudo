// <copyright file="GetProprietariosDeVeiculosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.ConhecimentosTransporte.V1.Veiculos.Proprietarios.Lista;
using Glass.Data.DAL.CTe;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ConhecimentosTransporte.Veiculos.Proprietarios
{
    /// <summary>
    /// Controller de proprietários de veículos.
    /// </summary>
    public partial class ProprietariosDeVeiculosController : BaseController
    {
        /// <summary>
        /// Recupera a lista de proprietários de veículos.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Proprietários de veículos encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Proprietários de veículos não encontrados.")]
        [SwaggerResponse(206, "Proprietários de veículos paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterProprietariosDeVeiculos([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var proprietariosDeVeiculos = ProprietarioVeiculoDAO.Instance.GetList(
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    proprietariosDeVeiculos.Select(p => new ListaDto(p)),
                    filtro,
                    () => ProprietarioVeiculoDAO.Instance.GetCount());
            }
        }
    }
}