using System.Collections.Generic;
using Glass.Data.Helper;
using Glass.Data.Model;

namespace Glass.Configuracoes
{
    public static partial class PedidoConfig
    {
        /// <summary>
        /// Define se a empresa calcula vidro sempre em múltiplos de 10cm
        /// </summary>
        public static bool CalcularMultiplo10
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CalcularMultiplo10); }
        }

        /// <summary>
        /// Permite que dê desconto no pedido desde que todos os produtos do mesmo seja iguais, o desconto máximo será o que está configurado na tela de desconto por produto (por qtd máxima).
        /// </summary>
        public static bool DescontoPedidoVendedorUmProduto
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.DescontoPedidoVendedorUmProduto); }
        }

        public static bool RatearDescontoProdutos
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.RatearDescontoProdutos); }
        }

        public static bool EmpresaTrabalhaAlturaLargura
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EmpresaTrabalhaAlturaLargura); }
        }

        /// <summary>
        /// Identifica se a empresa trabalha com liberação de pedido
        /// </summary>
        public static bool LiberarPedido
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.LiberarPedido); }
        }

        /// <summary>
        /// Impede que o pedido seja confirmado se houver sinal a receber ou pagamento antecipado a receber.
        /// </summary>
        public static bool ImpedirConfirmacaoPedidoPagamento
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ImpedirConfirmacaoPedidoPagamento); }
        }

        /// <summary>
        /// Exibir opção de gerar pedido de produção para corte no cadastro do pedido
        /// </summary>
        public static bool GerarPedidoProducaoCorte
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarPedidoProducaoCorte); }
        }        

        /// <summary>
        /// Define o número de dias que o pedido deve estar pronto para ser considerado atrasado,
        /// bloqueando emissão de pedido para o cliente até serem liberados.
        /// </summary>
        public static int NumeroDiasPedidoProntoAtrasado
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroDiasPedidoProntoAtrasado); }
        }

        /// <summary>
        /// A empresa verifica se o código do cliente já foi usado?
        /// (Evita duplicidade de códigos)
        /// </summary>
        public static bool CodigoClienteUsado
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CodigoClienteUsado); }
        }

        /// <summary>
        /// Retorna o tipo de entrega padrão para o pedido.
        /// </summary>
        public static Pedido.TipoEntregaPedido? TipoEntregaPadraoPedido
        {
            get { return Config.GetConfigItem<Pedido.TipoEntregaPedido?>(Config.ConfigEnum.TipoEntregaPadraoPedido); }
        }

        /// <summary>
        /// Verifica se a empresa permite finalizar pedidos à vista sem verificar o limite do cliente, no entanto ao confirmar o pedido para o PCP o limite é validado junto com este pedido.
        /// </summary>
        public static bool EmpresaPermiteFinalizarPedidoAVistaSemVerificarLimite
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EmpresaPermiteFinalizarPedidoAVistaSemVerificarLimite); }
        }

        /// <summary>
        /// Define se será possível visualizar os produtos liberados mesmo sem a opção liberar parcial estar marcada
        /// </summary>
        public static bool ExibirProdutosPedidoAoLiberar
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirProdutosPedidoAoLiberar); }
        }

        /// <summary>
        /// Número de dias mínimos para entrega para cada tipo de entrega do pedido.
        /// </summary>
        public static Dictionary<Pedido.TipoEntregaPedido, int> DiasMinimosEntregaTipo
        {
            get
            {
                Dictionary<Pedido.TipoEntregaPedido, int> retorno = new Dictionary<Pedido.TipoEntregaPedido, int>();

                string[] itens = Config.GetConfigItem<string>(Config.ConfigEnum.DiasMinimosEntregaTipo).Split(';');

                int index = 0;
                foreach (GenericModel m in DataSources.Instance.GetTipoEntrega())
                {
                    /* Chamado 52289. */
                    if (retorno.ContainsKey((Pedido.TipoEntregaPedido)m.Id.Value))
                        retorno.Remove((Pedido.TipoEntregaPedido)m.Id.Value);

                    retorno.Add((Pedido.TipoEntregaPedido)m.Id.Value, itens.Length <= index ? 0 : itens[index++].StrParaInt());
                }

                return retorno;
            }
        }

        /// <summary>
        /// Utilizar troca por pedido?
        /// </summary>
        public static bool PermitirTrocaPorPedido
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirTrocaPorPedido); }
        }

        /// <summary>
        /// Verifica se na tela de cadastro do pedido a opção "Deve transferir" será exibida.
        /// </summary>
        public static bool ExibirOpcaoDeveTransferir
        {
            get
            {
                if (!OrdemCargaConfig.UsarControleOrdemCarga || OrdemCargaConfig.UsarOrdemCargaParcial)
                    return false;

                return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirOpcaoDeveTransferir);
            }
        }

        public static bool AplicarComissaoDescontoAcrescimoAoInserirAtualizarApagarProdutoPedido
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AplicarComissaoDescontoAcrescimoAoInserirAtualizarApagarProdutoPedido); }
        }

        /// <summary>
        /// Define se o pedido deve ser pago total ou parcialmente antes da produção caso seja um pedido de Revenda.
        /// </summary>
        public static bool NaoObrigarPagamentoAntesProducaoParaPedidoRevenda(Pedido.TipoPedidoEnum tipoPedido)
        {
            if (tipoPedido != Pedido.TipoPedidoEnum.Revenda)
                return false;

            return Config.GetConfigItem<bool>(Config.ConfigEnum.NaoObrigarPagamentoAntesProducaoParaPedidoRevenda);
        }

        public static bool SalvarLogPecasImpressasNoPedido
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SalvarLogPecasImpressasNoPedido); }
        }

        public static bool ExibirLoja
        {
            get
            {
                if (ExibirOpcaoDeveTransferir)
                    return true;
                else
                    return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirLojaCadastroPedido);
            }
        }

        public static bool AlterarLojaPedido
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AlterarLojaPedido); }
        }

        /// <summary>
        ///Tempo para emitir o alerta que o comercial esta inoperante, sem emitir pedidos
        /// </summary>
        public static int TempoAlertaComercialInoperante
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.TempoAlertaComercialInoperante); }
        }

        /// <summary>
        /// Define se clientes do e-commerce podem visualizar pedidos liberados
        /// </summary>
        public static bool ExibirPedidosLiberadosECommerce
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPedidosLiberadosECommerce); }
        }

        /// <summary>
        /// Define se clientes do e-commerce podem visualizar pedidos liberados até que sejam entregues
        /// </summary>
        public static bool ExibirPedidosNaoEntregueCommerce
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPedidosNaoEntregueCommerce); }
        }

        /// <summary>
        /// Verifica se é permitido editar pedidos gerados pelo WebGlass Parceiros.
        /// </summary>
        public static bool PodeEditarPedidoGeradoParceiro
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PodeEditarPedidoGeradoParceiro); }
        }

        /// <summary>
        /// Verifica se é permitido reabrir pedidos gerados pelo WebGlass Parceiros.
        /// </summary>
        public static bool PodeReabrirPedidoGeradoParceiro
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PodeReabrirPedidoGeradoParceiro); }
        }
 
        /// <summary>
        /// Verifica se é permitido, ao parceiro, reabrir pedidos gerados por ele que estejam conferidos.
        /// </summary>
        public static bool ParceiroPodeReabrirPedidoConferido
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ParceiroPodeReabrirPedidoConferido); }
        }

        /// <summary>
        /// Define que mesmo que o pedido possua pagamento antecipado o mesmo pode ser reaberto para alteração
        /// </summary>
        public static bool ReabrirPedidoComPagamentoAntecipado
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ReabrirPedidoComPagamentoAntecipado); }
        }

        /// <summary>
        /// Verifica se é permitido, ao parceiro, editar pedidos gerados por ele que estejam abertos.
        /// </summary>
        public static bool ParceiroPodeEditarPedido
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ParceiroPodeEditarPedido); }
        }

        /// <summary>
        /// Define que não será permitido para vendedores reabrir pedidos confirmados PCP
        /// </summary>
        public static bool ReabrirPedidoConfirmadoPCPTodosMenosVendedor
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ReabrirPedidoConfirmadoPCPTodosMenosVendedor); }
        }

        /// <summary>
        /// Define que não será permitido para reabrir pedido com sinal recebido
        /// </summary>
        public static bool ReabrirPedidoNaoPermitidoComSinalRecebido
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ReabrirPedidoNaoPermitidoComSinalRecebido); }
        }

        /// <summary>
        /// Define que será usado o campo ValorReposicao do produto no pedido ao inserir o mesmo em pedidos de reposição (apenas para empresas que não cobram pedido de reposição do cliente)
        /// </summary>
        public static bool UsarValorReposicaoProduto
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarValorReposicaoProduto); }
        }

        /// <summary>
        /// Define se o m² do pedido será enviado para o cliente ao notificá-lo, por SMS, quando o mesmo ficar pronto
        /// </summary>
        public static bool EnviarTotM2SMSPedidoPronto
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EnviarTotM2SMSPedidoPronto); }
        }

        /// <summary>
        /// Define se o desconto de tabela do cliente será considerado no total de desconto do pedido
        /// </summary>
        public static bool ConsiderarDescontoClienteDescontoTotalPedido
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ConsiderarDescontoClienteDescontoTotalPedido); }
        }

        /// <summary>
        /// Verifica se deve bloquear ou informar na emissão do pedido caso não haja materia-prima
        /// suficiente de acordo com o posição de matéria-prima
        /// </summary>
        public static DataSources.BloqEmisPedidoPorPosicaoMateriaPrima BloqEmisPedidoPorPosicaoMateriaPrima
        {
            get { return Config.GetConfigItem<DataSources.BloqEmisPedidoPorPosicaoMateriaPrima>(Config.ConfigEnum.BloqEmisPedidoPorPosicaoMateriaPrima); }
        }

        /// <summary>
        /// Define se o desconto de tabela do cliente será considerado no total de desconto do pedido
        /// </summary>
        public static bool PermitirApenasPedidosDeVendaNoEcommerce
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirApenasPedidosDeVendaNoEcommerce); }
        }

        /// <summary>
        /// Verifica se sera cobra o valor do frete no pedido
        /// </summary>
        public static bool ExibirValorFretePedido
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirValorFretePedido); }
        }

        /// <summary>
        /// Verifica se deve ou não recalcular o valor do produto 
        /// </summary>
        public static bool NaoRecalcularValorProdutoComposicaoAoAlterarAlturaLargura
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.NaoRecalcularValorProdutoComposicaoAoAlterarAlturaLargura); }
        }

        /// <summary>
        /// Verifica se a empresa utiliza tabela de desconto para pedidos à vista
        /// </summary>
        public static bool UsarTabelaDescontoAcrescimoPedidoAVista
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarTabelaDescontoAcrescimoPedidoAVista); }
        }
    }
}
