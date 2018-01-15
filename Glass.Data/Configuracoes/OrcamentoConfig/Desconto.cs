using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public partial class OrcamentoConfig
    {
        public static class Desconto
        {
            /// <summary>
            /// Retorna o desconto máximo permitido para o orçamento.
            /// </summary>
            public static float DescontoMaximoOrcamento
            {
                get { return GetDescontoMaximoOrcamento(UserInfo.GetUserInfo.CodUser); }
            }

            public static bool DescontoAcrescimoItensOrcamento
            {
                get { return OrcamentoConfig.ItensProdutos.ItensProdutosOrcamento && Config.GetConfigItem<bool>(Config.ConfigEnum.DescontoAcrescimoItensOrcamento); }
            }

            public static float GetDescontoMaximoOrcamento(uint idFunc)
            {
                return GetDescontoMaximoOrcamento(null, idFunc);
            }

            public static float GetDescontoMaximoOrcamento(GDA.GDASession sessao, uint idFunc)
            {
                if (Config.PossuiPermissao((int)idFunc, Config.FuncaoMenuPedido.IgnorarBloqueioDescontoOrcamentoPedido))
                    return 100F;

                return GetDescMaxOrcamentoConfigurado;
            }

            public static float GetDescMaxOrcamentoConfigurado
            {
                get { return Config.GetConfigItem<float>(Config.ConfigEnum.DescontoMaximoOrcamento); }
            }
        }
    }
}
