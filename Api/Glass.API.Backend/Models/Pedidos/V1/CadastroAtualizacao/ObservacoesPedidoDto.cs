// <copyright file="ObservacoesPedidoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de comissionado do pedido.
    /// </summary>
    [DataContract(Name = "Observacoes")]
    public class ObservacoesPedidoDto
    {
        /// <summary>
        /// Obtém ou define a observação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define a observação de liberação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("observacaoLiberacao")]
        public string ObservacaoLiberacao { get; set; }
    }
}