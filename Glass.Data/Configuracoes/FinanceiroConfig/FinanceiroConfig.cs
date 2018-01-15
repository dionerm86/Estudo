using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public static partial class FinanceiroConfig
    {
        public static bool PlanoContaBloquearEntradaSaida
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PlanoContaBloquearEntradaSaida); }
        }

        /// <summary>
        /// Indica se o caixa diário pode efetuar retiradas.
        /// </summary>
        public static bool EfetuarRetiradaCaixaDiario
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EfetuarRetiradaCaixaDiario); }
        }

        /// <summary>
        /// Define o período, em dias, que será usado para inativar o cliente dependendo da data da sua última compra
        /// (0 para desabilitar).
        /// </summary>
        public static int PeriodoInativarClienteUltimaCompra
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.PeriodoInativarClienteUltimaCompra); }
        }

        /// <summary>
        /// Define o período, em dias, que será usado para ignorar o cliente recém ativo ao inativar automaticamente
        /// (0 para considerar todos os clientes).
        /// </summary>
        public static int NumeroDiasIgnorarClientesRecemAtivosInativarAutomaticamente
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroDiasIgnorarClientesRecemAtivosInativarAutomaticamente); }
        }

        /// <summary>
        /// Define o período, em dias, que será usado para inativar o cliente dependendo da data da última consulta
        /// da situação do mesmo no sintegra.
        /// (0 para desabilitar).
        /// </summary>
        public static int PeriodoInativarClienteUltimaConsultaSintegra
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.PeriodoInativarClienteUltimaConsultaSintegra); }
        }

        /// <summary>
        /// Define o número de dias para considerar que uma conta a receber está atrasada
        /// </summary>
        public static int NumeroDiasContaRecAtrasada
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroDiasContaRecAtrasada); }
        }

        /// <summary>
        /// Define se o controle de pagamento antecipado de forncedor deve se habilitado no sistema
        /// </summary>
        public static bool UsarPgtoAntecipFornec
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarPgtoAntecipFornec); }
        }

        /// <summary>
        /// Define se vai ser usado o controle de envio de e-mail de cobrança para o cliente.
        /// </summary>
        public static bool UsarControleCobrancaEmail
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarControleCobrancaEmail); }
        }

        /// <summary>
        /// Define o número de dias anteriores ao vencimento de uma conta a receber para avisar o cliente
        /// </summary>
        public static uint? NumDiasAnteriorVencContaRecEnviarEmailCli
        {
            get { return Config.GetConfigItem<uint?>(Config.ConfigEnum.NumDiasAnteriorVencContaRecEnviarEmailCli); }
        }

        /// <summary>
        /// Define se a empresa usa o valor mínimo como definido no produto.
        /// </summary>
        public static bool UsarValorMinimoProduto
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarValorMinimoProduto); }
        }

        /// <summary>
        /// Busca o limite padrão que o cliente deve ter
        /// </summary>
        /// <returns></returns>
        public static float LimitePadraoCliente
        {
            get { return Config.GetConfigItem<float>(Config.ConfigEnum.LimitePadraoCliente); }
        }

        /// <summary>
        /// Define se vai ser usado o controle de vigência de preço de fornecedores.
        /// </summary>
        public static bool UsarDataVigenciaPrecoFornec
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarDataVigenciaPrecoFornec); }
        }

        /// <summary>
        /// Define se haverá separação dos valores reais e fiscais nas contas a receber.
        /// </summary>
        public static bool SepararValoresFiscaisEReaisContasReceber
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SepararValoresFiscaisEReaisContasReceber); }
        }

        /// <summary>
        /// Define se haverá separação dos valores reais e fiscais nas contas a pagar.
        /// </summary>
        public static bool SepararValoresFiscaisEReaisContasPagar
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SepararValoresFiscaisEReaisContasPagar); }
        }

        /// <summary>
        /// A empresa considera cheques abertos nos débitos do cliente?
        /// </summary>
        public static bool BloquearEmissaoPedidoLimiteExcedido
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearEmissaoPedidoLimiteExcedido); }
        }

        /// <summary>
        /// A empresa considera a opção "Pagar antes da produção" como padrão no cadastro de clientes?
        /// </summary>
        public static bool OpcaoPagtoAntecipadoPadraoMarcada
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.OpcaoPagtoAntecipadoPadraoMarcada); }
        }

        /// <summary>
        /// Percentual de sinal mínimo no pedido padrão ao inserir novo cliente.
        /// </summary>
        public static float PercMinimoSinalPedidoPadrao
        {
            get { return Config.GetConfigItem<float>(Config.ConfigEnum.PercMinimoSinalPedidoPadrao); }
        }

        /// <summary>
        /// Define se o limite de cheques por CPF/CNPJ será utilizado.
        /// </summary>
        public static bool LimitarChequesPorCpfOuCnpj
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.LimitarChequesPorCpfOuCnpj); }
        }

        /// <summary>
        /// Indica se o Financeiro também pode confirmar pedidos.
        /// </summary>
        public static bool PermitirConfirmacaoPedidoPeloFinanceiro
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirConfirmacaoPedidoPeloFinanceiro); }
        }

        /// <summary>
        /// Perguntar ao vendedor se o pedido deverá ser enviado ao financeiro em caso de erro na confirmação?
        /// </summary>
        public static bool PerguntarVendedorConfirmacaoFinanceiro
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PerguntarVendedorConfirmacaoFinanceiro); }
        }

        /// <summary>
        /// Indica se o Financeiro também pode finalizar pedidos.
        /// </summary>
        public static bool PermitirFinalizacaoPedidoPeloFinanceiro
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirFinalizacaoPedidoPeloFinanceiro); }
        }

        /// <summary>
        /// Perguntar ao vendedor se o pedido deverá ser enviado ao financeiro em caso de erro na finalização?
        /// </summary>
        public static bool PerguntarVendedorFinalizacaoFinanceiro
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PerguntarVendedorFinalizacaoFinanceiro); }
        }

        public static uint PlanoContaTaxaAntecip
        {
            get { return Config.GetConfigItem<uint>(Config.ConfigEnum.PlanoContaTaxaAntecip); }
        }

        public static uint PlanoContaJurosAntecip
        {
            get { return Config.GetConfigItem<uint>(Config.ConfigEnum.PlanoContaJurosAntecip); }
        }

        public static uint PlanoContaIOFAntecip
        {
            get { return Config.GetConfigItem<uint>(Config.ConfigEnum.PlanoContaIOFAntecip); }
        }

        public static uint PlanoContaJurosReceb
        {
            get { return Config.GetConfigItem<uint>(Config.ConfigEnum.PlanoContaJurosReceb); }
        }

        public static uint PlanoContaMultaReceb
        {
            get { return Config.GetConfigItem<uint>(Config.ConfigEnum.PlanoContaMultaReceb); }
        }

        public static uint PlanoContaJurosPagto
        {
            get { return Config.GetConfigItem<uint>(Config.ConfigEnum.PlanoContaJurosPagto); }
        }

        public static uint PlanoContaMultaPagto
        {
            get { return Config.GetConfigItem<uint>(Config.ConfigEnum.PlanoContaMultaPagto); }
        }

        public static uint PlanoContaEstornoJurosReceb
        {
            get { return Config.GetConfigItem<uint>(Config.ConfigEnum.PlanoContaEstornoJurosReceb); }
        }

        public static uint PlanoContaEstornoMultaReceb
        {
            get { return Config.GetConfigItem<uint>(Config.ConfigEnum.PlanoContaEstornoMultaReceb); }
        }

        public static uint PlanoContaEstornoJurosPagto
        {
            get { return Config.GetConfigItem<uint>(Config.ConfigEnum.PlanoContaEstornoJurosPagto); }
        }

        public static uint PlanoContaEstornoMultaPagto
        {
            get { return Config.GetConfigItem<uint>(Config.ConfigEnum.PlanoContaEstornoMultaPagto); }
        }

        public static uint PlanoContaComissao
        {
            get { return Config.GetConfigItem<uint>(Config.ConfigEnum.PlanoContaComissao); }
        }

        public static uint PlanoContaJurosCartao
        {
            get { return Config.GetConfigItem<uint>(Config.ConfigEnum.PlanoContaJurosCartao); }
        }

        public static uint PlanoContaEstornoJurosCartao
        {
            get { return Config.GetConfigItem<uint>(Config.ConfigEnum.PlanoContaEstornoJurosCartao); }
        }

        public static uint PlanoContaTarifaUsoBoleto
        {
            get { return Config.GetConfigItem<uint>(Config.ConfigEnum.PlanoContaTarifaUsoBoleto); }
        }

        public static uint PlanoContaTarifaUsoProtesto
        {
            get { return Config.GetConfigItem<uint>(Config.ConfigEnum.PlanoContaTarifaUsoProtesto); }
        }

        /// <summary>
        /// Verifica se a empresa vai emitir boletos apenas se a forma de pagto. da conta for boleto
        /// </summary>
        public static bool EmitirBoletoApenasContaTipoPagtoBoleto
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EmitirBoletoApenasContaTipoPagtoBoleto); }
        }

        /// <summary>
        /// Define se será permitido emitir boleto sem ter nota fiscal
        /// </summary>
        public static bool EmitirBoletoSemNota
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EmitirBoletoSemNota); }
        }

        /// <summary>
        /// Verifica se a loja da conta do boleto deve ser a mesma loja da nota fiscal
        /// </summary>
        public static bool BloquearGeracaoBoletoApenasParaLojaQueForFeitoNf
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearGeracaoBoletoApenasParaLojaQueForFeitoNf); }
        }

        /// <summary>
        /// Verifica se ao receber um pagamento que utilize conta bancaria deva mostrar apenas as contas bancarias da loja do cadastro do cliente
        /// </summary>
        public static bool ReceberApenasComLojaContaBancoIgualLojaCliente
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ReceberApenasComLojaContaBancoIgualLojaCliente); }
        }

        public static bool UsarControleLiberarFinanc
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarControleLiberarFinanc); }
        }

        /// <summary>
        /// Define que apenas administradores podem cancelar devoluções de pagamento
        /// </summary>
        public static bool ApenasAdminCancelaDevolucao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ApenasAdminCancelaDevolucao); }
        }

        /// <summary>
        /// Verifica se na geração do cnab e no boleto deve ser usado o numero da nf-e no campos numero do documento
        /// Mesmo se a empresa não utilizar separação de valores
        /// </summary>
        public static bool UsarNumNfBoletoSemSeparacao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarNumNfBoletoSemSeparacao); }
        }

        public static bool UsarDescontoEmParcela
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarDescontoEmParcela); }
        }

        public static bool FiltroContasVinculadasMarcadoPorPadrao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.FiltroContasVinculadasMarcadoPorPadrao); }
        }

        /// <summary>
        /// Percentual de redução para produtos de revenda
        /// </summary>
        public static float PercDescontoRevendaPadrao
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.PercDescontoRevendaPadrao); }
        }

        /// <summary>
        /// Define a opção que deve ser marcada por padrão, na tela de contas a receber, para o filtro Contas Antecipadas.
        /// </summary>
        public static int OpcaoPadraoFiltroContasAntecipadasContasReceber
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.OpcaoPadraoFiltroContasAntecipadasContasReceber); }
        }

        /// <summary>
        ///Tempo para emitir o alerta que o faturamento esta inoperante, sem liberar pedidos
        /// </summary>
        public static int TempoAlertaFaturamentoInoperante
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.TempoAlertaFaturamentoInoperante); }
        }

        /// <summary>
        /// Define que a referência da nota será exibida na listagem de cheques
        /// </summary>
        public static bool ExibirReferenciaDeNotaListaCheques
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirReferenciaDeNotaListaCheques); }
        }

        /// <summary>
        /// Define se a empresa gera arquivo GCon
        /// </summary>
        public static bool GerarArquivoGCon
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarArquivoGCon); }
        }

        /// <summary>
        /// Define se a empresa gera arquivo Prosoft
        /// </summary>
        public static bool GerarArquivoProsoft
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarArquivoProsoft); }
        }

        /// <summary>
        /// Define se a empresa gera arquivo Dominio
        /// </summary>
        public static bool GerarArquivoDominio
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarArquivoDominio); }
        }

        /// <summary>
        /// Define qual relatório de cheques será usado
        /// </summary>
        public static bool UsarRelatorioChequePaisagem
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarRelatorioChequePaisagem); }
        }

        /// <summary>
        /// Impede que contas CF sejam geradas no CNAB
        /// </summary>
        public static bool ImpedirGeracaoCnabContaCF
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ImpedirGeracaoCnabContaCF); }
        }

        /// <summary>
        /// Define se a opção de usar crédito virá marcada por padrão na tela de devolução de pagto
        /// </summary>
        public static bool OpcaoUsarCreditoMarcadaDevolucaoPagto
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.OpcaoUsarCreditoMarcadaDevolucaoPagto); }
        }

        /// <summary>
        /// Define se será usado boletos da Lumen nos planos de conta
        /// </summary>
        public static bool UsarPlanoContaBoletoLumen
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarPlanoContaBoletoLumen); }
        }

        /// <summary>
        /// Define se será subtraído o juros das movimentações no DRE
        /// </summary>
        public static bool SubtrairJurosDRE
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SubtrairJurosDRE); }
        }

        /// <summary>
        /// Define que o boleto ficará visível apenas se for contábil
        /// </summary>
        public static bool BoletoVisivelApenasContabil
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BoletoVisivelApenasContabil); }
        }

        /// <summary>
        /// Define se deverá ser exibida a conta bancária do primeiro boleto impresso.
        /// </summary>
        public static bool ExibirContaBancariaPrimeiroBoletoImpresso
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirContaBancariaPrimeiroBoletoImpresso); }
        }

        /// <summary>
        /// Define se será utilizado o controle de desconto por forma de pagamento e dados do produto, só funciona se UsarDescontoEmParcela estiver desabilitada
        /// </summary>
        public static bool UsarControleDescontoFormaPagamentoDadosProduto
        {
            get
            {
                if (UsarDescontoEmParcela)
                    return false;

                return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarControleDescontoFormaPagamentoDadosProduto);
            }
        }

        /// <summary>
        /// Define se a empresa utilizará uma constante para definição do campo (nosso número no boleto)
        /// </summary>
        public static bool UtilizarConstanteNossoNumeroBoleto
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UtilizarConstanteNossoNumeroBoleto); }
        }
    }
}
