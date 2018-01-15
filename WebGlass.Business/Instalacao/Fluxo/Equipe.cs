namespace WebGlass.Business.Instalacao.Fluxo
{
    public sealed class Equipe : BaseFluxo<Equipe>
    {
        private Equipe() { }

        #region Ajax

        private static Ajax.IEquipe _ajax = null;

        public static Ajax.IEquipe Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.Equipe();

                return _ajax;
            }
        }

        #endregion
    }
}
