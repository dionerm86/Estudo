namespace Glass.Negocios.Componentes.Seguranca
{
    /// <summary>
    /// Implementação do gerenciador de provedores de identidade.
    /// </summary>
    public class GerenciadorProvedorIdentidade : Colosoft.Security.Authentication.IIdentityProviderManager
    {
        #region Public Methods

        /// <summary>
        /// Recupera o provedor identidade.
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns></returns>
        public Colosoft.Security.Authentication.IdentityProvider GetProviderById(int providerId)
        {
            return new Colosoft.Security.Authentication.IdentityProvider
            {
                FullName = typeof(ProvedorUsuario).FullName,
                IdentityProviderId = 0,
                TypeString = string.Format("{0}, {1}", typeof(AutenticacaoUsuario).FullName, typeof(AutenticacaoUsuario).Assembly.FullName),
                WarningDays = 10
            };
        }

        /// <summary>
        /// Recupera o provedor identidade.
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public Colosoft.Security.Authentication.IdentityProvider GetProviderByName(string providerName)
        {
            return GetProviderById(0);
        }

        #endregion
    }
}
