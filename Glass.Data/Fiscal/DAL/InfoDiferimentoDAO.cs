using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class InfoDiferimentoDAO : BaseDAO<InfoDiferimento, InfoDiferimentoDAO>
    {
        //private InfoDiferimentoDAO() { }

        private string Sql(uint idInfoDiferimento, bool selecionar, out string filtroAdicional)
        {
            StringBuilder sql = new StringBuilder("select ");
            sql.Append(selecionar ? @"id.*" : 
                "count(distinct id.idInfoDiferimento)");

            sql.AppendFormat(@"
                from info_diferimento id
                where 1 {0}
                {1}", FILTRO_ADICIONAL, selecionar ? "group by id.idInfoDiferimento" : String.Empty);

            StringBuilder fa = new StringBuilder();

            if (idInfoDiferimento > 0)
                fa.AppendFormat(" and id.idInfoDiferimento={0}", idInfoDiferimento);

            filtroAdicional = fa.ToString();
            return sql.ToString();
        }

        public IList<InfoDiferimento> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
                return new[] { new InfoDiferimento() };

            string filtroAdicional, sql = Sql(0, true, out filtroAdicional);
            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, filtroAdicional);
        }

        public int GetCount()
        {
            int count = GetCountReal();
            return count > 0 ? count : 1;
        }

        public int GetCountReal()
        {
            string filtroAdicional, sql = Sql(0, true, out filtroAdicional);
            return GetCountWithInfoPaging(sql, false, filtroAdicional);
        }

        public InfoDiferimento GetElement(uint idInfoDiferimento)
        {
            string filtroAdicional, sql = Sql(idInfoDiferimento, true, out filtroAdicional);
            return objPersistence.LoadOneData(sql.Replace(FILTRO_ADICIONAL, filtroAdicional));
        }

        #region Busca para EFD

        public IList<InfoDiferimento> GetForEFD(DateTime inicio, DateTime fim, int codCont, float aliqImposto, 
            int tipoImposto)
        {
            string sql = @"
                select * from info_diferimento
                where data>=?inicio and data<=?fim and aliqImposto=?aliq
                    and codCont=" + codCont + " and tipoImposto=" + tipoImposto;

            return objPersistence.LoadData(sql, new GDAParameter("?inicio", inicio.Date),
                new GDAParameter("?fim", fim.Date.AddDays(1).AddMilliseconds(-1)),
                new GDAParameter("?aliq", aliqImposto)).ToList();
        }

        #endregion
    }
}
