// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Parcelas.V1.Parcelas.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de grupos de produtos.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.UsarDescontoEmParcela = FinanceiroConfig.UsarDescontoEmParcela;
            this.UsarTabelaDescontoAcrescimoPedidoAVista = PedidoConfig.UsarTabelaDescontoAcrescimoPedidoAVista;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa trabalha com Tabela Desconto/Acrescimo em Pedido a vista.
        /// </summary>
        [DataMember]
        [JsonProperty("usarTabelaDescontoAcrescimoPedidoAVista")]
        public bool UsarTabelaDescontoAcrescimoPedidoAVista { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa trabalha com desconto em parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("usarDescontoEmParcela")]
        public bool UsarDescontoEmParcela { get; set; }
    }
}
