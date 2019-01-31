// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Exportacao.Lista
{
    /// <summary>
    /// Classe que encapsula os dados das permissões para um item da lista de pedidos para exportação.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se a situação do pedido para exportação pode ser consultada.
        /// </summary>
        [DataMember]
        [JsonProperty("consultarSituacao")]
        public bool ConsultarSituacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o log de alterações para do pedido para exportação possui registros.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }
    }
}