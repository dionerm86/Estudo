// <copyright file="SituacaoProducaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados da situação da peça na produção.
    /// </summary>
    [DataContract(Name = "SituacaoProducao")]
    public class SituacaoProducaoDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se a peça está parada na produção.
        /// </summary>
        [DataMember]
        [JsonProperty("pecaParada")]
        public bool PecaParada { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a peça foi reposta.
        /// </summary>
        [DataMember]
        [JsonProperty("pecaReposta")]
        public bool PecaReposta { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a peça possui leitura em algum setor
        /// que não é exibido na tela de consulta de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("possuiLeituraSetorOculto")]
        public bool PossuiLeituraSetorOculto { get; set; }

        /// <summary>
        /// Obtém ou define os dados de perda da peça, se houverem.
        /// </summary>
        [DataMember]
        [JsonProperty("perda")]
        public PerdaDto Perda { get; set; }
    }
}
