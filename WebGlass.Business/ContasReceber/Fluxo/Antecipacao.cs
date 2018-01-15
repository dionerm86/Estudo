namespace WebGlass.Business.ContasReceber.Fluxo
{
    public sealed class Antecipacao : BaseFluxo<Antecipacao>
    {
        private Antecipacao() { }

        #region Ajax

        private static Ajax.IAntecipacao _ajax = null;

        public static Ajax.IAntecipacao Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.Antecipacao();

                return _ajax;
            }
        }

        #endregion
    }
}
