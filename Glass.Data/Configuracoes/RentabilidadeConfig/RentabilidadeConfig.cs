using System.Collections.Generic;
using Glass.Data.Helper;
using Glass.Data.Model;

namespace Glass.Configuracoes
{
    /// <summary>
    /// Configuração relaciondas com a rentabilidade.
    /// </summary>
    public static partial class RentabilidadeConfig
    {
        /// <summary>
        /// Define e está pode calcular valores de rentabilidade.
        /// </summary>
        public static bool CalcularRentabilidade
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CalcularRentabilidade); }
        }

        /// <summary>
        /// Identifica se é para exibir a rentabilidade no orçamento.
        /// </summary>
        public static bool ExibirRentabilidadeOrcamento
        {
            get
            {
                return CalcularRentabilidade &&
                    Config.PossuiPermissao(
                        (int)(UserInfo.GetUserInfo?.CodUser ?? 0), 
                        Config.FuncaoMenuOrcamento.ExibirRentabilidade);
            }
        }

        /// <summary>
        /// Identifica se é para exibir a rentabilidade no pedido.
        /// </summary>
        public static bool ExibirRentabilidadePedido
        {
            get
            {
                return CalcularRentabilidade &&
                    Config.PossuiPermissao(
                        (int)(UserInfo.GetUserInfo?.CodUser ?? 0),
                        Config.FuncaoMenuPedido.ExibirRentabilidade);
            }
        }

        /// <summary>
        /// Identifica se é para exibir a rentabilidade no pedido espelho.
        /// </summary>
        public static bool ExibirRentabilidadePedidoEspelho
        {
            get
            {
                return CalcularRentabilidade &&
                    Config.PossuiPermissao(
                        (int)(UserInfo.GetUserInfo?.CodUser ?? 0),
                        Config.FuncaoMenuPCP.ExibirRentabilidade);
            }
        }

        /// <summary>
        /// Identifica se é para exibir a rentabilidade na nota fiscal.
        /// </summary>
        public static bool ExibirRentabilidadeNotaFiscal
        {
            get
            {
                return CalcularRentabilidade &&
                    Config.PossuiPermissao(
                        (int)(UserInfo.GetUserInfo?.CodUser ?? 0),
                        Config.FuncaoMenuFiscal.ExibirRentabilidade);
            }
        }
    }
}
