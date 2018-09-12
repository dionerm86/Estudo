// <copyright file="EntradaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.NotasFiscais.Contingencia
{
    /// <summary>
    /// Classe que encapsula os dados de entrada do método de alterar a contingência da nota fiscal.
    /// </summary>
    [DataContract]
    public class EntradaDto
    {
        /// <summary>
        /// Obtém ou define o tipo de contingência que será habilitado.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoContingencia")]
        public DataSources.TipoContingenciaNFe TipoContingencia { get; set; }
    }
}
