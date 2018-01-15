namespace WebGlass.Business.NotaFiscal.Fluxo
{
    public sealed class Gerar : BaseFluxo<Gerar>
    {
        private Gerar() { }

        #region Ajax

        private static Ajax.IGerar _ajax = null;

        public static Ajax.IGerar Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.Gerar();

                return _ajax;
            }
        }

        #endregion
    }
}
