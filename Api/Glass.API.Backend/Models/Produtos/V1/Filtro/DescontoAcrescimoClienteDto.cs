// <copyright file="DescontoAcrescimoClienteDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.Filtro
{
    /// <summary>
    /// Classe que encapsula os dados de desconto e acréscimo por cliente.
    /// </summary>
    [DataContract(Name = "DescontoAcrescimoCliente")]
    public class DescontoAcrescimoClienteDto
    {
        /// <summary>
        /// Obtém ou define o percentual de desconto ou acréscimo do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("percentual")]
        public double Percentual { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o desconto ou acréscimo também
        /// se aplica aos beneficiamentos dos produtos.
        /// </summary>
        [DataMember]
        [JsonProperty("usarNosBeneficiamentos")]
        public bool UsarNosBeneficiamentos { get; set; }
    }
}
