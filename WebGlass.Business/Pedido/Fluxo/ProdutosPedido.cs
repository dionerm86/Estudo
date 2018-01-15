namespace WebGlass.Business.Pedido.Fluxo
{
    public sealed class ProdutosPedido : BaseFluxo<ProdutosPedido>
    {
        private ProdutosPedido() { }

        #region Ajax

        private static Ajax.IProdutosPedido _ajax = null;

        public static Ajax.IProdutosPedido Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.ProdutosPedido();

                return _ajax;
            }
        }

        #endregion
    }
}
