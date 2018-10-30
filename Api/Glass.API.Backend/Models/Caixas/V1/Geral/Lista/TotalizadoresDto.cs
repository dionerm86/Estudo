// <copyright file="TotalizadoresDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.V1.Geral.Lista
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
        [JsonProperty("totaisAcumulados")]
        public TotaisAcumuladosDto TotaisAcumulados { get; set; }

        /// <summary>
        /// Obtém ou define totais de cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("totaisCheques")]
        public TotaisChequesDto TotaisCheques { get; set; }

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
        [JsonProperty("totaisCredito")]
        public TotaisCreditoDto TotaisCredito { get; set; }

        /// <summary>
        /// Obtém ou define totais de contas a receber/recebidas.
        /// </summary>
        [DataMember]
        [JsonProperty("totaisParcelas")]
        public TotaisParcelasDto TotaisParcelas { get; set; }
    }
}
