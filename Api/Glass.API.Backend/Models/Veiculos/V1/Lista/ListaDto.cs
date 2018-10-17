// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Veiculos.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um veículo para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Veiculo")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="veiculo">A model de veículo.</param>
        internal ListaDto(Global.Negocios.Entidades.Veiculo veiculo)
        {
            this.Placa = veiculo.Placa;
            this.Modelo = veiculo.Modelo;
            this.AnoFabricacao = veiculo.Anofab;
            this.Cor = veiculo.Cor;
            this.QuilometragemInicial = veiculo.Kminicial;
            this.ValorIpva = (decimal)veiculo.Valoripva;
            this.Situacao = veiculo.Situacao.ToString();
        }

        /// <summary>
        /// Obtém ou define a placa do veículo.
        /// </summary>
        [DataMember]
        [JsonProperty("placa")]
        public string Placa { get; set; }

        /// <summary>
        /// Obtém ou define o modelo do veículo.
        /// </summary>
        [DataMember]
        [JsonProperty("modelo")]
        public string Modelo { get; set; }

        /// <summary>
        /// Obtém ou define o ano de fabricação do veículo.
        /// </summary>
        [DataMember]
        [JsonProperty("anoFabricacao")]
        public int AnoFabricacao { get; set; }

        /// <summary>
        /// Obtém ou define a cor do veículo.
        /// </summary>
        [DataMember]
        [JsonProperty("cor")]
        public string Cor { get; set; }

        /// <summary>
        /// Obtém ou define a quilometragem inicial do veículo.
        /// </summary>
        [DataMember]
        [JsonProperty("quilometragemInicial")]
        public int QuilometragemInicial { get; set; }

        /// <summary>
        /// Obtém ou define o valor do IPVA do veículo.
        /// </summary>
        [DataMember]
        [JsonProperty("valorIpva")]
        public decimal ValorIpva { get; set; }

        /// <summary>
        /// Obtém ou define a situação do veículo.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }
    }
}
