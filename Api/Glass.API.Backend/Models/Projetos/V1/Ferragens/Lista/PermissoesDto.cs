// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Projetos.V1.Ferragens.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão da ferragem.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se a ferragem pode ser excluída.
        /// </summary>
        [DataMember]
        [JsonProperty("excluir")]
        public bool Excluir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se poderá alterar a situação da ferragem.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarSituacao")]
        public bool AlterarSituacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a ferragem possui log.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }
    }
}
