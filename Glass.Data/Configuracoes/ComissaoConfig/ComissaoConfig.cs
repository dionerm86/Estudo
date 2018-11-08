using Glass.Data.Helper;
using Glass.Data.Model;

namespace Glass.Configuracoes
{
    public static class ComissaoConfig
    {
        #region Enumeradores

        public enum TotalComissaoEnum
        {
            TotalSemIcms,
            TotalComImpostos,
            TotalSemImpostos
        }

        #endregion

        /// <summary>
        /// Indica se a empresa trabalha com percentual de redução na comissão de acordo com o desconto no pedido.
        /// </summary>
        public static bool DescontarComissaoPerc
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.DescontarComissaoPerc); }
        }

        /// <summary>
        /// A empresa usa o controle de comissão com percentual definido no cliente?
        /// </summary>
        public static bool UsarPercComissaoCliente
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarPercComissaoCliente); }
        }

        /// <summary>
        /// Verifica qual total vai ser utilizado para a comissao
        /// </summary>
        public static TotalComissaoEnum TotalParaComissao
        {
            get
            {
                var config = Config.GetConfigItem<string>(Config.ConfigEnum.TotalParaComissao);

                return (TotalComissaoEnum)System.Enum.Parse(typeof(TotalComissaoEnum), config);
            }
        }

        /// <summary>
        /// Recupera o tipo de controle de comissão de contas recebidas que será utilizado.
        /// </summary>
        public static TipoComissaoContaRec ComissaoPorContasRecebidas
        {
            get
            {
                var config = Config.GetConfigItem<string>(Config.ConfigEnum.ComissaoPorContasRecebidas);
                return (TipoComissaoContaRec)System.Enum.Parse(typeof(TipoComissaoContaRec), config);
            }
        }

        /// <summary>
        /// Verifica se ao gerar comissão de instalador, deve pegar apenas o valor dos produtos instalados
        /// </summary>
        public static bool UsarComissaoPorProdutoInstalado
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UsarComissaoPorProdutoInstalado); }
        }

        /// <summary>
        /// Verifica se a observação do comissionado/funcionário será exibida na impressão.
        /// </summary>
        public static bool ExibirObsComissionadoOuFuncionarioRelatorioComissao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirObsComissionadoOuFuncionarioRelatorioComissao); }
        }
    }
}
