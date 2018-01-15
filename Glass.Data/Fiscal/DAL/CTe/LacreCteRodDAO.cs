using System.Collections.Generic;
using Glass.Data.Model.Cte;
using GDA;

namespace Glass.Data.DAL.CTe
{
    public sealed class LacreCteRodDAO : BaseDAO<LacreCteRod, LacreCteRodDAO>
    {
        //private LacreCteRodDAO() { }

        #region Busca padrão

        private string Sql(uint idCte, bool selecionar)
        {
            string sql = "Select * From lacre_cte_rod Where 1";

            if (idCte > 0)
                sql += " And IDCTE=" + idCte;

            return sql;
        }

        public LacreCteRod GetElement(uint idCte)
        {
            try
            {
                return objPersistence.LoadOneData(Sql(idCte, true));
            }
            catch
            {
                return new LacreCteRod();
            }
        }

        public LacreCteRod GetElementNumConcatenado(uint idCte)
        {
            string numeroLacre = string.Empty;
            var lacres = objPersistence.LoadData(Sql(idCte, true)).ToList();
            if (lacres.Count > 0)
            {
                foreach (var i in lacres)
                {
                    numeroLacre = numeroLacre + ";" + i.NumeroLacre;
                }
                return new LacreCteRod { IdCte = idCte, NumeroLacre = numeroLacre };
            }
            return new LacreCteRod();
        }

        public IList<LacreCteRod> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, true), sortExpression, startRow, pageSize, null);
        }

        public List<LacreCteRod> GetLacresByIdCte(uint idCte)
        {
            return GetLacresByIdCte(null, idCte);
        }

        public List<LacreCteRod> GetLacresByIdCte(GDASession session, uint idCte)
        {
            return objPersistence.LoadData(session, Sql(idCte, true)).ToList();
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, false), null);
        }

        #endregion

        public void Delete(uint idCte)
        {
            Delete(null, idCte);
        }

        public void Delete(GDASession sessao, uint idCte)
        {
            string sql = "delete from lacre_cte_rod where IDCTE=" + idCte;
            objPersistence.ExecuteCommand(sessao, sql);
        }
    }
}
