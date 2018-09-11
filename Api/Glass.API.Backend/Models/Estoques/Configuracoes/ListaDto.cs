// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de estoques.
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
            this.NaoVendeVidro = Geral.NaoVendeVidro();
            this.MarcarSaidaEstoqueAoLiberarPedido = Liberacao.Estoque.SaidaEstoqueAoLiberarPedido;
            this.MarcarSaidaEstoqueAutomaticaAoConfirmar = FinanceiroConfig.Estoque.SaidaEstoqueAutomaticaAoConfirmar;
            this.AlterarEstoqueManualmente = Config.PossuiPermissao(Config.FuncaoMenuEstoque.AlterarEstoqueManualmente);
        }

        /// <summary>
        /// Obtém ou define um valor que indica se será utilizada a liberação de pedidos.
        /// </summary>
        [DataMember]
        [JsonProperty("usarLiberacaoPedido")]
        public bool UsarLiberacaoPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empres não vende vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("naoVendeVidro")]
        public bool NaoVendeVidro { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será feita saída de estoque ao liberar o pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("marcarSaidaEstoqueAoLiberarPedido")]
        public bool MarcarSaidaEstoqueAoLiberarPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será feita saída de estoque ao confirmar o pedido (sistemas de confirmação).
        /// </summary>
        [DataMember]
        [JsonProperty("marcarSaidaEstoqueAutomaticaAoConfirmar")]
        public bool MarcarSaidaEstoqueAutomaticaAoConfirmar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário tem permissão para alterar estoque manualmente.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarEstoqueManualmente")]
        public bool AlterarEstoqueManualmente { get; set; }
    }
}
