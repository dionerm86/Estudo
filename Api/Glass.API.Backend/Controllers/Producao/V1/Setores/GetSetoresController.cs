// <copyright file="GetSetoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Helper;
using Glass.Data.Model;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Producao.V1.Setores
{
    /// <summary>
    /// Controller de setores de produção.
    /// </summary>
    public partial class SetoresController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de setores.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Producao.V1.Setores.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaPedidos()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Producao.V1.Setores.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de setores.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos setores.</param>
        /// <returns>Uma lista JSON com os dados dos setores.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Setores sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Producao.V1.Setores.Lista.ListaDto>))]
        [SwaggerResponse(204, "Setores não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Setores paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Producao.V1.Setores.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaSetores([FromUri] Models.Producao.V1.Setores.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Producao.V1.Setores.Lista.FiltroDto();

                var setores = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<PCP.Negocios.ISetorFluxo>()
                    .PesquisarSetores();

                ((Colosoft.Collections.IVirtualList)setores).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)setores).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    setores
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(c => new Models.Producao.V1.Setores.Lista.ListaDto(c)),
                    filtro,
                    () => setores.Count);
            }
        }

        /// <summary>
        /// Recupera a lista de tipos de setor.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos dos tipos de setor.</returns>
        [HttpGet]
        [Route("tipos")]
        [SwaggerResponse(200, "Tipos de setor encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de setor não encontradas.")]
        public IHttpActionResult ObterTiposSetor()
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = new ConversorEnum<TipoSetor>()
                    .ObterTraducao();

                return this.Lista(tipos);
            }
        }

        /// <summary>
        /// Recupera a lista de cores de setor.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos das cores de setor.</returns>
        [HttpGet]
        [Route("cores")]
        [SwaggerResponse(200, "Cores de setor encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Cores de setor não encontradas.")]
        public IHttpActionResult ObterCoresSetor()
        {
            using (var sessao = new GDATransaction())
            {
                var cores = new ConversorEnum<CorSetor>()
                    .ObterTraducao();

                return this.Lista(cores);
            }
        }

        /// <summary>
        /// Recupera a lista de cores de tela de setor.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos das cores de tela de setor.</returns>
        [HttpGet]
        [Route("coresTela")]
        [SwaggerResponse(200, "Cores de tela de setor encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Cores de tela de setor não encontradas.")]
        public IHttpActionResult ObterCoresTelaSetor()
        {
            using (var sessao = new GDATransaction())
            {
                var cores = new ConversorEnum<CorTelaSetor>()
                    .ObterTraducao();

                return this.Lista(cores);
            }
        }

        /// <summary>
        /// Recupera a lista de situações de setor.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos das situações de setor.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações não encontradas.")]
        public IHttpActionResult ObterSituacoesSetor()
        {
            using (var sessao = new GDATransaction())
            {
                var cores = new ConversorEnum<Situacao>()
                    .ObterTraducao();

                return this.Lista(cores);
            }
        }

        /// <summary>
        /// Recupera uma lista com os setores de produção para uso no controle.
        /// </summary>
        /// <param name="incluirSetorImpressao">Indica se o setor de impressão de etiquetas deve ser retornado.</param>
        /// <param name="incluirEtiquetaNaoImpressa">Indica se deve ser retornado um setor 'Etiqueta não impressa'.</param>
        /// <returns>Uma lista JSON com os dados básicos das situações de produção.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Setores de produção encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Setores de produção não encontrados.")]
        public IHttpActionResult ObterParaControle(bool incluirSetorImpressao, bool incluirEtiquetaNaoImpressa)
        {
            using (var sessao = new GDATransaction())
            {
                var setores = Utils.GetSetores
                    .Select(s => new IdNomeDto
                    {
                        Id = s.IdSetor,
                        Nome = s.Descricao,
                    })
                    .ToList();

                if (!incluirSetorImpressao)
                {
                    const int ID_SETOR_IMPRESSAO_ETIQUETA = 1;

                    setores = setores
                        .Where(s => s.Id != ID_SETOR_IMPRESSAO_ETIQUETA)
                        .ToList();
                }

                if (incluirEtiquetaNaoImpressa)
                {
                    setores.Insert(0, new IdNomeDto
                    {
                        Id = -1,
                        Nome = "Etiqueta não impressa",
                    });
                }

                return this.Lista(setores);
            }
        }
    }
}
