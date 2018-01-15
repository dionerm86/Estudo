namespace WebGlass.Business.Pedido.Fluxo
{
    public sealed class DadosPedido : BaseFluxo<DadosPedido>
    {
        private DadosPedido() { }

        #region Ajax

        private static Ajax.IDadosPedido _ajax = null;

        public static Ajax.IDadosPedido Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.DadosPedido();

                return _ajax;
            }
        }

        #endregion
    }
}
