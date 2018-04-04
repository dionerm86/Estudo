using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using System.Linq;

namespace Glass.Data.Helper
{
    public static class Config
    {
        #region Enumeradores

        /// <summary>
        /// Enumeração com os itens do banco de dados.
        /// </summary>
        public enum ConfigEnum
        {
            PrazoEntregaOrcamento = 1,
            ValidadeOrcamento = 2,
            FormaPagtoOrcamento = 3,
            DescontoMaximoPedidoAVista = 4,
            TaxaFastDelivery = 5,
            PrazoFastDelivery = 6,
            M2MaximoFastDelivery = 7,
            CobrarTranspasse = 10,
            ExibirItensProdutosRelatorio = 12,
            AgruparBenefRelatorio = 13,
            ExibirM2CalcRelatorio = 14,
            BloqueioPedidoMaoDeObra = 16,
            NumeroFormasPagto = 17,
            DescontoMaximoOrcamento = 18,
            ProcessoInstalacao = 19,
            AplicacaoInstalacao = 20,
            ProcessoCaixilho = 21,
            AplicacaoCaixilho = 22,
            UsarConferenciaFluxo = 23,
            AliquotaICMSSimplesNacional = 24,
            LiberarProdutosProntos = 25,
            NumParcelasPedido = 26,
            SIntegraRegistro50 = 27,
            SIntegraRegistro51 = 28,
            SIntegraRegistro53 = 29,
            SIntegraRegistro54 = 30,
            SIntegraRegistro61 = 31,
            SIntegraRegistro75 = 32,
            LimiteDiarioMedicoes = 34,
            SIntegraRegistro70 = 35,
            AgruparProdutosGerarNFe = 37,
            CalcularAreaMinimaApenasVidroBeneficiado = 40,
            UsarTamanhoMaximoVidro = 41,
            AlturaMaximaVidro = 42,
            LarguraMaximaVidro = 43,
            InformarPedidoNaNfe = 44,
            NumeroParcelasNFe = 45,
            DescontoAcrescimoItensOrcamento = 47,
            PlanoContaBloquearEntradaSaida = 48,
            PlanoContaTaxaAntecip = 49,
            PlanoContaJurosAntecip = 50,
            PlanoContaIOFAntecip = 51,
            SIntegraRegistro74 = 52,
            PlanoContaJurosReceb = 53,
            PlanoContaMultaReceb = 54,
            PlanoContaJurosPagto = 55,
            PlanoContaMultaPagto = 56,
            ContingenciaNFe = 57,
            JustificativaContingenciaNFe = 58,
            PlanoContaEstornoJurosReceb = 59,
            PlanoContaEstornoMultaReceb = 60,
            PlanoContaEstornoJurosPagto = 61,
            PlanoContaEstornoMultaPagto = 62,
            NFeModoProducao = 63,
            ControlarCreditoFornecedor = 64,
            SoImprimirPedidoConfirmado = 65,
            EfetuarRetiradaCaixaDiario = 66,
            NumeroParcelasRenegociar = 67,
            CobrarMedidasExatasPedido = 68,
            ComissaoPedido = 70,
            NegociarParcialmente = 72,
            GerarCreditoFormasPagto = 74,
            SempreInserirItensProdutosOrcamento = 75,
            NumeroDiasUteisDataEntregaPedido = 76,
            BloquearDataEntregaPedidoVendedor = 77,
            FastDelivery = 78,
            NumMaxDiasFastDelivery = 79,
            LiberacaoParcial = 82,
            LiberarClienteRotaSemEstarPronto = 83,
            NumeroParcelasCompra = 84,
            TaxaJurosConstrucard = 85,
            CobrarJurosCartaoCliente = 86,
            SaidaEstoqueAoLiberarPedido = 87,
            AlterarPercComissionado = 88,
            NumeroViasNotaPromissoria = 89,
            SepararTiposChequesRecebimento = 90,
            NumeroFormasPagtoContasPagar = 91,
            UsarControleNovoObra = 93,
            SaidaEstoqueBoxLiberar = 94,
            ExibirDatasParcelasPedido = 95,
            BloquearDadosClientePedido = 96,
            DescontoPedidoApenasAVista = 97,
            BloquearLiberacaoDadosPedido = 98,
            DescontoLiberarPedido = 99,
            UsarMenorPrazoLiberarPedido = 100,
            NumeroDiasChequeVistaLiberarPedido = 101,
            ImpedirDescontoSomativo = 102,
            ImpedirConfirmacaoPedidoPagamento = 103,
            BloquearChequesDataRetroativa = 104,
            PeriodoApuracaoIpi = 105,
            BloquearChequesDigitoVerificador = 106,
            GerarEFD = 107,
            InformarOrcamentoNaNFe = 108,
            NumeroDiasPedidoProntoAtrasado = 109,
            NumeroDiasUteisDataEntregaPedidoRevenda = 110,
            EnviarEmailPedidoPronto = 111,
            TipoEntregaPadraoPedido = 112,
            PlanoContaComissao = 113,
            EnviarEmailPedidoConfirmado = 114,
            GerarArquivoMesaCorte = 115,
            UsarRelatorioLiberacao4Vias = 116,
            CalcularIcmsPedido = 117,
            CalcularIcmsLiberacao = 118,
            CalcularIpiPedido = 119,
            CalcularIpiLiberacao = 120,
            RatearIpiNfPedido = 121,
            AliquotaIcmsStRatearIpiNfPedido = 122,
            CalculoAliquotaIcmsSt = 123,
            RelatorioPrecoTabelaClientes = 124,
            AlterarValorUnitarioProduto = 125,
            TipoExportacaoEtiqueta = 126,
            TipoDataEtiqueta = 127,
            TipoControleReposicao = 128,
            DescontoPedidoLista = 129,
            ObrigarProcAplVidros = 130,
            EnviarPedidoAnexoEmail = 131,
            PedidoJurosCartao = 132,
            QuitarParcCartaoDebito = 133,
            ObrigatorioCorAlumFerragem = 134,
            PlanoContaJurosCartao = 135,
            PlanoContaEstornoJurosCartao = 136,
            BloquearItensTipoPedido = 137,
            NumeroDiasImpedirGerarCreditoCheque = 138,
            UsarControleCompraContabilNF = 140,
            ExibirItensProdutosPedido = 141,
            ApenasAdminCancelaLiberacao = 142,
            LiberarPedidoAtrasadoParcialmente = 143,
            PeriodoInativarClienteUltimaCompra = 144,
            EnviarSMSPedidoPronto = 145,
            UsarPlanoCorte = 146,
            ApenasVidrosPadrao = 147,
            DiasDataFabrica = 148,
            CompraCalcMult5 = 149,
            CartaoMovimentaCxGeralDiario = 150,
            ExibirDadosPcpListaAposConferencia = 151,
            ExibePopupVidrosEstoque = 152,
            CompraSemValores = 153,
            AmbientePedido = 155,
            ExibirImpressaoPcpListaPedidos = 156,
            DiasMinimosEntregaTipo = 157,
            EntradaEstoqueManual = 158,
            SaidaEstoqueManual = 159,
            LimitePadraoCliente = 161,
            NumeroDiasContaRecAtrasada = 163,
            AgruparResumoLiberacaoProduto = 164,
            DescontoMaximoLiberacao = 165,
            TipoNomeExibirRelatorioPedido = 166,
            AlterarUrlWebGlassFornec = 168,
            GerarOrcamentoFerragesAluminiosPCP = 169,
            DescontoPadraoPedidoAVista = 170,
            EnviarSMSAdministrador = 171,
            NumeroDiasUteisDataEntregaPedidoMaoDeObra = 172,
            NumeroCasasDecimaisTotM = 173,
            UsarComissionadoCliente = 174,
            RatearIcmsStNfPedido = 175,
            ExibirDescricaoParcelaLiberacao = 176,
            BuscarVendedorEmitirPedido = 177,
            ExibirBotoesConfirmacaoPedido = 178,
            UsarTabelasDescontoAcrescimoCliente = 179,
            ObrigarInformarPedidoCliente = 181,
            ComissaoAlteraValor = 182,
            ListaApenasOrcamentosVendedor = 183,
            ListaApenasPedidosVendedor = 184,
            LimitarCadastroCliente = 185,
            LimitarCadastroProduto = 186,
            ObrigarLeituraSetorImpedirAvanco = 188,
            EmpresaConsideraChequeLimite = 189,
            EmpresaConsideraPedidoConferidoLimite = 190,
            EmpresaConsideraPedidoAtivoLimite = 191,
            CodigoClienteUsado = 192,
            DescontoPorProduto = 193,
            IndicadorIncidenciaTributaria = 194,
            MetodoApropriacaoCreditos = 195,
            TipoContribuicaoApurada = 196,
            ExibirValorProdutosInstalacao = 197,
            TipoContribuicaoSocialPadrao = 198,
            TipoNotaBuscarContribuicaoSocialPadrao = 199,
            TipoCreditoPadrao = 200,
            TipoNotaBuscarCreditoPadrao = 201,
            UsarControleChapaCorte = 202,
            PerComissaoPedido = 203,
            TipoControleSaldoCreditoIcms = 204,
            PercAproveitamentoCreditoIcms = 205,
            CodigoAjusteAproveitamentoCreditoIcms = 206,
            PerfilArquivoEfdFiscal = 207,
            UsarBeneficiamentosTodosOsGrupos = 208,
            FolgaRetalho = 209,
            PeriodoInativarClienteUltimaConsultaSintegra = 210,
            UsarControleRetalhos = 211,
            DescontoPedidoUmaParcela = 212,
            NumeroViasAlmoxarifeLiberacao = 213,
            NumeroViasExpedicaoLiberacao = 214,
            UsarAmbienteInstalacao = 215,
            GerarInstalacaoManual = 218,
            GerarInstalacaoAutomaticamente = 219,
            ConsiderarChequeDepositadoVencidoNoLimite = 220,
            UtilizaFCI = 223,
            CorAluminiosProjetosApenasVidrosNFe = 224,
            CorFerragensProjetosApenasVidrosNFe = 225,
            FuncVisualizaDadosApenasSuaLoja = 226,
            BloquearItensCorEspessura = 229,
            UsarPgtoAntecipFornec = 230,
            NumDiasAposVencContaRecEnviarEmailCli = 231,
            NumDiasAnteriorVencContaRecEnviarEmailCli = 232,
            UsarControleCobrancaEmail = 233,
            EnviarEmailAdministrador = 234,
            SepararValoresFiscaisEReaisContasReceber = 235,
            BloquearEmissaoPedidoLimiteExcedido = 237,
            UsarControleFinalizacaoCompra = 238,
            LimitarChequesPorCpfOuCnpj = 239,
            PermitirFinalizacaoPedidoPeloFinanceiro = 240,
            OpcaoPagtoAntecipadoPadraoMarcada = 241,
            PercMinimoSinalPedidoPadrao = 242,
            DestacarProdutoChapaImportada = 243,
            SepararValoresFiscaisEReaisContasPagar = 244,
            CTeModoProducao = 245,
            TipoUsoAntecipacaoFornecedor = 247,
            UsarControleOrdemCarga = 248,
            MetaProducaoDiaria = 249,
            NaoExigirEnderecoConsumidorFinal = 250,
            ContingenciaCTe = 251,
            JustificativaContingenciaCTe = 252,
            PermitirApenasContasMesmoTipoEncontroContas = 253,
            PerguntarVendedorFinalizacaoFinanceiro = 255,
            EnviarEmailAdministradorDescontoMaior = 256,
            AdministradorEnviarEmailDescontoMaior = 257,
            ControlarEstoqueVidrosClientes = 258,
            PermitirTrocaPorPedido = 260,
            ConsiderarMetaProducaoM2PecasPorDataFabrica = 261,
            UtilizarSequenciaRoteiroProducao = 262,
            EnviarEmailAoLiberarPedido = 263,
            SaidaEstoqueVolume = 264,
            UtilizarEditorImagensProjeto = 265,
            UsarTipoCalculoNfParaCompra = 266,
            ExibirApenasViaExpAlmPedidosEntrega = 267,
            ExibirApenasViaClienteNoEnvioEmail = 268,
            InformarFormaPagtoNaNFe = 271,
            InformarPedidoClienteNaNFe = 272,
            BloquearMaisDeUmaNfeParaUmPedido = 273,
            InformarPedCliNoProdutoNFe = 274,
            BloquearEmissaoNFeApenasPedidosLiberados = 275,
            ExibirLojaCadastroPedido = 277,
            NumeroDiasIgnorarClientesRecemAtivosInativarAutomaticamente = 278,
            BloquearExpedicaoApenasPecasProntas = 281,
            AliquotaPis = 282,
            AliquotaCofins = 283,
            PlanoContaTarifaUsoBoleto = 285,
            PlanoContaTarifaUsoProtesto = 286,
            GerarNotaApenasDeLiberacao = 287,
            PermitirConfirmacaoPedidoPeloFinanceiro = 288,
            PerguntarVendedorConfirmacaoFinanceiro = 289,
            ValidacaoProjetoConfiguravel = 290,
            ExibirCnab = 291,
            InserirImagemProjetoPedido = 292,
            ExibirRelatoriosCompras = 293,
            ControleMedicao = 295,
            ControleInstalacao = 296,
            BloquearLeituraPlanoCorteLoteGenerico = 297,
            BloquearLeituraPecaLoteGenerico = 298,
            CobrarPedidoReposicao = 299,
            ManterDescontoAdministrador = 300,
            BloquearClienteAoDevolverProtestarCheque = 301,
            UtilizaNFCe = 302,
            ConfigAresta = 303,
            TempoAlertaComercialInoperante = 304,
            TempoAlertaFaturamentoInoperante = 305,
            ControleConferencia = 306,
            ControlePCP = 308,
            ControleCaixaDiario = 309,
            SistemaLite = 310,
            ComissaoPorContasRecebidas = 311,
            QuantidadeDiasSomarDataMovimentacaoBancariaCartaoDebito = 312,
            QuantidadeDiasSomarDataMovimentacaoBancariaCartaoCredito = 313,
            BloqEmisPedidoPorPosicaoMateriaPrima = 314,
            PercentualFundoPobreza = 315,
            UsarM2CalcBeneficiamentos = 316,
            UsarControleCentroCusto = 317,
            ConsiderarM2CalcNotaFiscal = 318,
            AdicionalVidroRedondoAte12mm = 319,
            AdicionalVidroRedondoAcima12mm = 320,
            PermitirInserirClienteInativoBloqueado = 321,
            GerarVolumeApenasDePedidosEntrega = 322,
            ExibirNomeFantasiaClienteRptCarregamento = 323,
            ExibirRazaoSocialClienteVolume = 324,
            HorariosEnvioEmailSmsAdmin = 325,
            ImpedirLiberacaoPedidoSemPCP = 326,
            UsarControleGerenciamentoProjCnc = 327,
            ConcatenarEspAltLargAoNumEtiqueta = 328,
            GerarMarcacaoPecaReposta = 329,
            GerarDxf = 330,
            GerarFml = 331,
            CalcularMultiplo10 = 332,
            ManterCodInternoCampoAoInserirProduto = 333,
            ImpedirLeituraChapaComPlanoCorteVinculado = 334,
            ImpedirLeituraTodasPecasPedido = 335,
            MedidaExataPadrao = 336,
            DescontarComissaoPerc = 337,
            EnviarEmailPedidoConfirmadoVendedor = 338,
            ExibirApenasViaExpAlmPedidosBalcao = 339,
            GerarPedidoProducaoCorte = 340,
            HabilitarFaturamentoCarregamento = 341,
            ExibirCADecommerce = 342,
            PermitirLiberacaoPedidosLojasDiferentes = 343,
            ControleCavalete = 344,
            AdministradorEnviarEmailSmsMensagemPrecoProdutoAlterado = 345,
            HabilitarOtimizacaoAluminio = 346,
            ArestaBarraAluminioOtimizacao = 347,
            ArestaGrau45AluminioOtimizacao = 348,
            ArestaGrau90AluminioOtimizacao = 349,
            AcrescimoBarraAluminioOtimizacaoProjetoTemperado = 350,
            GerenciamentoFornada = 351,
            GerarSGlass = 352,
            PrazoMaxDiaUtilRealizarTrocaDev = 353,
            PermitirApenasPedidosDeVendaNoEcommerce = 354,
            GerarArquivoIntermac = 355,
            PlanoContaQuitacaoParcelaCartao = 356,
            PlanoContaEstornoQuitacaoParcelaCartao = 357,
            OrdemCargaParcial = 358,
            MDFeModoProducao = 359,
            ContingenciaMDFe = 360,
            JustificativaContingenciaMDFe = 361,
            UsarComissaoPorTipoPedido = 362,
            DescontoMaximoPedidoAVistaGerente = 363,
            ClienteInativoBloqueadoEmitirPedidoComAutorizacaoFinanceiro = 364,
            DescontoMaximoPedidoAPrazo = 365,
            DescontoMaximoPedidoAPrazoGerente = 366,
            PermitirEmitirNotaParaClienteBloqueadoOuInativo = 367,

