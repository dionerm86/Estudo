// <copyright file="GetFornecedoresController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Fornecedores.V1.Lista;
using Glass.API.Backend.Models.Genericas.V1;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Fornecedores.V1
{
    /// <summary>
    /// Controller de fornecedores.
    /// </summary>
    public partial class FornecedoresController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de fornecedores.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Fornecedores.V1.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaFornecedores()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Fornecedores.V1.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de fornecedores.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos itens.</param>
        /// <returns>Uma lista JSON com os dados dos itens.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Fornecedores encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(204, "Fornecedores não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Fornecedores paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterFornecedores([FromUri] FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new FiltroDto();

                var fluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.IFornecedorFluxo>();

                var fornecedores = fluxo.PesquisarFornecedores(
                    filtro.Id ?? 0,
                    filtro.Nome,
                    filtro.Situacao,
                    filtro.CpfCnpj,
                    filtro.ComCredito,
                    null,
                    filtro.IdPlanoConta ?? 0,
                    filtro.IdParcela ?? 0,
                    filtro.Endereco,
                    filtro.Vendedor);

                ((Colosoft.Collections.IVirtualList)fornecedores).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)fornecedores).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    fornecedores
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(entidade => new ListaDto(entidade)),
                    filtro,
                    () => fornecedores.Count);
            }
        }

        /// <summary>
        /// Recupera as situações de fornecedor para o controle de pesquisa.
        /// </summary>
        /// <returns>Uma lista JSON com as situações de fornecedor.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações não encontradas.")]
        public IHttpActionResult ObterSituacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = new ConversorEnum<Data.Model.SituacaoFornecedor>()
                    .ObterTraducao();

                return this.Lista(situacoes);
            }
        }

        /// <summary>
        /// Recupera os fornecedores para o controle de pesquisa da tela de exportação de pedidos.
        /// </summary>
        /// <returns>Uma lista JSON com os fornecedores para uso no controle de busca.</returns>
        [HttpGet]
        [Route("exportacao")]
        [SwaggerResponse(200, "Fornecedores encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Fornecedores não encontrados.")]
        public IHttpActionResult ObterParaControleExportacao()
        {
            using (var sessao = new GDATransaction())
            {
                var fornecedores = Data.DAL.FornecedorDAO.Instance.ObterFornecedoresComUrlSistema()
                    .Select(f => new IdNomeDto
                    {
                        Id = f.IdFornec,
                        Nome = f.Nome,
                    });

                return this.Lista(fornecedores);
            }
        }
    }
}
