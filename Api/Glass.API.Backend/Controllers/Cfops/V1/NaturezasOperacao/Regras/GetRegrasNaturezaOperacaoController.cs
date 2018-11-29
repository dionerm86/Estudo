// <copyright file="GetRegrasNaturezaOperacaoController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao
{
    /// <summary>
    /// Controller de regras de natureza de operação.
    /// </summary>
    public partial class RegrasNaturezaOperacaoController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de regras de natureza de operação.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaRegrasNaturezaOperacao()
        {
            var configuracoes = new Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.Configuracoes.ListaDto();
            return this.Item(configuracoes);
        }

        /// <summary>
        /// Recupera a lista de regras de natureza de operação.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das regras de natureza de operação.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Regras de natureza de operação sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.Lista.ListaDto>))]
        [SwaggerResponse(204, "Regras de natureza de operação não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Regras de natureza de operação paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaCfops([FromUri] Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.Lista.FiltroDto filtro)
        {
            filtro = filtro ?? new Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.Lista.FiltroDto();

            var regras = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Fiscal.Negocios.ICfopFluxo>()
                .PesquisarRegrasNaturezaOperacao(
                    filtro.IdLoja.GetValueOrDefault(),
                    filtro.IdTipoCliente.GetValueOrDefault(),
                    filtro.IdGrupoProduto.GetValueOrDefault(),
                    filtro.IdSubgrupoProduto.GetValueOrDefault(),
                    filtro.IdCorVidro.GetValueOrDefault(),
                    filtro.IdCorFerragem.GetValueOrDefault(),
                    filtro.IdCorAluminio.GetValueOrDefault(),
                    filtro.Espessura.GetValueOrDefault(),
                    filtro.IdNaturezaOperacao.GetValueOrDefault(),
                    filtro.UfsDestino != null ? string.Join(",", filtro.UfsDestino) : string.Empty);

            ((Colosoft.Collections.IVirtualList)regras).Configure(filtro.NumeroRegistros);
            ((Colosoft.Collections.ISortableCollection)regras).ApplySort(filtro.ObterTraducaoOrdenacao());

            return this.ListaPaginada(
                regras
                    .Skip(filtro.ObterPrimeiroRegistroRetornar())
                    .Take(filtro.NumeroRegistros)
                    .Select(c => new Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.Lista.ListaDto(c)),
                filtro,
                () => regras.Count);
        }
    }
}
