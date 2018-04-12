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
        /// Identifica se o vendedor pode alterar orçamentos de qualquer loja
        /// </summary>
        public static bool VendedorPodeAlterarOrcamentoQualquerLoja
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.VendedorPodeAlterarOrcamentoQualquerLoja); }
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

        public static bool AlterarLojaOrcamento
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.AlterarLojaOrcamento); }
        }

        /// <summary>
        /// Indica se deve mostrar o ícone de envio de email do orçamento na listagem
        /// </summary>
        public static bool MostrarIconeEnvioEmailListagem
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.MostrarIconeEnvioEmailListagemOrcamento); }
        }

        /// <summary>
        /// Verifica se deve ou não cir marcado o checkbox de cobrar beneficiamento opcional.
        /// </summary>
        public static bool CheckBenefOpcionalDesmascadoPadrao
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.CheckBenefOpcionalDesmascadoPadrao); }
        }

        /// <summary>
        /// verifica se deve ou não Permitir salvar orcamento sem o processo e aplicação da peça
        /// </summary>
        public static bool PermirtirSalvarOrcamentoSemProcAplic
        {
            get { return Config.GetConfigItem<bool>(Config.ConfigEnum.PermirtirSalvarOrcamentoSemProcAplic); }
        }
    }
}
