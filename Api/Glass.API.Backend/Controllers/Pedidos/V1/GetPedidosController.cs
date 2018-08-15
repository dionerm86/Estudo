// <copyright file="GetPedidosController.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.API.Backend.Helper.Respostas;
using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Pedidos.CadastroAtualizacao;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Glass.API.Backend.Controllers.Pedidos.V1
{
    /// <summary>
    /// Controller de pedidos.
    /// </summary>
    public partial class PedidosController : BaseController
    {
        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de pedidos.
        /// </summary>
        /// <param name="id">O identificador do pedido.</param>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("{id}/configuracoes")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Pedidos.Configuracoes.DetalheDto))]
        public IHttpActionResult ObterConfiguracoesDetalhePedido(int id)
        {
            using (var sessao = new GDATransaction())
            {
                int idLoja = id > 0
                    ? (int)PedidoDAO.Instance.ObtemIdLoja(sessao, (uint)id)
                    : (int)UserInfo.GetUserInfo.IdLoja;

                var configuracoes = new Models.Pedidos.Configuracoes.DetalheDto(idLoja);
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera as configurações usadas pela tela de listagem de pedidos.
        /// </summary>
        /// <param name="exibirFinanceiro">Indica se a tela de listagem deve exibir detalhes do financeiro.</param>
        /// <returns>Um objeto JSON com as configurações da tela.</returns>
        [HttpGet]
        [Route("configuracoes/lista")]
        [SwaggerResponse(200, "Configurações recuperadas.", Type = typeof(Models.Pedidos.Configuracoes.ListaDto))]
        public IHttpActionResult ObterConfiguracoesListaPedidos(bool exibirFinanceiro = false)
        {
            using (var sessao = new GDATransaction())
            {
                var configuracoes = new Models.Pedidos.Configuracoes.ListaDto(exibirFinanceiro);
                return this.Item(configuracoes);
            }
        }

        /// <summary>
        /// Recupera a lista de pedidos.
        /// </summary>
        /// <param name="filtro">Os filtros para a busca dos pedidos.</param>
        /// <returns>Uma lista JSON com os dados dos pedidos.</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "Pedidos sem paginação (apenas uma página de retorno) ou última página retornada.", Type = typeof(IEnumerable<Models.Pedidos.Lista.ListaDto>))]
        [SwaggerResponse(204, "Pedidos não encontrados para o filtro informado.")]
        [SwaggerResponse(206, "Pedidos paginados (qualquer página, exceto a última).", Type = typeof(IEnumerable<Models.Pedidos.Lista.ListaDto>))]
        [SwaggerResponse(400, "Filtro inválido informado (campo com valor ou formato inválido).", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterListaPedidos([FromUri] Models.Pedidos.Lista.FiltroDto filtro)
        {
            using (var sessao = new GDATransaction())
            {
                filtro = filtro ?? new Models.Pedidos.Lista.FiltroDto();

                var pedidos = PedidoDAO.Instance.GetList(
                    (uint)(filtro.Id ?? 0),
                    (uint)(filtro.IdLoja ?? 0),
                    (uint)(filtro.IdCliente ?? 0),
                    filtro.NomeCliente,
                    filtro.CodigoPedidoCliente,
                    (uint)(filtro.IdCidade ?? 0),
                    filtro.Endereco,
                    filtro.NomeBairro,
                    filtro.Complemento,
                    filtro.Situacao != null ? ((int)filtro.Situacao.Value).ToString() : null,
                    filtro.SituacaoProducao != null ? ((int)filtro.SituacaoProducao.Value).ToString() : null,
                    filtro.ObterVendedorFixoConsulta(),
                    filtro.ObterMaoDeObraConsulta(),
                    filtro.ObterMaoDeObraEspecialConsulta(),
                    filtro.ObterProducaoConsulta(),
                    (uint)(filtro.IdOrcamento ?? 0),
                    filtro.AlturaProduto ?? 0,
                    filtro.LarguraProduto ?? 0,
                    filtro.DiferencaDiasEntreProntoELiberado ?? 0,
                    (float)(filtro.ValorPedidoMinimo ?? 0),
                    (float)(filtro.ValorPedidoMaximo ?? 0),
                    filtro.PeriodoCadastroInicio != null ? filtro.PeriodoCadastroInicio.Value.ToShortDateString() : null,
                    filtro.PeriodoCadastroFim != null ? filtro.PeriodoCadastroFim.Value.ToShortDateString() : null,
                    filtro.PeriodoFinalizacaoInicio != null ? filtro.PeriodoFinalizacaoInicio.Value.ToShortDateString() : null,
                    filtro.PeriodoFinalizacaoFim != null ? filtro.PeriodoFinalizacaoFim.Value.ToShortDateString() : null,
                    filtro.CodigoUsuarioFinalizacao != null && filtro.CodigoUsuarioFinalizacao.Any() ? string.Join(",", filtro.CodigoUsuarioFinalizacao) : null,
                    filtro.TipoPedido != null && filtro.TipoPedido.Any() ? string.Join(",", filtro.TipoPedido.Select(t => (int)t)) : null,
                    filtro.ObterFastDeliveryConsulta(),
                    filtro.TipoVenda != null ? (int)filtro.TipoVenda.Value : 0,
                    filtro.Origem ?? 0,
                    filtro.Observacao,
                    filtro.ObterTraducaoOrdenacao(),
                    filtro.ObterPrimeiroRegistroRetornar(),
                    filtro.NumeroRegistros,
                    filtro.ObservacaoLiberacao);

                return this.ListaPaginada(
                    pedidos.Select(p => new Models.Pedidos.Lista.ListaDto(p)),
                    filtro,
                    () => PedidoDAO.Instance.GetCount(
                        (uint)(filtro.Id ?? 0),
                        (uint)(filtro.IdLoja ?? 0),
                        (uint)(filtro.IdCliente ?? 0),
                        filtro.NomeCliente,
                        filtro.CodigoPedidoCliente,
                        (uint)(filtro.IdCidade ?? 0),
                        filtro.Endereco,
                        filtro.NomeBairro,
                        filtro.Complemento,
                        filtro.Situacao != null ? ((int)filtro.Situacao.Value).ToString() : null,
                        filtro.SituacaoProducao != null ? ((int)filtro.SituacaoProducao.Value).ToString() : null,
                        filtro.ObterVendedorFixoConsulta(),
                        filtro.ObterMaoDeObraConsulta(),
                        filtro.ObterMaoDeObraEspecialConsulta(),
                        filtro.ObterProducaoConsulta(),
                        (uint)(filtro.IdOrcamento ?? 0),
                        filtro.AlturaProduto ?? 0,
                        filtro.LarguraProduto ?? 0,
                        filtro.DiferencaDiasEntreProntoELiberado ?? 0,
                        (float)(filtro.ValorPedidoMinimo ?? 0),
                        (float)(filtro.ValorPedidoMaximo ?? 0),
                        filtro.PeriodoCadastroInicio != null ? filtro.PeriodoCadastroInicio.Value.ToShortDateString() : null,
                        filtro.PeriodoCadastroFim != null ? filtro.PeriodoCadastroFim.Value.ToShortDateString() : null,
                        filtro.PeriodoFinalizacaoInicio != null ? filtro.PeriodoFinalizacaoInicio.Value.ToShortDateString() : null,
                        filtro.PeriodoFinalizacaoFim != null ? filtro.PeriodoFinalizacaoFim.Value.ToShortDateString() : null,
                        filtro.CodigoUsuarioFinalizacao != null && filtro.CodigoUsuarioFinalizacao.Any() ? string.Join(",", filtro.CodigoUsuarioFinalizacao) : null,
                        filtro.TipoPedido != null && filtro.TipoPedido.Any() ? string.Join(",", filtro.TipoPedido.Select(t => (int)t)) : null,
                        filtro.ObterFastDeliveryConsulta(),
                        filtro.TipoVenda != null ? (int)filtro.TipoVenda.Value : 0,
                        filtro.Origem ?? 0,
                        filtro.Observacao,
                        filtro.ObservacaoLiberacao,
                        false));
            }
        }

        /// <summary>
        /// Recupera os detalhes de um pedido.
        /// </summary>
        /// <param name="id">O identificador do pedido.</param>
        /// <returns>Um objeto JSON com os dados do pedido.</returns>
        [HttpGet]
        [Route("{id}")]
        [SwaggerResponse(200, "Pedido encontrado.", Type = typeof(Models.Pedidos.Detalhe.DetalheDto))]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido do campo id.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Pedido não encontrado.", Type = typeof(MensagemDto))]
        public IHttpActionResult ObterPedido(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarIdPedido(id);

                if (validacao != null)
                {
                    return validacao;
                }

                var pedido = PedidoDAO.Instance.GetElement(sessao, (uint)id);

                if (pedido == null)
                {
                    return this.NaoEncontrado(string.Format("Pedido {0} não encontrado.", id));
                }

                try
                {
                    return this.Item(new Models.Pedidos.Detalhe.DetalheDto(pedido));
                }
                catch (Exception e)
                {
                    return this.ErroInternoServidor("Erro ao recuperar o pedido.", e);
                }
            }
        }

        /// <summary>
        /// Recupera a lista de situações de pedido.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos de situações de pedido.</returns>
        [HttpGet]
        [Route("situacoes")]
        [SwaggerResponse(200, "Situações de produção encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Situações de produção não encontradas.")]
        public IHttpActionResult ObterSituacoes()
        {
            using (var sessao = new GDATransaction())
            {
                var situacoes = DataSources.Instance.GetSituacaoPedido()
                    .Select(s => new IdNomeDto
                    {
                        Id = (int)(s.Id ?? 0),
                        Nome = s.Descr,
                    });

                return this.Lista(situacoes);
            }
        }

        /// <summary>
        /// Recupera a lista de tipos de pedido.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos para tipos de pedido.</returns>
        [HttpGet]
        [Route("tipos")]
        [SwaggerResponse(200, "Tipos de pedidos encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de pedidos não encontrados.")]
        public IHttpActionResult ObterTipos()
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = DataSources.Instance.GetTipoPedidoFilter()
                    .Select(s => new IdNomeDto
                    {
                        Id = (int)(s.Id ?? 0),
                        Nome = s.Descr,
                    });

                return this.Lista(tipos);
            }
        }

        /// <summary>
        /// Recupera a lista de tipos de pedido que o funcionário tem acesso.
        /// </summary>
        /// <param name="maoDeObra">Identifica se o pedido é de mão de obra.</param>
        /// <param name="producao">Identifica se o pedido é de produção.</param>
        /// <returns>Uma lista JSON com os dados básicos para tipos de pedido.</returns>
        [HttpGet]
        [Route("tiposPorFuncionario")]
        [SwaggerResponse(200, "Tipos de pedidos encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de pedidos não encontrados.")]
        public IHttpActionResult ObterTiposPorFuncionario(bool maoDeObra, bool producao)
        {
            using (var sessao = new GDATransaction())
            {
                var tipos = DataSources.Instance.GetTipoPedidoFilter()
                    .Select(s => new IdNomeDto
                    {
                        Id = (int)(s.Id ?? 0),
                        Nome = s.Descr,
                    });

                if (maoDeObra)
                {
                    tipos = tipos.Where(f => f.Id == (int)Data.Model.Pedido.TipoPedidoEnum.MaoDeObra);
                }
                else if (producao)
                {
                    tipos = tipos.Where(f => f.Id == (int)Data.Model.Pedido.TipoPedidoEnum.Producao);
                }
                else if (PedidoConfig.DadosPedido.BloquearItensTipoPedido)
                {
                    var tiposPedidoFunc = FuncionarioDAO.Instance.ObtemTipoPedido(UserInfo.GetUserInfo.CodUser);

                    if (!string.IsNullOrEmpty(tiposPedidoFunc))
                    {
                        var tiposPedidoPermitidos = tiposPedidoFunc.Split(',');
                        tipos = tipos.Where(f => tiposPedidoPermitidos.Contains(f.Id.ToString()));
                    }
                }

                return this.Lista(tipos);
            }
        }

        /// <summary>
        /// Recupera os tipos de venda possíveis para o pedido.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos para os tipos de venda de pedido.</returns>
        [HttpGet]
        [Route("tiposVenda")]
        [SwaggerResponse(200, "Tipos de vendas encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de vendas não encontrados.")]
        public IHttpActionResult ObterTiposVenda()
        {
            using (var sessao = new GDATransaction())
            {
                var tiposVenda = DataSources.Instance.GetTipoVenda(false, true)
                    .Select(s => new IdNomeDto
                    {
                        Id = (int)(s.Id ?? 0),
                        Nome = s.Descr,
                    });

                return this.Lista(tiposVenda);
            }
        }

        /// <summary>
        /// Recupera os tipos de venda possíveis para o cliente no pedido.
        /// </summary>
        /// <param name="idCliente">O identificador do cliente</param>
        /// <returns>Uma lista JSON com os dados básicos para os tipos de venda do cliente.</returns>
        [HttpGet]
        [Route("tiposVendaCliente")]
        [SwaggerResponse(200, "Tipos de vendas do cliente encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de vendas do cliente não encontrados.")]
        public IHttpActionResult ObterTiposVendaCliente(int? idCliente)
        {
            using (var sessao = new GDATransaction())
            {
                var tiposVenda = WebGlass.Business.Pedido.Fluxo.DadosPedido.Ajax.ObterTiposVendaCliente((uint?)idCliente, false)
                    .Select(s => new IdNomeDto
                    {
                        Id = (int)(s.Id ?? 0),
                        Nome = s.Descr,
                    });

                return this.Lista(tiposVenda);
            }
        }

        /// <summary>
        /// Recupera as formas de pagamento possíveis para o pedido.
        /// </summary>
        /// <param name="idCliente">O identificador do cliente</param>
        /// <param name="tipoVenda">O tipo de venda selecionado no pedido</param>
        /// <returns>Uma lista JSON com os dados básicos para as formas de pagamento de pedido.</returns>
        [HttpGet]
        [Route("formasPagamento")]
        [SwaggerResponse(200, "Formas de pagamento encontradas.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Formas de pagamento não encontradas.")]
        public IHttpActionResult ObterFormasPagamento(int? idCliente, Data.Model.Pedido.TipoVendaPedido? tipoVenda)
        {
            if (!FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto)
            {
                tipoVenda = null;
            }

            using (var sessao = new GDATransaction())
            {
                var formasPagto = FormaPagtoDAO.Instance.GetForPedido(sessao, idCliente.GetValueOrDefault(), (int)tipoVenda.GetValueOrDefault(0))
                    .Select(s => new IdNomeDto
                    {
                        Id = (int)(s.IdFormaPagto ?? 0),
                        Nome = s.Descricao,
                    });

                return this.Lista(formasPagto);
            }
        }

        /// <summary>
        /// Recupera os tipos de entrega possíveis para o pedido.
        /// </summary>
        /// <returns>Uma lista JSON com os dados básicos para os tipos de entrega de pedido.</returns>
        [HttpGet]
        [Route("tiposEntrega")]
        [SwaggerResponse(200, "Tipos de entrega encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Tipos de entrega não encontrados.")]
        public IHttpActionResult ObterTiposEntrega()
        {
            using (var sessao = new GDATransaction())
            {
                var tiposEntrega = DataSources.Instance.GetTipoEntrega()
                    .Select(s => new IdNomeDto
                    {
                        Id = (int)(s.Id ?? 0),
                        Nome = s.Descr,
                    });

                return this.Lista(tiposEntrega);
            }
        }

        /// <summary>
        /// Recupera as datas de entrega mínima possíveis para o pedido.
        /// </summary>
        /// <param name="id">O identificador do pedido.</param>
        /// <param name="idCliente">O identificador do cliente do pedido.</param>
        /// <param name="tipoPedido">Tipo do pedido</param>
        /// <param name="tipoEntrega">Tipo de entrega do pedido</param>
        /// <param name="dataBase">Data base a ser usada no cálculo da data de entrega</param>
        /// <returns>Um JSON com a data de entrega mínima e a data de entrega se for fast delivery.</returns>
        [HttpGet]
        [Route("{id}/dataEntregaMinima")]
        [SwaggerResponse(200, "Data de entrega mínima calculada.", Type = typeof(DataEntregaMinimaDto))]
        public IHttpActionResult CalcularDataEntregaMinima(int id, int? idCliente = null, [FromUri] Data.Model.Pedido.TipoPedidoEnum? tipoPedido = null, [FromUri] Data.Model.Pedido.TipoEntregaPedido? tipoEntrega = null, DateTime? dataBase = null)
        {
            using (var sessao = new GDATransaction())
            {
                DateTime dataEntregaMinima, dataFastDelivery;
                bool desabilitarCampo;
                var dataEntregaMinimaDto = new DataEntregaMinimaDto();

                if (PedidoDAO.Instance.GetDataEntregaMinima(sessao, (uint)idCliente.GetValueOrDefault(), id > 0 ? (uint?)id : null, (int?)tipoPedido, (int?)tipoEntrega, dataBase, out dataEntregaMinima, out dataFastDelivery, out desabilitarCampo))
                {
                    dataEntregaMinimaDto.DataMinimaCalculada = dataEntregaMinima;
                    dataEntregaMinimaDto.DataMinimaPermitida = dataEntregaMinima;
                    dataEntregaMinimaDto.DataFastDelivery = dataFastDelivery;
                    dataEntregaMinimaDto.DesabilitarCampo = desabilitarCampo;
                }
                else if (PedidoConfig.TelaCadastro.BuscarDataEntregaDeHojeSeDataVazia)
                {
                    dataEntregaMinimaDto.DataMinimaCalculada = DateTime.Now;
                    dataEntregaMinimaDto.DataMinimaPermitida = DateTime.Now;
                    dataEntregaMinimaDto.DataFastDelivery = DateTime.Now;
                    dataEntregaMinimaDto.DesabilitarCampo = false;
                }

                if (PedidoDAO.Instance.IsMaoDeObra(sessao, (uint)id)
                    || PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedido == 0
                    || Config.PossuiPermissao(Config.FuncaoMenuPedido.IgnorarBloqueioDataEntrega)
                    || UserInfo.GetUserInfo.IsAdministrador)
                {
                    dataEntregaMinimaDto.DataMinimaPermitida = DateTime.Now;
                }

                return this.Item(dataEntregaMinimaDto);
            }
        }

        /// <summary>
        /// Verifica se pode marcar fast delivery no pedido.
        /// </summary>
        /// <param name="id">O identificador do pedido a ser verificado.</param>
        /// <returns>Um status HTTP que indica se pode ou não marcar fast delivery no pedido.</returns>
        [HttpGet]
        [Route("{id}/verificarFastDelivery")]
        [SwaggerResponse(200, "Pode marcar fast delivery.")]
        [SwaggerResponse(400, "Erro de validação ou de valor ou formato inválido do campo id.", Type = typeof(MensagemDto))]
        [SwaggerResponse(404, "Não pode marcar fast delivery.", Type = typeof(MensagemDto))]
        public IHttpActionResult VerificarMarcacaoFastDelivery(int id)
        {
            using (var sessao = new GDATransaction())
            {
                var validacao = this.ValidarIdPedido(id);

                if (validacao != null)
                {
                    return validacao;
                }

                var aplicacoesSemPermissaoFastDelivery = EtiquetaAplicacaoDAO.Instance.ObterSemPermissaoFastDelivery(sessao, id);

                if (aplicacoesSemPermissaoFastDelivery.Any())
                {
                    return this.NaoEncontrado($"O pedido possui alguns produtos com aplicações que não podem ser usadas com Fast Delivery: {string.Join(", ", aplicacoesSemPermissaoFastDelivery)}.");
                }

                return this.Ok();
            }
        }

        /// <summary>
        /// Obtém uma lista de vendedores para cadastro de pedido.
        /// </summary>
        /// <param name="idVendedor">Identificador do vendedor já selecionado no pedido.</param>
        /// <returns>Uma lista JSON com os dados básicos dos vendedores.</returns>
        [HttpGet]
        [Route("vendedores")]
        [SwaggerResponse(200, "Vendedores encontrados.", Type = typeof(IEnumerable<IdNomeDto>))]
        [SwaggerResponse(204, "Vendedores não encontrados.")]
        public IHttpActionResult ObterVendedores(int? idVendedor = null)
        {
            using (var sessao = new GDATransaction())
            {
                var vendedores = FuncionarioDAO.Instance.GetVendedores()
                    .Select(f => new IdNomeDto
                    {
                        Id = f.IdFunc,
                        Nome = f.Nome,
                    })
                    .ToList();

                if (idVendedor > 0 && !vendedores.Any(v => v.Id == idVendedor))
                {
                    vendedores.Add(new IdNomeDto()
                    {
                        Id = idVendedor.GetValueOrDefault(),
                        Nome = FuncionarioDAO.Instance.GetNome((uint)idVendedor),
                    });
                }

                return this.Lista(vendedores);
            }
        }

        /// <summary>
        /// Obtem o desconto configurado para os dados passados por parâmetro.
        /// </summary>
        /// <param name="id">O identificador do pedido.</param>
        /// <param name="descontoTela">Desconto preenchido na tela.</param>
        /// <param name="tipoDescontoTela">Tipo de desconto preenchido na tela.</param>
        /// <param name="tipoVenda">Tipo de venda do pedido.</param>
        /// <param name="idFormaPagamento">Forma de pagamento do pedido.</param>
        /// <param name="idTipoCartao">Tipo do cartão.</param>
        /// <param name="idParcela">O identificador da parcela.</param>
        /// <returns>O desconto configurado para os parâmetros passados.</returns>
        [HttpGet]
        [Route("{id}/validacaoDescontoPedido")]
        public IHttpActionResult ValidarDescontoPedido(int id, decimal descontoTela, int tipoDescontoTela, Data.Model.Pedido.TipoVendaPedido? tipoVenda = null, int? idFormaPagamento = null, int? idTipoCartao = null, int? idParcela = null)
        {
            if (tipoVenda == null)
            {
                return this.Item(0);
            }

            using (var sessao = new GDATransaction())
            {
                var descontoFormaPagto = this.ObterDescontoFormaPagamentoDadosProduto(sessao, id, tipoVenda, idFormaPagamento, idTipoCartao, idParcela);

                var validacaoDesconto = this.ValidarDescontoPedido(sessao, id, descontoTela, tipoDescontoTela, tipoVenda.Value, idParcela, descontoFormaPagto);

                if (validacaoDesconto != null)
                {
                    return validacaoDesconto;
                }

                return this.Item(descontoFormaPagto);
            }
        }

        private decimal ObterDescontoMaximoPermitido(GDASession sessao, int idPedido, decimal descontoTela, int tipoDescontoTela, Data.Model.Pedido.TipoVendaPedido tipoVenda, int? idParcela)
        {
            var idFunc = UserInfo.GetUserInfo.CodUser;
            var idFuncDesc = Geral.ManterDescontoAdministrador ? PedidoDAO.Instance.ObtemIdFuncDesc(sessao, (uint)idPedido).GetValueOrDefault() : 0;
            var alterouDesconto = PedidoDAO.Instance.ObterDesconto(sessao, idPedido) != descontoTela ||
                PedidoDAO.Instance.ObterTipoDesconto(sessao, idPedido) != tipoDescontoTela;

            if (!UserInfo.GetUserInfo.IsAdministrador && idFuncDesc > 0 && !alterouDesconto)
            {
                idFunc = idFuncDesc;
            }

            return (decimal)PedidoConfig.Desconto.GetDescontoMaximoPedido(sessao, idFunc, (int)tipoVenda, idParcela);
        }

        private decimal ObterDescontoFormaPagamentoDadosProduto(GDASession sessao, int idPedido, Data.Model.Pedido.TipoVendaPedido? tipoVenda, int? idFormaPagamento, int? idTipoCartao, int? idParcela)
        {
            if (!FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto || idPedido == 0 || idFormaPagamento.GetValueOrDefault() == 0)
            {
                return 0;
            }

            var idProd = ProdutosPedidoDAO.Instance.ObterIdProdPrimeiroProduto(sessao, idPedido);

            var idGrupoProduto = idProd > 0 ? (int?)ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, idProd.Value) : null;
            var idSubgrupoProduto = idProd > 0 ? ProdutoDAO.Instance.ObtemIdSubgrupoProd(sessao, idProd.Value) : null;

            var desconto = DescontoFormaPagamentoDadosProdutoDAO.Instance.ObterDesconto(sessao, (uint?)tipoVenda, (uint?)idFormaPagamento, (uint)idTipoCartao, (uint?)idParcela, (uint?)idGrupoProduto, (uint?)idSubgrupoProduto);

            return desconto;
        }

        private IHttpActionResult ValidarDescontoPedido(GDASession sessao, int idPedido, decimal descontoTela, int tipoDescontoTela, Data.Model.Pedido.TipoVendaPedido tipoVenda, int? idParcela, decimal descontoFormaPagto)
        {
            var percDescontoMaximo = this.ObterDescontoMaximoPermitido(sessao, idPedido, descontoTela, tipoDescontoTela, tipoVenda, idParcela);

            if (percDescontoMaximo == 0 || percDescontoMaximo == 100)
            {
                return null;
            }

            var totalPedidoSemDesconto = PedidoDAO.Instance.GetTotalSemDesconto(sessao, (uint)idPedido, PedidoDAO.Instance.GetTotal(sessao, (uint)idPedido));
            var valorDescontoProdutos = PedidoDAO.Instance.GetDescontoProdutos(sessao, (uint)idPedido);
            var percDescontoProdutos = Math.Round((valorDescontoProdutos / (totalPedidoSemDesconto > 0 ? totalPedidoSemDesconto : 1)) * 100, 2);
            var percDescontoPedido = tipoDescontoTela == 1 ? descontoTela : ((descontoTela / totalPedidoSemDesconto) * 100);

            if (descontoFormaPagto == percDescontoProdutos)
            {
                return null;
            }

            if ((percDescontoPedido + percDescontoProdutos) > percDescontoMaximo)
            {
                var descontoMaximoPedido = tipoDescontoTela == 1 ? percDescontoMaximo : totalPedidoSemDesconto * (percDescontoMaximo / 100);

                var mensagemValidacao = $"O desconto máximo permitido é de {(tipoDescontoTela == 2 ? "R$ " : string.Empty)}{descontoMaximoPedido}{(tipoDescontoTela == 1 ? "%" : string.Empty)}";

                if (percDescontoProdutos > 0)
                {
                    var descontoTotalProdutos = tipoDescontoTela == 1 ? percDescontoProdutos : valorDescontoProdutos;

                    mensagemValidacao += $"\nO desconto já aplicado aos produtos é de {(tipoDescontoTela == 2 ? "R$ " : string.Empty)}{descontoTotalProdutos}{(tipoDescontoTela == 1 ? "%" : string.Empty)}";
                }

                return this.ErroValidacao(mensagemValidacao);
            }

            return null;
        }
    }
}
