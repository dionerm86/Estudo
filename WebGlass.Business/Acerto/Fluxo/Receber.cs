namespace WebGlass.Business.Acerto.Fluxo
{
    public sealed class Receber : BaseFluxo<Receber>
    {
        private Receber() { }

        #region Ajax

        private static Ajax.IReceber _ajax = null;

        public static Ajax.IReceber Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.Receber();

                return _ajax;
            }
        }

        #endregion
    }
}
