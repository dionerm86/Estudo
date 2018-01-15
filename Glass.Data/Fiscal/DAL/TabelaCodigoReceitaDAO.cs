using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class TabelaCodigoReceitaDAO : BaseDAO<TabelaCodigoReceita, TabelaCodigoReceitaDAO>
    {
        //private TabelaCodigoReceitaDAO() { }

        private GDAParameter[] GetParam(uint codigo, string descricao, string uf, string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (codigo > 0)
                lstParam.Add(new GDAParameter("?codigo", codigo));

            if (!string.IsNullOrEmpty(descricao))
                lstParam.Add(new GDAParameter("?descricao", "%" + descricao.Replace('%', ' ') + "%"));

            if (!string.IsNullOrEmpty(uf))
                lstParam.Add(new GDAParameter("?uf", uf));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        private string Sql(uint codigo, string descricao, string uf, string dataIni, string dataFim, bool selecionar)
        {
            string campos = selecionar ? "*" : "count(*)";
            string sql = "select " + campos + "from sped_tabela_codigos_receita where 1";

            if (codigo > 0)
                sql += " and Codigo=?codigo";

            if (!string.IsNullOrEmpty(descricao))
                sql += " and Descricao like ?descricao";

            if (!string.IsNullOrEmpty(uf))
                sql += " and UF=?uf";

            if (!String.IsNullOrEmpty(dataIni))
                sql += " and Data >=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sql += " and Data <=?dataFim";

            return sql;
        }

        public int GetCount(uint codigo, string descricao, string uf, string dataIni, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(codigo, descricao, uf, dataIni, dataFim, false), GetParam(codigo, descricao, uf, dataIni, dataFim));
        }

        public IList<TabelaCodigoReceita> GetList(string descricao, string uf, string dataIni, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            if (GetCount(0, descricao, uf, dataIni, dataFim) == 0)
                return new TabelaCodigoReceita[] { new TabelaCodigoReceita() };

            return LoadDataWithSortExpression(Sql(0, descricao, uf, dataIni, dataFim, true), sortExpression, startRow, pageSize, GetParam(0, descricao, uf, dataIni, dataFim));
        }

        public TabelaCodigoReceita GetElement(uint codigo)
        {
            List<TabelaCodigoReceita> item = objPersistence.LoadData(Sql(codigo, null, null, null, null, true), GetParam(codigo, null, null, null, null));
            return item.Count > 0 ? item[0] : null;
        }

    }
}
