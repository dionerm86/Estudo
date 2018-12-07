// <copyright file="GetImpressoesEtiquetasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.ImpressoesEtiquetas.V1.Lista;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.ImpressoesEtiquetas.V1
{
    /// <summary>
    /// Controller de impressões de etiquetas.
    /// </summary>
    public partial class ImpressoesEtiquetasController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de impressões de etiqueta.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.ImpressoesEtiquetas.V1.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaImpressoesEtiquetas()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.ImpressoesEtiquetas.V1.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de impressões de etiquetas.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Impressões de etiquetas encontradas sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Impressões de etiquetas não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Impressões de etiquetas paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterImpressoesEtiquetas([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var impressoes = ImpressaoEtiquetaDAO.Instance.GetList(
                    (uint)(filtro.IdPedido ?? 0),
                    (uint)(filtro.NumeroNotaFiscal ?? 0),
                    (uint)(filtro.Id ?? 0),
                    filtro.PlanoCorte,
                    filtro.LoteProdutoNotaFiscal,
                    filtro.PeriodoCadastroInicio?.ToShortDateString(),
                    filtro.PeriodoCadastroFim?.ToShortDateString(),
                    filtro.CodigoEtiqueta,
                    (int)(filtro.TipoImpressao ?? 0),
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros);

                return this.ListaPaginada(
                    impressoes.Select(c => new ListaDto(c)),
                    filtro,
                    () => ImpressaoEtiquetaDAO.Instance.GetCount(
                        (uint)(filtro.IdPedido ?? 0),
                        (uint)(filtro.NumeroNotaFiscal ?? 0),
                        (uint)(filtro.Id ?? 0),
                        filtro.PlanoCorte,
                        filtro.LoteProdutoNotaFiscal,
                        filtro.PeriodoCadastroInicio?.ToShortDateString(),
                        filtro.PeriodoCadastroFim?.ToShortDateString(),
                        filtro.CodigoEtiqueta,
                        (int)(filtro.TipoImpressao ?? 0)));
            }
        }

        /// <summary>
        /// Recupera os tipos de impressão de etiqueta para o controle de pesquisa.
        /// </summary>
        /// <returns>Uma lista JSON com os tipos de impressão de etiqueta.</returns>
        [HttpGet]
        [Route("tipos")]
        [SwaggerResponse(200, "Tipos de impressão de etiqueta encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de impressão de etiqueta não encontrados.")]
        public IHttpActionResult ObterTipos()
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = new ConversorEnum<Data.DAL.ProdutoImpressaoDAO.TipoEtiqueta>()
                    .ObterTraducao();

                return this.Lista(tipos);
            }
        }
    }
}