            #region Configs Suporte (Antiga config interna)

            NomeFuncaoJavascriptCalculoValorAdicional = 1002,
            NumeroDigitosCheque = 1004,
            CadastrarClienteInativo = 1005,
            ListarAtivosPadrao = 1006,
            ExigirFuncionarioAoInserir = 1008,
            PermitirCpfCnpjTudo9AoInserir = 1010,
            ExibirIgnorarBloqueioApenasAdministrador = 1011,
            MarcarBloquearChequesAoInserir = 1012,
            ExigirEmailClienteAoInserirOuAtualizar = 1013,
            ExibirInformacoesFinanceiras = 1014,
            UsarPercComissaoCliente = 1016,
            TotalParaComissao = 1017,
            UsarComissaoPorProdutoInstalado = 1018,
            ExibirObsComissionadoOuFuncionarioRelatorioComissao = 1019,
            AtualizarValorProdutoFinalizarCompraComBaseMarkUp = 1021,
            ExibicaoDescrBenefCustomizada = 1022,
            EnviarEmailAdministradorDescontoMaiorIgualDescontoParcela = 1027,
            EnviarEmailDescontoMaiorApenasAdminConfig = 1028,
            ConsiderarReposicaoGarantiaTotalPedidosEmitidos = 1029,
            UsuariosQueDevemReceberEmailDescontoMaior = 1030,
            TextoEmailPedidoFinalizadoPcp = 1031,
            TextoEmailPedidoProntoBalcao = 1032,
            TextoEmailPedidoProntoEntrega = 1033,
            ConsiderarTotalProdNfMovEstoqueFiscal = 1034,
            ExibirQuantidadeProdutosAbaixoOuNoEstoqueMinimoTopoTela = 1035,
            PermitirAlterarFuncionarioPedidoInterno = 1036,
            ExibirPedidosEstoqueFiscal = 1037,
            PermitirAlterarFuncionarioTrocaDevolucao = 1038,
            ExibirValoresNegativosProdutoTrocado = 1039,
            PermitirAlterarCreditoGerado = 1040,
            Girar90GrausCodigoDeBarras = 1041,
            UsarThreadRelatorioEtiqueta = 1042,
            CarregarDescricaoGrupoProjeto = 1043,
            UsarNumeroSequencial = 1044,
            ModeloEtiquetaPorLoja = 1045,
            InformarValorTransf = 1046,
            ExibirPedidosDoSinal = 1047,
            ExibirPedidosDoAcerto = 1048,
            ExibirPedidosDaLiberacao = 1049,
            CxGeralSaldoTotal = 1050,
            CxGeralTotalCumulativo = 1051,
            ExibirDebitoCredito = 1053,
            ExibirPedidosLiberacaoDebitos = 1055,
            DesabilitarCamposImpessaoBoletoBancoDoBrasil = 1057,
            ExibirTelefoneComNomeCliente = 1058,
            ExibirTelefoneComNomeClienteEEndereco = 1059,
            ExibirNumeroEtiquetaLiberacao = 1060,
            ExibirAsQuatroPrimeirasEtiquetasNaLiberacao = 1061,
            ExibirAcrescimoNaLiberacao = 1063,
            MarcaPedidoRevendaEntregueAoLiberar = 1064,
            EmitirBoletoApenasContaTipoPagtoBoleto = 1065,
            EmitirBoletoSemNota = 1066,
            BloquearGeracaoBoletoApenasParaLojaQueForFeitoNf = 1067,
            ReceberApenasComLojaContaBancoIgualLojaCliente = 1068,
            UsarControleLiberarFinanc = 1069,
            ApenasAdminCancelaDevolucao = 1070,
            UsarNumNfBoletoSemSeparacao = 1071,
            UsarDescontoEmParcela = 1072,
            FiltroContasVinculadasMarcadoPorPadrao = 1073,
            PercDescontoRevendaPadrao = 1074,
            ExibirReferenciaDeNotaListaCheques = 1076,
            GerarArquivoGCon = 1077,
            GerarArquivoProsoft = 1078,
            GerarArquivoDominio = 1079,
            UsarRelatorioChequePaisagem = 1080,
            ImpedirGeracaoCnabContaCF = 1081,
            OpcaoUsarCreditoMarcadaDevolucaoPagto = 1082,
            UsarPlanoContaBoletoLumen = 1083,
            SubtrairJurosDRE = 1084,
            BoletoVisivelApenasContabil = 1085,
            ExibirContaBancariaPrimeiroBoletoImpresso = 1086,
            UsarControleDescontoFormaPagamentoDadosProduto = 1087,
            ImpedirPagamentoPorLoja = 1088,
            SepararListagemCompras = 1089,
            CompraLojaPadrao = 1090,
            ExibirRazaoSocialContasPagarPagas = 1091,
            ExibirObsCompraContasPagar = 1092,
            ExibirObsImpostoServico = 1093,
            SubtrairICMSCalculoComissao = 1094,
            ExibirIdPedidoComLiberacaoContasRec = 1095,
            ExibirIdPedidoComAcertoContasRec = 1096,
            ExibirPedCliComIdPedidoContasRec = 1097,
            ImpedirRecebimentoPorLoja = 1098,
            SelecionarContaBancoQuitarParcCartao = 1099,
            ImprimirCompraComBenef = 1101,
            ApenasFinancGeralAdminSelFuncCxGeral = 1103,
            EsconderInfoCreditoAcerto = 1104,
            EsconderInfoCreditoLiberacao = 1105,
            UsarClienteDaNotaNoBoleto = 1107,
            UsarClienteDaNotaNoCnab = 1108,
            UsarClienteLiberacaoSeparacaoDeValores = 1109,
            UsarLojaDoBancoNoBoleto = 1110,
            DiasConsiderarChequeCompensado = 1111,
            ImpedirSeparacaoValorSePossuirPagtoAntecip = 1113,
            ExibirPedidosDaLiberacaoMovCredito = 1114,
            PermitirRecebimentoObraClienteDataAnteriorDataAtual = 1115,
            PermitirChequeDataAtual = 1116,
            PermitirFormaPagtoPermutaApenasAdministrador = 1117,
            FormaPagtoPadraoDesmarcada = 1118,
            ExibirApenasCartaoDebito = 1119,
            NomeArquivoRelatorioContasPagar = 1120,
            NomeArquivoRelatorioContasReceber = 1122,
            ExibirPedCli = 1123,
            NomeArquivoRelatorioContasRecebida = 1124,
            ExibirPedidos = 1125,
            ExibirComissao = 1126,
            PagamentoChequeDevolvidoOutrosConsiderarCredito = 1127,
            BloquearRecebimentoPagtoAntecipadoPedidoAtivo = 1128,
            PermitirCaixaAlterarDataConsulta = 1132,
            ExibirRazaoSocial = 1133,
            ExibirGridNotaFiscal = 1134,
            ExibirLogomarcaNoDacte = 1135,
            EsconderComplEDadosTransRodCteSaida = 1136,
            ZerarDadosIpiCstIgual49RegistroC190 = 1137,
            ZerarDadosRegistroC170NotaSaidaSeCst60 = 1138,
            ZerarDadosIcmsRegistroC190NotaEntradaSaidaSeCst60 = 1139,
            IgnorarUsoEConsumoSPED = 1140,
            PermitirGerarNotaPedidoConferido = 1141,
            SomarImpostosValorUnMovFiscal = 1142,
            CorrecaoGeracaoCodigoBarrasDanfe = 1143,
            PercentualAGerarDeSobraDeProducao = 1144,
            CstOrigPadraoNotaFiscalSaida = 1145,
            GerarNotaFiscalCompraGerarEtiqueta = 1146,
            PermitirGerarNFPedMaoDeObra = 1147,
            CalcularIcmsStUtilizandoWebServiceMT = 1148,
            BloquearEmissaoNotaFiscalDePedidoMaiorQueOsPedidos = 1149,
            AgruparProdutosGerarNFeTerceiros = 1150,
            ExibirEnderecoEntregaInfCompl = 1151,
            BloquearEmissaoAVistaComContaAReceberDeLiberacao = 1152,
            EsconderLogotipoDANFEComLogo = 1153,
            ExibirEmailFiscalDANFE = 1154,
            DestacarNFe = 1155,
            ExibirFormaPagamentoLiberacaoInfCompl = 1156,
            CaractereConcatenarCodigoProdutoClienteComReducao = 1157,
            ConcatenarPrimeiraLetraBeneficiamentoCodigoProduto = 1158,
            ExibirLarguraEAlturaInfAdicProduto = 1159,
            ExibirQtdLarguraEAlturaInfAdicProduto = 1160,
            SeriePadraoNFe = 1162,
            CstIpiPadraoNotaEntrada = 1163,
            IpiIntegraBcPISCOFINS = 1164,
            ExportarNotaFiscalOutroBD = 1166,
            UsarNomeFantasiaNotaFiscal = 1167,
            PreencheTransporteSeBalcao = 1168,
            PreencheTransporteSeNaoForBalcao = 1169,
            PreencheTransporteComVeiculoOC = 1170,
            EspeciePadraoSeMesmoVeiculoOC = 1171,
            BloquearEmissaoNotaSePedidoPossuirContaRecebida = 1172,
            TextoPadraoInfComplNota = 1173,
            EsconderIcmsIcmsStIpiNotaSaida = 1174,
            DeduzirQtdTrocaProdutoNF = 1175,
            ExibirDescricaoPedidoInfCompl = 1176,
            ExibirTransportadorCampoDestinatario = 1177,
            AcrescentarLapBisProdutoNota = 1178,
            UsarProdutoCestaSeApenasVidros = 1180,
            PercentualCargaTributariaParaClienteSimplesNacionalCfop5401Cst10 = 1183,            
            RecuperarTotalCst60 = 1185,
            UsarDataCadNotaLivroRegistro = 1186,
            FormaPagtoPadraoNFCe = 1187,
            ReponsavelSeguroCtePadraoCteSaida = 1188,
            TipoPeriodoDataEntregaCtePadraoCteSaida = 1189,
            TipoPeriodoHoraEntregaCtePadraoCteSaida = 1190,
            ProdutoPredominanteInfoCtePadraoCteSaida = 1191,
            TipoUnidadeInfoCargaCtePadraoCteSaida = 1192,
            QuantidadeInfoCargaCtePadraoCteSaida = 1193,
            TipoMedidaInfoCargaCtePadraoCteSaida = 1194,
            CSTICMSImpostoCtePadraoCteSaida = 1195,
            CSTPISImpostoCtePadraoCteSaida = 1196,
            BCSTRetidoPISImpostoCtePadraoCteSaida = 1197,
            AliquotaPISImpostoCtePadraoCteSaida = 1198,
            ValorSTRetidoPISImpostoCtePadraoCteSaida = 1199,
            PISObrigatorioCTeSaida = 1200,
            CSTCOFINSImpostoCtePadraoCteSaida = 1201,
            BCSTRetidoCOFINSImpostoCtePadraoCteSaida = 1202,
            AliquotaCOFINSImpostoCtePadraoCteSaida = 1203,
            ValorSTRetidoCOFINSImpostoCtePadraoCteSaida = 1204,
            COFINSObrigatorioCTeSaida = 1205,
            TransportadorOrdemColetaCteRodPadraoCteSaida = 1206,
            TipoCtePadraoCteSaida = 1207,
            TipoServicoPadraoCteSaida = 1208,
            CidadeEnvioPadraoCteSaida = 1210,
            CidadeInicioPadraoCteSaida = 1211,
            SeriePadraoCteSaida = 1212,
            LotacaoConhecimentoTransRodPadraoCteSaida = 1213,
            ExibirTiposBoleto = 1214,
            NaoVendeVidro = 1216,
            EmpresaSomenteRevendeBox = 1217,
            ExibirAlertasAdministrador = 1218,
            ConsiderarLojaClientePedidoFluxoSistema = 1219,
            TextoAdicionalSMS = 1220,
            TextoSMSPedidoPronto = 1221,
            DataInicioEnvioSMSEmailAdministradores = 1222,
            DataFimEnvioSMSEmailAdministradores = 1223,
            ExibirNomeLojaTotalPedidoImportadoSMS = 1224,
            ArredondarBoxPara1900 = 1225,
            ArredondarBoxPara1900SubgrupoBoxPadrao = 1226,
            MultiploParaCalculoDeAramado = 1227,
            ConsiderarSabadoDiaUtil = 1228,
            ConsiderarDomingoDiaUtil = 1229,
            HoraInicioLogin = 1230,
            HoraFimLogin = 1231,
            ExibirRazaoSocialTelaSugestao = 1232,
            ExibirCompraCaixa = 1235,
            ExibirConsultaProducaoECommerce = 1236,
            ExibiPrecoTabelaECommerce = 1237,
            ExibirCartaoNaoIdentificado = 1238,
            FiltrarPorLojaComoPadrao = 1242,
            ExibirViaEmpresaPedidoReposicao = 1244,
            UsarCreditoMarcadoTelaLiberacaoPedido = 1245,
            UsarViaAlmoxarifadoIgualClienteSeMaoDeObra = 1246,
            ExibirProdutosViaEmpresa = 1247,
            NaoMostrarValorObraLiberacao = 1248,
            BloquearLiberacaoParcelasDiferentes = 1249,
            InformacaoAdicionalLiberacao = 1250,
            ExibirViaEmpresaRelatorio4Vias = 1251,
            TextoResumosCorteRelatorio4Vias = 1252,
            TextoResumoClienteRelatorio4Vias = 1253,
            TextoResumoEmpresaRelatorio4Vias = 1254,
            ExibirObsLiberacaoClienteViaExpedicao = 1255,
            ExibirObsLiberacaoClienteViaCliente = 1256,
            ExibirResumoCorteNaViaCliente = 1258,
            ExibirValoresResumosCorte = 1259,
            ExibirObservacaoCliente = 1260,
            ExibirObservacaoPedidos = 1261,
            NaoMostrarValorPedidoGarantia = 1263,
            ExibirResumoLiberacaoViaEmpresa = 1264,
            DuplicarViasDaLiberacaoSeClienteRota = 1265,
            NaoMostrarObsLiberacaoNaLiberacao = 1266,
            NaoMostrarObsLiberacaoClienteNaLiberacao = 1267,
            ExibirObsLiberacaoClienteApenasViaEmpresa = 1268,
            ExibirObsLibApenasViaEmpresa = 1269,
            ExibirObsLiberacaoResumo = 1270,
            TextoParcelasInvertido = 1271,
            OrdenarProdutosPeloCodInterno = 1273,
            ConsiderarVidroQualquerProdutoDoGrupoVidro = 1274,
            CorExibirObservacaoCliente = 1275,
            TiposPedidosSelecionadosPadrao = 1276,
            ExibirRelatorioCliente = 1277,
            EsconderLogotipoRelatorios = 1278,
            MedicaoApenasClienteCadastrado = 1280,
            OrdenarPeloIdMedicao = 1282,
            ImpedirEnvioDeMensagemAoDescontarParcela = 1283,
            ExibirBotaoExcluirMensagem = 1284,
            ExibirMensagemWebGlassParceiros = 1285,
            UploadImagensOrcamento = 1286,
            AlterarOrcamentoVendedor = 1287,
            AmbienteOrcamento = 1288,
            OrcamentoGeraMedicaoDefinitiva = 1289,
            FiltroPadraoAtivoListagem = 1291,
            ApenasAdminImprimeOrcamento = 1292,
            TipoOrcamentoPadrao = 1299,
            PermitirInserirSemTipoOrcamento = 1301,
            AtualizarDescricaoPaiAoInserirOuRemoverProduto = 1302,
            SituacoesClienteNaoGerarOC = 1304,
            ControlarPedidosImportados = 1305,
            ExibirPedCliRelCarregamento = 1306,
            ExibirEnderecoClienteRptOC = 1307,
            BloquearInclusaoPedidoComNotaGerada = 1308,
            PerguntarSeEnviaraEmailAoFinalizar = 1309,
            OpcaoAtualizarAutomaticamenteMarcada = 1310,
            DestacarAlturaLarguraSeAresta0OuCNC = 1312,
            ExportarEtiquetasOptywayOrdenadasCorEEspessura = 1313,
            ExibirTotalM2LidoNoDia = 1315,
            ExibirTotalQtdeLidoNoDia = 1316,
            ContabilizarPerdaChapaVidroNoTotalDePerdaPainel = 1317,
            BuscarProdutoPedidoAssociadoAoIdLojaFuncionarioAoBuscarProdutos = 1318,
            ExigirConferenciaPCP = 1319,
            ReabrirPCPSomenteAdmin = 1320,
            BloquearExclusaoProdutosPCP = 1321,
            ExibirPecasCancMaoObraPadrao = 1322,
            ExibirPecasCancLiberacao = 1323,
            UsarFormaProdutoSeForFormaInexistente = 1324,
            CriarClone = 1326,
            ExibirCustoVendaRelatoriosProducao = 1327,
            UsarNovoControleExpBalcao = 1328,
            PermirtirLerEtqBoxDuasVezesSeTransferencia = 1329,
            GerarPCPNaoProjetadoPedidosImportados = 1330,
            SituacaoRetalhoAoCriar = 1331,
            ImpedirCriarRetalhoPecaRepostaNaoCortada = 1332,
            ExportarProcessoObsRotaOptyway = 1333,
            ExibirOpcaoExportarApenasNaoExportadasOptyway = 1334,
            ExportarSerigrafiaPinturaOptyway = 1335,
            CriarArquivoSAGComNumDecimal = 1336,
            QtdDigitosNomeArquivoMesa = 1337,
            PreenchimentoFormaNaoExistente = 1338,
            GerarShapeIdVazioSePecaRepostaEEtiquetaSemForma = 1339,
            PrioridadeMaximaArquivoOptywaySeFastDeliveryOuPecaReposta = 1340,
            TipoArquivoMesaPadrao = 1341,
            PreencherReposicaoGarantiaCampoForma = 1342,
            PreencherRedondoCampoForma = 1343,
            FiltroPadraoDiaAtualTelaReposicao = 1344,
            NomeArquivoMesaBarraPorPontoVirgula = 1345,
            NomeArquivoMesaRecriado = 1347,
            EnviarOptywayRotacaoNSeCanelado = 1349,
            ExibirLarguraAlturaCorteCerto = 1350,
            SalvarArquivoBasicoAoFinalizarPCP = 1351,
            ExibirNumeroEtiquetaAcimaImagemPecaTelaMarcacao = 1352,
            GerarCreditoValorExcedente = 1355,
            PerguntarGerarCreditoAoFinalizar = 1356,
            PermitirFinalizarComDiferencaEPagtoAntecip = 1357,
            ExibirPercentualCom2CasasDecimais = 1358,
            ManterTipoEntregaPedido = 1359,
            EsconderPedidosLiberados = 1360,
            BuscarTodosProdutosNaoExportados = 1361,
            IntervaloDiasExportarPedido = 1362,
            ConsiderarTurnoFastDelivery = 1363,
            DescontoPedidoVendedorUmProduto = 1364,
            RatearDescontoProdutos = 1366,
            EmpresaTrabalhaAlturaLargura = 1367,
            LiberarPedido = 1368,
            EmpresaPermiteFinalizarPedidoAVistaSemVerificarLimite = 1370,
            ExibirProdutosPedidoAoLiberar = 1371,
            ExibirOpcaoDeveTransferir = 1372,
            AplicarComissaoDescontoAcrescimoAoInserirAtualizarApagarProdutoPedido = 1373,
            NaoObrigarPagamentoAntesProducaoParaPedidoRevenda = 1374,
            SalvarLogPecasImpressasNoPedido = 1375,
            AlterarLojaPedido = 1376,
            ExibirPedidosLiberadosECommerce = 1377,
            ExibirPedidosNaoEntregueCommerce = 1378,
            ReabrirPedidoComPagamentoAntecipado = 1379,
            ReabrirPedidoNaoPermitidoParceiros = 1380,
            ReabrirPedidoConfirmadoPCPTodosMenosVendedor = 1381,
            ReabrirPedidoNaoPermitidoComSinalRecebido = 1382,
            UsarValorReposicaoProduto = 1383,
            EnviarTotM2SMSPedidoPronto = 1384,
            ConsiderarDescontoClienteDescontoTotalPedido = 1385,
            ExibirRelatorioListaPedidosPaisagem = 1386,
            ExibirTotaisVendedorCliente = 1387,
            ExibirTotalML = 1389,
            MarcarPedidosRotaComoEntrega = 1390,
            AbrirImpressaoPedidoAoFinalizar = 1391,
            FinalizarConferenciaAoGerarEspelho = 1392,
            TipoPedidoPadrao = 1393,
            BuscarEnderecoClienteSeEstiverVazio = 1395,
            ExibirM2ChapaDeVidro = 1397,
            BuscarDataEntregaDeHojeSeDataVazia = 1398,
            ExibirCreditoClienteAoBuscar = 1399,
            SelecionarLojaGerarPedidoReposicao = 1404,
            GerarPedidoMarcado = 1408,
            FinalizarPedidoAoGerarEspelho = 1409,
            SubgruposDestacar = 1410,
            ExibirPedidosVendaPopUpConfirmarPedido = 1411,
            UsarImpressaoProjetoPcp = 1412,
            ExibirApenasPedidosComercialVendedor = 1413,
            ApenasAdminVisualizaTotais = 1414,
            ExibirLinhaAzulSePedidoPronto = 1415,
            ExibirLinhaPretaSeRevenda = 1416,
            ExibirLinhaVermelhaSePendenteOuTemAlteracaoPCP = 1417,
            CorLinhaSeImportadoOuGeradoParceiro = 1418,
            ExibirSituacaoPendenteECommerce = 1419,
            AuxAdministrativoAlteraPedidoLojaDele = 1420,
            ExibirCampoProdutosRelatorio = 1421,
            ExibirValorIPI = 1422,
            ObrigarMotivoPerda = 1424,
            BuscarDataFabricaConsultaProducao = 1425,
            ExpedirSomentePedidosLiberados = 1426,
            BloquearLeituraPVBDuplicada = 1427,
            SairDeProduzindoSePassarNoForno = 1428,
            BloquearLeituraPecaNaChapaDiasDiferentes = 1429,
            Adiciona2mmNaPeca = 1430,
            OrdenaPeloPedido = 1431,
            ConsiderarApenasPedidosEntregaDeRotaPainelComercial = 1433,
            ExibirTotalEtiquetaNaoImpressaPainel = 1434,
            DescrUsarTipoProducao = 1435,
            EsconderLinksImpressaoParaVendedores = 1436,
            EsconderPecasParaVendedores = 1437,
            ExibirNumeroEtiquetaNoInicioDaTabela = 1438,
            TelaVaziaPorPadrao = 1439,
            BuscarNomeFantasiaConsultaProducao = 1440,
            OrdenarPeloNumSeqSetor = 1441,
            ExibirImpressaoPedidoTelaConsulta = 1443,
            ExibirAnexosPedidosMaoDeObraAoConsultarPeca = 1444,
            SempreExibirAnexosPedidosAoConsultarPeca = 1445,
            ImpedirLeituraSetorEntregueTodasPecasPedido = 1446,
            ExibirPainelSetores = 1448,
            CancelarLeiturasSeUmaFalhar = 1449,
            ExibirObsProjeto = 1450,
            UsarRelatorioProdutosDiferente = 1452,
            BuscarApenasProdutosAtivosConsultaProd = 1454,
            AlterarSubgruposSelecionados = 1455,
            SubgruposPadraoFiltro = 1456,
            UsarRelatorioPrecoTabelClienteRetrato = 1457,
            ExibirColunaCustoEmPrecoBeneficiamento = 1458,
            UsarFiltroDataPedidoComoPadrao = 1459,
            InverterExibicaoPlanoConta = 1461,
            BuscarPedCliAoInserirProjeto = 1462,
            ManterImagensEditadasAoConfirmarProjeto = 1463,
            AlturaPadraoProjetoBox6mm = 1464,
            SelecionarEspessuraAoCalcularProjeto = 1465,
            PermitirAlterarMedidasPecasProjetoCalculoVaoPCP = 1466,
            FMLBasicoSalvarMaiorMedidaNoCampoAltura = 1467,
            PercentualTamanhoImagemRelatorio = 1468,
            CorObsNoRelatorio = 1469,
            AlterarTextoLabelAmbiente = 1470,
            ExibirBotaoImprimirProjeto = 1471,
            AumentarNumeroMateriaisListagem = 1472,
            AreaTotalItemProjetoAreaPecas = 1473,
            EsconderCamposTipoEntrega = 1474,
            BloquearTipoEntregaClientesRota = 1475,
            BloquearTipoEntregaEntrega = 1476,
            EsconderCamposAlteraCorItemProjeto = 1478,
            ExibirCorAluminioFerragemWebGlassParceiros = 1479,
            IdLojaPorTipoPedidoSemVidro = 1480,
            ConfirmarPedidoGerarPCPFinalizarPCPAoGerarPedido = 1481,
            UsarParcelasPedido = 1482,
            UsarDiasCorridosCalculoRota = 1483,
            DebitarIcmsDoIcmsSt = 1484,
            DebitarIcmsDoIcmsStSeCliente = 1485,
            AlturaPadraoProjetoBoxAcima6mm = 1486,
            IdLojaPorTipoPedidoComVidro = 1487,
            PodeEditarPedidoGeradoParceiro = 1488,
            PodeReabrirPedidoGeradoParceiro = 1489,
            UtilizarConstanteNossoNumeroBoleto = 1490,
            ExibirValorPedidoMaoDeObraViaAlmoxarife = 1491,
            AlterarLojaOrcamento = 1492,
            BloquearGerarCarregamentoAcimaCapacidadeVeiculo = 1494,
            NaoExibirViaClienteSeTodosPedidosForemTipoEntrega = 1496,
            NaoEnviarEmailPedidoProntoPedidoImportado = 1497,
            ExibirRazaoSocialGraficoVendasCurvaABC = 1499,
            ExibirTotalM2RetalhoCorEspessura = 1500,
            UsarNovoControleExpBalcaoSemCarregamento = 1501,
            EnviarEmailEmitirBoleto = 1502,
            PermitirGerarConferenciaPedidoRevenda = 1503,
            UtilizarControleContaReceberJuridico = 1504,
            BloquearCadastroMedicaoSemEmailCliente = 1505,
            MostrarIconeEnvioEmailListagemOrcamento = 1506,
            ExibirSaldoDevedorRelsRecebimento = 1507,
            ParceiroPodeReabrirPedidoConferido = 1508,
            PermitirInserirVidroComumComComposicao = 1509,
            ExibirValorFretePedido = 1510,
            ConcatenarDescontoPorQuantidadeNaDescricaoDoProduto = 1511,
            ExibirCheckGerarProdutoConjunto = 1512,
            CheckBenefOpcionalDesmascadoPadrao = 1513,
            PermitirGerarArqOtimizacaoSemSag = 1514,
            NaoExibirObsPecaAoImprimirEtiqueta = 1515,
            VendedorPodeAlterarOrcamentoQualquerLoja = 1516,
            RatearDescontoProdutosNotaFiscal = 1517,
            SubtrairImpostosValorUnMovFiscal = 1518,
            ParceiroPodeEditarPedido = 1519,
            NomeArquivoMesaComHifenEOCraseado = 1520,
            NaoRecalcularValorProdutoComposicaoAoAlterarAlturaLargura = 1521,
            UsarImpressaoLiberacaoPorTipoEntrega = 1522,
            VersaoArquivoOptyway = 1523,
            UsarTabelaDescontoAcrescimoPedidoAVista = 1524,
            GerarPecasComMedidasIncoerentesDaImagemEditada = 1525,
            PermirtirSalvarOrcamentoSemProcAplic = 1526,
            ExibirFolgaProjetoEcommerce = 1527,
            UsarCreditoMarcadoTelaPagamentoAntecipadoPedido = 1528,
            NomeArquivoDxfComAspasECedilha = 1529,
            PermitirImpressaoDePedidosImportadosApenasConferidos = 1530,
            NomeArquivoMesaBarraPorCeCedilha = 1531

