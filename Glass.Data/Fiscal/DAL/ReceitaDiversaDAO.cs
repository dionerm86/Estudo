using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ReceitaDiversaDAO : BaseDAO<ReceitaDiversa, ReceitaDiversaDAO>
    {
        //private ReceitaDiversaDAO() { }

        #region Listagem padrão

        private string Sql(uint idReceita, string dataIni, string dataFim, bool selecionar)
        {
            string campos = selecionar ? @"r.*, Coalesce(l.NomeFantasia, l.RazaoSocial) As NomeLoja,
                pc.Descricao As DescricaoContaContabil, cc.Descricao As DescricaoCentroCusto" : "Count(*)";

            string sql = "Select " + campos + @" From receitas_diversas r 
                Left Join loja l On(l.IdLoja=r.IdLoja) 
                Left Join plano_conta_contabil pc On(pc.IdContaContabil=r.IdContaContabil) 
                Left Join centro_custo cc On(cc.IdCentroCusto=r.IdCentroCusto) 
                Where 1 ";

            if (idReceita > 0)
                sql += " And idReceita=" + idReceita;

            if (!String.IsNullOrEmpty(dataIni))
                sql += " and dataReceita >=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sql += " and dataReceita <=?dataFim";

            return sql;
        }

        public ReceitaDiversa GetElement(uint idReceita)
        {
            return objPersistence.LoadOneData(Sql(idReceita, null, null, true));
        }

        public IList<ReceitaDiversa> GetList(string dataIni, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, dataIni, dataFim, true),sortExpression, startRow, pageSize,
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
        public ReceitaDiversa[] GetForEFD(string idsLojas, DateTime inicio, DateTime fim)
        {
            string sql = @"select rd.*, pcc.codInterno as codInternoContaContabil from receitas_diversas rd 
                    left join plano_conta_contabil pcc on (rd.idContaContabil=pcc.idContaContabil)
                where rd.idLoja in (" + idsLojas + ") and rd.dataReceita>=?ini and rd.dataReceita<=?fim";
            return objPersistence.LoadData(sql, new GDAParameter("?ini", inicio.Date),
                new GDAParameter("?fim", fim.Date.AddDays(1).AddMilliseconds(-1))).ToList().ToArray();
        }

        #endregion

        public override int Delete(ReceitaDiversa objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdReceita);
        }
    }
}
