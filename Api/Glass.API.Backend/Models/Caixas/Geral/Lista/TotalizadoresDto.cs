// <copyright file="TotalizadoresDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.Geral.Lista
{
    /// <summary>
    /// Classe que encapsula totalizadores a serem exibidos na tela.
    /// </summary>
    [DataContract(Name = "Totalizadores")]
    public class TotalizadoresDto
    {
        /// <summary>
        /// Obtém ou define o valor total recebido.
        /// </summary>
        [DataMember]
        [JsonProperty("recebido")]
        public TotaisAcumuladosDto Recebido { get; set; }

        /// <summary>
        /// Obtém ou define totais de cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("cheques")]
        public ChequesDto Cheques { get; set; }

        /// <summary>
        /// Obtém ou define totais de entrada por forma de pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("entrada")]
        public TotaisFormaPagamentoDto Entrada { get; set; }

        /// <summary>
        /// Obtém ou define totais de saída por forma de pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("saida")]
        public TotaisFormaPagamentoDto Saida { get; set; }

        /// <summary>
        /// Obtém ou define saldos por forma de pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("saldo")]
        public TotaisFormaPagamentoDto Saldo { get; set; }

        /// <summary>
        /// Obtém ou define totais de crédito.
        /// </summary>
        [DataMember]
        [JsonProperty("credito")]
        public CreditoDto Credito { get; set; }

        /// <summary>
        /// Obtém ou define totais de contas a receber/recebidas.
        /// </summary>
        [DataMember]
        [JsonProperty("parcelas")]
        public ParcelasDto Parcelas { get; set; }
    }
}
