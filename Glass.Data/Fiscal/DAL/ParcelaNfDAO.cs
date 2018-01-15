using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ParcelaNfDAO : BaseDAO<ParcelaNf, ParcelaNfDAO>
    {
        //private ParcelaNfDAO() { }

        private string Sql(uint idNf, bool selecionar)
        {
            var campos = selecionar ? "p.*" : "Count(*)";

            var sql = "Select " + campos + " From parcela_nf p " +
                "Where idNf=" + idNf;

            return sql;
        }

        public IList<ParcelaNf> GetByNf(uint idNf)
        {
            return GetByNf(null, idNf);
        }

        public IList<ParcelaNf> GetByNf(GDASession sessao, uint idNf)
        {
            return objPersistence.LoadData(sessao, Sql(idNf, true) + " Order By p.NumParcela asc").ToList();
        }

        public IList<ParcelaNf> GetList(uint idNf, string sortExpression, int startRow, int pageSize)
        {
            var sort = String.IsNullOrEmpty(sortExpression) ? "p.NumParcela asc" : sortExpression;
            return LoadDataWithSortExpression(Sql(idNf, true), sort, startRow, pageSize, null);
        }

        public int GetCount(uint idNf)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idNf, false), null);
        }

        public IList<ParcelaNf> GetForRpt(uint idNf)
        {
            return objPersistence.LoadData(Sql(idNf, true)).ToList();
        }

        public void DeleteFromNf(GDASession sessao, uint idNf)
        {
            objPersistence.ExecuteCommand(sessao, "Delete From parcela_nf Where IdNf=" + idNf);
        }

        /// <summary>
        /// Retorna o total somado das parcelas da nota
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public decimal GetTotal(uint idNf)
        {
            var sql = "Select Sum(Coalesce(valor, 0)) From parcela_nf Where idNf=" + idNf;
            return ExecuteScalar<decimal>(sql);
        }
    }
}
