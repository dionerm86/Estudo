using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class DeducaoDiversaDAO : BaseDAO<DeducaoDiversa, DeducaoDiversaDAO>
    {
        //private DeducaoDiversaDAO() { }

        #region Listagem padrão

        private string Sql(uint idDeducao, string dataIni, string dataFim, bool selecionar)
        {
            string campos = selecionar ? @"d.*, Coalesce(l.NomeFantasia, l.RazaoSocial) As NomeLoja" : "Count(*)";

            string sql = "Select " + campos + @" From deducoes_diversas d 
                Left Join Loja l On(l.IdLoja=d.IdLoja) 
                Where 1 ";

            if (idDeducao > 0)
                sql += " And idDeducao=" + idDeducao;

            if (!String.IsNullOrEmpty(dataIni))
                sql += " and dataDeducao >=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sql += " and dataDeducao <=?dataFim";

            return sql;
        }

        public DeducaoDiversa GetElement(uint idDeducao)
        {
            return objPersistence.LoadOneData(Sql(idDeducao, null, null, true));
        }

        public IList<DeducaoDiversa> GetList(string dataIni, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, dataIni, dataFim, true), sortExpression, startRow, pageSize,
               GetParam(dataIni, dataFim));
        }

        public int GetListCount(string dataIni, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, dataIni, dataFim, false), GetParam(dataIni, dataFim));
        }

        private GDAParameter[] GetParam(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca para EFD

        /// <summary>
        /// Busca para EFD.
        /// </summary>
        /// <param name="idsLojas"></param>
        /// <param name="inicio"></param>
        /// <param name="fim"></param>
        /// <returns></returns>
        public IList<DeducaoDiversa> GetForEFD(string idsLojas, DateTime inicio, DateTime fim)
        {
            string sql = "select * from deducoes_diversas where idLoja in (" + idsLojas + ") and dataDeducao>=?ini and dataDeducao<=?fim";
            return objPersistence.LoadData(sql, new GDAParameter("?ini", inicio.Date),
                new GDAParameter("?fim", fim.Date.AddDays(1).AddMilliseconds(-1))).ToList();
        }

        #endregion

        public override int Delete(DeducaoDiversa objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdDeducao);
        }
    }
}
