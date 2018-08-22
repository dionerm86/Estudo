// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Pedidos;
using Glass.API.Backend.Models.Genericas;
using System;

namespace Glass.API.Backend.Models.Pedidos.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de pedidos.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaPedidos(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do orçamento.
        /// </summary>
        public int? IdOrcamento { get; set; }

        /// <summary>
        /// Obtém ou define o código do pedido do cliente.
        /// </summary>
        public string CodigoPedidoCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente.
        /// </summary>
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define o valor mínimo do pedido.
        /// </summary>
        public decimal? ValorPedidoMinimo { get; set; }

        /// <summary>
        /// Obtém ou define o valor máximo do pedido.
        /// </summary>
        public decimal? ValorPedidoMaximo { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja emitente.
        /// </summary>
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define a situação do pedido.
        /// </summary>
        public Data.Model.Pedido.SituacaoPedido? Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a situação da produção do pedido.
        /// </summary>
        public Data.Model.Pedido.SituacaoProducaoEnum? SituacaoProducao { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do pedido.
        /// </summary>
        public Data.Model.Pedido.TipoPedidoEnum[] TipoPedido { get; set; }

        /// <summary>
        /// Obtém ou define se apenas pedido fast delivery serão retornados.
        /// </summary>
        public bool? FastDelivery { get; set; }

        /// <summary>
        /// Obtém ou define a origem do pedido.
        /// </summary>
        public int? Origem { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da cidade de entrega do pedido.
        /// </summary>
        public int? IdCidade { get; set; }

        /// <summary>
        /// Obtém ou define o nome do bairro de entrega do pedido.
        /// </summary>
        public string NomeBairro { get; set; }

        /// <summary>
        /// Obtém ou define o endereço de entrega do pedido.
        /// </summary>
        public string Endereco { get; set; }

        /// <summary>
        /// Obtém ou define o complemento do endereço de entrega do pedido.
        /// </summary>
        public string Complemento { get; set; }

        /// <summary>
        /// Obtém ou define a altura do produto do pedido.
        /// </summary>
        public float? AlturaProduto { get; set; }

        /// <summary>
        /// Obtém ou define a largura do produto do pedido.
        /// </summary>
        public int? LarguraProduto { get; set; }

        /// <summary>
        /// Obtém ou define a diferença de dias entre o pedido estar pronto e ser liberado.
        /// </summary>
        public int? DiferencaDiasEntreProntoELiberado { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial de cadastro do pedido.
        /// </summary>
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final de cadastro do pedido.
        /// </summary>
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial de finalização do pedido.
        /// </summary>
        public DateTime? PeriodoFinalizacaoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final de finalização do pedido.
        /// </summary>
        public DateTime? PeriodoFinalizacaoFim { get; set; }

        /// <summary>
        /// Obtém ou define o código do usuário que finalizou o pedido.
        /// </summary>
        public int[] CodigoUsuarioFinalizacao { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de venda do pedido.
        /// </summary>
        public Data.Model.Pedido.TipoVendaPedido? TipoVenda { get; set; }

        /// <summary>
        /// Obtém ou define a observação do pedido.
        /// </summary>
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define a observação da liberação.
        /// </summary>
        public string ObservacaoLiberacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se apenas pedidos do vendedor atual devem ser retornados.
        /// </summary>
        public bool? VendedorFixo { get; set; }

        /// <summary>
        /// Obtém ou define se o filtro de pedido fixo é aplicável.
        /// </summary>
        public Data.Model.Pedido.TipoPedidoEnum? TipoPedidoFixo { get; set; }

        /// <summary>
        /// Obtém o valor do fast delivery para a consulta.
        /// </summary>
        /// <returns>Um número que indica o tipo do fast delivery.</returns>
        internal int ObterFastDeliveryConsulta()
        {
            int filtroFastDelivery = 0;

            if (this.FastDelivery.HasValue)
            {
                filtroFastDelivery = this.FastDelivery.Value
                    ? 1
                    : 2;
            }

            return filtroFastDelivery;
        }

        /// <summary>
        /// Obtém o valor do campo 'byVend' para a consulta.
        /// </summary>
        /// <returns>Uma string que indica o valor do campo para a consulta.</returns>
        internal string ObterVendedorFixoConsulta()
        {
            return this.VendedorFixo.GetValueOrDefault()
                ? "1"
                : null;
        }

        /// <summary>
        /// Obtém o valor do campo 'maoObra' para a consulta.
        /// </summary>
        /// <returns>Uma string que indica o valor do campo para a consulta.</returns>
        internal string ObterMaoDeObraConsulta()
        {
            return this.TipoPedidoFixo.GetValueOrDefault() == Data.Model.Pedido.TipoPedidoEnum.MaoDeObra
                ? "1"
                : null;
        }

        /// <summary>
        /// Obtém o valor do campo 'maoObraEspecial' para a consulta.
        /// </summary>
        /// <returns>Uma string que indica o valor do campo para a consulta.</returns>
        internal string ObterMaoDeObraEspecialConsulta()
        {
            return this.TipoPedidoFixo.GetValueOrDefault() == Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial
                ? "1"
                : null;
        }

        /// <summary>
        /// Obtém o valor do campo 'producao' para a consulta.
        /// </summary>
        /// <returns>Uma string que indica o valor do campo para a consulta.</returns>
        internal string ObterProducaoConsulta()
        {
            return this.TipoPedidoFixo.GetValueOrDefault() == Data.Model.Pedido.TipoPedidoEnum.Producao
                ? "1"
                : null;
        }
    }
}
