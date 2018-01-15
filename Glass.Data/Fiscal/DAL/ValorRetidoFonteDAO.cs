using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ValorRetidoFonteDAO : BaseDAO<ValorRetidoFonte, ValorRetidoFonteDAO>
    {
        //private ValorRetidoFonteDAO() { }

        #region Listagem padrão

        private string Sql(uint idValorRetidoFonte, string dataIni, string dataFim, bool selecionar)
        {
            string campos = selecionar ? @"v.*, Coalesce(l.NomeFantasia, l.RazaoSocial) As NomeLoja" : "Count(*)";

            string sql = "Select " + campos + @" From valor_retido_fonte v 
                Left Join Loja l On(l.IdLoja=v.IdLoja) 
                Where 1 ";

            if (idValorRetidoFonte > 0)
                sql += " And idValorRetidoFonte=" + idValorRetidoFonte;

            if (!String.IsNullOrEmpty(dataIni))
                sql += " and dataRetencao >=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sql += " and dataRetencao <=?dataFim";

            return sql;
        }

        public ValorRetidoFonte GetElement(uint idValorRetidoFonte)
        {
            return objPersistence.LoadOneData(Sql(idValorRetidoFonte, null, null, true));
        }

        public IList<ValorRetidoFonte> GetList(string dataIni, string dataFim, string sortExpression, int startRow, int pageSize)
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
        public IList<ValorRetidoFonte> GetForEFD(string idsLojas, DateTime inicio, DateTime fim)
        {
            string sql = "select * from valor_retido_fonte where idLoja in (" + idsLojas + ") and dataRetencao>=?ini and dataRetencao<=?fim";
            return objPersistence.LoadData(sql, new GDAParameter("?ini", inicio.Date), 
                new GDAParameter("?fim", fim.Date.AddDays(1).AddMilliseconds(-1))).ToList();
        }

        #endregion

        public override int Delete(ValorRetidoFonte objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdValorRetidoFonte);
        }
    }
}
