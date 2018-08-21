// <copyright file="MensagemDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Helper.Respostas
{
    /// <summary>
    /// Classe com os dados das respostas de mensagem.
    /// </summary>
    [DataContract(Name = "Mensagem")]
    internal class MensagemDto
    {
        /// <summary>
        /// Obtém ou define o código de retorno da resposta.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public int Codigo { get; set; }

        /// <summary>
        /// Obtém ou define a mensagem da resposta.
        /// </summary>
        [DataMember]
        [JsonProperty("mensagem")]
        public string Mensagem { get; set; }
    }
}
