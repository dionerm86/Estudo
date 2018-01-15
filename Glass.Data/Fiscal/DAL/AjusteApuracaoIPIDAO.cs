using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.EFD;

namespace Glass.Data.DAL
{
    public sealed class AjusteApuracaoIPIDAO : BaseDAO<AjusteApuracaoIPI, AjusteApuracaoIPIDAO>
    {
        //private AjusteApuracaoIPIDAO() { }

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

        private string Sql(ConfigEFD.TipoImpostoEnum tipoImposto, uint id, string dataInicio, string dataFim, bool selecionar)
        {
            string campos = selecionar ? "*" : "count(*)";
            string sql = "select " + campos + @"
                from sped_ajustes_apuracoes_ipi
                where 1";

            if ((int)tipoImposto > 0)
                sql += " and tipoImposto=" + (int)tipoImposto;

            if (!string.IsNullOrEmpty(dataInicio))
                sql += " and data >= ?dataIni";

            if (!string.IsNullOrEmpty(dataFim))
                sql += " and data <= ?dataFim";

            if (id > 0)
                sql += " and id=?id";

            sql += " order by tipoImposto ASC, data";

            return sql;
        }

        public int GetCount(ConfigEFD.TipoImpostoEnum tipoImposto, string dataInicio, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(tipoImposto, 0, dataInicio, dataFim, false), GetParam(0, dataInicio, dataFim));
        }

        public IList<AjusteApuracaoIPI> GetList(ConfigEFD.TipoImpostoEnum tipoImposto, string dataInicio, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            if (GetCount(tipoImposto, dataInicio, dataFim) == 0)
                return new AjusteApuracaoIPI[] { new AjusteApuracaoIPI() };

            return LoadDataWithSortExpression(Sql(tipoImposto, 0, dataInicio, dataFim, true), sortExpression, startRow, pageSize, GetParam(0, dataInicio, dataFim));
        }

        public AjusteApuracaoIPI GetElement(uint id)
        {
            var item = objPersistence.LoadData(Sql(0, id, null, null, true), GetParam(id, null, null)).ToList();
            return item.Count > 0 ? item[0] : null;
        }

        public List<AjusteApuracaoIPI> GetList(ConfigEFD.TipoImpostoEnum tipoImposto, string dataInicio, string dataFim)
        {
            return objPersistence.LoadData(Sql(tipoImposto, 0, dataInicio, dataFim, true), GetParam(0, dataInicio, dataFim));
        }

        public override int Update(AjusteApuracaoIPI objUpdate)
        {
            int retorno = base.Update(objUpdate);

            return retorno;
        }
    }
}
