using System.Collections.Generic;
using Glass.Data.Model.Cte;
using GDA;

namespace Glass.Data.DAL.CTe
{
    public sealed class ConhecimentoTransporteRodoviarioDAO : BaseDAO<ConhecimentoTransporteRodoviario, ConhecimentoTransporteRodoviarioDAO>
    {
        //private ConhecimentoTransporteRodoviarioDAO() { }

        #region Busca padrão

        private string Sql(uint idCte, bool selecionar)
        {
            string sql = "Select * From conhecimento_transporte_rodoviario Where 1";

            if (idCte > 0)
                sql += " And IDCTE=" + idCte;

            return sql;
        }

        public ConhecimentoTransporteRodoviario GetElement(uint idCte)
        {
            return GetElement(null, idCte);
        }

        public ConhecimentoTransporteRodoviario GetElement(GDASession session, uint idCte)
        {
            try
            {
                return objPersistence.LoadOneData(session, Sql(idCte, true));
            }
            catch
            {
                return new ConhecimentoTransporteRodoviario();
            }
        }

        public IList<ConhecimentoTransporteRodoviario> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, false), null);
        }

        #endregion
    }
}
