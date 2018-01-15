using Glass.Data.DAL;

namespace WebGlass.Business.InventarioEstoque.Fluxo
{
    public sealed class Cancelar : BaseFluxo<Cancelar>
    {
        private Cancelar() { }

        public void CancelarInventario(uint codigoInventario, string motivo)
        {
            var inventario = CRUD.Instance.ObtemItem(codigoInventario);

            LogCancelamentoDAO.Instance.LogInventarioEstoque(inventario._inventario, motivo, true);
            InventarioEstoqueDAO.Instance.AlterarSituacao(null, codigoInventario, Glass.Data.Model.InventarioEstoque.SituacaoEnum.Cancelado);
        }
    }
}
