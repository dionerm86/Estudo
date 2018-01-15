namespace WebGlass.Business.Medicao.Fluxo
{
    public sealed class Confirmar : BaseFluxo<Confirmar>
    {
        private Confirmar() { }

        #region Ajax

        private static Ajax.IConfirmar _ajax = null;

        public static Ajax.IConfirmar Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.Confirmar();

                return _ajax;
            }
        }

        #endregion
    }
}
