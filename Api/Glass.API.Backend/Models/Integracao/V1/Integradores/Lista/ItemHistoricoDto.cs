// <copyright file="ItemHistoricoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Integracao.V1.Integradores.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um item do histórico.
    /// </summary>
    [DataContract(Name = "ItemHistorico")]
    public class ItemHistoricoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ItemHistoricoDto"/>.
        /// </summary>
        /// <param name="item">Item cujo os dados serão encapsulados.</param>
        public ItemHistoricoDto(Glass.Integracao.Historico.Item item)
        {
            this.Tipo = item.Tipo.ToString();
            this.Identificadores = item.Identificadores.ToArray();
            this.Mensagem = item.Mensagem;
            this.Falha = item.Falha != null ? new FalhaHistoricoDto(item.Falha) : null;
            this.Data = item.Data;
        }

        /// <summary>
        /// Obtém o tipo do item.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public string Tipo { get; }

        /// <summary>
        /// Obtém os identificadores do item.
        /// </summary>
        [DataMember]
        [JsonProperty("identificadores")]
        public IEnumerable<object> Identificadores { get; }

        /// <summary>
        /// Obtém a mensagem do item.
        /// </summary>
        [DataMember]
        [JsonProperty("mensagem")]
        public string Mensagem { get; }

        /// <summary>
        /// Obtém a falha associada.
        /// </summary>
        [DataMember]
        [JsonProperty("falha")]
        public FalhaHistoricoDto Falha { get; }

        /// <summary>
        /// Obtém a data do item.
        /// </summary>
        [DataMember]
        [JsonProperty("data")]
        public DateTime Data { get; }
    }
}
