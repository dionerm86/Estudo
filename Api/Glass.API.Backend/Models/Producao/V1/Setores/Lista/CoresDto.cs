// <copyright file="CoresDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Setores.Lista
{
    /// <summary>
    /// Classe que encapsula cores do setor.
    /// </summary>
    [DataContract(Name = "Cores")]
    public class CoresDto
    {
        /// <summary>
        /// Obtém ou define as cores do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("setor")]
        public IdNomeDto Setor { get; set; }

        /// <summary>
        /// Obtém ou define as cores da tela do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("tela")]
        public IdNomeDto Tela { get; set; }
    }
}
