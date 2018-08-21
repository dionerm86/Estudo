// <copyright file="CriadoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Helper.Respostas
{
    /// <summary>
    /// Classe que contém os dados da mensagem de item criado.
    /// </summary>
    /// <typeparam name="T">O tipo do identificador do item.</typeparam>
    [DataContract(Name = "Criado")]
    internal class CriadoDto<T> : MensagemDto
    {
        /// <summary>
        /// Obtém ou define o ID criado para o item.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public T Id { get; set; }
    }
}