            #endregion
        }

        /// <summary>
        /// Enumeração com o tipo do item no banco de dados.
        /// </summary>
        public enum TipoConfigEnum
        {
            Inteiro             = 1,
            Decimal,
            Logico,
            Texto,
            TextoCurto,         //5
            Enum,
            ListaMetodo,
            GrupoEnumMetodo,
            Data
        }

        public enum ModuloEmpresa
        {
            Conferencia = 1,
            Instalacao,
            RastrearEquipeInst,
            PCP,
            NFe,
            CaixaDiario,
            Medicao
        }

        #endregion

        #region Funções menu

        /// <summary>
        /// Módulos do sistema
        /// </summary>
        public enum Modulo
        { 
            Cadastros = 1,
            Medicao,
            Orcamento,
            Conferencia,
            Projeto,
            Pedido,
            PCP,
            Estoque,
            OrdemDeCarga,
            Instalacao,
            CaixaDiario,
            Financeiro,
            FinanceiroPagto,
            Fiscal,
            Relatorios
        }

        /// <summary>
        /// Atributo usado para mapear os enumeradores das funções 
        /// </summary>
        private class ModuloAttr : Attribute
        {
            public ModuloAttr(Modulo modulo)
            {
                Modulo = modulo;
            }

            public Modulo Modulo { get; set; }
        }

