using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Glass.API.Backend.Models.Funcionarios.Configuracoes
{
    public class DetalheDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se o ICMS é calculado no pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("habilitarChat")]
        public bool HabilitarChat { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o IPI é calculado no pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("enviarEmailPedidoConfirmado")]
        public bool EnviarEmailPedidoConfirmado { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa não vende vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("habilitarControleUsuarios")]
        public bool HabilitarControleUsuarios { get; set; }
    }
}
