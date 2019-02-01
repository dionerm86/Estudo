// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ArquivosRemessa.V1.Lista
{
    /// <summary>
    /// Classe que encapsula as permissões para um item da lista de arquivos de remessa.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o arquivo de remessa pode ser excluído.
        /// </summary>
        [DataMember]
        [JsonProperty("excluir")]
        public bool Excluir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o arquivo de remessa possui log de importação.
        /// </summary>
        [DataMember]
        [JsonProperty("logImportacao")]
        public bool LogImportacao { get; set; }
    }
}