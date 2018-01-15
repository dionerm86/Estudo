using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ParcelasCompraDAO : BaseDAO<ParcelasCompra, ParcelasCompraDAO>
    {
        //private ParcelasCompraDAO() { }

        private string Sql(uint idCompra, bool selecionar)
        {
            string campos = selecionar ? "p.*" : "Count(*)";

            string sql = "Select " + campos + " From parcelas_compra p " +
                "Where IdCompra=" + idCompra;

            return sql;
        }

        public IList<ParcelasCompra> GetByCompra(uint idCompra)
        {
            return GetByCompra(null, idCompra);
        }

        public IList<ParcelasCompra> GetByCompra(GDASession sessao, uint idCompra)
        {
            return objPersistence.LoadData(sessao, Sql(idCompra, true) + " Order By p.NumParc asc").ToList();
        }

        public IList<ParcelasCompra> GetList(uint idCompra, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "p.NumParc asc" : sortExpression;
            return LoadDataWithSortExpression(Sql(idCompra, true), sort, startRow, pageSize, null);
        }

        public int GetCount(uint idCompra)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idCompra, false), null);
        }

        public IList<ParcelasCompra> GetForRpt(uint idCompra)
        {
            return objPersistence.LoadData(Sql(idCompra, true)).ToList();
        }

        public void DeleteFromCompra(uint idCompra)
        {
            DeleteFromCompra(null, idCompra);
        }

        public void DeleteFromCompra(GDASession session, uint idCompra)
        {
            objPersistence.ExecuteCommand(session, "Delete From parcelas_compra Where IdCompra=" + idCompra);
        }

        public decimal ObtemTotalPorCompra(GDASession sessao, uint idCompra)
        {
            return ObtemValorCampo<decimal>(sessao, "Sum(Valor)", "idCompra=" + idCompra);
        }
    }
}
