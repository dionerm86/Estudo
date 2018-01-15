using System;
using System.Collections.Generic;
using Glass.Data.Model.Cte;
using GDA;

namespace Glass.Data.DAL.CTe
{
    public sealed class NotaFiscalCteDAO : BaseDAO<NotaFiscalCte, NotaFiscalCteDAO>
    {
        //private NotaFiscalCteDAO() { }

        #region Busca padrão

        private string Sql(uint idCte, uint idNf, bool selecionar)
        {
            string sql = "Select * From nfe_cte Where 1";

            if (!selecionar)
            {
                sql = @"Select Count(*) From nfe_cte
                    Where 1";
            }

            if (idCte > 0)
                sql += " And IDCTE=" + idCte;
            if (idNf > 0)
                sql += " And IDNF=" + idNf;

            return sql;
        }

        public NotaFiscalCte GetElement(uint idCte, uint idNf)
        {
            try
            {
                return objPersistence.LoadOneData(Sql(idCte, idNf, true));
            }
            catch
            {
                return new NotaFiscalCte();
            }
        }

        public List<NotaFiscalCte> GetList(uint idCte)
        {
            return objPersistence.LoadData(Sql(idCte, 0, true));
        }

        public IList<NotaFiscalCte> GetList(uint idCte, string sortExpression, int startRow, int pageSize)
        {            
            string sort = String.IsNullOrEmpty(sortExpression) ? " Order By IdNf asc" : "";

            return LoadDataWithSortExpression(Sql(idCte, 0,true) + sort, sortExpression, startRow, pageSize, null);
        }

        public int GetCount(uint idCte)
        {
            return GetCount(null, idCte);
        }

        public int GetCount(GDASession session, uint idCte)
        {
            return objPersistence.ExecuteSqlQueryCount(session, Sql(idCte, 0, false), null);
        }
               
        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, 0, false), null);
        }

        #endregion

        public override uint Insert(NotaFiscalCte objInsert)
        {
            uint idCte = base.Insert(objInsert);

            return idCte;
        }

        public override int Update(NotaFiscalCte objUpdate)
        {
            return base.Update(objUpdate);
        }

        public override int Delete(NotaFiscalCte nfCte)
        {
            return base.Delete(nfCte);
        }

        public void DeleteByIdCte(GDASession session, uint idCte)
        {
            objPersistence.ExecuteCommand(session, "delete from nfe_cte where idCte=" + idCte);
        }
    }
}
