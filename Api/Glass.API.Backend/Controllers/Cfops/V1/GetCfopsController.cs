// <copyright file="GetCfopsController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cfops.V1
{
    /// <summary>
    /// Controller de cfops.
    /// </summary>
    public partial class CfopsController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de CFOP's.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Cfops.V1.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaCfops()
        {
            var configuracoes = new Models.Cfops.V1.Configuracoes.ListaDto();
            return this.Item(configuracoes);
        }

        /// <summary>
        /// Recupera os cfops para o controle de pesquisa.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos cfops encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "CFOPs encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "CFOPs não encontrados.")]
        public IHttpActionResult ObterFiltro()
        {
            var cfops = CfopDAO.Instance.ObterListaOrdenadaPeloCodInterno()
                .Select(c => new IdNomeDto()
                {
                    Id = c.IdCfop,
                    Nome = c.CodInterno,
                });

            return this.Lista(cfops);
        }

        /// <summary>
        /// Recupera os tipos de CFOP para o controle de pesquisa.
        /// </summary>
        /// <returns>Uma lista JSON com os dados dos tipos de CFOP encontrados.</returns>
        [HttpGet]
        [Route("tipos")]
        [SwaggerResponse(200, "Tipos de CFOP encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de CFOP não encontrados.")]
        public IHttpActionResult ObterTipos()
        {
            var tiposCfop = TipoCfopDAO.Instance.GetAll()
                .Select(c => new IdNomeDto()
                {
                    Id = c.IdTipoCfop,
                    Nome = c.Descricao,
                });

            return this.Lista(tiposCfop);
        }

        /// <summary>
        /// Recupera a lista de CFOP.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos CFOP's.</param>
        /// <returns>Uma lista JSON com os dados dos CFOP's.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "CFOP's sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Cfops.V1.Lista.ListaDto>))]
        [SwaggerResponse(204, "Grupos de produto não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Grupos de produto paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Cfops.V1.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaCfops([FromUri] Models.Cfops.V1.Lista.FiltroDto filtro)
        {
            filtro = filtro ?? new Models.Cfops.V1.Lista.FiltroDto();

            var cfops = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Fiscal.Negocios.ICfopFluxo>()
                .PesquisarCfops(filtro.Codigo, filtro.Descricao);

            ((Colosoft.Collections.IVirtualList)cfops).Configure(filtro.NumeroRegistros);
            ((Colosoft.Collections.ISortableCollection)cfops).ApplySort(filtro.ObterTraducaoOrdenacao());

            return this.ListaPaginada(
                cfops
                    .Skip(filtro.ObterPrimeiroRegistroRetornar())
                    .Take(filtro.NumeroRegistros)
                    .Select(c => new Models.Cfops.V1.Lista.ListaDto(c)),
                filtro,
                () => cfops.Count);
        }
    }
}
