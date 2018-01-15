using System;

namespace Glass
{
    /// <summary>
    /// Provedor dos contextos da aplicação.
    /// </summary>
    public abstract class ProvedorContexto
    {
        #region Local Variables

        private static Glass.Seguranca.IContextoUsuario _contextoUsuario;        

        #endregion

        #region Properties

        /// <summary>
        /// Recupera o contexto do usuário.
        /// </summary>
        /// <returns></returns>
        public static Glass.Seguranca.IContextoUsuario ContextoUsuario
        {
            get
            {
                return _contextoUsuario;
            }
        }        

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Configura o provedor.
        /// </summary>
        /// <param name="userContext">Instancia do contexto do usuário que será configurada.</param>
        public static void Configure(Glass.Seguranca.IContextoUsuario userContext)
        {
            if (userContext == null)
                throw new ArgumentNullException("userContext");
            
            _contextoUsuario = userContext;
        }

        #endregion
    }
}
