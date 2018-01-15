using Glass.Data.Helper;
using System.Drawing;
using Glass.Data.Model;
using System;

namespace Glass.Configuracoes
{
    public partial class Liberacao
    {
        /// <summary>
        /// Classe com as configurações da tela de liberação de pedido.
        /// </summary>
        public static class TelaLiberacao
        {
            /// <summary>
            /// Indica a cor que deve ser usada para exibir a observação do cliente
            /// (caso seja exibida).
            /// </summary>
            public static Color CorExibirObservacaoCliente
            {
                get
                {
                    var config = Config.GetConfigItem<string>(Config.ConfigEnum.CorExibirObservacaoCliente);

                    if (string.IsNullOrEmpty(config))
                        return Color.Black;

                    return Color.FromName(config);
                }
            }

            /// <summary>
            /// Define se a empresa cobra os pedidos de reposição dos clientes.
            /// </summary>
            public static bool CobrarPedidoReposicao
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CobrarPedidoReposicao); }
            }

            /// <summary>
            /// Determina os tipos de pedido selecionados por padrão ao iniciar
            /// a tela de liberação de pedidos.
            /// </summary>
            public static string TiposPedidosSelecionadosPadrao
            {
                get { return Config.GetConfigItem<string>(Config.ConfigEnum.TiposPedidosSelecionadosPadrao); }
            }

            /// <summary>
            /// Indica se a impressão apenas da via do cliente deve ser mostrada.
            /// </summary>
            public static bool ExibirRelatorioCliente
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirRelatorioCliente); }
            }
        }
    }
}