        [ModuloAttr(Modulo.Cadastros)]
        public enum FuncaoMenuCadastro
        {
            AlterarTabelaDescontoAcrescimoCliente = 1,
            AlterarLimiteCliente,
            AlterarPercRedNfe,
            AlterarSinalMinimoCliente,
            AnexarArquivosCliente,                      // 5
            AtivarInativarCliente,
            CadastrarCliente,
            ExportarImprimirDadosClientes=9,
            ExibirTotalCompradoCliente,                 // 10
            MarcarClienteRevenda,
            DescontoAcrescimoProdutoCliente,
            AlterarBloqueioPedidoCadCliente,
            AlterarVendedorCadCliente,
            CadastrarFornecedor,                        // 15
            AtivarInativarFornecedor,
            CadastrarFuncionario,
            AtrasarDataPedidoFunc,
            EfetuarLoginQualquerMomento,
            IgnorarFuncVisualizarDadosApenasSuaLoja,    // 20
            CadastrarComissionado,
            CadastrarRota,
            CadastrarTransportadora,
            CadastrarSugestoesClientes = 25,            // 25
            CadastrarVeiculo,
            CadastrarProduto,
            PermitirEnvioMensagemInterna,
            PermitirCancelarCni,
            AnexarArquivosFornecedor                    //30
        }

