using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProcessoAdministrativoDAO : BaseDAO<ProcessoAdministrativo, ProcessoAdministrativoDAO>
    {
        //private ProcessoAdministrativoDAO() { }

        private string Sql(string numeroProcesso, int natureza, DateTime? dataDecisaoIni, DateTime? dataDecisaoFim, bool selecionar)
        {
            string campos = selecionar ? "pa.*" : "count(*)";

            string sql = "select " + campos + @"
                from processos_administrativos pa
                where 1";

            if (!String.IsNullOrEmpty(numeroProcesso))
                sql += " and pj.numeroProcesso = ?numeroProcesso";

            if (natureza > 0)
                sql += " and pj.natureza = ?natureza";

            if (dataDecisaoIni != null)
                sql += " and pj.dataDecisao >= ?dataDecisaoIni";

            if (dataDecisaoFim != null)
                sql += " and pj.dataDecisao <= ?dataDecisaoFim";

            return sql;
        }

        private GDAParameter[] GetParams(string numeroProcesso, int natureza, DateTime? dataDecisaoIni, DateTime? dataDecisaoFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(numeroProcesso))
                lst.Add(new GDAParameter("?numeroProcesso", numeroProcesso));

            if (natureza > 0)
                lst.Add(new GDAParameter("?natureza", natureza));

            if (dataDecisaoIni != null)
                lst.Add(new GDAParameter("?dataDecisaoIni", dataDecisaoIni));

            if (dataDecisaoFim != null)
                lst.Add(new GDAParameter("?dataDecisaoFim", dataDecisaoFim));

            return lst.ToArray();
        }

        public IList<ProcessoAdministrativo> GetList(string numeroProcesso, int natureza, DateTime? dataDecisaoIni, DateTime? dataDecisaoFim, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(numeroProcesso, natureza, dataDecisaoIni, dataDecisaoFim) == 0)
                return new ProcessoAdministrativo[] { new ProcessoAdministrativo() };

            return LoadDataWithSortExpression(Sql(numeroProcesso, natureza, dataDecisaoIni, dataDecisaoFim, true), sortExpression, startRow, pageSize, GetParams(numeroProcesso, natureza, dataDecisaoIni, dataDecisaoFim));
        }

        public int GetCount(string numeroProcesso, int natureza, DateTime? dataDecisaoIni, DateTime? dataDecisaoFim)
        {
            int count = GetCountReal(numeroProcesso, natureza, dataDecisaoIni, dataDecisaoFim);
            return count > 0 ? count : 1;
        }

        public int GetCountReal(string numeroProcesso, int natureza, DateTime? dataDecisaoIni, DateTime? dataDecisaoFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(numeroProcesso, natureza, dataDecisaoIni, dataDecisaoFim, false), GetParams(numeroProcesso, natureza, dataDecisaoIni, dataDecisaoFim));
        }

        public ProcessoAdministrativo[] GetForEFD(DateTime inicio, DateTime fim)
        {
            return new ProcessoAdministrativo[0];
        }
    }
}
