namespace WebGlass.Business.ContasPagar.Fluxo
{
    public sealed class Pagar : BaseFluxo<Pagar>
    {
        private Pagar() { }

        #region Ajax

        private static Ajax.IPagar _ajax = null;

        public static Ajax.IPagar Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.Pagar();

                return _ajax;
            }
        }

        #endregion
    }
}