        [ModuloAttr(Modulo.Medicao)]
        public enum FuncaoMenuMedicao
        {
            EfetuarMedicao = 1,
            Medidor
        }

        [ModuloAttr(Modulo.Orcamento)]
        public enum FuncaoMenuOrcamento
        {
            EmitirOrcamento = 1
        }

        [ModuloAttr(Modulo.Conferencia)]
        public enum FuncaoMenuConferencia
        {
            ControleConferenciaMedicao = 1,
            Conferente
        }

        [ModuloAttr(Modulo.Projeto)]
        public enum FuncaoMenuProjeto
        {

        }

        [ModuloAttr(Modulo.Pedido)]
        public enum FuncaoMenuPedido
        {
            CancelarPedido = 1,
            AlterarPedidoCofirmadoListaPedidosVendedor,
            AlterarDataEntregaPedidoListaPedidos,
            AlterarPedidoConfirmadoListaPedidos,
            AlterarVendedorPedido,                          // 5
            AnexarArquivoLiberacaoListaPedido,
            AnexarArquivoPedido,
            EmitirPedido,
            EmitirPedidoGarantiaReposicao,
            EmitirPedidoFuncionario,                        // 10
            IgnorarBloqueioDataEntrega,
            IgnorarBloqueioDescontoOrcamentoPedido,
            PermitirExcluirPecaPedidoConfirmado,
            PermitirMarcarFastDelivery,
            VisualizarMemoriaCalculo,                       // 15
            ExportarImportarPedido,
            ConfirmarPedidoLiberacao,
            GerarReposicao,                                 
            ReposicaoDePeca
        }

