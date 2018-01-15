using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ProdutoInventarioEstoqueDAO : BaseDAO<ProdutoInventarioEstoque, ProdutoInventarioEstoqueDAO>
    {
        //private ProdutoInventarioEstoqueDAO() { }

        public IList<ProdutoInventarioEstoque> ObtemPorInventarioEstoque(uint idInventarioEstoque)
        {
            return ObtemPorInventarioEstoque(null, idInventarioEstoque);
        }

        public IList<ProdutoInventarioEstoque> ObtemPorInventarioEstoque(GDA.GDASession session, uint idInventarioEstoque)
        {
            string sql = "select * from produto_inventario_estoque where idInventarioEstoque=" + idInventarioEstoque;
            return objPersistence.LoadData(session, sql).ToList();
        }
    }
}
