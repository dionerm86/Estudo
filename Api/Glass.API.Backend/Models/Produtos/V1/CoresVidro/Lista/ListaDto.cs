// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Global.Negocios.Entidades;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.CoresVidro.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de cores de vidro.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="corVidro">A cor da vidro que será retornada.</param>
        public ListaDto(CorVidro corVidro)
        {
            this.Id = corVidro.IdCorVidro;
            this.Descricao = corVidro.Descricao;
            this.Sigla = corVidro.Sigla;
        }

        /// <summary>
        /// Obtém ou define a descrição da cor do vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define a sigla da cor do vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("sigla")]
        public string Sigla { get; set; }
    }
}
