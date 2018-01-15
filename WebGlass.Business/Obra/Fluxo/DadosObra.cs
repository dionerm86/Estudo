namespace WebGlass.Business.Obra.Fluxo
{
    public sealed class DadosObra : BaseFluxo<DadosObra>
    {
        private DadosObra() { }

        #region Ajax

        private static Ajax.IDadosObra _ajax = null;

        public static Ajax.IDadosObra Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.DadosObra();

                return _ajax;
            }
        }

        #endregion
    }
}
