using System;
using System.Web.UI;

namespace Glass.UI.Web
{
    /// <summary>
    /// Implementa��o base dos UserControls.
    /// </summary>
    public abstract class BaseUserControl : UserControl
    {
        #region Finalizador
    
        /// <summary>
        /// Liberae a cole��o de controles.
        /// </summary>
        /// <param name="controles"></param>
        private static void DisposeObject(ControlCollection controles)
        {
            if (controles != null)
                foreach (Control c in controles)
                    DisposeObject(c);
        }
    
        /// <summary>
        /// Libera os objetos informados.
        /// </summary>
        /// <param name="objetos"></param>
        private static void DisposeObject(params IDisposable[] objetos)
        {
            if (objetos != null)
                foreach (IDisposable o in objetos)
                    if (o != null)
                        o.Dispose();
        }
    
        /// <summary>
        /// Libera a instancia.
        /// </summary>
        public override void Dispose()
        {
            DisposeObject(this.Controls);
            base.Dispose();
        }
    
        #endregion

        #region M�todos Protegidos

        /// <summary>
        /// Verifica se o controle est� vis�vel.
        /// </summary>
        /// <returns></returns>
        protected bool IsVisible()
        {
            Control c = this;
            while (c.Visible && (c = c.Parent) != null);
    
            return c == null;
        }

        #endregion

        #region Construtores
        
        /// <summary>
        /// Construtor padr�o.
        /// </summary>
        public BaseUserControl()
        {

        }

        #endregion
    }
}