        [ModuloAttr(Modulo.PCP)]
        public enum FuncaoMenuPCP
        {
            EditarCancelarConferencia = 1,
            AlterarImagemPecaAposImpressao,
            VisualizarPedidosEmConferencia,
            ImprimirEtiquetas,
            ImprimirEtiquetasNFe,                   // 5
            ImprimirEtiquetasMaoDeObra,
            ReimprimirEtiquetas,
            CancelarImpressaoEtiqueta,
            GerarConferenciaPedido,
            ReimprimirEtiquetaBox,                  // 10
            VoltarSetorPecaProducao,
            PararRetornarPecaProducao
        }

        [ModuloAttr(Modulo.Estoque)]
        public enum FuncaoMenuEstoque
        {
            ControleEstoque = 1,
            AlterarEstoqueManualmente,
            EfetuarTrocaDevolucao,
            AutorizarPedidoInterno,
            FinalizarTrocaDevolucao,
            AnexarArquivoPedidoInterno
        }

        [ModuloAttr(Modulo.OrdemDeCarga)]
        public enum FuncaoMenuOrdemDeCarga
        {

        }

        [ModuloAttr(Modulo.Instalacao)]
        public enum FuncaoMenuInstalacao
        {
            ControleInstalacaoComum = 1,
            ControleInstalacaoTemperado
        }

