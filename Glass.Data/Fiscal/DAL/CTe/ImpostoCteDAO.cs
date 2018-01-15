using System.Collections.Generic;
using Glass.Data.Model.Cte;
using Glass.Data.EFD;
using GDA;

namespace Glass.Data.DAL.CTe
{
    public sealed class ImpostoCteDAO : BaseDAO<ImpostoCte, ImpostoCteDAO>
    {
        //private ImpostoCteDAO() { }

        #region Busca padrão

        private string Sql(uint idCte, int tipoImposto, bool selecionar)
        {
            string sql = "Select * From imposto_cte Where 1";

            if (!selecionar)
                sql = "Select count(*) From imposto_cte Where 1";

            if (idCte > 0)
                sql += " And IDCTE=" + idCte;

            if (tipoImposto > 0)
                sql += " and tipoImposto=" + tipoImposto;

            return sql;
        }

        public ImpostoCte GetElement(uint idCte, DataSourcesEFD.TipoImpostoEnum tipoImposto)
        {
            try
            {
                return objPersistence.LoadOneData(Sql(idCte, (int)tipoImposto, true));
            }
            catch
            {
                return new ImpostoCte();
            }
        }

        public IList<ImpostoCte> GetList(uint idCte)
        {
            return GetList(null, idCte);
        }

        public IList<ImpostoCte> GetList(GDASession session, uint idCte)
        {
            return objPersistence.LoadData(session, Sql(idCte, 0, true)).ToList();
        }

        public IList<ImpostoCte> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, 0, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount(uint idCte)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idCte, 0, false), null);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, 0, false), null);
        }

        #endregion

        public void Delete(GDASession session, uint idCte, int tipoImposto)
        {
            string sql = "delete from imposto_cte where IDCTE= " + idCte + " AND tipoImposto= " + tipoImposto;
            objPersistence.ExecuteCommand(session, sql);
        }
    }
}
