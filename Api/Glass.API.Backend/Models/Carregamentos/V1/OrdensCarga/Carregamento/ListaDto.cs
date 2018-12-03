// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Carregamentos.V1.OrdensCarga.Carregamento
{
    /// <summary>
    /// Classe que encapsula um item para a lista de ordens de carga na lista de carregamentos.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="ordemCarga">A ordem de carga que será retornada.</param>
        public ListaDto(OrdemCarga ordemCarga)
        {
            this.Id = (int)ordemCarga.IdOrdemCarga;
            this.IdCarregamento = (int)ordemCarga.IdCarregamento;
            this.Loja = ordemCarga.NomeLoja;
            this.TipoOrdemCarga = ordemCarga.TipoOrdemCargaStr;
            this.Peso = new QuantitativoDto
            {
                Total = (decimal)ordemCarga.Peso,
                Pendente = (decimal)ordemCarga.PesoPendenteProducao,
            };

            this.MetroQuadrado = new QuantitativoDto
            {
                Total = (decimal)ordemCarga.TotalM2,
                Pendente = (decimal)ordemCarga.TotalM2PendenteProducao,
            };

            this.QuantidadePecas = new QuantitativoDto
            {
                Total = (decimal)ordemCarga.QtdePecasVidro,
                Pendente = (decimal)ordemCarga.QtdePecaPendenteProducao,
            };

            this.QuantidadeVolumes = (decimal)ordemCarga.QtdeVolumes;
            this.Situacao = ordemCarga.SituacaoStr;
        }

        /// <summary>
        /// Obtém ou define o identificador do carregamento.
        /// </summary>
        [DataMember]
        [JsonProperty("idCarregamento")]
        public int IdCarregamento { get; set; }

        /// <summary>
        /// Obtém ou define a loja da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("loja")]
        public string Loja { get; set; }

        /// <summary>
        /// Obtém ou define o tipo da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoOrdemCarga")]
        public string TipoOrdemCarga { get; set; }

        /// <summary>
        /// Obtém ou define dados do peso da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("peso")]
        public QuantitativoDto Peso { get; set; }

        /// <summary>
        /// Obtém ou define dados de metro quadrado da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("metroQuadrado")]
        public QuantitativoDto MetroQuadrado { get; set; }

        /// <summary>
        /// Obtém ou define dados de quantidade de peças da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadePecas")]
        public QuantitativoDto QuantidadePecas { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de volumes da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeVolumes")]
        public decimal QuantidadeVolumes { get; set; }

        /// <summary>
        /// Obtém ou define a situação da ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }
    }
}
