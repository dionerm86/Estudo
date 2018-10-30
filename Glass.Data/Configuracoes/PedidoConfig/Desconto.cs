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
                get { return GetDescontoMaximoPedido(UserInfo.GetUserInfo.CodUser, 0, 0); }
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

            public static float GetDescontoMaximoPedido(uint idFunc, int tipoVendaPedido, int? idParcela)
            {
                return GetDescontoMaximoPedido(null, idFunc, tipoVendaPedido, idParcela);
            }

            public static float GetDescontoMaximoPedido(GDA.GDASession sessao, uint idFunc, int tipoVendaPedido, int? idParcela)
            {
                var idFuncAtual = UserInfo.GetUserInfo.CodUser;
                if (idFuncAtual > 0 && idFuncAtual != idFunc && UserInfo.IsAdministrador(idFuncAtual))
                {
                    idFunc = idFuncAtual;
                }

                // Se o funcionário tiver permissão de ignorar bloqueio de desconto no orçamento e pedido
                if (Config.PossuiPermissao((int)idFunc, Config.FuncaoMenuPedido.IgnorarBloqueioDescontoOrcamentoPedido))
                {
                    return 100;
                }

                return GetDescontoMaximoPedidoConfigurado(sessao, idFunc, tipoVendaPedido, idParcela);
            }

            public static float GetDescontoMaximoPedidoConfigurado(GDA.GDASession sessao, uint idFunc, int tipoVendaPedido, int? idParcela)
            {
                // Se o funcionário for gerente, retorna o desconto máximo para gerente no pedido.
                if (Data.DAL.FuncionarioDAO.Instance.ObtemIdTipoFunc(sessao, idFunc) == (uint)Seguranca.TipoFuncionario.Gerente)
                {
                    if (tipoVendaPedido == (int)Data.Model.Pedido.TipoVendaPedido.APrazo && !Data.DAL.ParcelasDAO.Instance.ObterParcelaAVista(sessao, idParcela ?? 0))
                    {
                        return ObterDescontoMaximoPedidoAPrazoGerenteConfigurado;
                    }

                    return ObterDescontoMaximoPedidoAVistaGerenteConfigurado;
                }
                else
                {
                    if (tipoVendaPedido == (int)Data.Model.Pedido.TipoVendaPedido.APrazo && !Data.DAL.ParcelasDAO.Instance.ObterParcelaAVista(sessao, idParcela ?? 0))
                    {
                        return GetDescMaxPedidoAPrazoConfigurado;
                    }

                    return GetDescMaxPedidoAVistaConfigurado;
                }
            }

            /// <summary>
            /// Retorna o desconto máximo definido para o pedido à vista
            /// </summary>
            public static float GetDescMaxPedidoAVistaConfigurado
            {
                get { return Config.GetConfigItem<float>(Config.ConfigEnum.DescontoMaximoPedidoAVista); }
            }

            /// <summary>
            /// Retorna o desconto máximo definido para o pedido à prazo
            /// </summary>
            public static float GetDescMaxPedidoAPrazoConfigurado
            {
                get { return Config.GetConfigItem<float>(Config.ConfigEnum.DescontoMaximoPedidoAPrazo); }
            }

            /// <summary>
            /// Retorna o desconto máximo definido para o pedido quando o funcionário é gerente à vista
            /// </summary>
            public static float ObterDescontoMaximoPedidoAVistaGerenteConfigurado
            {
                get { return Config.GetConfigItem<float>(Config.ConfigEnum.DescontoMaximoPedidoAVistaGerente); }
            }

            /// <summary>
            /// Retorna o desconto máximo definido para o pedido quando o funcionário é gerente à prazo
            /// </summary>
            public static float ObterDescontoMaximoPedidoAPrazoGerenteConfigurado
            {
                get { return Config.GetConfigItem<float>(Config.ConfigEnum.DescontoMaximoPedidoAPrazoGerente); }
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
