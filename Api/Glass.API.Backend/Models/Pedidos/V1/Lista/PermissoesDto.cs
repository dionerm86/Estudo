// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão do pedido.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o pedido pode ser editado.
        /// </summary>
        [DataMember]
        [JsonProperty("editar")]
        public bool Editar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido pode exibir a impressão do PCP.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimirPcp")]
        public bool ImprimirPcp { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido pode ser impresso.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimir")]
        public bool Imprimir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido pode imprimir a memória de cálculo.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimirMemoriaCalculo")]
        public bool ImprimirMemoriaCalculo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido pode imprimir a nota promissória.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimirNotaPromissoria")]
        public bool ImprimirNotaPromissoria { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido permite imprimir o projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimirProjeto")]
        public bool ImprimirProjeto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido possui alterações no PCP.
        /// </summary>
        [DataMember]
        [JsonProperty("temAlteracaoPcp")]
        public bool TemAlteracaoPcp { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido pode ser cancelado.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelar")]
        public bool Cancelar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido pode exibir a tela de descontos.
        /// </summary>
        [DataMember]
        [JsonProperty("desconto")]
        public bool Desconto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido exibe imagens de peça.
        /// </summary>
        [DataMember]
        [JsonProperty("imagemPeca")]
        public bool ImagemPeca { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido permite impressão de itens a liberar.
        /// </summary>
        [DataMember]
        [JsonProperty("impressaoItensLiberar")]
        public bool ImpressaoItensLiberar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido permite alterar processo e aplicação.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarProcessoEAplicacao")]
        public bool AlterarProcessoEAplicacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido pode ser reaberto.
        /// </summary>
        [DataMember]
        [JsonProperty("reabrir")]
        public bool Reabrir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido permite exibir anexos de liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("anexosLiberacao")]
        public bool AnexosLiberacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido exibe finalizações do financeiro.
        /// </summary>
        [DataMember]
        [JsonProperty("finalizacoesFinanceiro")]
        public bool FinalizacoesFinanceiro { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido possui log de alterações.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se as observações do pedido podem ser alteradas na listagem do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarObservacoes")]
        public bool AlterarObservacoes { get; set; }
    }
}
