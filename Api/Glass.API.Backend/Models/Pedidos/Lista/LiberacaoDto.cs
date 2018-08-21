// <copyright file="LiberacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de liberação do pedido.
    /// </summary>
    [DataContract(Name = "Liberacao")]
    public class LiberacaoDto : DataFuncionarioDto
    {
        /// <summary>
        /// Obtém ou define o identificador da liberação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define a observação da liberação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }
    }
}
