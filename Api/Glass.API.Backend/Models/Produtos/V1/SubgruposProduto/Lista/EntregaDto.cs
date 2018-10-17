// <copyright file="EntregaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.SubgruposProduto.Lista
{
    /// <summary>
    /// Classe que encapsula dados de entrega do subgrupo.
    /// </summary>
    [DataContract(Name = "Entrega")]
    public class EntregaDto
    {
        /// <summary>
        /// Obtém ou define a quantidade de dias mínimos para entrega dos produtos deste subgrupo.
        /// </summary>
        [DataMember]
        [JsonProperty("diasMinimo")]
        public int? DiasMinimo { get; set; }

        /// <summary>
        /// Obtém ou define o dia da semana para entrega dos produtos deste subgrupo.
        /// </summary>
        [DataMember]
        [JsonProperty("diaSemana")]
        public IdNomeDto DiaSemana { get; set; }
    }
}
