using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class Liberacao
    {
        public static class Estoque
        {
            /// <summary>
            /// Indica se, ao liberar um pedido, os produtos desse dão baixa automaticamente no estoque.
            /// </summary>
            public static bool SaidaEstoqueAoLiberarPedido
            {
                get
                {
                    //if (OrdemCargaConfig.UsarControleOrdemCarga)
                        //return false;

                    return Config.GetConfigItem<bool>(Config.ConfigEnum.SaidaEstoqueAoLiberarPedido);
                }
            }

            /// <summary>
            /// Verifica se os produtos de produção darão saída do estoque automaticamente ao liberar.
            /// </summary>
            public static bool SaidaEstoqueBoxLiberar
            {
                get
                {
                    if (OrdemCargaConfig.UsarControleOrdemCarga)
                        return false;

                    return Config.GetConfigItem<bool>(Config.ConfigEnum.SaidaEstoqueBoxLiberar);
                }
            }
        }
    }
}
