using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PagtoPagtoDAO : BaseDAO<PagtoPagto, PagtoPagtoDAO>
    {
        //private PagtoPagtoDAO() { }

        private string Sql(uint idPagto, bool selecionar)
        {
            string campos = selecionar ? "pp.*, if(pp.idFormaPagto!=" + (uint)Glass.Data.Model.Pagto.FormaPagto.Obra + ", fp.descricao, 'Obra') as DescrFormaPagto" : "Count(*)";
            string sql = "select " + campos + " from pagto_pagto pp " +
                "left join formapagto fp on (pp.idFormaPagto=fp.idFormaPagto) " +
                "where 1";

            if (idPagto > 0)
                sql += " And pp.idPagto=" + idPagto;

            return sql + " order by pp.NumFormaPagto asc";
        }

        public IList<PagtoPagto> GetByPagto(uint idPagto)
        {
            return GetByPagto(null, idPagto);
        }

        public IList<PagtoPagto> GetByPagto(GDASession session, uint idPagto)
        {
            return objPersistence.LoadData(session, Sql(idPagto, true)).ToList();
        }

        public void DeleteByPagto(uint idPagto)
        {
            objPersistence.ExecuteCommand("delete from pagto_pagto where idPagto=" + idPagto);
        }
    }
}
