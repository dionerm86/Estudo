using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model.Cte;
using GDA;

namespace Glass.Data.DAL.CTe
{
    public sealed class InfoCargaCteDAO : BaseDAO<InfoCargaCte, InfoCargaCteDAO>
    {
        //private InfoCargaCteDAO() { }

        #region Busca padrão

        private string Sql(uint idCte, bool selecionar)
        {
            string sql = "Select * From info_carga_cte Where 1";

            if (!selecionar)
                sql = "Select count(*) From info_carga_cte Where 1";

            if (idCte > 0)
                sql += " And IDCTE=" + idCte;

            return sql;
        }

        public InfoCargaCte GetElement(uint idCte)
        {
            try
            {
                return objPersistence.LoadOneData(Sql(idCte, true));
            }
            catch
            {
                return new InfoCargaCte();
            }
        }

        public InfoCargaCte[] GetList(uint idCte)
        {
            return GetList(null, idCte);
        }

        public InfoCargaCte[] GetList(GDASession session, uint idCte)
        {
            if (GetCount(session, idCte) > 0)
                return objPersistence.LoadData(session, Sql(idCte, true)).ToArray();

            return new InfoCargaCte[] { new InfoCargaCte() };
        }

        public IList<InfoCargaCte> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount(uint idCte)
        {
            return GetCount(null, idCte);
        }

        public int GetCount(GDASession session, uint idCte)
        {
            return objPersistence.ExecuteSqlQueryCount(session, Sql(idCte, false), null);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, false), null);
        }

        #endregion

        public int AtualizaInfoCargaCte(GDASession session, IEnumerable<InfoCargaCte> objUpdate)
        {
            try
            {
                string sql = "delete from info_carga_cte where IDCTE=" + objUpdate.Select(f => f.IdCte).FirstOrDefault();
                objPersistence.ExecuteCommand(session, sql);

                foreach (var i in objUpdate)
                    base.Insert(session, i);

                return 1;
            }
            catch
            {
                return 0;
            }            
        }

        public void Delete(GDASession session, uint idCte, int tipoUnidade)
        {
            string sql = "delete from info_carga_cte where IDCTE= " + idCte + " AND TIPOUNIDADE= " + tipoUnidade;
            objPersistence.ExecuteCommand(session, sql);
        }
    }
}
