using System;
using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class TextoImprPedidoDAO : BaseDAO<TextoImprPedido, TextoImprPedidoDAO>
    {
        //private TextoImprPedidoDAO() { }

        private string Sql(bool selecionar)
        {
            string sql = "Select " + (selecionar ? "*" : "Count(*)") + " From texto_impr_pedido ";

            return sql;
        }

        public IList<TextoImprPedido> GetList(string sortExpression, int startRow, int pageSize)
        {
            string filtro = String.IsNullOrEmpty(sortExpression) ? "idTextoImprPedido" : sortExpression;

            return LoadDataWithSortExpression(Sql(true), filtro, startRow, pageSize, null);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(false));
        }

        public override int Update(TextoImprPedido objUpdate)
        {
            LogAlteracaoDAO.Instance.LogTextoImprPedido(objUpdate);

            return base.Update(objUpdate);
        }
    }
}
