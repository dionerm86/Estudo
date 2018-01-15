using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ProcessoReferenciadoDAO : BaseDAO<ProcessoReferenciado, ProcessoReferenciadoDAO>
    {
        //private ProcessoReferenciadoDAO() { }

        private string Sql(uint idNf, uint idCte, bool selecionar)
        {
            string campos = selecionar ? "pr.*" : "count(*)";
            string sql = "select " + campos + @"
                from processo_referenciado pr
                where 1";

            if (idNf > 0)
                sql += " and pr.idNf=" + idNf;

            if (idCte > 0)
                sql += " and pr.idCte=" + idCte;

            return sql;
        }

        public IList<ProcessoReferenciado> GetList(uint idNf, uint idCte, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idNf, idCte) == 0)
                return new ProcessoReferenciado[] { new ProcessoReferenciado() };

            return LoadDataWithSortExpression(Sql(idNf, idCte, true), sortExpression, startRow, pageSize);
        }

        public int GetCount(uint idNf, uint idCte)
        {
            int retorno = GetCountReal(idNf, idCte);
            return retorno > 0 ? retorno : 1;
        }

        public int GetCountReal(uint idNf, uint idCte)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idNf, idCte, false));
        }

        public IList<ProcessoReferenciado> GetForEFD(uint idNf, uint idCte)
        {
            return objPersistence.LoadData(Sql(idNf, idCte, true)).ToList();
        }
    }
}
