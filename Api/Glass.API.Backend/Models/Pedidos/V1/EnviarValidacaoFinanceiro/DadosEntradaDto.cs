// <copyright file="DadosEntradaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.EnviarValidacaoFinanceiro
{
    /// <summary>
    /// Classe com os dados de entrada para o método de envio para validação do financeiro.
    /// </summary>
    [DataContract(Name = "DadosEntrada")]
    public class DadosEntradaDto
    {
        /// <summary>
        /// Obtém ou define a mensagem para análise do financeiro.
        /// </summary>
        [DataMember]
        [JsonProperty("mensagem")]
        public string Mensagem { get; set; }
    }
}