        [ModuloAttr(Modulo.CaixaDiario)]
        public enum FuncaoMenuCaixaDiario
        {
            ControleCaixaDiario = 1,
            GerarCreditoAvulsoCliente,
        }

        [ModuloAttr(Modulo.Financeiro)]
        public enum FuncaoMenuFinanceiro
        {
            ControleFinanceiroRecebimento = 1,
            GerarCreditoAvulsoCliente,
            FinalizarConfirmarPedidoPeloFinanceiro = 4,
            MarcarContaJuridicoCartorio,
            CancelarRecebimentos
        }

        [ModuloAttr(Modulo.FinanceiroPagto)]
        public enum FuncaoMenuFinanceiroPagto
        {
            ControleFinanceiroPagamento = 1,
            EditarQualquerCompra,
            RealizarConciliacaoBancaria,
            EditarDataVencimentoContaPagar,
            InserirImpostoServicoAvulsoParaQualquerLoja
        }

        [ModuloAttr(Modulo.Fiscal)]
        public enum FuncaoMenuFiscal
        {
            AlteracaoManualNFe = 1,
            AtivarContingenciaNFe,
            SepararValoresFiscaisReais,
            InserirNotaFiscalParaQualquerLoja
        }


        [ModuloAttr(Modulo.Relatorios)]
        public enum FuncaoMenuRelatorios
        {
            ImprimirRelatorioPedidos = 1
        }

