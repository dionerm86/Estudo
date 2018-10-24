// <copyright file="GetContasBancariasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ContasBancarias.V1
{
    /// <summary>
    /// Controller de contas bancárias.
    /// </summary>
    public partial class ContasBancariasController : BaseController
    {
        /// <summary>
        /// Recupera a lista de contas bancárias.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das contas bancárias.</param>
        /// <returns>Uma lista JSON com os dados das contas bancárias.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Contas bancárias sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.ContasBancarias.V1.Lista.ListaDto>))]
        [SwaggerResponse(204, "Contas bancárias não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Contas bancárias paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.ContasBancarias.V1.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaContasBancarias([FromUri] Models.ContasBancarias.V1.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.ContasBancarias.V1.Lista.FiltroDto();

                var contasBancarias = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Financeiro.Negocios.IContaBancariaFluxo>()
                    .PesquisarContasBanco();

                ((Colosoft.Collections.IVirtualList)contasBancarias).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)contasBancarias).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    contasBancarias
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(c => new Models.ContasBancarias.V1.Lista.ListaDto(c)),
                    filtro,
                    () => contasBancarias.Count);
            }
        }

        /// <summary>
        /// Recupera a lista de bancos.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos bancos.</returns>
        [HttpGet]
        [Route("bancos")]
        [SwaggerResponse(200, "Bancos encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Bancos não encontrados.")]
        public IHttpActionResult ObterSequencias()
        {
            using (var sessao = new GDATransaction())
            {
                var bancos = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Financeiro.Negocios.IContaBancariaFluxo>()
                    .ObtemBancos()
                    .Select(f => new IdNomeDto
                    {
                        Id = f.Id,
                        Nome = f.Name,
                    });

                return this.Lista(bancos);
            }
        }
    }
}
