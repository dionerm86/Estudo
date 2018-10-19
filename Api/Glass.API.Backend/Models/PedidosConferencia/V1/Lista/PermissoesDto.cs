// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.PedidosConferencia.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão do pedido em conferência.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o pedido em conferência poderá ser editada.
        /// </summary>
        [DataMember]
        [JsonProperty("editar")]
        public bool Editar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido em conferência poderá ser cancelado.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelar")]
        public bool Cancelar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido em conferência poderá ser reaberto.
        /// </summary>
        [DataMember]
        [JsonProperty("reabrir")]
        public bool Reabrir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido em conferência poderá ser impresso.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimir")]
        public bool Imprimir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a memória de cálculo do pedido em conferência poderá ser impressa.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimirMemoriaCalculo")]
        public bool ImprimirMemoriaCalculo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se está sendo usado controle de reposição.
        /// </summary>
        [DataMember]
        [JsonProperty("usarControleReposicao")]
        public bool UsarControleReposicao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido anexar arquivos ao pedido em conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("anexarArquivos")]
        public bool AnexarArquivos { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se os projetos do pedido em conferência poderão ser impressos.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimirProjeto")]
        public bool ImprimirProjeto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido associar imagens às peças do pedido em conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("associarImagemAsPecas")]
        public bool AssociarImagemAsPecas { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se peças a comprar do pedido em conferência poderão ser impressas.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimirProdutosAComprar")]
        public bool ImprimirProdutosAComprar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a opção para alterar a situação do CNC deverá ser exibida.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirSituacaoCnc")]
        public bool ExibirSituacaoCnc { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a opção para alterar a situação do CNC em conferência deverá ser exibida.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirSituacaoCncConferencia")]
        public bool ExibirSituacaoCncConferencia { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a opção de alterar pedido para conferido será exibida.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirConferirPedido")]
        public bool ExibirConferirPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a nota fiscal possui log de alterações.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido pode ter arquivos gerados.
        /// </summary>
        [DataMember]
        [JsonProperty("podeGerarArquivo")]
        public bool PodeGerarArquivo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido importado pode gerar arquivo.
        /// </summary>
        [DataMember]
        [JsonProperty("pedidoImportadoPodeGerarArquivo")]
        public bool PedidoImportadoPodeGerarArquivo { get; set; }
    }
}
