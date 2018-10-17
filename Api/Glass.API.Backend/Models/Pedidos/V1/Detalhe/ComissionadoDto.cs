// <copyright file="ComissionadoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Detalhe
{
    /// <summary>
    /// Classe que encapsula os dados de comissionado.
    /// </summary>
    [DataContract(Name = "Comissionado")]
    public class ComissionadoDto : IdNomeDto
    {
        /// <summary>
        /// Obtém ou define os dados de comissão do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("comissao")]
        public PercentualValorDto Comissao { get; set; }
    }
}
