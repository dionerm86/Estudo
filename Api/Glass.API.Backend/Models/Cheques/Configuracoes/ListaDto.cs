// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cheques.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de cheques.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.UsarLiberacaoPedido = PedidoConfig.LiberarPedido;
            this.TipoChequeTerceiros = (int)Data.Model.Cheques.TipoCheque.Terceiros;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se será utilizada a liberação de pedidos.
        /// </summary>
        [DataMember]
        [JsonProperty("usarLiberacaoPedido")]
        public bool UsarLiberacaoPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual o tipo de cheque é o tipo "Terceiros".
        /// </summary>
        [DataMember]
        [JsonProperty("tipoChequeTerceiros")]
        public int TipoChequeTerceiros { get; set; }
    }
}
