using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    partial class PCPConfig
    {
        public class RelatorioPedidoPcp
        {
            /// <summary>
            /// Define que na impressão do pedido PCP sempre irá agrupar por produto, altura e largura
            /// </summary>
            public static bool SempreAgruparProdutosPorProdutoAlturaLargura
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.SempreAgruparProdutosPorProdutoAlturaLargura); }
            }
        }
    }
}
