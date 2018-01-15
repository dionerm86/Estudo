namespace WebGlass.Business.CustoFixo.Fluxo
{
    public sealed class Retificar : BaseFluxo<Retificar>
    {
        private Retificar() { }

        #region Ajax

        private static Ajax.IRetificar _ajax = null;

        public static Ajax.IRetificar Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.Retificar();

                return _ajax;
            }
        }

        #endregion
    }
}
