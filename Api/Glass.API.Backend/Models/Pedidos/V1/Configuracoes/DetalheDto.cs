// <copyright file="DetalheDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Newtonsoft.Json;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para tela de cadastro de pedido.
    /// </summary>
    [DataContract(Name = "Detalhe")]
    public class DetalheDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="DetalheDto"/>.
        /// </summary>
        /// <param name="idLoja">O identificador da loja do pedido.</param>
        internal DetalheDto(int idLoja)
        {
            if (idLoja > 0)
            {
                this.CalcularIcms = LojaDAO.Instance.ObtemCalculaIcmsStPedido(null, (uint)idLoja);
                this.CalcularIpi = LojaDAO.Instance.ObtemCalculaIpiPedido(null, (uint)idLoja);
            }
            else
            {
                this.CalcularIcms = false;
                this.CalcularIpi = false;
            }

            this.EmpresaNaoVendeVidro = Geral.NaoVendeVidro();
            this.ExibirFastDelivery = PedidoConfig.Pedido_FastDelivery.FastDelivery;
            this.MarcarFastDelivery = Config.PossuiPermissao(Config.FuncaoMenuPedido.PermitirMarcarFastDelivery);
            this.ExibirDeveTransferir = PedidoConfig.ExibirOpcaoDeveTransferir;
            this.ExibirRentabilidade = RentabilidadeConfig.CalcularRentabilidade;
            this.ExibirBotoesConfirmacao = PedidoConfig.DadosPedido.ExibirBotoesConfirmacaoPedido;
            this.ExibirAmbientes = PedidoConfig.DadosPedido.AmbientePedido;
            this.NumeroDiasUteisDataEntregaPedido = PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedido;
            this.UsarControleOrdemCarga = OrdemCargaConfig.UsarControleOrdemCarga;
            this.AlterarLojaPedido = PedidoConfig.AlterarLojaPedido;
            this.BloquearItensTipoPedido = PedidoConfig.DadosPedido.BloquearItensTipoPedido;
            this.GerarPedidoProducaoCorte = PedidoConfig.GerarPedidoProducaoCorte;
            this.ExibirValorFretePedido = PedidoConfig.ExibirValorFretePedido;
            this.DescontoApenasAVista = PedidoConfig.Desconto.DescontoPedidoApenasAVista;
            this.DescontoPedidoUmaParcela = PedidoConfig.Desconto.DescontoPedidoUmaParcela;
            this.AlterarVendedor = Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarVendedorPedido);
            this.UsarComissaoNoPedido = PedidoConfig.Comissao.ComissaoPedido;
            this.UsarComissionadoDoCliente = PedidoConfig.Comissao.UsarComissionadoCliente;
            this.UsarControleMedicao = Geral.ControleMedicao;
            this.UsarLiberacaoPedido = PedidoConfig.LiberarPedido;
            this.IgnorarBloqueioDataEntrega = Config.PossuiPermissao(Config.FuncaoMenuPedido.IgnorarBloqueioDataEntrega);
            this.UsarControleDescontoFormaPagamentoDadosProduto = FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto;
            this.AlterarPercentualComissionado = PedidoConfig.Comissao.AlterarPercComissionado;
            this.TipoPedidoPadrao = PedidoConfig.TelaCadastro.TipoPedidoPadrao;
            this.PermitirAlterarDataParcelas = !PedidoConfig.FormaPagamento.ExibirDatasParcelasPedido;
            this.UsarDescontoEmParcela = FinanceiroConfig.UsarDescontoEmParcela;
            this.TipoEntregaPadrao = PedidoConfig.TipoEntregaPadraoPedido;
            this.PerguntarVendedorFinalizacaoFinanceiro = FinanceiroConfig.PerguntarVendedorFinalizacaoFinanceiro;
            this.IdClienteProducao = (int)ClienteDAO.Instance.GetClienteProducao();
            this.ObrigarInformarPedidoCliente = PedidoConfig.DadosPedido.ObrigarInformarPedidoCliente;

            this.TipoEntregaBalcao = Data.Model.Pedido.TipoEntregaPedido.Balcao;
            this.TipoPedidoVenda = Data.Model.Pedido.TipoPedidoEnum.Venda;
            this.TipoPedidoRevenda = Data.Model.Pedido.TipoPedidoEnum.Revenda;
            this.TipoPedidoMaoDeObra = Data.Model.Pedido.TipoPedidoEnum.MaoDeObra;
            this.TipoPedidoProducao = Data.Model.Pedido.TipoPedidoEnum.Producao;
            this.TipoPedidoMaoDeObraEspecial = Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial;
            this.TipoVendaAVista = Data.Model.Pedido.TipoVendaPedido.AVista;
            this.TipoVendaAPrazo = Data.Model.Pedido.TipoVendaPedido.APrazo;
            this.TipoVendaReposicao = Data.Model.Pedido.TipoVendaPedido.Reposição;
            this.TipoVendaGarantia = Data.Model.Pedido.TipoVendaPedido.Garantia;
            this.TipoVendaObra = Data.Model.Pedido.TipoVendaPedido.Obra;
            this.TipoVendaFuncionario = Data.Model.Pedido.TipoVendaPedido.Funcionario;
            this.IdFormaPagamentoCartao = (int)Data.Model.Pagto.FormaPagto.Cartao;
            this.CorTextoObservacoesCliente = this.ObterCorParaHtml(Liberacao.TelaLiberacao.CorExibirObservacaoCliente);
            this.SituacaoObraConfirmada = Data.Model.Obra.SituacaoObra.Confirmada;
            this.ObrigarProcessoEAplicacaoRoteiro = PedidoConfig.DadosPedido.ObrigarProcAplVidros
                && PCPConfig.ControlarProducao
                && Utils.GetSetores.Any(x => x.SetorPertenceARoteiro);

            this.ExibirColunasProcessoEAplicacao = Geral.ControlePCP;
            this.SistemaLite = Geral.SistemaLite;
            this.AcrescimoDescontoItens = OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o ICMS é calculado no pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("calcularIcms")]
        public bool CalcularIcms { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o IPI é calculado no pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("calcularIpi")]
        public bool CalcularIpi { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa não vende vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("empresaNaoVendeVidro")]
        public bool EmpresaNaoVendeVidro { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa trabalha com "fast delivery".
        /// </summary>
        [DataMember]
        [JsonProperty("exibirFastDelivery")]
        public bool ExibirFastDelivery { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário pode marcar/desmarcar "fast delivery" no pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("marcarFastDelivery")]
        public bool MarcarFastDelivery { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa trabalha com "deve transferir".
        /// </summary>
        [DataMember]
        [JsonProperty("exibirDeveTransferir")]
        public bool ExibirDeveTransferir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se os dados de rentabilidade serão exibidos.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirRentabilidade")]
        public bool ExibirRentabilidade { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se os botões de confirmação serão exibidos.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirBotoesConfirmacao")]
        public bool ExibirBotoesConfirmacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se os ambientes de pedido serão exibidos.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirAmbientes")]
        public bool ExibirAmbientes { get; set; }

        /// <summary>
        /// Obtém ou define o número de dias úteis para a data de entrega do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroDiasUteisDataEntregaPedido")]
        public int NumeroDiasUteisDataEntregaPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o cliente trabalha com ordem de carga.
        /// </summary>
        [DataMember]
        [JsonProperty("usarControleOrdemCarga")]
        public bool UsarControleOrdemCarga { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário poderá alterar a loja do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarLojaPedido")]
        public bool AlterarLojaPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário poderá alterar o tipo do pedido ao editá-lo.
        /// </summary>
        [DataMember]
        [JsonProperty("bloquearItensTipoPedido")]
        public bool BloquearItensTipoPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário poderá marcar que o pedido deverá gerar um outro pedido de produção para corte.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarPedidoProducaoCorte")]
        public bool GerarPedidoProducaoCorte { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será possível informar um valor de frete no pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirValorFretePedido")]
        public bool ExibirValorFretePedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será possível aplicar desconto apenas em pedidos à vista.
        /// </summary>
        [DataMember]
        [JsonProperty("descontoApenasAVista")]
        public bool DescontoApenasAVista { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será possível aplicar desconto em pedidos a prazo se tiver apenas uma parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("descontoPedidoUmaParcela")]
        public bool DescontoPedidoUmaParcela { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será possível alterar o vendedor no pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarVendedor")]
        public bool AlterarVendedor { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será possível informar comissionado no pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("usarComissaoNoPedido")]
        public bool UsarComissaoNoPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será usado o comissionado definido no cliente no pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("usarComissionadoDoCliente")]
        public bool UsarComissionadoDoCliente { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o cliente trabalha com controle de medição.
        /// </summary>
        [DataMember]
        [JsonProperty("usarControleMedicao")]
        public bool UsarControleMedicao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o cliente trabalha com liberação de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("usarLiberacaoPedido")]
        public bool UsarLiberacaoPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deve ignorar o bloqueio de data de entrega.
        /// </summary>
        [DataMember]
        [JsonProperty("ignorarBloqueioDataEntrega")]
        public bool IgnorarBloqueioDataEntrega { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deve usar o controle de desconto por forma de pagamento.
        /// </summary>
        [DataMember]
        [JsonProperty("usarControleDescontoFormaPagamentoDadosProduto")]
        public bool UsarControleDescontoFormaPagamentoDadosProduto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido alterar o percentual de comissão do comissionado.
        /// </summary>
        [DataMember]
        [JsonProperty("alterarPercentualComissionado")]
        public bool AlterarPercentualComissionado { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual o tipo de pedido padrão a ser usado em novos pedidos.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoPedidoPadrao")]
        public Data.Model.Pedido.TipoPedidoEnum TipoPedidoPadrao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o usuário poderá alterar a data das parcelas.
        /// </summary>
        [DataMember]
        [JsonProperty("permitirAlterarDataParcelas")]
        public bool PermitirAlterarDataParcelas { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa utiliza desconto em parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("usarDescontoEmParcela")]
        public bool UsarDescontoEmParcela { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual é p tipo de entrega padrão.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoEntregaPadrao")]
        public Data.Model.Pedido.TipoEntregaPedido? TipoEntregaPadrao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será perguntado ao vendedor se ele deseja enviar o pedido para validação no financeiro, quando for o caso.
        /// </summary>
        [DataMember]
        [JsonProperty("perguntarVendedorFinalizacaoFinanceiro")]
        public bool PerguntarVendedorFinalizacaoFinanceiro { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual o id do cliente de produção da empresa.
        /// </summary>
        [DataMember]
        [JsonProperty("idClienteProducao")]
        public int IdClienteProducao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual tipo de entrega é balcão.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoEntregaBalcao")]
        public Data.Model.Pedido.TipoEntregaPedido TipoEntregaBalcao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual tipo de pedido é de venda.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoPedidoVenda")]
        public Data.Model.Pedido.TipoPedidoEnum TipoPedidoVenda { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual tipo de pedido é de revenda.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoPedidoRevenda")]
        public Data.Model.Pedido.TipoPedidoEnum TipoPedidoRevenda { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual tipo de pedido é de mão de obra.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoPedidoMaoDeObra")]
        public Data.Model.Pedido.TipoPedidoEnum TipoPedidoMaoDeObra { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual tipo de pedido é de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoPedidoProducao")]
        public Data.Model.Pedido.TipoPedidoEnum TipoPedidoProducao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual tipo de pedido é de mão de obra especial.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoPedidoMaoDeObraEspecial")]
        public Data.Model.Pedido.TipoPedidoEnum TipoPedidoMaoDeObraEspecial { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual tipo de venda é à vista.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoVendaAVista")]
        public Data.Model.Pedido.TipoVendaPedido TipoVendaAVista { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual tipo de venda é à prazo.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoVendaAPrazo")]
        public Data.Model.Pedido.TipoVendaPedido TipoVendaAPrazo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual tipo de venda é reposição.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoVendaReposicao")]
        public Data.Model.Pedido.TipoVendaPedido TipoVendaReposicao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual tipo de venda é garantia.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoVendaGarantia")]
        public Data.Model.Pedido.TipoVendaPedido TipoVendaGarantia { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual tipo de venda é de obra.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoVendaObra")]
        public Data.Model.Pedido.TipoVendaPedido TipoVendaObra { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual tipo de venda é de funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoVendaFuncionario")]
        public Data.Model.Pedido.TipoVendaPedido TipoVendaFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica qual forma de pagamento é cartão.
        /// </summary>
        [DataMember]
        [JsonProperty("idFormaPagamentoCartao")]
        public int IdFormaPagamentoCartao { get; set; }

        /// <summary>
        /// Obtém ou define a cor do texto das observações do cliente na tela.
        /// </summary>
        [DataMember]
        [JsonProperty("corTextoObservacoesCliente")]
        public string CorTextoObservacoesCliente { get; set; }

        /// <summary>
        /// Obtém ou define a situação 'Confirmada' da obra.
        /// </summary>
        [DataMember]
        [JsonProperty("situacaoObraConfirmada")]
        public Data.Model.Obra.SituacaoObra SituacaoObraConfirmada { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se os dados de etiqueta (processo e aplicação) são obrigatórios.
        /// </summary>
        [DataMember]
        [JsonProperty("obrigarProcessoEAplicacaoRoteiro")]
        public bool ObrigarProcessoEAplicacaoRoteiro { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se as colunas de processo e aplicação serão exibidas.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirColunasProcessoEAplicacao")]
        public bool ExibirColunasProcessoEAplicacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o sistema atual é a versão Lite.
        /// </summary>
        [DataMember]
        [JsonProperty("sistemaLite")]
        public bool SistemaLite { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se pode-se aplicar acréscimo e desconto aos itens.
        /// </summary>
        [DataMember]
        [JsonProperty("acrescimoDescontoItens")]
        public bool AcrescimoDescontoItens { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o código do Ped. Cli. deve ser informado.
        /// </summary>
        [DataMember]
        [JsonProperty("obrigarInformarPedidoCliente")]
        public bool ObrigarInformarPedidoCliente { get; set; }

        private string ObterCorParaHtml(Color cor)
        {
            return ColorTranslator.ToHtml(cor);
        }
    }
}
