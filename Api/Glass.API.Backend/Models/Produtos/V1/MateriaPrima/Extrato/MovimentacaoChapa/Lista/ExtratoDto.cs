// <copyright file="ExtratoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.MateriaPrima.Extrato.MovimentacaoChapa.Lista
{
    /// <summary>
    /// Classe que encapsula os dados do extrato para um item da listagem de extrato de movimentações de chapa.
    /// </summary>
    [DataContract(Name = "Extrato")]
    public class ExtratoDto
    {
        /// <summary>
        /// Obtém ou define a data da leitura associado a movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("dataLeitura")]
        public DateTime? DataLeitura { get; set; }

        /// <summary>
        /// Obtém ou define o código da etiqueta da chapa.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoEtiquetaChapa")]
        public string CodigoEtiquetaChapa { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("produto")]
        public string Produto { get; set; }

        /// <summary>
        /// Obtém ou define os dados referentes ao metro quadrado da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("metroQuadrado")]
        public MetroQuadradoDto MetroQuadrado { get; set; }

        /// <summary>
        /// Obtém ou define a cor da linha.
        /// </summary>
        [DataMember]
        [JsonProperty("corLinha")]
        public string CorLinha { get; set; }
    }
}