using System.Collections.Generic;
using Glass.Data.Model.Cte;
using GDA;

namespace Glass.Data.DAL.CTe
{
    public sealed class ComponenteValorCteDAO : BaseDAO<ComponenteValorCte, ComponenteValorCteDAO>
    {
        //private ComponenteValorCteDAO() { }

        #region Busca padrão

        private string Sql(uint idCte, bool selecionar)
        {
            string sql = "Select * From componente_valor_cte Where 1";

            if (idCte > 0)
                sql += " And IDCTE=" + idCte;

            return sql;
        }

        public ComponenteValorCte GetElement(uint idCte)
        {
            try
            {
                return objPersistence.LoadOneData(Sql(idCte, true));
            }
            catch
            {
                return new ComponenteValorCte();
            }
        }

        public IList<ComponenteValorCte> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, true), sortExpression, startRow, pageSize, null);
        }

        public List<ComponenteValorCte> GetComponentesByIdCte(uint idCte)
        {
            return GetComponentesByIdCte(null, idCte);
        }

        public List<ComponenteValorCte> GetComponentesByIdCte(GDASession session, uint idCte)
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
            string sql = "delete from componente_valor_cte where IDCTE= " + idCte;
            objPersistence.ExecuteCommand(sessao, sql);
        }
    }
}
