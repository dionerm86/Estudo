using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.Configuracoes
{
    public partial class Liberacao
    {
        public static class RelatorioLiberacaoPedido
        {         
            /// <summary>
            /// Define o número de vias de almoxarife da liberação que aparecem.
            /// </summary>
            public static int NumeroViasAlmoxarifeLiberacao
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroViasAlmoxarifeLiberacao); }
            }

            /// <summary>
            /// Define o número de vias de expedição da liberação que aparecem.
            /// </summary>
            public static int NumeroViasExpedicaoLiberacao
            {
                get { return Config.GetConfigItem<int>(Config.ConfigEnum.NumeroViasExpedicaoLiberacao); }
            }

            /// <summary>
            /// Define se será montada uma informação adicional a ser concatenada com o nome do cliente na impressão da liberação
            /// </summary>
            public static string InformacaoAdicionalLiberacao
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.InformacaoAdicionalLiberacao); }
            }

            /// <summary>
            /// Exibir a via da empresa no relatório de 4 vias?
            /// </summary>
            public static bool ExibirViaEmpresaRelatorio4Vias
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirViaEmpresaRelatorio4Vias); }
            }


            /// <summary>
            /// Texto a ser exibido nos resumos de corte (relatório 4 vias).
            /// </summary>
            public static string TextoResumosCorteRelatorio4Vias(uint idLiberarPedido)
            {
                return Config.GetConfigItem<string>(Config.ConfigEnum.TextoResumosCorteRelatorio4Vias);
            }

            /// <summary>
            /// Define se a observação de liberação do cliente será exibida na impressão da liberação, na via de expedição.
            /// </summary>
            public static bool ExibirObsLiberacaoClienteViaExpedicao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirObsLiberacaoClienteViaExpedicao); }
            }

            /// <summary>
            /// Define se a observação de liberação do cliente será exibida na impressão da liberação, na via do cliente.
            /// </summary>
            public static bool ExibirObsLiberacaoClienteViaCliente
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirObsLiberacaoClienteViaCliente); }
            }

            /// <summary>
            /// Texto a ser exibido no resumo do cliente (relatório 4 vias)?
            /// </summary>
            public static string TextoResumoClienteRelatorio4Vias
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.TextoResumoClienteRelatorio4Vias); }
            }

            public static string TextoResumoEmpresaRelatorio4Vias
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.TextoResumoEmpresaRelatorio4Vias); }
            }

            /// <summary>
            /// Exibir o resumo de corte na via do cliente?
            /// </summary>
            public static bool ExibirResumoCorteNaViaCliente
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirResumoCorteNaViaCliente); }
            }

            /// <summary>
            /// Exibir valores nos resumos de corte?
            /// </summary>
            public static bool ExibirValoresResumosCorte
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirValoresResumosCorte); }
            }

            /// <summary>
            /// Exibir a observação do cliente no relatório?
            /// </summary>
            public static bool ExibirObservacaoCliente
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirObservacaoCliente); }
            }

            /// <summary>
            /// Exibir as observações dos pedidos no relatório?
            /// </summary>
            public static bool ExibirObservacaoPedidos
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirObservacaoPedidos); }
            }

            // <summary>
            /// Verifica se não deve mostrar o valor do pedido se o mesmo for de garantia.
            /// </summary>
            public static bool NaoMostrarValorPedidoGarantia
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.NaoMostrarValorPedidoGarantia); }
            }

            /// <summary>
            /// Define se o resumo da liberação será exibido na via da empresa
            /// </summary>
            public static bool ExibirResumoLiberacaoViaEmpresa
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirResumoLiberacaoViaEmpresa); }
            }

            /// <summary>
            /// Indica qual nome será usado no relatório de pedido/liberação.
            /// </summary>
            public static DataSources.TipoNomeExibirRelatorioPedido TipoNomeExibirRelatorioPedido
            {
                get { return Config.GetConfigItem<DataSources.TipoNomeExibirRelatorioPedido>(Config.ConfigEnum.TipoNomeExibirRelatorioPedido); }
            }

            /// <summary>
            /// Multiplicador do número de vias de resumo de liberação.
            /// </summary>
            /// <param name="idCliente"></param>
            /// <returns></returns>
            public static bool DuplicarViasDaLiberacaoSeClienteRota
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.DuplicarViasDaLiberacaoSeClienteRota); }
            }

            // <summary>
            /// Verifica se devem ser exibidos os valores de pedido de mão de obra na via almoxarife.
            /// </summary>
            public static bool ExibirValorPedidoMaoDeObraViaAlmoxarife
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirValorPedidoMaoDeObraViaAlmoxarife); }
            }

            /// <summary>
            /// Verifica se não deve mostrar a 'Obs. da Liberação' na liberação
            /// </summary>
            public static bool NaoMostrarObsLiberacaoNaLiberacao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.NaoMostrarObsLiberacaoNaLiberacao); }
            }

            /// <summary>
            /// Verifica se não deve mostrar a 'Obs. da Liberação' do cliente na liberação.
            /// </summary>
            public static bool NaoMostrarObsLiberacaoClienteNaLiberacao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.NaoMostrarObsLiberacaoClienteNaLiberacao); }
            }

            /// <summary>
            /// Verifica se a observação de liberação do cliente deve ser exibida somente na via da empresa.
            /// </summary>
            public static bool ExibirObsLiberacaoClienteApenasViaEmpresa
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirObsLiberacaoClienteApenasViaEmpresa); }
            }

            /// <summary>
            /// Verifica se deve mostrar apenas a via da expedição e ou almoxarife caso todos os pedidos da liberação sejam do tipo entrega.
            /// </summary>
            public static bool ExibirApenasViaExpAlmPedidosEntrega
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirApenasViaExpAlmPedidosEntrega); }
            }

            /// <summary>
            /// Verifica se deve mostrar apenas a via da expedição e ou almoxarife caso todos os pedidos da liberação sejam do tipo balcão.
            /// </summary>
            public static bool ExibirApenasViaExpAlmPedidosBalcao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirApenasViaExpAlmPedidosBalcao); }
            }

            /// <summary>
            /// Verifica se ao enviar email da liberação envia apenas a via do cliente.
            /// </summary>
            public static bool ExibirApenasViaClienteNoEnvioEmail
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirApenasViaClienteNoEnvioEmail); }
            }

            /// <summary>
            /// Retorna o nome do relatório a ser usado.
            /// </summary>
            public static string NomeRelatorio
            {
                get
                {
                    switch (ControleSistema.GetSite())
                    {
                        case ControleSistema.ClienteSistema.MsVidros:
                        case ControleSistema.ClienteSistema.ScComercio:
                            return "Relatorios/ModeloLiberacao/MSVidros/rptLiberacaoPedidoMSVidros.rdlc";
                    }

                    return
                        DadosLiberacao.UsarRelatorioLiberacao4Vias
                            ? "Relatorios/ModeloLiberacao/rptLiberacaoPedido4Vias.rdlc"
                            : "Relatorios/ModeloLiberacao/rptLiberacaoPedido.rdlc";
                }
            }

            /// <summary>
            /// Define que as parcelas serão exibidas na liberação de forma invertida (escritas de trás pra frente)
            /// </summary>
            public static bool TextoParcelasInvertido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.TextoParcelasInvertido); }
            }

            /// <summary>
            /// Define que a observação de liberação informada no pedido será exibida somente na via da empresa
            /// </summary>
            public static bool ExibirObsLibApenasViaEmpresa
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirObsLibApenasViaEmpresa); }
            }

            /// <summary>
            /// Define que na impressão da liberação os produtos serão ordenados pelo código interno e não pelo m²
            /// </summary>
            public static bool OrdenarProdutosPeloCodInterno
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.OrdenarProdutosPeloCodInterno); }
            }

            /// <summary>
            /// Define que a observação de liberação informada no pedido será exibida no resumo
            /// </summary>
            public static bool ExibirObsLiberacaoResumo
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirObsLiberacaoResumo); }
            }

            /// <summary>
            /// Considerar qualquer produto do grupo vidro como vidro, para ser exibido na via da expedição da liberação.
            /// </summary>
            public static bool ConsiderarVidroQualquerProdutoDoGrupoVidro
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ConsiderarVidroQualquerProdutoDoGrupoVidro); }
            }

            /// <summary>
            /// Se todos os pedidos da liberação forem do tipo entrega não exibe a via do cliente.
            /// </summary>
            public static bool NaoExibirViaClienteSeTodosPedidosForemTipoEntrega
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.NaoExibirViaClienteSeTodosPedidosForemTipoEntrega); }
            }

            /// <summary>
            /// Define se o relatório de liberação será impresso de acordo com o tipo de entrega dos pedidos liberados.
            /// </summary>
            public static bool UsarImpressaoLiberacaoPorTipoEntrega
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarImpressaoLiberacaoPorTipoEntrega); }
            }
        }
    }
}
