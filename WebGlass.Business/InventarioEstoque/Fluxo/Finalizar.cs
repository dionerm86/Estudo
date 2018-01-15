using System.Collections.Generic;
using Glass.Data.DAL;

namespace WebGlass.Business.InventarioEstoque.Fluxo
{
    public sealed class Finalizar : BaseFluxo<Finalizar>
    {
        private Finalizar() { }

        public IList<Entidade.ProdutoInventarioEstoque> ObtemProdutos(uint codigoInventario)
        {
            var inventario = CRUD.Instance.ObtemItem(codigoInventario);
            return inventario.Produtos;
        }

        public void FinalizarProduto(Entidade.ProdutoInventarioEstoque produto)
        {
            ProdutoInventarioEstoqueDAO.Instance.Update(produto._produto);
        }

        public void FinalizarInventario(uint codigoInventario)
        {
            bool finalizar = true;
            var inventario = CRUD.Instance.ObtemItem(codigoInventario);

            // Removido para não obrigar a preencher todos os campos
            /*foreach (var p in inventario.Produtos)
            {
                if (p.QtdeFinalizacao == null && p.M2Finalizacao == null)
                {
                    finalizar = false;
                    break;
                }
            }*/

            if (finalizar)
                InventarioEstoqueDAO.Instance.Finalizar(codigoInventario);
        }
    }
}
