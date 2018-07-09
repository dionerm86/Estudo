// <copyright file="ComissionadoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Clientes.Filtro
{
    /// <summary>
    /// Classe que encapsula os dados de comissionado para o controle de busca de clientes.
    /// </summary>
    [DataContract(Name = "Comissionado")]
    public class ComissionadoDto : IdNomeDto
    {
        /// <summary>
        /// Obtém ou define o percentual de comissão para o comissionado.
        /// </summary>
        [DataMember]
        [JsonProperty("percentual")]
        public double Percentual { get; set; }
    }
}
