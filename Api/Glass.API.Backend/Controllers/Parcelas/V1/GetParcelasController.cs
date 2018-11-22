// <copyright file="GetParcelasController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.API.Backend.Models.Parcelas.V1.Filtro;
using Glass.Data.DAL;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Parcelas.V1
{
    /// <summary>
    /// Controller de parcelas.
    /// </summary>
    public partial class ParcelasController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de parcelas.
        /// </summary>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Parcelas.V1.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaParcelas()
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Parcelas.V1.Configuracoes.ListaDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de parcelas.
        /// </summary>
        /// <param name="id">Identificador da parcela que está sendo alterada.</param>
        /// <returns>Um objeto JSON com as configurações da tela de cadastro.</returns>
        [HttpGet]
        [Route("{id:int}/configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Parcelas.V1.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesCadastroParcela(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Parcelas.V1.Configuracoes.CadastroDto();
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera os Tipos pagamentos usadas pela tela de cadastro de parcelas.
        /// </summary>
        /// <returns>Um objeto JSON com os tipos de pagamentos para cadastro de parcelas.</returns>
        [HttpGet]
        [Route("tiposPagamento")]
        [SwaggerResponse(200, "Tipos de pagamentos encontrados.", typeof(IEnumerable<IdNomeDto>))]
        public IHttpActionResult ObterTiposPagamento()
        {
            using (var sessao = new GDATransaction())
            {
                var tiposPagamentos = new Helper.ConversorEnum<Models.Parcelas.V1.Configuracoes.TipoPagamento>()
                    .ObterTraducao();

                return this.Lista(tiposPagamentos);
            }
        }

        /// <summary>
        /// Recupera a lista de parcelas para a tela de listagem.
        /// </summary>
        /// <param name="filtro">O filtro para a busca de parcelas.</param>
        /// <returns>Uma lista JSON com os dados das parcelas.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Parcelas encontrados sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Parcelas.V1.Lista.ListaDto>))]
        [SwaggerResponse(204, "Parcelas não encontradas para o filtro informado.")]
        [SwaggerResponse(206, "Parcelas paginadas (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Parcelas.V1.Lista.ListaDto>))]
        [SwaggerResponse(400, "Erro de validação.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaParcelas([FromUri] Models.Parcelas.V1.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Parcelas.V1.Lista.FiltroDto();

                var parcelas = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.Financeiro.Negocios.IParcelasFluxo>()
                    .PesquisarParcelas();

                ((Colosoft.Collections.IVirtualList)parcelas).Configure(filtro.NumeroRegistros);
                ((Colosoft.Collections.ISortableCollection)parcelas).ApplySort(filtro.ObterTraducaoOrdenacao());

                return this.ListaPaginada(
                    parcelas
                        .Skip(filtro.ObterPrimeiroRegistroRetornar())
                        .Take(filtro.NumeroRegistros)
                        .Select(s => new Models.Parcelas.V1.Lista.ListaDto(s)),
                    filtro,
                    () => parcelas.Count);
            }
        }

        /// <summary>
        /// Recupera as parcelas do sistema para os controles de filtro das telas.
        /// </summary>
        /// <param name="id">O identificador da parcela.</param>
        /// <returns>Uma lista JSON com a parcela encontrada.</returns>
        [HttpGet]
        [Route("{id:int}")]
        [SwaggerResponse(200, "Parcela encontrada.", Type = typeof(Models.Parcelas.V1.CadastroAtualizacao.CadastroAtualizacaoDto))]
        [SwaggerResponse(400, "Identificador com valor ou formato inválido", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Parcela não encontrada para o id informado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterParcela(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var parcela = Microsoft.Practices.ServiceLocation.ServiceLocator
                    .Current.GetInstance<Glass.Financeiro.Negocios.IParcelasFluxo>()
                    .ObtemParcela(id);

                if (parcela == null)
                {
                    return this.NaoEncontrado(string.Format("parcela {0} não encontrada.", id));
                }

                try
                {
                    return this.Item(new Models.Parcelas.V1.Detalhe.DetalheDto(parcela));
                }
                catch (Exception e)
                {
                    return this.ErroInternoServidor("Erro ao recuperar a parcela.", e);
                }
            }
        }

        /// <summary>
        /// Recupera as parcelas do sistema para a edição de parcelas.
        /// </summary>
        /// <returns>Uma lista JSON com as parcelas encontradas.</returns>
        [HttpGet]
        [Route("filtro")]
        [SwaggerResponse(200, "Parcelas encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Parcelas não encontradas.")]
        public IHttpActionResult ObterParcela()
        {
            using (var sessao = new GDATransaction())
            {
                var parcelas = ParcelasDAO.Instance.GetAll()
                    .Select(p => new IdNomeDto
                    {
                        Id = p.IdParcela,
                        Nome = p.Descricao,
                    });

                return this.Lista(parcelas);
            }
        }

        /// <summary>
        /// Recupera as parcelas que o cliente tem acesso para os controles de filtro das telas.
        /// </summary>
        /// <param name="idCliente">Identificador do cliente que será usado para buscar as parcelas.</param>
        /// <returns>Uma lista JSON com as parcelas encontradas.</returns>
        [HttpGet]
        [Route("cliente")]
        [SwaggerResponse(200, "Parcelas encontradas.", Type = typeof(IEnumerable<ParcelaDto>))]
        [SwaggerResponse(204, "Parcelas não encontradas.")]
        public IHttpActionResult ObterParcelasCliente(int? idCliente)
        {
            using (var sessao = new GDATransaction())
            {
                string msgErro;

                var parcelas = ParcelasDAO.Instance.GetForControleSelecionar((uint)idCliente.GetValueOrDefault(), 0, null, false, ParcelasDAO.TipoConsulta.Prazo, out msgErro)
                    .Where(p => p.Situacao == Situacao.Ativo)
                    .Select(p => new ParcelaDto
                    {
                        Id = p.IdParcela,
                        Nome = p.Descricao,
                        NumeroParcelas = p.NumParcelas,
                        Dias = this.ObterDias(p.Dias),
                    });

                return this.Lista(parcelas);
            }
        }

        private IEnumerable<int> ObterDias(string dias)
        {
            if (string.IsNullOrWhiteSpace(dias))
            {
                return new int[0];
            }

            return dias.Split(',')
                .Select(dia => dia.Trim().StrParaInt());
        }
    }
}
