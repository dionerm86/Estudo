// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de regras de natureza de operação.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.IdGrupoVidro = (int)NomeGrupoProd.Vidro;
            this.IdGrupoFerragem = (int)NomeGrupoProd.Ferragem;
            this.IdGrupoAluminio = (int)NomeGrupoProd.Alumínio;
        }

        /// <summary>
        /// Obtém ou define o identificador do grupo de vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("idGrupoVidro")]
        public int IdGrupoVidro { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do grupo de ferragem.
        /// </summary>
        [DataMember]
        [JsonProperty("idGrupoFerragem")]
        public int IdGrupoFerragem { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do grupo de alumínio.
        /// </summary>
        [DataMember]
        [JsonProperty("idGrupoAluminio")]
        public int IdGrupoAluminio { get; set; }
    }
}
