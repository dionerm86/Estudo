using Glass.Data.DAL;

namespace WebGlass.Business.InventarioEstoque.Fluxo
{
    public sealed class IniciarContagem : BaseFluxo<IniciarContagem>
    {
        private IniciarContagem() { }

        public void Iniciar(uint codigoInventario)
        {
            InventarioEstoqueDAO.Instance.Iniciar(codigoInventario);
        }
    }
}
