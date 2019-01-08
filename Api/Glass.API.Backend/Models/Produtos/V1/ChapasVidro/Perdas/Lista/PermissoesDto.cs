// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.ChapasVidro.Perdas.Lista
{
    /// <summary>
    /// Classe que encapsula as permissões concebidas a uma perda de chapa de vidro.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se a perda de chapa de vidro pode ser cancelada.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelar")]
        public bool Cancelar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a perda de chapa de vidro possui log de alteração.
        /// </summary>
        [DataMember]
        [JsonProperty("logCancelamento")]
        public bool LogCancelamento { get; set; }
    }
}