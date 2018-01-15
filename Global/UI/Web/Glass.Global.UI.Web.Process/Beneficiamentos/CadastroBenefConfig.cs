namespace Glass.Global.UI.Web.Process.Beneficiamentos
{
    /// <summary>
    /// Classe que auxilia no cadastro da configuração do beneficiamento.
    /// </summary>
    public class CadastroBenefConfig
    {
        #region Variáveis Locais

        private Global.Negocios.IBeneficiamentoFluxo _beneficiamentoFluxo;

        #endregion

        #region Propriedades

        /// <summary>
        /// Fluxo de negócio do beneficiamento.
        /// </summary>
        private Global.Negocios.IBeneficiamentoFluxo BeneficiamentoFluxo
        {
            get
            {
                if (_beneficiamentoFluxo == null)
                    _beneficiamentoFluxo = Microsoft.Practices.ServiceLocation.ServiceLocator
                        .Current.GetInstance<Global.Negocios.IBeneficiamentoFluxo>();

                return _beneficiamentoFluxo;
            }
        }

        #endregion

        #region Métodos Públicos
        
        /// <summary>
        /// Recupera os dados da configuração do beneficiamento.
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <returns></returns>
        public BenefConfigWrapper ObtemBenefConfig(int idBenefConfig)
        {
            return BeneficiamentoFluxo.ObtemBenefConfig(idBenefConfig);
        }

        /// <summary>
        /// Salva os dados da configuração do beneficiamento.
        /// </summary>
        /// <param name="benefConfig"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarBenefConfig(BenefConfigWrapper benefConfig)
        {
            // Pixa os dados cadastrados
            benefConfig.Fixar();

            return BeneficiamentoFluxo.SalvarBenefConfig(benefConfig);
        }

        #endregion
    }
}
