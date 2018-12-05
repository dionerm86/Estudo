// <copyright file="SetorDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Setores.Filtro
{
    /// <summary>
    /// Classe que encapsula os setores para os controles de filtro.
    /// </summary>
    [DataContract(Name = "Setor")]
    public class SetorDto : IdNomeDto
    {
        /// <summary>
        /// Obtém ou define a posição do setor na ordem.
        /// </summary>
        [DataMember]
        [JsonProperty("ordem")]
        public int Ordem { get; set; }
    }
}
