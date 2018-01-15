using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PagtoObraDAO : BaseDAO<PagtoObra, PagtoObraDAO>
    {
        //private PagtoObraDAO() { }

        private string Sql(uint idObra, bool selecionar)
        {
            string campos = selecionar ? "po.*, fp.descricao as FormaPagto" : "Count(*)";

            string sql = @"
                Select " + campos + @" 
                From pagto_obra po 
                    Left Join formapagto fp on (po.idFormaPagto=fp.idFormaPagto) 
                Where 1";

            if (idObra > 0)
                sql += " And po.idObra=" + idObra;

            return sql + " order by po.NumFormaPagto asc";
        }

        public IList<PagtoObra> GetByObra(uint idObra)
        {
            return GetByObra(null, idObra);
        }

        public IList<PagtoObra> GetByObra(GDASession session, uint idObra)
        {
            return objPersistence.LoadData(session, Sql(idObra, true)).ToList();
        }

        public void DeleteByObra(uint idObra)
        {
            DeleteByObra(null, idObra);
        }

        public void DeleteByObra(GDASession sessao, uint idObra)
        {
            objPersistence.ExecuteCommand(sessao, "delete from pagto_obra where idObra=" + idObra);
        }
    }
}
