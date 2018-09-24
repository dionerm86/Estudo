// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Global.Negocios.Entidades;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.CoresFerragem.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de cores de ferragem.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdCodigoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="corFerragem">A cor da ferragem que será retornada.</param>
        public ListaDto(CorFerragem corFerragem)
        {
            this.Id = corFerragem.IdCorFerragem;
            this.Descricao = corFerragem.Descricao;
            this.Sigla = corFerragem.Sigla;
        }

        /// <summary>
        /// Obtém ou define o identificador da cor da ferragem.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define a descrição da cor da ferragem.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define a sigla da cor da ferragem.
        /// </summary>
        [DataMember]
        [JsonProperty("sigla")]
        public string Sigla { get; set; }
    }
}
