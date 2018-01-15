namespace WebGlass.Business.Produto.Fluxo
{
    public sealed class Valor : BaseFluxo<Valor>
    {
        private Valor() { }

        #region Ajax

        private static Ajax.IValor _ajax = null;

        public static Ajax.IValor Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.Valor();

                return _ajax;
            }
        }

        #endregion
    }
}
