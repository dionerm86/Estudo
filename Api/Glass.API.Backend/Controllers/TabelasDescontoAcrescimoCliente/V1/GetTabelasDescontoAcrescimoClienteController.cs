// <copyright file="GetTabelasDescontoAcrescimoClienteController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.TabelasDescontoAcrescimoCliente.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.TabelasDescontoAcrescimoCliente.V1
{
    /// <summary>
    /// Controller de tabelas de desconto/acréscimo de cliente.
    /// </summary>
    public partial class TabelasDescontoAcrescimoClienteController : BaseController
    {
        /// <summary>
        /// Recupera a lista de tabelas de desconto/acréscimo de cliente para a tela de listagem.
        /// </summary>
        /// <param name="filtro">O filtro para a busca de tabelas de desconto/acréscimo de cliente.</param>
        /// <returns>Uma lista JSON com os dados das tabelas de desconto/acréscimo de cliente.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Tabelas de desconto/acréscimo de cliente encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Tabelas de desconto/acréscimo de cliente não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Tabelas de desconto/acréscimo de cliente paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        public IHttpActionResult ObterLista([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.IClienteFluxo>();

                var tipos = fluxo.PesquisarTabelasDescontosAcrescimos();

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
        /// Recupera as tabelas de desconto/acréscimo para os controles de filtro das telas.
        /// </summary>
        /// <returns>Uma lista JSON com as tabelas de desconto/acréscimo encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Tabelas de desconto/acréscimo encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tabelas de desconto/acréscimo não encontradas.")]
        public IHttpActionResult ObterFiltro()
        {
            using (var sessao = new GDATransaction())
            {
                var tabelas = TabelaDescontoAcrescimoClienteDAO.Instance.GetSorted()
                    .Select(p => new IdNomeDto
                    {
                        Id = p.IdTabelaDesconto,
                        Nome = p.Descricao,
                    });

                return this.Lista(tabelas);
            }
        }
    }
}
