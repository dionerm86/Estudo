// <copyright file="CadastroAtualizacaoRecebidaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasReceber.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de atualização de uma conta recebida.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacaoRecebida")]
    public class CadastroAtualizacaoRecebidaDto
    {
        /// <summary>
        /// Obtém ou define a observação da conta recebida.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }
    }
}