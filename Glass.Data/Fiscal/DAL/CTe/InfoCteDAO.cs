﻿using System.Collections.Generic;
using Glass.Data.Model.Cte;
using GDA;

namespace Glass.Data.DAL.CTe
{
    public sealed class InfoCteDAO : BaseDAO<InfoCte, InfoCteDAO>
    {
        //private InfoCteDAO() { }

        #region Busca padrão

        private string Sql(uint idCte, bool selecionar)
        {
            string sql = "Select * From info_cte Where 1";
            
            if(!selecionar)
                sql = "Select count(*) From info_cte Where 1";

            if (idCte > 0)
                sql += " And IDCTE=" + idCte;

            return sql;
        }

        public InfoCte GetElement(uint idCte)
        {
            return GetElement(null, idCte);
        }

        public InfoCte GetElement(GDASession session, uint idCte)
        {
            try
            {
                return objPersistence.LoadOneData(session, Sql(idCte, true));
            }
            catch
            {
                return new InfoCte();
            }
        }

        public IList<InfoCte> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount(uint idCte)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idCte, false), null);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, false), null);
        }

        #endregion
    }
}
