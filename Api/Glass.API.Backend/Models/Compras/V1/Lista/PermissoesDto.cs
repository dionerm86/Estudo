// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Compras.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão da lista de compras.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o compra pode ser editada.
        /// </summary>
        [DataMember]
        [JsonProperty("editar")]
        public bool? Editar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a compra pode ser cancelada.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelar")]
        public bool? Cancelar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se as fotos associadas a compra podem ser gerenciadas.
        /// </summary>
        [DataMember]
        [JsonProperty("gerenciarFotos")]
        public bool? GerenciarFotos { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se pode ser gerada uma nota fiscal a partir da compra.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarNotaFiscal")]
        public bool GerarNotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a compra pode ser reaberta.
        /// </summary>
        [DataMember]
        [JsonProperty("reabrir")]
        public bool? Reabrir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se podem ser exibidas as notas fiscais geradas para a compra.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirNotasFiscaisGeradas")]
        public bool? ExibirNotasFiscaisGeradas { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o link 'Produto chegou' será exibido.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirLinkProdutoChegou")]
        public bool? ExibirLinkProdutoChegou { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se pode ser exibida a finalização da entrega para a compra.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirFinalizacaoEntrega")]
        public bool? ExibirFinalizacaoEntrega { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se pode ser exibido o centro de custo para a compra.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirCentroCusto")]
        public bool? ExibirCentroCusto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a compra possui log de alteracoes.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool? LogAlteracoes { get; set; }
    }
}