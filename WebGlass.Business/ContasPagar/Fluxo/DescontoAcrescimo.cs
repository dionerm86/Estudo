namespace WebGlass.Business.ContasPagar.Fluxo
{
    public sealed class DescontoAcrescimo : BaseFluxo<DescontoAcrescimo>
    {
        private DescontoAcrescimo() { }

        #region Ajax

        private static Ajax.IDescontoAcrescimo _ajax = null;

        public static Ajax.IDescontoAcrescimo Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.DescontoAcrescimo();

                return _ajax;
            }
        }

        #endregion
    }
}
