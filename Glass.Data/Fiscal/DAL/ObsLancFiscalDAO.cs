using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ObsLancFiscalDAO : BaseDAO<ObsLancFiscal, ObsLancFiscalDAO>
    {
        //private ObsLancFiscalDAO() { }

        private string Sql(uint idNfAdd, uint idCteAdd, bool selecionar)
        {
            string campos = selecionar ? "olf.*" : "count(*)";
            string sql = "select " + campos + @"
                from obs_lanc_fiscal olf
                where 1";

            if (idNfAdd > 0)
                sql += @" and olf.idObsLancFiscal not in (
                    select idObsLancFiscal from obs_lanc_fiscal_nf where idNf=" + idNfAdd + ")";

            if (idCteAdd > 0)
                sql += @" and olf.idObsLancFiscal not in (
                    select idObsLancFiscal from obs_lanc_fiscal_cte where idCte=" + idCteAdd + ")";

            return sql;
        }

        public IList<ObsLancFiscal> GetList(uint idNfAdd, uint idCteAdd, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idNfAdd, idCteAdd) == 0)
                return new ObsLancFiscal[] { new ObsLancFiscal() };

            return LoadDataWithSortExpression(Sql(idNfAdd, idCteAdd, true), sortExpression, startRow, pageSize);
        }

        public int GetCount(uint idNfAdd, uint idCteAdd)
        {
            int real = GetCountReal(idNfAdd, idCteAdd);
            return real > 0 ? real : 1;
        }

        public int GetCountReal(uint idNfAdd, uint idCteAdd)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idNfAdd, idCteAdd, false));
        }

        #region Métodos sobrescritos

        public override int Delete(ObsLancFiscal objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdObsLancFiscal);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            if (objPersistence.ExecuteSqlQueryCount("select count(*) from obs_lanc_fiscal_nf where idObsLancFiscal=" + Key) > 0)
                throw new Exception("Essa observação está vinculada a uma ou mais notas fiscais e não pode ser excluída.");

            return GDAOperations.Delete(new ObsLancFiscal { IdObsLancFiscal = Key });
        }

        #endregion
    }
}
