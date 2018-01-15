using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProdutoSaidaEstoqueDAO : BaseDAO<ProdutoSaidaEstoque, ProdutoSaidaEstoqueDAO>
    {
        //private ProdutoSaidaEstoqueDAO() { }

        private string Sql(uint idSaidaEstoque, string idsSaidaEstoque)
        {
            string sql = @"
                select pse.*, p.codInterno, p.descricao as descrProd
                from produto_saida_estoque pse
                    left join produtos_pedido pp on (pse.idProdPed=pp.idProdPed)
                    left join produto p on (pp.idProd=p.idProd)
                where 1";
            
            if (idSaidaEstoque > 0)
                sql += " and idSaidaEstoque=" + idSaidaEstoque;
            else if (!String.IsNullOrEmpty(idsSaidaEstoque))
                sql += " and idSaidaEstoque in (" + idsSaidaEstoque + ")";
            
            return sql;
        }

        public ProdutoSaidaEstoque[] GetForRpt(uint idSaidaEstoque)
        {
            return GetForRpt(null, idSaidaEstoque);
        }

        public ProdutoSaidaEstoque[] GetForRpt(GDASession sessao, uint idSaidaEstoque)
        {
            return objPersistence.LoadData(sessao, Sql(idSaidaEstoque, null)).ToList().ToArray();
        }

        public IList<ProdutoSaidaEstoque> GetByVariasSaidas(string idsSaidaEstoque)
        {
            return objPersistence.LoadData(Sql(0, idsSaidaEstoque)).ToList();
        }
    }
}
