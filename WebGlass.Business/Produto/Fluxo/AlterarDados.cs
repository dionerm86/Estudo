namespace WebGlass.Business.Produto.Fluxo
{
    public sealed class AlterarDados : BaseFluxo<AlterarDados>
    {
        private AlterarDados() { }

        #region Ajax

        private static Ajax.IAlterarDados _ajax = null;

        public static Ajax.IAlterarDados Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.AlterarDados();

                return _ajax;
            }
        }

        #endregion
    }
}
