// <copyright file="CorVidroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.MateriaPrima.Posicao.Lista
{
    /// <summary>
    /// Classe que encapsula os dados referentes a cor do vidro da posição de matéria prima.
    /// </summary>
    [DataContract(Name = "CorVidro")]
    public class CorVidroDto : IdDto
    {
        /// <summary>
        /// Obtém ou define a descrição da cor do vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }
    }
}
