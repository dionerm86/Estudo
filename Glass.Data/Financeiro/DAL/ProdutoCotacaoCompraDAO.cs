using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProdutoCotacaoCompraDAO : BaseDAO<ProdutoCotacaoCompra, ProdutoCotacaoCompraDAO>
    {
        //private ProdutoCotacaoCompraDAO() { }

        private string Sql(uint idCotacaoCompra, bool selecionar)
        {
            StringBuilder sql = new StringBuilder("select ");
            
            sql.Append(selecionar ? "pcc.*" : "count(*)"); 
            
            sql.AppendFormat(@"
                from produto_cotacao_compra pcc
                where 1 {0}", FILTRO_ADICIONAL);
            
            if (idCotacaoCompra > 0)
                sql.AppendFormat(" and pcc.idCotacaoCompra={0}", idCotacaoCompra);

            return sql.ToString();
        }

        public ProdutoCotacaoCompra[] ObtemProdutos(GDASession session, uint idCotacaoCompra)
        {
            string sql = Sql(idCotacaoCompra, true).Replace(FILTRO_ADICIONAL, "");
            return objPersistence.LoadData(session, sql).ToArray();
        }

        public IList<ProdutoCotacaoCompra> ObtemProdutos(uint idCotacaoCompra, string sortExpression,
            int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idCotacaoCompra, true), sortExpression, startRow, pageSize);
        }

        public int ObtemNumeroProdutos(uint idCotacaoCompra)
        {
            return GetCountWithInfoPaging(Sql(idCotacaoCompra, true), false);
        }

        private void CalculaTotM2(ref ProdutoCotacaoCompra obj)
        {
            List<int> tipoCalcM2 = new List<int> { 
                (int)Glass.Data.Model.TipoCalculoGrupoProd.M2, 
                (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto, 
                (int)Glass.Data.Model.TipoCalculoGrupoProd.QtdM2 
            };

            int tipoCalc = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo((int)obj.IdProd);
            obj.TotM = !tipoCalcM2.Contains(tipoCalc) ? 0 :
                Glass.Conversoes.StrParaFloat(MetodosAjax.CalcM2Compra(obj.IdProd.ToString(), tipoCalc.ToString(), 
                obj.Altura.ToString(), obj.Largura.ToString(), obj.Qtde.ToString()));
        }

        public override uint Insert(ProdutoCotacaoCompra objInsert)
        {
            CalculaTotM2(ref objInsert);
            return base.Insert(objInsert);
        }

        public override int Update(ProdutoCotacaoCompra objUpdate)
        {
            CalculaTotM2(ref objUpdate);
            return base.Update(objUpdate);
        }
    }
}
