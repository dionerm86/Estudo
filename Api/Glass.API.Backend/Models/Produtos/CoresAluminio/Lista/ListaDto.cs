// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Global.Negocios.Entidades;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.CoresAluminio.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de cores de alumínio.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="corAluminio">A cor da alumínio que será retornada.</param>
        public ListaDto(CorAluminio corAluminio)
        {
            this.Id = corAluminio.IdCorAluminio;
            this.Descricao = corAluminio.Descricao;
            this.Sigla = corAluminio.Sigla;
        }

        /// <summary>
        /// Obtém ou define a descrição da cor do alumínio.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define a sigla da cor do alumínio.
        /// </summary>
        [DataMember]
        [JsonProperty("sigla")]
        public string Sigla { get; set; }
    }
}
