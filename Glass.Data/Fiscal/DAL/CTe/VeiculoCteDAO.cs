using System.Collections.Generic;
using Glass.Data.Model.Cte;
using GDA;

namespace Glass.Data.DAL.CTe
{
    public sealed class VeiculoCteDAO : BaseDAO<VeiculoCte, VeiculoCteDAO>
    {
        //private VeiculoCteDAO() { }

        #region Busca padrão

        private string Sql(uint idCte, bool selecionar)
        {
            string sql = "Select * From veiculo_cte Where 1";

            if (idCte > 0)
                sql += " And IDCTE=" + idCte;

            return sql;
        }

        public VeiculoCte GetElement(uint idCte)
        {
            try
            {
                return objPersistence.LoadOneData(Sql(idCte, true));
            }
            catch
            {
                return new VeiculoCte();
            }
        }

        public IList<VeiculoCte> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, true), sortExpression, startRow, pageSize, null);
        }

        public List<VeiculoCte> GetVeiculosCteByIdCte(uint idCte)
        {
            return GetVeiculosCteByIdCte(null, idCte);
        }

        public List<VeiculoCte> GetVeiculosCteByIdCte(GDASession session, uint idCte)
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
            string sql = "delete from veiculo_cte where IDCTE=" + idCte;
            objPersistence.ExecuteCommand(sessao, sql);
        }
    }
}
