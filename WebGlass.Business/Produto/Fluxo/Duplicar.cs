namespace WebGlass.Business.Produto.Fluxo
{
    public sealed class Duplicar : BaseFluxo<Duplicar>
    {
        private Duplicar() { }

        #region Ajax

        private static Ajax.IDuplicar _ajax = null;

        public static Ajax.IDuplicar Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.Duplicar();

                return _ajax;
            }
        }

        #endregion
    }
}
