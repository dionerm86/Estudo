using Glass.Data.DAL;

namespace WebGlass.Business.Pedido.Ajax
{
    public interface IProdutosPedido
    {
        string TotalProdPed(string idPedido);
    }

    internal class ProdutosPedido : IProdutosPedido
    {
        public string TotalProdPed(string idPedido)
        {
            return ProdutosPedidoDAO.Instance.GetTotalByPedido(Glass.Conversoes.StrParaUint(idPedido));
        }
    }
}
