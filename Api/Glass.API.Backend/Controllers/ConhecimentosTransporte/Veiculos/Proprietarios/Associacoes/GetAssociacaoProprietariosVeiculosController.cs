// <copyright file="GetAssociacaoProprietariosVeiculosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.ConhecimentosTransporte.V1.Veiculos.Proprietarios.Lista;
using Glass.Data.DAL.CTe;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ConhecimentosTransporte.Veiculos.Proprietarios.Associacoes
{
    /// <summary>
    /// Controller de associações de proprietários com veículos.
    /// </summary>
    public partial class AssociacaoProprietariosVeiculosController : BaseController
    {
        /// <summary>
        /// Recupera a lista de associações de proprietários com veículos.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Associações de proprietários com veículos encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.ConhecimentosTransporte.V1.Veiculos.Proprietarios.Associacoes.ListaDto>))]
        [SwaggerResponse(204, "Associações de proprietários com veículos não encontradas.")]
        [SwaggerResponse(206, "Associações de proprietários com veículos paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.ConhecimentosTransporte.V1.Veiculos.Proprietarios.Associacoes.ListaDto>))]
        public IHttpActionResult ObterListaAssociacaoProprietariosVeiculos([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var associacoesProprietariosVeiculos = ProprietarioVeiculo_VeiculoDAO.Instance.GetList(
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    associacoesProprietariosVeiculos.Select(p => new Models.ConhecimentosTransporte.V1.Veiculos.Proprietarios.Associacoes.ListaDto(p)),
                    filtro,
                    () => ProprietarioVeiculo_VeiculoDAO.Instance.GetCount());
            }
        }
    }
}