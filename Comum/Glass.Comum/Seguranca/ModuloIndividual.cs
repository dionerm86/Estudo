namespace Glass.Seguranca
{
    public enum ModuloIndividual : int
    {
        CadastroFuncionário = 1,
        CadastroProduto,
        MenuSite,
        CadastroFornecedor,
        CancelarPedido,                         // 5
        CadastroVeículo,
        CalculoArea,
        CaixaDiario,
        PCP,
        CadastroCliente,                        // 10 - Permite excluir clientes
        InativarCliente,
        EfetuarMedicao,
        Medidor,                                // Permite ser adicionado como medidor na tela de efetuar medição
        InserirPrecoCusto,                      // Dá acesso à tela de inserção de preço de custo rápido
        ControleInstalacaoComum,                // 15
        ControleInstalacaoTemperado,
        ControleConferenciaMedicao,
        CadastroComissionado,
        Conferente,
        MarcarRevenda,                          // 20
        ControleEstoque,
        FinanceiroRecebimento,
        FinanceiroPagamento,
        ConfirmarPedidoLiberacao,
        NFe,                                    // 25
        GerarReposicao,
        EmitirPedido,
        EfetuarLoginQualquerMomento,            // Permite efetuar login no sistema a qualquer momento, sem restrição de horário
        EfetuarPedidoInterno,
        MarcarPerdaProducao,                    // 30
        VisualizarMemoriaCalculo,
        EmitirPedidoFuncionario,
        AlteracaoManualNFe,
        AlterarSinalMinimoCliente,
        EmitirPedidoGarantiaReposicao,          // 35
        AlterarNfeContingencia,
        ReimprimirEtiqueta,
        IgnorarBloqueioDataEntregaVendedor,
        PermitirMarcarFastDelivery,
        VoltarSituacaoPecaProducao,             // 40 - Permite apagar a última leitura da peça na produção
        ImprimirEtiquetaMaoDeObra,
        IgnorarBloqueioDesconto,
        AnexarArquivoPedido,
        EditarQualquerCompra,
        ConfiguracoesProjeto,                   // 45
        DescontoAcrescimoContaReceber,
        MovimentacaoCreditoCliente,
        AlterarEstoqueManualmente,
        AtrasarPedidoFunc,
        EfetuarTrocaDevolucao,                  // 50
        ExibirIconeAlterarPedidoConfirmado,
        AlterarPercRedNfeCliente,
        AlterarLimiteCliente,
        ExibirIconeAlterarPedidoConfirmadoVend,
        ExportarImportarPedido,                 // 55
        ConsultaRapidaCliente,
        DescontoAcrescimoCliente,
        CadastrarRota,
        ExibirSubMenusCadastro,
        GerarCreditoAvulsoCliente,              // 60
        AlterarImagemPecaAposImpressao,
        ApenasConsultarInstalacao,
        RelatorioVendas,
        RelatorioProdutos,
        CadastroSugestoes,                      // 65
        AnexarArquivoCliente,
        RelatorioOrcamentos,
        RelatorioEntregaPorRota,
        ImprimirEtiquetaNfe,
        PagtoAntecipadoObraVendedor,             // 70
        DefinicaoCargaRota,
        PedidosM2Peso,
        PedidosEmConferencia,
        GerarConferenciaPedido,
        ImpressaoPedido,                        //75
        CancelarImpressaoEtiqueta,
        ImprimirEtiquetas,
        AlterarVendedorPedido,
        MarcarPerdaChapaVidro,
        EnviarSMS,                              //80
        AlterarVendedorCadCliente,
        IgnorarFuncVisualizaDadosApenasSuaLoja,
        InativarFuncionario,
        PagtoAntecipadoPedido,
        AlterarAliquotaICMSSimplesNacional,      //85
        ExibirTotalCompradoCliente,
        AnexarArquivoLiberacaoListaPedido,
        ReimprimirEtiquetaBox = 102
    }
}
