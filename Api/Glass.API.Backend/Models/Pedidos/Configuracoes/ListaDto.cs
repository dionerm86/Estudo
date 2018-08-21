// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de pedidos.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="exibirFinanceiro">Indica se a tela deve considerar exibição de dados do financeiro.</param>
        internal ListaDto(bool exibirFinanceiro)
        {
            this.ExibirFiltroValores = false;
            this.ExibirColunaLoja = LojaDAO.Instance.GetCount() > 1;
            this.ExibirColunaLiberadoFinanceiro = FinanceiroConfig.UsarControleLiberarFinanc && exibirFinanceiro;
            this.ExibirColunaSituacaoProducao = PCPConfig.ControlarProducao;
            this.ExibirColunaConfirmacao = !PedidoConfig.LiberarPedido;
            this.ExibirColunaLiberacao = PedidoConfig.LiberarPedido;
            this.ExibirColunaPedidoPronto = PCPConfig.ControlarProducao && PedidoConfig.NumeroDiasPedidoProntoAtrasado > 0;
            this.ExibirFiltroPronto = PedidoConfig.LiberarPedido && PedidoConfig.NumeroDiasPedidoProntoAtrasado > 0;
            this.UsarImpressaoProjetoPcp = PedidoConfig.TelaListagem.UsarImpressaoProjetoPcp;
            this.ExibirSugestoes = Config.PossuiPermissao(Config.FuncaoMenuCadastro.CadastrarSugestoesClientes);
            this.ExibirRentabilidade = RentabilidadeConfig.CalcularRentabilidade;
            this.ControlarProducao = PCPConfig.ControlarProducao;
            this.ExibirBotoesTotais = UserInfo.GetUserInfo.IsAdministrador || !PedidoConfig.TelaListagem.ApenasAdminVisualizaTotais;
            this.EmitirPedido = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedido);
        }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser exibido o filtro de valores.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirFiltroValores")]
        public bool ExibirFiltroValores { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser exibida a coluna de loja na listagem.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirColunaLoja")]
        public bool ExibirColunaLoja { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser exibida a coluna de liberação do financeiro.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirColunaLiberadoFinanceiro")]
        public bool ExibirColunaLiberadoFinanceiro { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser exibida a coluna de situação de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirColunaSituacaoProducao")]
        public bool ExibirColunaSituacaoProducao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser exibida a coluna de data de confirmação.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirColunaConfirmacao")]
        public bool ExibirColunaConfirmacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser exibida a coluna de data de liberação.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirColunaLiberacao")]
        public bool ExibirColunaLiberacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser exibida a coluna de data de pedido pronto.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirColunaPedidoPronto")]
        public bool ExibirColunaPedidoPronto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser exibido o filtro de pedido pronto.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirFiltroPronto")]
        public bool ExibirFiltroPronto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser exibida a impressão de projeto PCP.
        /// </summary>
        [DataMember]
        [JsonProperty("usarImpressaoProjetoPcp")]
        public bool UsarImpressaoProjetoPcp { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser exibido o botão de sugestão de clientes.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirSugestoes")]
        public bool ExibirSugestoes { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser exibido o botão de dados de rentabilidade do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirRentabilidade")]
        public bool ExibirRentabilidade { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverão ser exibidos os dados de controle de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("controlarProducao")]
        public bool ControlarProducao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverão ser exibidos os botões de totais.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirBotoesTotais")]
        public bool ExibirBotoesTotais { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá ser exibido o link para emissão de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("emitirPedido")]
        public bool EmitirPedido { get; set; }
    }
}
