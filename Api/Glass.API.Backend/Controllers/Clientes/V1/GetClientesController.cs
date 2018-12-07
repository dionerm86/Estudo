// <copyright file="GetClientesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Clientes;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Clientes.V1.Filtro;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Swashbuckle.Swagger.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Clientes.V1
{
    /// <summary>
    /// Controller de clientes.
    /// </summary>
    public partial class ClientesController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de clientes.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Clientes.V1.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaClientes()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Clientes.V1.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de clientes.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos clientes.</param>
        /// <returns>Uma lista JSON com os dados dos clientes.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Clientes sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Clientes.V1.Lista.ListaDto>))]
        [SwaggerResponse(204, "Clientes não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Clientes paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Clientes.V1.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaClientes([FromUri] Models.Clientes.V1.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Clientes.V1.Lista.FiltroDto();

                var clientes = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.IClienteFluxo>()
                    .PesquisarClientes(
                        filtro.Id,
                        filtro.NomeCliente,
                        filtro.CpfCnpj,
                        filtro.IdLoja,
                        filtro.Telefone,
                        filtro.Endereco,
                        filtro.Bairro,
                        filtro.IdCidade,
                        filtro.Tipo,
                        filtro.Situacao != null ? filtro.Situacao.Select(f => (int)f).ToArray() : null,
                        filtro.CodigoRota,
                        filtro.IdVendedor,
                        filtro.TipoFiscal != null ? filtro.TipoFiscal.ToArray() : null,
                        filtro.FormasPagamento,
                        filtro.PeriodoCadastroInicio,
                        filtro.PeriodoCadastroFim,
                        filtro.PeriodoSemCompraInicio,
                        filtro.PeriodoSemCompraFim,
                        filtro.PeriodoInativadoInicio,
                        filtro.PeriodoInativadoFim,
                        null,
                        null,
                        filtro.IdTabelaDescontoAcrescimo,
                        filtro.ApenasSemRota,
                        0,
                        filtro.Uf,
                        null,
                        false);

                ((Colosoft.Collections.IVirtualList)clientes).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)clientes).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    clientes
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(c => new Models.Clientes.V1.Lista.ListaDto(c)),
                    filtro,
                    () => clientes.Count);
            }
        }

        /// <summary>
        /// Recupera os clientes para o controle de pesquisa.
        /// </summary>
        /// <param name="id">O identificador do cliente para pesquisa.</param>
        /// <param name="nome">O nome do cliente para pesquisa.</param>
        /// <param name="tipoValidacao">O tipo de validação que será feita para a pesquisa.</param>
        /// <returns>Uma lista JSON com os dados dos clientes encontrados.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Clientes encontrados.", Type = typeof(IEnumerable<ClienteDto>))]
        [SwaggerResponse(204, "Clientes não encontrados.")]
        [SwaggerResponse(400, "Filtros não informados.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterFiltro(int? id = null, string nome = null, string tipoValidacao = null)
        {
            if (id == null && string.IsNullOrWhiteSpace(nome))
            {
                return this.ErroValidacao("Pelo menos um filtro (id ou nome) deve ser informado.");
            }

            var estrategiaValidacao = ValidacaoFactory.ObterEstrategiaFiltro(this, tipoValidacao);

            using (var sessao = new GDATransaction())
            {
                var validacao = estrategiaValidacao.ValidarAntesBusca(sessao, id, nome);

                if (validacao != null)
                {
                    return validacao;
                }

                var clientes = ClienteDAO.Instance.ObterClientesPorIdENome(
                    sessao,
                    id.GetValueOrDefault(),
                    nome,
                    0,
                    10);

                clientes = clientes.ToList();
                validacao = estrategiaValidacao.ValidarDepoisBusca(sessao, id, nome, ref clientes);

                if (validacao != null)
                {
                    return validacao;
                }

                return this.Lista(clientes.Select(c => new ClienteDto(c, tipoValidacao)));
            }
        }

        /// <summary>
        /// Recupera a lista de situações de cliente.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos de situações de cliente.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações não encontradas.")]
        public IHttpActionResult ObterSituacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = DataSources.Instance.GetSituacaoCliente()
                    .Select(s => new IdNomeDto
                    {
                        Id = (int)(s.Id ?? 0),
                        Nome = s.Descr,
                    });

                return this.Lista(situacoes);
            }
        }
    }
}
