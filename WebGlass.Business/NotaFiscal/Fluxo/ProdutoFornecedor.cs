namespace WebGlass.Business.NotaFiscal.Fluxo
{
    public sealed class ProdutoFornecedor : BaseFluxo<ProdutoFornecedor>
    {
        private ProdutoFornecedor() { }

        #region Ajax

        private static Ajax.IProdutoFornecedor _ajax = null;

        public static Ajax.IProdutoFornecedor Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.ProdutoFornecedor();

                return _ajax;
            }
        }

        #endregion
    }
}
