// <copyright file="GetTiposClienteController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Models.Clientes.Tipos.Lista;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Clientes.V1.Tipos
{
    /// <summary>
    /// Controller de tipos de cliente.
    /// </summary>
    public partial class TiposClienteController : BaseController
    {
        /// <summary>
        /// Recupera a lista de tipos de cliente para a tela de listagem.
        /// </summary>
        /// <param name="filtro">O filtro para a busca de tipos de cliente.</param>
        /// <returns>Uma lista JSON com os dados dos tipos de cliente.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Tipos de cliente encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Tipos de cliente não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Tipos de cliente paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterLista([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.IClienteFluxo>();

                var tipos = fluxo.PesquisaTiposCliente();

                ((Colosoft.Collections.IVirtualList)tipos).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)tipos).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    tipos
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(entidade => new ListaDto(entidade)),
                    filtro,
                    () => tipos.Count);
            }
        }

        /// <summary>
        /// Recupera a lista de tipos de cliente.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos de tipos de cliente.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Tipos encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos não encontrados.")]
        public IHttpActionResult ObterTipos()
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = TipoClienteDAO.Instance.GetAll(sessao)
                    .Select(s => new IdNomeDto
                    {
                        Id = s.IdTipoCliente,
                        Nome = s.Descricao,
                    });

                return this.Lista(tipos);
            }
        }

        /// <summary>
        /// Recupera a lista de tipos fiscal de cliente.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos de tipos fiscal de cliente.</returns>
        [HttpGet]
        [Route("fiscal/filtro")]
        [SwaggerResponse(200, "Tipos fiscal encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos fiscal não encontrados.")]
        public IHttpActionResult ObterTiposFiscal()
        {
            using (var sessao = new GDATransaction())
            {
                var tiposFiscal = new ConversorEnum<Data.Model.TipoFiscalCliente>()
                    .ObterTraducao();

                return this.Lista(tiposFiscal);
            }
        }
    }
}
