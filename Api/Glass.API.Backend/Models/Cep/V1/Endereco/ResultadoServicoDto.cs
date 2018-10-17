// <copyright file="ResultadoServicoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;

namespace Glass.API.Backend.Models.Cep.V1.Endereco
{
    /// <summary>
    /// Classe que encapsula os dados do resultado da consulta ao serviço de CEP.
    /// </summary>
    internal class ResultadoServicoDto
    {
        /// <summary>
        /// Obtém ou define a UF do endereço.
        /// </summary>
        [JsonProperty("uf")]
        public string Uf { get; set; }

        /// <summary>
        /// Obtém ou define a cidade do endereço.
        /// </summary>
        [JsonProperty("cidade")]
        public string Cidade { get; set; }

        /// <summary>
        /// Obtém ou define o bairro do endereço.
        /// </summary>
        [JsonProperty("bairro")]
        public string Bairro { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de logradouro do endereço.
        /// </summary>
        [JsonProperty("tipo_logradouro")]
        public string TipoLogradouro { get; set; }

        /// <summary>
        /// Obtém ou define o logradouro do endereço.
        /// </summary>
        [JsonProperty("logradouro")]
        public string Logradouro { get; set; }

        /// <summary>
        /// Obtém ou define o código do resultado da consulta.
        /// </summary>
        [JsonProperty("resultado")]
        public int Resultado { get; set; }

        /// <summary>
        /// Obtém ou define o texto do resultado da consulta.
        /// </summary>
        [JsonProperty("resultado_txt")]
        public string TextoResultado { get; set; }
    }
}