        /// <summary>
        /// Obtém o id da função com base no IdFuncao e no IdFuncao
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idFunc"></param>
        /// <param name="funcao"></param>
        /// <returns></returns>
        public static int ObterIdFuncaoMenu<T>(T funcao) where T : struct, IConvertible
        {
            // Recupera o atributo (módulo) da função passada
            var atributo = funcao.GetType().GetCustomAttributes(false).Where(f => f is ModuloAttr).FirstOrDefault();

            if (atributo == null)
                return 0;

            // Recupera o IdModulo e o IdFuncao
            var idModulo = (int)((ModuloAttr)atributo).Modulo;
            var idFuncao = funcao.ToInt32(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            
            return FuncaoMenuDAO.Instance.ObterIdFuncaoMenu(idFuncao, idModulo);
        }

        private static Dictionary<int, List<Tuple<int, int>>> _funcoesUsuario = new Dictionary<int, List<Tuple<int, int>>>();

        /// <summary>
        /// Verifica se o usuário tem permissão de uso da função passada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool PossuiPermissao<T>(T funcao) 
        {
            if (UserInfo.GetUserInfo == null)
                return false;

            return PossuiPermissao((int)UserInfo.GetUserInfo.CodUser, funcao);
        }

        /// <summary>
        /// Verifica se o usuário tem permissão de uso da função passada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idFunc"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool PossuiPermissao<T>(int idFunc, T funcao) 
        {
            // Recupera o atributo (módulo) da função passada
            var atributo = funcao.GetType().GetCustomAttributes(false).Where(f => f is ModuloAttr).FirstOrDefault();
            
            if (atributo == null)
                return false;

            // Recupera o IdModulo e o IdFuncao
            var idModulo = (int)((ModuloAttr)atributo).Modulo;
            var idFuncao = Convert.ToInt32(funcao);
            
            // Verifica se as funções do usuário já foram carregadas em memória e carrega caso não tenha sido
            if (!_funcoesUsuario.ContainsKey(idFunc))
            {
                // Carrega as funções do funcionário
                var funcoes = FuncaoMenuDAO.Instance.ObterFuncoesFuncioario(idFunc);

                // Salva em um dicionário as permissões do usuário por módulo
                var funcaoModulo = new List<Tuple<int, int>>();
                foreach (var funcaoFunc in funcoes)
                    funcaoModulo.Add(new Tuple<int, int>((int)funcaoFunc.IdModulo, funcaoFunc.IdFuncao));

                /* Chamado 48832. */
                if (!_funcoesUsuario.ContainsKey(idFunc))
                {
                    // Remover o try catch caso o problema do chamado 48832 seja resolvido definitivamente.
                    try
                    {
                        _funcoesUsuario.Add(idFunc, funcaoModulo);
                    }
                    catch (Exception ex)
                    {
                        ErroDAO.Instance.InserirFromException(string.Format("PossuiPermissao - IdFunc: {0}", idFunc), ex);
                    }
                }
            }

            // Verifica se o usuário possui a permissão passada
            return _funcoesUsuario[idFunc].Any(f => f.Item1 == idModulo && f.Item2 == idFuncao);
        }

        /// <summary>
        /// Remove o usuário da lista de permissões do sistema
        /// </summary>
        /// <param name="idFunc"></param>
        public static void ResetModulosUsuario(int idFunc)
        {
            if (_funcoesUsuario.ContainsKey(idFunc))
                _funcoesUsuario.Remove(idFunc);
        }

        #endregion

        #region Carrega menus

        private static Dictionary<int, List<System.Web.UI.WebControls.MenuItem>> _menusUsuario = new Dictionary<int, List<System.Web.UI.WebControls.MenuItem>>();

        /// <summary>
        /// Carrega os menus do usuários salvos em memória
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public static List<System.Web.UI.WebControls.MenuItem> CarregaMenusUsuario(int idFunc)
        {
            if (_menusUsuario.ContainsKey(idFunc))
                return _menusUsuario[idFunc];

            return new List<System.Web.UI.WebControls.MenuItem>();
        }

        /// <summary>
        /// Salva os menus do usuário em memória para rápido acesso
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="lstMenu"></param>
        public static void SalvaMenuUsuario(int idFunc, List<System.Web.UI.WebControls.MenuItem> lstMenu)
        {
            if (!_menusUsuario.ContainsKey(idFunc))
                _menusUsuario.Add(idFunc, lstMenu);
        }

        /// <summary>
        /// Remove da memória o menu do usuário
        /// </summary>
        /// <param name="idFunc"></param>
        public static void RemoveMenuUsuario(int idFunc)
        {
            if (_menusUsuario.ContainsKey(idFunc))
                _menusUsuario.Remove(idFunc);
        }

        /// <summary>
        /// Remove tudo da memória
        /// </summary>
        public static void LimpaMenuUsuario()
        {
            _menusUsuario.Clear();
        }

        #endregion

        #region Configuráveis pelo Config.aspx

        private static Dictionary<KeyValuePair<ConfigEnum, uint>, object> _config = new Dictionary<KeyValuePair<ConfigEnum, uint>, object>();

        public static T GetConfigItem<T>(ConfigEnum item)
        {
            object valor;

            KeyValuePair<ConfigEnum, uint> chave = new KeyValuePair<ConfigEnum, uint>(item, UserInfo.GetUserInfo != null ? UserInfo.GetUserInfo.IdLoja : 0);
            if (!_config.ContainsKey(chave))
                lock (_config)
                {
                    valor = ConfigDAO.Instance.GetValue(chave.Key, chave.Value);

                    // Necessário verificar novamente, neste ponto dava o erro "Já foi adicionado um item com a mesma chave"
                    if (!_config.ContainsKey(chave))
                        _config.Add(chave, valor);
                }
            else
                valor = _config[chave];

            return Conversoes.ConverteValor<T>(valor);
        }

        internal static void RemoveConfigItem(ConfigEnum item)
        {
            KeyValuePair<ConfigEnum, uint>[] chaves = new KeyValuePair<ConfigEnum, uint>[_config.Count];
            _config.Keys.CopyTo(chaves, 0);

            foreach (KeyValuePair<ConfigEnum, uint> chave in chaves)
                if (chave.Key == item)
                {
                    lock (_config)
                    {
                        _config.Remove(chave);
                    }
                }
        }                                                                                    
      
        #endregion                          
    }
}