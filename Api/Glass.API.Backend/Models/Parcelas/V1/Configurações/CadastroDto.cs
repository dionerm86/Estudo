// <copyright file="CadastroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Parcelas.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de cadastro de parcelas.
    /// </summary>
    [DataContract(Name = "Cadastro")]
    public class CadastroDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="CadastroDto"/>.
        /// </summary>
        internal CadastroDto()
        {
            this.UsarDescontoEmParcela = FinanceiroConfig.UsarDescontoEmParcela;
            this.UsarTabelaDescontoAcrescimoPedidoAVista = PedidoConfig.UsarTabelaDescontoAcrescimoPedidoAVista;
            this.TipoPagamentoAPrazo = TipoPagamento.TipoPagamentoAPrazo;
            this.TipoPagamentoAVista = TipoPagamento.TipoPagamentoAVista;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa trabalha com Tabela Desconto/Acrescimo em Pedido a vista.
        /// </summary>
        [DataMember]
        [JsonProperty("usarTabelaDescontoAcrescimoPedidoAVista")]
        public bool UsarTabelaDescontoAcrescimoPedidoAVista { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa trabalha com desconto em parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("usarDescontoEmParcela")]
        public bool UsarDescontoEmParcela { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de pagamento a prazo.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoPagamentoAPrazo")]
        public TipoPagamento TipoPagamentoAPrazo { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de pagamento a vista.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoPagamentoAVista")]
        public TipoPagamento TipoPagamentoAVista { get; set; }
    }
}
