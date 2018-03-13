using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class PedidoConfig
    {
        public static class Desconto
        {
            /// <summary>
            /// Retorna o desconto máximo que pode ser dado ao pedido, dependendo da empresa
            /// </summary>
            /// <param name="modulo"></param>
            /// <returns></returns>
            public static float GetDescMaxPedido
            {
                get { return GetDescontoMaximoPedido(UserInfo.GetUserInfo.CodUser); }
            }

            /// <summary>
            /// Indica se a empresa utiliza desconto de acordo com a quantidade de produtos vendidos.
            /// </summary>
            public static bool DescontoPorProduto
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.DescontoPorProduto); }
            }

            /// <summary>
            /// Define um desconto padrão em % para ser dado no pedido caso seja à vista
            /// </summary>
            public static decimal DescontoPadraoPedidoAVista
            {
                get { return Config.GetConfigItem<decimal>(Config.ConfigEnum.DescontoPadraoPedidoAVista); }
            }

            /// <summary>
            /// Indica se o desconto do pedido pode ser dado apenas para pedidos à vista.
            /// </summary>
            public static bool DescontoPedidoApenasAVista
            {
                get
                {
                    if (UserInfo.GetUserInfo.IsAdministrador)
                        return false;

                    return Config.GetConfigItem<bool>(Config.ConfigEnum.DescontoPedidoApenasAVista);
                }
            }

            /// <summary>
            /// Define se será permitido dar desconto também em pedidos com uma parcela, considerando que o desconto só é válido para pedidos à vista.
            /// </summary>
            public static bool DescontoPedidoUmaParcela
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.DescontoPedidoUmaParcela); }
            }

            /// <summary>
            /// Não permite que seja inserido desconto em qualquer lugar no sistema se o cliente possuir desconto por grupo
            /// </summary>
            public static bool ImpedirDescontoSomativo
            {
                get
                {
                    if (Config.PossuiPermissao(((int)UserInfo.GetUserInfo.CodUser), Config.FuncaoMenuPedido.IgnorarBloqueioDescontoOrcamentoPedido))
                        return false;

                    return Config.GetConfigItem<bool>(Config.ConfigEnum.ImpedirDescontoSomativo);
                }
            }

            /// <summary>
            /// Indica se o botão de desconto aparece na lista de pedidos.
            /// </summary>
            public static bool DescontoPedidoLista
            {
                get { return Config.GetConfigItem<bool>(Config.ConfigEnum.DescontoPedidoLista); }
            }

            public static float GetDescontoMaximoPedido(uint idFunc)
            {
                return GetDescontoMaximoPedido(null, idFunc);
            }

            public static float GetDescontoMaximoPedido(GDA.GDASession sessao, uint idFunc)
            {
                if (Config.PossuiPermissao((int)idFunc, Config.FuncaoMenuPedido.IgnorarBloqueioDescontoOrcamentoPedido))
                    return 100;
                // Se o funcionário for gerente, retorna o desconto máximo para gerente no pedido.
                else if(Data.DAL.FuncionarioDAO.Instance.ObtemIdTipoFunc(idFunc) == (uint)Seguranca.TipoFuncionario.Gerente)
                {
                    return ObterDescontoMaximoPedidoGerenteConfigurado;
                }

                return GetDescMaxPedidoConfigurado;
            }

            /// <summary>
            /// Retorna o desconto máximo definido para o pedido
            /// </summary>
            public static float GetDescMaxPedidoConfigurado
            {
                get { return Config.GetConfigItem<float>(Config.ConfigEnum.DescontoMaximoPedido); }
            }

            /// <summary>
            /// Retorna o desconto máximo definido para o pedido quando o funcionário é gerente
            /// </summary>
            public static float ObterDescontoMaximoPedidoGerenteConfigurado
            {
                get { return Config.GetConfigItem<float>(Config.ConfigEnum.DescontoMaximoPedidoGerente); }
            }

            /// <summary>
            /// Retorna o desconto máximo que deve ser dado na liberação
            /// </summary>
            public static int DescontoMaximoLiberacao
            {
                get
                {
                    if (UserInfo.GetUserInfo.TipoUsuario == (uint)Utils.TipoFuncionario.Administrador)
                        return 100;

                    return Config.GetConfigItem<int>(Config.ConfigEnum.DescontoMaximoLiberacao);
                }
            }
        }
    }
}
