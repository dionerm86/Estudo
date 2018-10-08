// <copyright file="GetSugestaoClienteController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.V1.SugestoesCliente
{
    /// <summary>
    /// Controller de sugestões de clientes.
    /// </summary>
    public partial class SugestaoClienteController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de sugestões de clientes.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Clientes.SugestoesCliente.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaSugestoesClientes()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Clientes.SugestoesCliente.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de sugestões de clientes.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca das sugestões de clientes.</param>
        /// <returns>Uma lista JSON com os dados dos clientes.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Sugestões de Clientes sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Clientes.SugestoesCliente.Lista.ListaDto>))]
        [SwaggerResponse(204, "Sugestões de Clientes não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Sugestões de Clientes paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Clientes.SugestoesCliente.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaSugestoesClientes([FromUri] Models.Clientes.SugestoesCliente.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Clientes.SugestoesCliente.Lista.FiltroDto();

                var sugestoesCliente = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.ISugestaoFluxo>()
                    .PesquisarSugestoes(
                        filtro.Id,
                        filtro.IdCliente,
                        filtro.IdFuncionario,
                        filtro.NomeFuncionario,
                        filtro.NomeCliente,
                        filtro.PeriodoCadastroInicio,
                        filtro.PeriodoCadastroFim,
                        filtro.Tipo,
                        filtro.Descricao,
                        filtro.Situacao != null ? new[] { filtro.Situacao.Value } : new int[] { },
                        filtro.IdRota,
                        filtro.IdPedido,
                        filtro.IdOrcamento,
                        filtro.IdVendedorAssociado);

                ((Colosoft.Collections.IVirtualList)sugestoesCliente).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)sugestoesCliente).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    sugestoesCliente
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(c => new Models.Clientes.SugestoesCliente.Lista.ListaDto(c)),
                    filtro,
                    () => sugestoesCliente.Count);
            }
        }

        /// <summary>
        /// Recupera a lista de sugestões de cliente.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos de tipos de sugestões de clientes.</returns>
        [HttpGet]
        [Route("tipos")]
        [SwaggerResponse(200, "Tipos de sugestão encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "tipos de sugestão não encontrados.")]
        public IHttpActionResult ObterTiposSugestaoCliente()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = TipoSugestaoClienteDAO.Instance.ObterTiposSugestaoCliente()
                    .Select(s => new IdNomeDto
                    {
                        Id = s.IdTipoSugestaoCliente,
                        Nome = s.Descricao,
                    });

                return this.Lista(situacoes);
            }
        }
    }
}
