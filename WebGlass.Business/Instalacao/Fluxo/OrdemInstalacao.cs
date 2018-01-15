namespace WebGlass.Business.Instalacao.Fluxo
{
    public sealed class OrdemInstalacao : BaseFluxo<OrdemInstalacao>
    {
        private OrdemInstalacao() { }

        #region Ajax

        private static Ajax.IOrdemInstalacao _ajax = null;

        public static Ajax.IOrdemInstalacao Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.OrdemInstalacao();

                return _ajax;
            }
        }

        #endregion
    }
}
