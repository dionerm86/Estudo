// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão do estoque.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o estoque poderá ser editado.
        /// </summary>
        [DataMember]
        [JsonProperty("editar")]
        public bool Editar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o link para visualizar detalhes da reserva poderá ser exibido.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirLinkReserva")]
        public bool ExibirLinkReserva { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o link para visualizar detalhes da reserva poderá ser exibido.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirLinkLiberacao")]
        public bool ExibirLinkLiberacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o estoque do produto possui log de alterações.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }
    }
}
