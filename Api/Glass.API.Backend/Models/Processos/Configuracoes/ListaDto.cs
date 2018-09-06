// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Processos.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de processos de etiqueta.
    /// </summary>
    [DataContract(Name = "Configuracoes")]
    public class ListaDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListaDto"/> class.
        /// </summary>
        public ListaDto()
        {
            this.TiposPedidosIgnorar = new[]
            {
                Data.Model.Pedido.TipoPedidoEnum.Revenda,
            };
        }

        /// <summary>
        /// Obtém ou define os tipos de pedido que serão ignorados.
        /// </summary>
        [DataMember]
        [JsonProperty("tiposPedidosIgnorar")]
        public Data.Model.Pedido.TipoPedidoEnum[] TiposPedidosIgnorar { get; set; }
    }
}
