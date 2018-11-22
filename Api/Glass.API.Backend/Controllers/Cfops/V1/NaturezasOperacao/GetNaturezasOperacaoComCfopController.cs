// <copyright file="GetNaturezasOperacaoComCfopController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Respostas;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Cfops.V1.NaturezasOperacao
{
    /// <summary>
    /// Controller de naturezas de operação.
    /// </summary>
    public partial class NaturezasOperacaoComCfopController : BaseController
    {
        /// <summary>
        /// Recupera a lista de naturezas de operação.
        /// </summary>
        /// <param name="idCfop">O identificador do CFOP que contém a natureza de operação.</param>
        /// <param name="filtro">Os filtros para a busca das naturezas de operação.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Naturezas de operação sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.Lista.ListaDto>))]
        [SwaggerResponse(204, "Naturezas de operação não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Naturezas de operação paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "CFOP não encontrado para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaNaturezasOperacao(int idCfop, [FromUri] Models.Cfops.V1.NaturezasOperacao.Lista.FiltroDto filtro)
        {
            var validacao = this.ValidarExistenciaIdCfop(null, idCfop);

            if (validacao != null)
            {
                return validacao;
            }

            filtro = filtro ?? new Models.Cfops.V1.NaturezasOperacao.Lista.FiltroDto();

            var regras = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Fiscal.Negocios.ICfopFluxo>()
                .PesquisarNaturezasOperacao(idCfop);

            ((Colosoft.Collections.IVirtualList)regras).Configure(filtro.NumeroRegistros);
            ((Colosoft.Collections.ISortableCollection)regras).ApplySort(filtro.ObterTraducaoOrdenacao());

            return this.ListaPaginada(
                regras
                    .Skip(filtro.ObterPrimeiroRegistroRetornar())
                    .Take(filtro.NumeroRegistros)
                    .Select(c => new Models.Cfops.V1.NaturezasOperacao.Lista.ListaDto(c)),
                filtro,
                () => regras.Count);
        }
    }
}
