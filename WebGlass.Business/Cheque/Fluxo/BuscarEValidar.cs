namespace WebGlass.Business.Cheque.Fluxo
{
    public sealed class BuscarEValidar : BaseFluxo<BuscarEValidar>
    {
        private BuscarEValidar() { }
        
        #region Ajax

        private static Ajax.IBuscarEValidar _ajax = null;

        public static Ajax.IBuscarEValidar Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.BuscarEValidar();

                return _ajax;
            }
        }

        #endregion
    }
}
