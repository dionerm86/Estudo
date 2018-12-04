// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Volumes.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um volume para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Volume")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="volume">A model de volume.</param>
        internal ListaDto(Volume volume)
        {
            this.Id = (int)volume.IdVolume;
            this.IdPedido = (int)volume.IdPedido;
            this.QuantidadePecas = (decimal)volume.QtdeItens;
            this.Peso = (decimal)volume.PesoTotal;
            this.MetroQuadrado = (decimal)volume.TotM;
            this.Situacao = volume.SituacaoStr;
            this.FuncionarioFinalizacao = volume.NomeFuncFinalizacao;
            this.DataFinalizacao = volume.DataFechamento;

            this.Permissoes = new PermissoesDto()
            {
                Editar = volume.EditarVisible,
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idPedido")]
        public int IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de peças do volume.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadePecas")]
        public decimal QuantidadePecas { get; set; }

        /// <summary>
        /// Obtém ou define o peso do volume.
        /// </summary>
        [DataMember]
        [JsonProperty("peso")]
        public decimal Peso { get; set; }

        /// <summary>
        /// Obtém ou define o metro quadrado total do volume.
        /// </summary>
        [DataMember]
        [JsonProperty("metroQuadrado")]
        public decimal MetroQuadrado { get; set; }

        /// <summary>
        /// Obtém ou define a situação do volume.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define o funcionário que finalizaou o volume.
        /// </summary>
        [DataMember]
        [JsonProperty("funcionarioFinalizacao")]
        public string FuncionarioFinalizacao { get; set; }

        /// <summary>
        /// Obtém ou define o funcionário que finalizaou o volume.
        /// </summary>
        [DataMember]
        [JsonProperty("dataFinalizacao")]
        public DateTime? DataFinalizacao { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas na tela.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
