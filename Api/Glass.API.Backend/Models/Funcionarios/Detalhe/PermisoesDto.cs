// <copyright file="PermisoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.Detalhe
{
    /// <summary>
    /// Classe que encapsula os dados de entrega do pedido.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermisoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se deve enviar email de pedido finalizado.
        /// </summary>
        [DataMember]
        [JsonProperty("enviarEmailPedidoFinalizado")]
        public bool EnviarEmailPedidoFinalizado { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o funcionário pode utilizar o chat.
        /// </summary>
        [DataMember]
        [JsonProperty("utilizarChat")]
        public bool UtilizarChat { get; set; }
    }
}
