using System;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        public static class RelatorioPedido
        {
            /// <summary>
            /// Nome do arquivo do relatório.
            /// </summary>
            /// <returns>Item1: é o caminho do arquivo do relatório.
            /// Item2: true/false - o arquivo é um dos relatórios padrões do sistema (A4 ou A4ProcApl), foi criado a partir de um deles ou não está mapeado no código.</returns>
            public static Tuple<string, bool> NomeArquivoRelatorio(uint idLoja)
            {
                var caminhoRelatorio = string.Format("Relatorios/ModeloPedido/rptPedido{0}.rdlc", ControleSistema.GetSite().ToString());

                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}", caminhoRelatorio))))
                    return new Tuple<string, bool>(caminhoRelatorio, true);

                return new Tuple<string, bool>("Relatorios/ModeloPedido/rptPedidoA4.rdlc", true);
            }

            public static bool ExibirTotalML
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirTotalML); }
            }

            /// <summary>
            /// Verifica se a empresa exibe o m² calculado no relatório.
            /// </summary>
            public static bool ExibirM2CalcRelatorio
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirM2CalcRelatorio); }
            }

            /// <summary>
            /// Verifica se a empresa agrupa os beneficiamentos no relatório.
            /// </summary>
            public static bool AgruparBenefRelatorio
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AgruparBenefRelatorio); }
            }

            /// <summary>
            /// Indica se o relatório de pedidos só aparece para pedidos confirmados.
            /// </summary>
            public static bool SoImprimirPedidoConfirmado
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SoImprimirPedidoConfirmado); }
            }

            /// <summary>
            /// Define se a empresa exibe o relatório de preço de tabela de clientes para os vendedores.
            /// </summary>
            public static bool RelatorioPrecoTabelaClientes
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.RelatorioPrecoTabelaClientes); }
            }

            /// <summary>
            /// Identifica se os itens dos produtos do pedido serão exibidos no relatório.
            /// </summary>
            public static bool ExibirItensProdutosPedido
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirItensProdutosPedido); }
            }
        }
    }
}
