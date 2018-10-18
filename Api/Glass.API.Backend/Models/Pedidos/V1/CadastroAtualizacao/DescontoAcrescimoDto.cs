// <copyright file="DescontoAcrescimoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de desconto e acréscimo do pedido.
    /// </summary>
    [DataContract(Name = "DescontoAcrescimo")]
    public class DescontoAcrescimoDto
    {
        /// <summary>
        /// Obtém ou define o tipo de desconto/acréscimo.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public TipoValor? Tipo { get; set; }

        /// <summary>
        /// Obtém ou define o valor (monetário ou percentual) de desconto/acréscimo.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public decimal? Valor { get; set; }
    }
}
