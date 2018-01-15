using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.NFeUtils;
using Glass.Data.EFD;

namespace Glass.Configuracoes
{
    partial class FiscalConfig
    {
        public class NotaFiscalConfig
        {
            /// <summary>
            /// Define o percentual que será gerado de sobra nos produtos de retalho automaticamente ao emitir NF-e, 0 (zero) define que este controle não será usado
            /// </summary>
            public static decimal PercentualAGerarDeSobraDeProducao
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.PercentualAGerarDeSobraDeProducao); }
            }

            /// <summary>
            /// Verifica se ao gerar nota fiscal a partir de pedido/liberação, será considerado o TotM2Calc
            /// </summary>
            public static bool ConsiderarM2CalcNotaFiscal
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ConsiderarM2CalcNotaFiscal); }
            }

            /// <summary>
            /// Define qual CST de origem deve ser buscado por padrão ao emitir NF de saída
            /// </summary>
            public static int CstOrigPadraoNotaFiscalSaida
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.CstOrigPadraoNotaFiscalSaida); }
            }

            /// <summary>
            /// Período de apuração padrão do IPI. Valor pode ser alterado para cada nota fiscal.
            /// </summary>
            public static NotaFiscal.PeriodoIpiEnum PeriodoApuracaoIpi
            {
                get { return Config.GetConfigItem<NotaFiscal.PeriodoIpiEnum>(Config.ConfigEnum.PeriodoApuracaoIpi); }
            }

            /// <summary>
            /// Indica se a empresa gera o arquivo do EFD.
            /// </summary>
            public static bool GerarEFD
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarEFD); }
            }

            /// <summary>
            /// Verifica se a empresa rateia o IPI ao gerar NF de pedido.
            /// </summary>
            public static bool RatearIpiNfPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.RatearIpiNfPedido); }
            }

            /// <summary>
            /// Verifica se a empresa rateia o ICMS ST ao gerar NF de pedido.
            /// </summary>
            public static bool RatearIcmsStNfPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.RatearIcmsStNfPedido); }
            }

            /// <summary>
            /// Define o tipo de alíquota de ICMS ST ao ratear IPI na NF de pedido.
            /// </summary>
            public static ConfigNFe.TipoCalculoIcmsStNf AliquotaIcmsStRatearIpiNfPedido
            {
                get
                {
                    return
                        Config.GetConfigItem<ConfigNFe.TipoCalculoIcmsStNf>(
                            Config.ConfigEnum.AliquotaIcmsStRatearIpiNfPedido);
                }
            }

            /// <summary>
            /// Tipo de contribuição social padrão para notas fiscais.
            /// EFD PIS/Cofins.
            /// </summary>
            public static DataSourcesEFD.CodContEnum? TipoContribuicaoSocialPadrao
            {
                get
                {
                    return
                        Config.GetConfigItem<DataSourcesEFD.CodContEnum?>(Config.ConfigEnum.TipoContribuicaoSocialPadrao);
                }
            }

            /// <summary>
            /// Tipo de nota fiscal onde buscar contribuição social padrão.
            /// </summary>
            public static DataSourcesEFD.TipoUsoCredCont? TipoNotaBuscarContribuicaoSocialPadrao
            {
                get
                {
                    return
                        Config.GetConfigItem<DataSourcesEFD.TipoUsoCredCont?>(
                            Config.ConfigEnum.TipoNotaBuscarContribuicaoSocialPadrao);
                }
            }

            /// <summary>
            /// Tipo de crédito padrão para notas fiscais.
            /// EFD PIS/Cofins.
            /// </summary>
            public static DataSourcesEFD.CodCredEnum? TipoCreditoPadrao
            {
                get { return Config.GetConfigItem<DataSourcesEFD.CodCredEnum?>(Config.ConfigEnum.TipoCreditoPadrao); }
            }

            /// <summary>
            /// Tipo de nota fiscal onde buscar crédito padrão.
            /// </summary>
            public static DataSourcesEFD.TipoUsoCredCont? TipoNotaBuscarCreditoPadrao
            {
                get
                {
                    return
                        Config.GetConfigItem<DataSourcesEFD.TipoUsoCredCont?>(
                            Config.ConfigEnum.TipoNotaBuscarCreditoPadrao);
                }
            }

            /// <summary>
            /// Retorna o tipo de cálculo da alíquota de ICMS ST.
            /// </summary>
            public static ConfigNFe.TipoCalculoIcmsSt CalculoAliquotaIcmsSt
            {
                get
                {
                    return Config.GetConfigItem<ConfigNFe.TipoCalculoIcmsSt>(Config.ConfigEnum.CalculoAliquotaIcmsSt);
                }
            }

            /// <summary>
            /// Cor usada para os alumínios dos projetos "Apenas vidros" na NF-e.
            /// </summary>
            public static uint? CorAluminiosProjetosApenasVidrosNFe
            {
                get { return Config.GetConfigItem<uint?>(Config.ConfigEnum.CorAluminiosProjetosApenasVidrosNFe); }
            }

            /// <summary>
            /// Cor usada para as ferragens dos projetos "Apenas vidros" na NF-e.
            /// </summary>
            public static uint? CorFerragensProjetosApenasVidrosNFe
            {
                get { return Config.GetConfigItem<uint?>(Config.ConfigEnum.CorFerragensProjetosApenasVidrosNFe); }
            }

            /// <summary>
            /// Não permite emitir mais de uma nota para um mesmo pedido
            /// </summary>
            public static bool NaoPermitirMaisDeUmaNfeParaUmPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearMaisDeUmaNfeParaUmPedido); }
            }

            #region NFe

            /// <summary>
            /// Retorna a alíquota de ICMS para aproveitamento de crédito usadas em empresas do Simples Nacional
            /// </summary>
            public static float AliqICMSSN
            {
                get { return Config.GetConfigItem<float>(Config.ConfigEnum.AliquotaICMSSimplesNacional); }
            }

            /// <summary>
            /// Indica se os produtos do pedido serão agrupados ao gerar a NFe.
            /// </summary>
            public static bool AgruparProdutosGerarNFe
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AgruparProdutosGerarNFe); }
            }

            /// <summary>
            /// Indica se os produtos de chapa de vidro importada serão destacados na NF-e.
            /// </summary>
            public static bool DestacarProdutoChapaImportada
            {
                get
                {
                    return !FiscalConfig.NotaFiscalConfig.AgruparProdutosGerarNFe &&
                           Config.GetConfigItem<bool>(Config.ConfigEnum.DestacarProdutoChapaImportada);
                }
            }

            /// <summary>
            /// Indica se os pedidos utilizados para gerar a NFe serão informados na inf. compl.
            /// </summary>
            public static bool InformarPedidoNFe
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.InformarPedidoNaNfe); }
            }

            /// <summary>
            /// Indica se o numero do pedido do cliente serão informados na inf. compl. da NF-e
            /// </summary>
            public static bool InformarPedidoClienteNFe
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.InformarPedidoClienteNaNFe); }
            }

            /// <summary>
            /// Indica se o numero do pedido do cliente sera informado na inf. compl. do produto da NF-e
            /// </summary>
            public static bool InformarPedidoClienteProdutoNFe
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.InformarPedCliNoProdutoNFe); }
            }

            /// <summary>
            /// Indica se os orçamentos utilizados para gerar a NFe serão informados na inf. compl.
            /// </summary>
            public static bool InformarOrcamentoNFe
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.InformarOrcamentoNaNFe); }
            }

            /// <summary>
            /// Indica se a forma de pagto. sera informada na inf. compl.
            /// </summary>
            public static bool InformarFormaPagtoNFe
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.InformarFormaPagtoNaNFe); }
            }

            /// <summary>
            /// Número de parcelas para a nota fiscal.
            /// </summary>
            public static int NumeroParcelasNFe
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroParcelasNFe); }
            }

            /// <summary>
            /// Indica se a empresa usará a contingência de NF-e.
            /// </summary>
            public static DataSources.TipoContingenciaNFe ContingenciaNFe
            {
                get { return Config.GetConfigItem<DataSources.TipoContingenciaNFe>(Config.ConfigEnum.ContingenciaNFe); }
            }

            /// <summary>
            /// Retorna a justificativa da contingência da NF-e.
            /// </summary>
            public static string JustificativaContingenciaNFE
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.JustificativaContingenciaNFe); }
            }

            /// <summary>
            /// Retorna se a NF-e de entrada de terceiros deve ser gerada com a opção "Gerar etiqueta de nota fiscal?" marcada ou não.
            /// </summary>
            public static bool GerarNotaFiscalCompraGerarEtiqueta
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.GerarNotaFiscalCompraGerarEtiqueta); }
            }

            #endregion

            public static bool PermitirGerarNFPedMaoDeObra
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermitirGerarNFPedMaoDeObra); }
            }

            /// <summary>
            /// Indica se o cálculo de ICMS ST será feito utilizando o WebService (Estimativa Simplificada).
            /// </summary>
            public static bool CalcularIcmsStUtilizandoWebServiceMT
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CalcularIcmsStUtilizandoWebServiceMT); }
            }

            /// <summary>
            /// A empresa bloqueia a emissão de notas fiscais se o valor da nota fiscal for maior que o total dos pedidos/liberações que a geraram?
            /// </summary>
            public static bool BloquearEmissaoNotaFiscalDePedidoMaiorQueOsPedidos
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearEmissaoNotaFiscalDePedidoMaiorQueOsPedidos); }
            }

            /// <summary>
            /// Agrupa os produtos da compra ao gerar a nota fiscal de entrada de terceiros.
            /// </summary>
            public static bool AgruparProdutosGerarNFeTerceiros
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AgruparProdutosGerarNFeTerceiros) && AgruparProdutosGerarNFe; }
            }

            /// <summary>
            /// Exibir o endereço de entrega do cliente nas informações complementares da nota fiscal, caso não esteja preenchido buscar o endereço padrão.
            /// </summary>
            public static bool ExibirEnderecoEntregaInfCompl
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirEnderecoEntregaInfCompl); }
            }

            /// <summary>
            /// Define se a emissão da nota fiscal deve ser bloqueada caso a forma de pagamento seja à vista e a liberação tenha contas a receber.
            /// </summary>
            public static bool BloquearEmissaoAVistaComContaAReceberDeLiberacao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearEmissaoAVistaComContaAReceberDeLiberacao); }
            }

            /// <summary>
            /// Define que o logo do DANFE com logo não será exibido
            /// </summary>
            public static bool EsconderLogotipoDANFEComLogo
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EsconderLogotipoDANFEComLogo); }
            }

            /// <summary>
            /// Define que o email fiscal da loja será exibido no DANFE
            /// </summary>
            public static bool ExibirEmailFiscalDANFE
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirEmailFiscalDANFE); }
            }

            /// <summary>
            /// Informa se o DANFE deverá ser destacado, é exibido um círculo preto no canto superior direito do documento, em caso de destaque.
            /// </summary>
            public static bool DestacarNFe
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.DestacarNFe); }
            }

            /// <summary>
            /// Define se a forma de pagamento da liberação será exibida no campo Informações Complementares da nota fiscal.
            /// </summary>
            public static bool ExibirFormaPagamentoLiberacaoInfCompl
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirFormaPagamentoLiberacaoInfCompl); }
            }

            /// <summary>
            /// Define se o caractere R será concatenado ao código do produto caso o cliente possua redução na NFe.
            /// </summary>
            public static string CaractereConcatenarCodigoProdutoClienteComReducao
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.CaractereConcatenarCodigoProdutoClienteComReducao); }
            }

            /// <summary>
            /// 
            /// </summary>
            public static bool ConcatenarPrimeiraLetraBeneficiamentoCodigoProduto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ConcatenarPrimeiraLetraBeneficiamentoCodigoProduto); }
            }

            /// <summary>
            /// Define que serão exibidas nas informações adicionais dos produtos da nota a largura e a altura dos produtos
            /// </summary>
            public static bool ExibirLarguraEAlturaInfAdicProduto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirLarguraEAlturaInfAdicProduto); }
            }

            /// <summary>
            /// Define que serão exibidas nas informações adicionais dos produtos da nota a quantidade, a largura e a altura dos produtos, desde que a opção de agrupar produtos não esteja marcada
            /// </summary>
            public static bool ExibirQtdLarguraEAlturaInfAdicProduto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirQtdLarguraEAlturaInfAdicProduto); }
            }

            /// <summary>
            /// Define a série que será inserida na nota caso o usuário não tenha preenchido este campo com 0, 1 e vazio ao inserir a nota fiscal
            /// </summary>
            public static int SeriePadraoNFe()
            {
                return SeriePadraoNFe(null, null, null);
            }

            /// <summary>
            /// Define a série que será inserida na nota caso o usuário não tenha preenchido este campo com 0, 1 e vazio ao inserir a nota fiscal
            /// </summary>
            public static int SeriePadraoNFe(string codCfop, string inscEstLoja, bool? notaDeAjuste)
            {
                var config = Config.GetConfigItem<int>(Config.ConfigEnum.SeriePadraoNFe);

                if (config != 1)
                    return config;

                if (ControleSistema.GetSite() == ControleSistema.ClienteSistema.LojaDosEspelhos &&
                    (inscEstLoja == "06.200.647-9" || inscEstLoja == "062006479"))
                    return 2;
 
                if (ControleSistema.GetSite() == ControleSistema.ClienteSistema.VitralManaus &&
                    (inscEstLoja == "06.200.648-7" || inscEstLoja == "062006487"))
                    return 6;

                if ((ControleSistema.GetSite() == ControleSistema.ClienteSistema.Divine ||
                    ControleSistema.GetSite() == ControleSistema.ClienteSistema.Dividros ||
                    ControleSistema.GetSite() == ControleSistema.ClienteSistema.Invitra) &&
                    (codCfop == "5103" || codCfop == "5104"))
                    return 3;

                if (ControleSistema.GetSite() == ControleSistema.ClienteSistema.ModeloVidros && notaDeAjuste.GetValueOrDefault())
                    return 3;

                if (ControleSistema.GetSite() == ControleSistema.ClienteSistema.TemperadosEstrela && notaDeAjuste.GetValueOrDefault())
                    return 2;

                return 1;
            }

            /// <summary>
            /// CST do IPI padrão a ser usado ao inserir notas de entrada
            /// </summary>
            public static int CstIpiPadraoNotaEntrada
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.CstIpiPadraoNotaEntrada); }
            }

            /// <summary>
            /// Define se o IPI integra BC do Pis se o CST for diferente de 0 e nota de entrada (segundo Julielberty), porém segundo o Higor,
            /// deve integrar somente se o ipi não gerar crédito e a empresa destinatária ser do lucro presumido e gerar crédito PIS/COFINS,
            /// portanto, a opção foi alterada para ficar assim somente para a Vipal
            /// </summary>
            public static bool IpiIntegraBcPISCOFINS
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.IpiIntegraBcPISCOFINS); }
            }

            /// <summary>
            /// Define se o icms deve ser debitado do icm st ao calcular este
            /// </summary>
            public static bool DebitarIcmsDoIcmsSt
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.DebitarIcmsDoIcmsSt); }
            }

            /// <summary>
            /// Define se o icms deve ser debitado do icms st ao calcular este e se tiver cliente na nota
            /// </summary>
            public static bool DebitarIcmsDoIcmsStSeCliente
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.DebitarIcmsDoIcmsStSeCliente); }
            }

            /// <summary>
            /// Define que a nota fiscal será exportada para outro banco de dados
            /// </summary>
            public static bool ExportarNotaFiscalOutroBD
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExportarNotaFiscalOutroBD); }
            }

            /// <summary>
            /// Define que será usado o nome fantasia na nota fiscal
            /// </summary>
            public static bool UsarNomeFantasiaNotaFiscal
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarNomeFantasiaNotaFiscal); }
            }

            /// <summary>
            /// Se habilitada, preenche o campo Especie com "VOLUMES", a modalidade do frete com o tipo "2" 
            /// e a quantidade de volumes com a quantidade de peças da OC se todos os pedidos que geraram a nota for Balcão
            /// </summary>
            public static bool PreencheTransporteSeBalcao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PreencheTransporteSeBalcao); }
            }

            /// <summary>
            /// Se habilitada, preenche o campo Especie com "VOLUMES", a modalidade do frete com o tipo "1", 
            /// a quantidade de volumes com a quantidade de peças da OC e a placa do veículo com a placa que estiver no carregamento (se for transferência) 
            /// se todos os pedidos que geraram a nota for diferente de balcão
            /// </summary>
            public static bool PreencheTransporteSeNaoForBalcao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PreencheTransporteSeNaoForBalcao); }
            }

            /// <summary>
            /// Preenche a modalidade do frete com "1", q QtdVol com "1" e a placa e a UF do veículo com o veículo associado à OC do pedido da nota,
            /// desde que todas as OCs tenham o mesmo veículo
            /// </summary>
            public static bool PreencheTransporteComVeiculoOC
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PreencheTransporteComVeiculoOC); }
            }

            /// <summary>
            /// Especie padrão na nota, caso PreencheTransporteComVeiculoOC = true
            /// </summary>
            public static string EspeciePadraoSeMesmoVeiculoOC
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.EspeciePadraoSeMesmoVeiculoOC); }
            }

            /// <summary>
            /// Não permite que a nota seja emitida se o pedido (ou liberação) que a gerou possuir contas já recebidas
            /// </summary>
            public static bool BloquearEmissaoNotaSePedidoPossuirContaRecebida
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.BloquearEmissaoNotaSePedidoPossuirContaRecebida); }
            }

            /// <summary>
            /// Texto padrão que deverá ser adicionado nas informações complementares das notas de saída
            /// </summary>
            public static string TextoPadraoInfComplNota
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.TextoPadraoInfComplNota); }
            }

            /// <summary>
            /// Define que os campos de ICMS, ICMS ST e IPI serão escondidos na nota de saída
            /// </summary>
            public static bool EsconderIcmsIcmsStIpiNotaSaida
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.EsconderIcmsIcmsStIpiNotaSaida); }
            }

            /// <summary>
            /// Define se será debitado da quantidade de produtos da nota produtos que tenham sido trocados
            /// </summary>
            public static bool DeduzirQtdTrocaProdutoNF
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.DeduzirQtdTrocaProdutoNF); }
            }

            /// <summary>
            /// Define se será exibido a descrição "Pedido(s):" ao informar os pedidos que compõem a nota nas informações complementares
            /// </summary>
            public static bool ExibirDescricaoPedidoInfCompl
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirDescricaoPedidoInfCompl); }
            }

            /// <summary>
            /// Define se será exibido o nome da transportadora no campo destinatário na nota 
            /// </summary>
            public static bool ExibirTransportadorCampoDestinatario
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirTransportadorCampoDestinatario); }
            }

            /// <summary>
            /// Define se será exibido o nome da transportadora no campo destinatário na nota 
            /// </summary>
            public static bool AcrescentarLapBisProdutoNota
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AcrescentarLapBisProdutoNota); }
            }

            /// <summary>
            /// Define se será gerado produto de cesta se o projeto for apenas vidros
            /// </summary>
            public static bool UsarProdutoCestaSeApenasVidros
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarProdutoCestaSeApenasVidros); }
            }

            /// <summary>
            /// Define se o checkbox que define se a produção será contabilizada no EFD poderá ser exibido.
            /// </summary>
            public static float? PercentualCargaTributariaParaClienteSimplesNacionalCfop5401Cst10
            {
                get
                {
                    var config = Config.GetConfigItem<float>(Config.ConfigEnum.PercentualCargaTributariaParaClienteSimplesNacionalCfop5401Cst10);

                    return config == 0 ? null : (float?)config;
                }
            }

            /// <summary>
            /// Define se o desconto será rateado no total dos produtos da nota fiscal,
            /// ou se o desconto será destacado no campo desconto da mesma.
            /// </summary>
            public static bool RatearDescontoProdutosNotaFiscal
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.RatearDescontoProdutosNotaFiscal); }
            }
        }
    }
}