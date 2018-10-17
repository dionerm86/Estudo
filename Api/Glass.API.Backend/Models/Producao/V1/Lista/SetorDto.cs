// <copyright file="SetorDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados do setor de uma leitura da peça.
    /// </summary>
    [DataContract(Name = "Setor")]
    public class SetorDto : IdNomeDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se a leitura no setor é obrigatória.
        /// </summary>
        [DataMember]
        [JsonProperty("obrigatorio")]
        public bool Obrigatorio { get; set; }
    }
}
