using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class AjusteApuracaoValorDeclaratorioDAO : BaseDAO<AjusteApuracaoValorDeclaratorio, AjusteApuracaoValorDeclaratorioDAO>
    {
        //private AjusteApuracaoValorDeclaratorioDAO() { }

        private GDAParameter[] GetParam(uint id, string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (id > 0)
                lstParam.Add(new GDAParameter("?id", id));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        private string Sql(uint id, string dataInicio, string dataFim, bool selecionar)
        {
            string campos = selecionar ? "av.*, a.Codigo, a.uf, concat_ws(' - ', a.Codigo, a.Descricao) as CodigoDescricao" : "count(*)";
            string sql = "select " + campos + @"
                from sped_ajuste_apuracao_valores_declaratorios av
                    left join ajuste_beneficio_incentivo a on (av.IDAJBENINC=a.IDAJBENINC)
                where 1";

            if (id > 0)
                sql += " and av.Id=?id";

            if (!string.IsNullOrEmpty(dataInicio))
                sql += " and av.Data >= ?dataIni";

            if (!string.IsNullOrEmpty(dataFim))
                sql += " and av.Data <= ?dataFim";

            return sql;
        }

        public int GetCount(string dataInicio, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, dataInicio, dataFim, false), GetParam(0, dataInicio, dataFim));
        }

        public IList<AjusteApuracaoValorDeclaratorio> GetList(string dataInicio, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            if (GetCount(dataInicio, dataFim) == 0)
                    return new AjusteApuracaoValorDeclaratorio[] { new AjusteApuracaoValorDeclaratorio() };

            return LoadDataWithSortExpression(Sql(0, dataInicio, dataFim, true), sortExpression, startRow, pageSize, GetParam(0, dataInicio, dataFim));
        }

        public AjusteApuracaoValorDeclaratorio GetElement(uint id)
        {
            List<AjusteApuracaoValorDeclaratorio> item = objPersistence.LoadData(Sql(id, null, null, true), GetParam(id, null, null));
            return item.Count > 0 ? item[0] : null;
        }

        public List<AjusteApuracaoValorDeclaratorio> GetList(string dataInicio, string dataFim)
        {
            return objPersistence.LoadData(Sql(0, dataInicio, dataFim, true), GetParam(0, dataInicio, dataFim));
        }
    }
}
