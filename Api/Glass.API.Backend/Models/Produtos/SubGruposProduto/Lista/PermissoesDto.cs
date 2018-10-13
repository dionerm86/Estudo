// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.SubgruposProduto.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão do subgrupo de produto.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se será permitido excluir o subgrupo de produto.
        /// </summary>
        [DataMember]
        [JsonProperty("excluir")]
        public bool Excluir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o subgrupo pertence ao grupo vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("grupoVidro")]
        public bool GrupoVidro { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o subgrupo de produto possui log de alterações.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }
    }
}
