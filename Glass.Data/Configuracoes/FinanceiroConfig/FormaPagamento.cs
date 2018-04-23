using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class FinanceiroConfig
    {
        public static class FormaPagamento
        {
            /// <summary>
            /// Verifica se a empresa trabalha com crédito de fornecedor
            /// </summary>
            /// <returns></returns>
            public static bool CreditoFornecedor
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ControlarCreditoFornecedor); }
            }

            /// <summary>
            /// Indica o número de parcelas
            /// </summary>
            public static int NumeroParcelasRenegociar
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroParcelasRenegociar); }
            }

            /// <summary>
            /// Indica se todas as formas de pagamento serão usadas ao gerar crédito.
            /// </summary>
            public static bool GerarCreditoFormasPagto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarCreditoFormasPagto); }
            }

            /// <summary>
            /// Define se a empresa separa os tipos de cheques (próprios e terceiros) nas telas de recebimento.
            /// </summary>
            public static bool SepararTiposChequesRecebimento
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SepararTiposChequesRecebimento); }
            }

            /// <summary>
            /// Retorna o número de formas de pagamento para as contas a pagar.
            /// </summary>
            public static int NumeroFormasPagtoContasPagar
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroFormasPagtoContasPagar); }
            }

            /// <summary>
            /// Impede que os cheques sejam cadastrados com data de vencimento inferior (ou igual, dependendo da config PermitirChequeDataAtual) a hoje.
            /// </summary>
            public static bool BloquearChequesDataRetroativa
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearChequesDataRetroativa); }
            }

            /// <summary>
            /// Define se será possível cadastrar cheques com data de venc. para hoje
            /// </summary>
            public static bool PermitirChequeDataAtual
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirChequeDataAtual); }
            }

            /// <summary>
            /// Impede que os cheques sejam cadastrados com o mesmo número e dígito verificador para o cliente especificado
            /// </summary>
            public static bool BloquearChequesDigitoVerificador
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearChequesDigitoVerificador); }
            }

            /// <summary>
            /// Define o número de dias que o controle de forma de pagamento bloqueia geração de crédito para pagamento com cheque (0 para desativar).
            /// </summary>
            public static int NumeroDiasImpedirGerarCreditoCheque
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroDiasImpedirGerarCreditoCheque); }
            }

            /// <summary>
            /// Define o número de dias após ao vencimento de uma conta a receber para avisar o cliente
            /// </summary>
            public static uint? NumDiasAposVencContaRecEnviarEmailCli
            {
                get { return Config.GetConfigItem<uint?>(Config.ConfigEnum.NumDiasAposVencContaRecEnviarEmailCli); }
            }

            /// <summary>
            /// Verifica se a forma de pagamento Permuta será permitida somente para administradores.
            /// </summary>
            public static bool PermitirFormaPagtoPermutaApenasAdministrador
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirFormaPagtoPermutaApenasAdministrador); }
            }

            /// <summary>
            /// Define que o padrão das formas de pagto às quais o cliente tem permissão virão desmarcadas por padrão ao inserir um novo.
            /// </summary>
            public static bool FormaPagtoPadraoDesmarcada
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.FormaPagtoPadraoDesmarcada); }
            }

            /// <summary>
            /// Define que apenas cartão de débito será exibido ao selecionar a forma de pagamento cartão
            /// </summary>
            public static bool ExibirApenasCartaoDebito
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirApenasCartaoDebito); }
            }
        }
    }
}
