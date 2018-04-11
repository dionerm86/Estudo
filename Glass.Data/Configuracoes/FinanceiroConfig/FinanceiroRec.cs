using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class FinanceiroConfig
    {
        public class FinanceiroRec
        {
            /// <summary>
            /// Define que os pedidos liberados nas contas a receber/recebidas geradas por liberação serão mostrados nas telas de consulta de contas a receber/recebidas e débitos do limite do cliente
            /// </summary>
            public static bool ExibirIdPedidoComLiberacaoContasRec
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirIdPedidoComLiberacaoContasRec); }
            }

            /// <summary>
            /// Define se os pedidos de um acerto serão exibidos na referência do contas recebidas
            /// </summary>
            public static bool ExibirIdPedidoComAcertoContasRec
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirIdPedidoComAcertoContasRec); }
            }

            /// <summary>
            /// Define se os ped cli dos pedidos serão exibidos na referência do contas recebidas
            /// </summary>
            public static bool ExibirPedCliComIdPedidoContasRec
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPedCliComIdPedidoContasRec); }
            }

            /// <summary>
            /// Define se será possível escolher a conta bancária que determinada parcela de cartão será quitada
            /// </summary>
            public static bool SelecionarContaBancoQuitarParcCartao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SelecionarContaBancoQuitarParcCartao); }
            }

            public static bool ExibirCnab
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirCnab) && !Glass.Configuracoes.Geral.SistemaLite; }
            }

            /// <summary>
            /// Define que será permitido gerar nota somente de liberação de pedido
            /// </summary>
            public static bool GerarNotaApenasDeLiberacao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarNotaApenasDeLiberacao); }
            }

            /// <summary>
            /// Define se a empresa imprime a compra com beneficiamenteo com valores
            /// </summary>
            public static bool ImprimirCompraComBenef
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ImprimirCompraComBenef); }
            }

            public static bool ApenasFinancGeralAdminSelFuncCxGeral
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ApenasFinancGeralAdminSelFuncCxGeral); }
            }

            /// <summary>
            /// Inativa o cliente caso o cheque do mesmo seja devolvido ou protestado
            /// </summary>
            public static bool BloquearClienteAoDevolverProtestarCheque
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearClienteAoDevolverProtestarCheque); }
            }

            /// <summary>
            /// Define se será usado, preferencialmente, o cliente da nota ao gerar o CNAB
            /// </summary>
            public static bool UsarClienteDaNotaNoCnab
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarClienteDaNotaNoCnab); }
            }

            /// <summary>
            /// Define se será usado o cliente da liberação ao fazer a separação de valores
            /// </summary>
            public static bool UsarClienteLiberacaoSeparacaoDeValores
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarClienteLiberacaoSeparacaoDeValores); }
            }

            /// <summary>
            /// Define se será usada a loja associada na conta bancária ao gerar boleto
            /// </summary>
            public static bool UsarLojaDoBancoNoBoleto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarLojaDoBancoNoBoleto); }
            }

            /// <summary>
            /// Define quantos dias após o vencimento do cheque compensado o sistema pode considerá-lo fora do limite do cliente
            /// </summary>
            public static int DiasConsiderarChequeCompensado
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.DiasConsiderarChequeCompensado); }
            }

            /// <summary>
            /// Define se será bloqueado separar valor quando o pedido estiver pagto antecipado
            /// </summary>
            public static bool ImpedirSeparacaoValorSePossuirPagtoAntecip
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ImpedirSeparacaoValorSePossuirPagtoAntecip); }
            }

            /// <summary>
            /// Define se na tela e impressão das movimentações de crédito do cliente serão exibidos os pedidos da liberação
            /// </summary>
            public static bool ExibirPedidosDaLiberacaoMovCredito
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirPedidosDaLiberacaoMovCredito); }
            }

            /// <summary>
            /// Define se será permitido receber obra de cliente caso ela não tenha sido cadastrada na data atual.
            /// </summary>
            public static bool PermitirRecebimentoObraClienteDataAnteriorDataAtual
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirRecebimentoObraClienteDataAnteriorDataAtual); }
            }

            /// <summary>
            /// 
            /// </summary>
            public static bool ExibirSaldoDevedorRelsRecebimento
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirSaldoDevedorRelsRecebimento); }
            }
        }
    }
}
