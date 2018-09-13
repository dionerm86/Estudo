// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de produtos.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.CadastrarProduto = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarProduto);
            this.UsarLiberacaoPedido = PedidoConfig.LiberarPedido;
            this.UsarDescontoPorQuantidade = PedidoConfig.Desconto.DescontoPorProduto;
            this.ExibirPrecoAnterior = UserInfo.GetUserInfo.IsAdministrador;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário tem permissão de cadastrar produtos.
        /// </summary>
        [DataMember]
        [JsonProperty("cadastrarProduto")]
        public bool CadastrarProduto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa trabalha com liberação de pedidos.
        /// </summary>
        [DataMember]
        [JsonProperty("usarLiberacaoPedido")]
        public bool UsarLiberacaoPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será utilização o controle de desconto de produto por quantidade.
        /// </summary>
        [DataMember]
        [JsonProperty("usarDescontoPorQuantidade")]
        public bool UsarDescontoPorQuantidade { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido visualizar o preço anterior do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirPrecoAnterior")]
        public bool ExibirPrecoAnterior { get; set; }
    }
}
