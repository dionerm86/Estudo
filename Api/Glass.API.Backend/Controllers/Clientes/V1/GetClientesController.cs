// <copyright file="GetClientesController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Clientes;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Clientes.Filtro;
using Glass.Data.DAL;
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
        /// Recupera a lista de clientes.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos clientes.</param>
        /// <returns>Uma lista JSON com os dados dos clientes.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Clientes sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Clientes.Lista.ListaDto>))]
        [SwaggerResponse(204, "Clientes não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Clientes paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Clientes.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaClientes([FromUri] Models.Clientes.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Clientes.Lista.FiltroDto();

                var clientes = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Global.Negocios.IClienteFluxo>()
                    .PesquisarClientes(
                        filtro.Id ?? 0,
                        filtro.NomeCliente,
                        filtro.CpfCnpj,
                        filtro.IdLoja ?? 0,
                        filtro.Telefone,
                        filtro.Endereco,
                        filtro.Bairro,
                        filtro.IdCidade ?? 0,
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
                        filtro.IdTabelaDesconto,
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
                        .Select(c => new Models.Clientes.Lista.ListaDto(c)),
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
    }
}
