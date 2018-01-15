using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProdutoEntradaEstoqueDAO : BaseDAO<ProdutoEntradaEstoque, ProdutoEntradaEstoqueDAO>
    {
        //private ProdutoEntradaEstoqueDAO() { }

        public IList<ProdutoEntradaEstoque> GetForRpt(uint idEntradaEstoque)
        {
            return GetForRpt(null, idEntradaEstoque);
        }

        public IList<ProdutoEntradaEstoque> GetForRpt(GDASession session, uint idEntradaEstoque)
        {
            string sql = @"
                select pee.*, coalesce(p.codInterno, p1.codInterno) as codInterno, coalesce(p.descricao, p1.descricao) as descrProd
                from produto_entrada_estoque pee
                    left join produtos_compra pc on (pee.idProdCompra=pc.idProdCompra)
                    left join produto p on (pc.idProd=p.idProd)
                    left join produtos_nf pnf on (pee.idProdNf=pnf.idProdNf)
                    left join produto p1 on (pnf.idProd=p1.idProd)
                where idEntradaEstoque=" + idEntradaEstoque;

            return objPersistence.LoadData(session, sql).ToList();
        }
    }
}
