using Glass.Data.DAL;

namespace WebGlass.Business.InventarioEstoque.Fluxo
{
    public sealed class Confirmar : BaseFluxo<Confirmar>
    {
        private Confirmar() { }

        #region Ajax

        private static Ajax.IConfirmar _ajax;

        public static Ajax.IConfirmar Ajax
        {
            get
            {
                if (_ajax == null)
                    _ajax = new Ajax.Confirmar();

                return _ajax;
            }
        }

        #endregion

        public void ConfirmarInventario(uint codigoInventario)
        {
            InventarioEstoqueDAO.Instance.Confirmar(codigoInventario);
        }
    }
}
