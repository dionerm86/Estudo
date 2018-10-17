// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Global.Negocios.Entidades;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Datas.V1.Feriados.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de feriados.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="feriado">O feriado que será retornado.</param>
        public ListaDto(Feriado feriado)
        {
            this.Id = feriado.IdFeriado;
            this.Descricao = feriado.Descricao;
            this.Dia = feriado.Dia;
            this.Mes = feriado.Mes;
        }

        /// <summary>
        /// Obtém ou define a descrição do feriado.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define o dia do feriado.
        /// </summary>
        [DataMember]
        [JsonProperty("dia")]
        public int Dia { get; set; }

        /// <summary>
        /// Obtém ou define o mês do feriado.
        /// </summary>
        [DataMember]
        [JsonProperty("mes")]
        public int Mes { get; set; }
    }
}
