using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.EFD;

namespace Glass.Data.DAL
{
    public sealed class AjusteBeneficioIncentivoApuracaoDAO : BaseDAO<AjusteBeneficioIncentivoApuracao, AjusteBeneficioIncentivoApuracaoDAO>
    {
        //private AjusteBeneficioIncentivoApuracaoDAO() { }

        #region Busca padrão

        private GDAParameter[] GetParam(uint id, string dataIni, string dataFim, string uf)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if(id > 0)
                lstParam.Add(new GDAParameter("?id", id));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            if (!String.IsNullOrEmpty(uf))
                lstParam.Add(new GDAParameter("?uf", uf));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        private string Sql(ConfigEFD.TipoImpostoEnum tipoImposto, uint id, string dataInicio, string dataFim, string uf, bool selecionar)
        {
            string campos = selecionar ? "ap.*, a.Codigo, concat_ws( ' - ', a.Codigo, a.Descricao) as CodigoDescricao" : "count(*)";
            string sql = "select " + campos + @"
                from sped_ajuste_beneficio_incentivo_apuracao ap
                    left join ajuste_beneficio_incentivo a on (ap.IDAJBENINC=a.IDAJBENINC)
                where 1";

            if((int)tipoImposto > 0)
                sql += " and ap.TipoImposto=" + (int)tipoImposto;

            if (!string.IsNullOrEmpty(dataInicio))
                sql += " and ap.Data >= ?dataIni";

            if (!string.IsNullOrEmpty(dataFim))
                sql += " and ap.Data <= ?dataFim";

            if (!String.IsNullOrEmpty(uf))
                sql += " and ap.uf=?uf";

            if (id > 0)
                sql += " and ap.Id=?id";

            sql += " order by ap.TipoImposto ASC";

            return sql;
        }

        public IList<AjusteBeneficioIncentivoApuracao> GetList(ConfigEFD.TipoImpostoEnum tipoImposto, string dataInicio, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            if (GetCount(tipoImposto, dataInicio, dataFim) == 0)
                return new AjusteBeneficioIncentivoApuracao[] { new AjusteBeneficioIncentivoApuracao() };

            return LoadDataWithSortExpression(Sql(tipoImposto, 0, dataInicio, dataFim, null, true), sortExpression, startRow, pageSize,
                GetParam(0, dataInicio, dataFim, null));
        }

        public int GetCount(ConfigEFD.TipoImpostoEnum tipoImposto, string dataInicio, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(tipoImposto, 0, dataInicio, dataFim, null, false),
                GetParam(0, dataInicio, dataFim, null));
        }

        public AjusteBeneficioIncentivoApuracao GetElement(ConfigEFD.TipoImpostoEnum tipoImposto, uint id)
        {
            List<AjusteBeneficioIncentivoApuracao> item = objPersistence.LoadData(Sql(tipoImposto, id, null, null, null, true),
                GetParam(id, null, null, null));

            return item.Count > 0 ? item[0] : null;
        }

        #endregion

        public IList<AjusteBeneficioIncentivoApuracao> ObterLista(ConfigEFD.TipoImpostoEnum tipoImposto)
        {
            return objPersistence.LoadData(Sql(tipoImposto, 0, null, null, null, true)).ToList();
        }

        public List<AjusteBeneficioIncentivoApuracao> GetList(ConfigEFD.TipoImpostoEnum tipoImposto, string dataInicio, string dataFim, string uf)
        {
            return objPersistence.LoadData(Sql(tipoImposto, 0, dataInicio, dataFim, uf, true), GetParam(0, dataInicio, dataFim, uf));
        }
    }
}
