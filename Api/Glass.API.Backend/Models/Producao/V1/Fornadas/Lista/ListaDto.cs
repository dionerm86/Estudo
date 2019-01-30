// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Fornadas.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um item para a tela de listagem de fornadas.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="fornada">A model de fornada preenchida.</param>
        public ListaDto(Fornada fornada)
        {
            this.Id = (int)fornada.IdFornada;
            this.UsuarioCadastro = fornada.DescrUsuCad;
            this.DataCadastro = fornada.DataCad;
            this.Capacidade = fornada.Capacidade;
            this.MetroQuadradoLido = fornada.M2Lido;
            this.QuantidadeLida = fornada.QtdeLida;
            this.Aproveitamento = fornada.Aproveitamento;
            this.Etiquetas = fornada.Etiquetas != null ? fornada.Etiquetas.Split(',') : null;
        }

        /// <summary>
        /// Obtém ou define o usuário que cadastrou a fornada.
        /// </summary>
        [DataMember]
        [JsonProperty("usuarioCadastro")]
        public string UsuarioCadastro { get; set; }

        /// <summary>
        /// Obtém ou define a data de cadastro da fornada.
        /// </summary>
        [DataMember]
        [JsonProperty("dataCadastro")]
        public DateTime? DataCadastro { get; set; }

        /// <summary>
        /// Obtém ou define a capacidade da fornada.
        /// </summary>
        [DataMember]
        [JsonProperty("capacidade")]
        public decimal? Capacidade { get; set; }

        /// <summary>
        /// Obtém ou define o metro quadrado lido da fornada.
        /// </summary>
        [DataMember]
        [JsonProperty("metroQuadradoLido")]
        public decimal? MetroQuadradoLido { get; set; }

        /// <summary>
        /// Obtém ou define o metro quadrado lido da fornada.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadeLida")]
        public int? QuantidadeLida { get; set; }

        /// <summary>
        /// Obtém ou define o aproveitamento da fornada.
        /// </summary>
        [DataMember]
        [JsonProperty("aproveitamento")]
        public decimal? Aproveitamento { get; set; }

        /// <summary>
        /// Obtém ou define as etiquetas associadas a fornada.
        /// </summary>
        [DataMember]
        [JsonProperty("etiquetas")]
        public IEnumerable<string> Etiquetas { get; set; }
    }
}