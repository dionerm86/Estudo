using System.Collections.Generic;
using Glass.Data.Model.Cte;
using GDA;

namespace Glass.Data.DAL.CTe
{
    public sealed class ComplPassagemCteDAO : BaseDAO<ComplPassagemCte, ComplPassagemCteDAO>
    {
        //private ComplPassagemCteDAO() { }

        #region Busca padrão

        private string Sql(uint idCte, bool selecionar)
        {
            string sql = "Select * From compl_passagem_cte Where 1";

            if (idCte > 0)
                sql += " And IDCTE=" + idCte;

            return sql;
        }

        public ComplPassagemCte GetElement(uint idCte)
        {
            return GetElement(null, idCte);
        }

        public ComplPassagemCte GetElement(GDASession session, uint idCte)
        {
            try
            {
                return objPersistence.LoadOneData(session, Sql(idCte, true));
            }
            catch
            {
                return new ComplPassagemCte();
            }
        }

        public IList<ComplPassagemCte> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, false), null);
        }

        #endregion

        public void Delete(GDASession session, uint idCte, int numSeqPassagem)
        {
            string sql = "delete from compl_passagem_cte where IDCTE= " + idCte + " AND NUMSEQPASSAGEM= " + numSeqPassagem;
            objPersistence.ExecuteCommand(session, sql);
        }
    }
}
