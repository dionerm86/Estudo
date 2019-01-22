// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.RelModel;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Liberacoes.V1.Movimentacoes.Lista
{
    /// <summary>
    /// Classe que encapsula os dados para a tela de listagem de movimentações de liberações.
    /// </summary>
    [DataContract(Name = "MovimentacaoLiberacao")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="liberarPedidoMov">A liberação do pedido que será retornada.</param>
        internal ListaDto(LiberarPedidoMov liberarPedidoMov)
        {
            this.Id = (int)liberarPedidoMov.IdLiberarPedido;

            this.Cliente = liberarPedidoMov.NomeCliente;

            this.Situacao = liberarPedidoMov.Situacao;
            this.Total = liberarPedidoMov.Total;
            this.Desconto = liberarPedidoMov.Desconto;
            this.FormasPagamento = new FormasPagamentoDto
            {
                Dinheiro = liberarPedidoMov.Dinheiro,
                Cheque = liberarPedidoMov.Cheque,
                Boleto = liberarPedidoMov.Boleto,
                Deposito = liberarPedidoMov.Deposito,
                Cartao = liberarPedidoMov.Cartao,
                Prazo = liberarPedidoMov.Prazo,
                Outros = liberarPedidoMov.Outros,
                Debito = liberarPedidoMov.Debito,
                Credito = liberarPedidoMov.Credito,
            };
        }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public string Cliente { get; set; }

        /// <summary>
        /// Obtém ou define a situação da liberação da movimentação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define o valor total para um item da lista.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtém ou define o desconto para um item da lista.
        /// </summary>
        [DataMember]
        [JsonProperty("desconto")]
        public decimal Desconto { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento para um item da lista.
        /// </summary>
        [DataMember]
        [JsonProperty("formasPagamento")]
        public FormasPagamentoDto FormasPagamento { get; set; }
    }
}
