using Glass.Data.Helper;

namespace Glass.Configuracoes
{
    public static partial class OrcamentoConfig
    {
        public static bool UploadImagensOrcamento
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.UploadImagensOrcamento); }
        }
        
        /// <summary>
        /// Identifica se o vendedor pode alterar orçamentos de outros vendedores
        /// </summary>
        public static bool AlterarOrcamentoVendedor
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AlterarOrcamentoVendedor); }
        }

        /// <summary>
        /// Define se a empresa trabalha com o ambiente no orçamento.
        /// </summary>
        public static bool AmbienteOrcamento
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AmbienteOrcamento); }
        }

        /// <summary>
        /// Indica se o orçamento gera medição definitiva.
        /// </summary>
        public static bool OrcamentoGeraMedicaoDefinitiva
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.OrcamentoGeraMedicaoDefinitiva); }
        }

        /// <summary>
        /// Identifica se os itens dos produtos do orçamento serão exibidos no relatório.
        /// </summary>
        public static bool ExibirItensProdutosRelatorio
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.ExibirItensProdutosRelatorio); }
        }
        
        /// <summary>
        /// Retorna o limite diário de medições.
        /// </summary>
        public static int LimiteDiarioMedicoes
        {
            get { return Config.GetConfigItem<int>(Config.ConfigEnum.LimiteDiarioMedicoes); }
        }

        /// <summary>
        /// Define se a empresa permite negociar orçamentos parcialmente.
        /// </summary>
        public static bool NegociarParcialmente
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.NegociarParcialmente); }
        }

        /// <summary>
        /// Indica se o orçamento deverá calcular desconto como no pedido, primeiro aplicar o acréscimo e depois aplicar o desconto.
        /// IMPORTANTE: Caso esta configuração seja habilitada para alguma empresa é importante que o valor do produto e o total do mesmo
        /// no relatório seja alterado para os campos TotalAmbiente e ValorProdAmbiente, vide relatório de orçamento da Vidrex.
        /// </summary>
        public static bool CalcularDescontoAposAcrescimo
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CalcularDescontoAposAcrescimo); }
        }

        /// <summary>
        /// Define que na tela de listagem, será filtrado por padrão apenas orçamentos ativos
        /// </summary>
        public static bool FiltroPadraoAtivoListagem
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.FiltroPadraoAtivoListagem); }
        }

        public static bool AlterarLojaOrcamento
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AlterarLojaOrcamento); }
        }
    }
}
