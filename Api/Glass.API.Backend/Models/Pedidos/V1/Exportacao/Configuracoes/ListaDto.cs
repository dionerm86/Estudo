// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Exportacao.Configuracoes
{
    /// <summary>
    /// Classe que encapsula os dados das configurações para a tela de listagem de pedidos para exportação.
    /// </summary>
    [DataContract(Name = "PedidoExportacao")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.ExibirProdutos = Glass.Configuracoes.Liberacao.DadosLiberacao.LiberarPedidoProdutos || Glass.Configuracoes.PedidoConfig.ExibirProdutosPedidoAoLiberar;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se os produtos do pedido para exportação podem ser exibidos.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirProdutos")]
        public bool ExibirProdutos { get; set; }
    }
}