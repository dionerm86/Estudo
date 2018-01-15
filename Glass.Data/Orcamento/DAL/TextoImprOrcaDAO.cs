using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class TextoImprOrcaDAO : BaseDAO<TextoImprOrca, TextoImprOrcaDAO>
    {
        //private TextoImprOrcaDAO() { }

        private string Sql(bool selecionar)
        {
            string sql = "Select " + (selecionar ? "*" : "Count(*)") + " From texto_impr_orca ";

            return sql;
        }

        public IList<TextoImprOrca> GetList(string sortExpression, int startRow, int pageSize)
        {
            var filtro = new InfoSortExpression(String.IsNullOrEmpty(sortExpression) ? "idTextoImprOrca" : sortExpression);

            return LoadDataWithSortExpression(Sql(true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount()
        {
            return this.objPersistence.ExecuteSqlQueryCount(Sql(false), null);
        }
    }
}