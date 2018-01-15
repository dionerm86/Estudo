using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProcessoJudicialDAO : BaseDAO<ProcessoJudicial, ProcessoJudicialDAO>
    {
        //private ProcessoJudicialDAO() { }

        private string Sql(string numeroProcesso, int natureza, string secaoJudiciaria, string vara, string descricao, DateTime? dataDecisaoIni, DateTime? dataDecisaoFim, bool selecionar)
        {
            string campos = selecionar ? "pj.*" : "count(*)";

            string sql = "select " + campos + @"
                from processos_judiciais pj
                where 1";

            if (!String.IsNullOrEmpty(numeroProcesso))
                sql += " and pj.numeroProcesso = ?numeroProcesso";

            if (natureza > 0)
                sql += " and pj.natureza = ?natureza";

            if (!String.IsNullOrEmpty(secaoJudiciaria))
                sql += " and pj.secaoJudiciaria = ?secaoJudiciaria";

            if (!String.IsNullOrEmpty(vara))
                sql += " and pj.vara = ?vara";

            if (!String.IsNullOrEmpty(descricao))
                sql += " and pj.descricao like ?descricao";
            
            if(dataDecisaoIni != null)
                sql += " and pj.dataDecisao >= ?dataDecisaoIni";

            if (dataDecisaoFim != null)
                sql += " and pj.dataDecisao <= ?dataDecisaoFim";

            return sql;
        }

        private GDAParameter[] GetParams(string numeroProcesso, int natureza, string secaoJudiciaria, string vara, string descricao, DateTime? dataDecisaoIni, DateTime? dataDecisaoFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(numeroProcesso))
                lst.Add(new GDAParameter("?numeroProcesso", numeroProcesso));

            if (natureza > 0)
                lst.Add(new GDAParameter("?natureza", natureza));

            if (!String.IsNullOrEmpty(secaoJudiciaria))
                lst.Add(new GDAParameter("?secaoJudiciaria", secaoJudiciaria));

            if (!String.IsNullOrEmpty(vara))
                lst.Add(new GDAParameter("?vara", vara));

            if (!String.IsNullOrEmpty(descricao))
                lst.Add(new GDAParameter("?descricao", "%" + descricao + "%"));

            if (dataDecisaoIni != null)
                lst.Add(new GDAParameter("?dataDecisaoIni", dataDecisaoIni));

            if (dataDecisaoFim != null)
                lst.Add(new GDAParameter("?dataDecisaoFim", dataDecisaoFim));

            return lst.ToArray();
        }

        public IList<ProcessoJudicial> GetList(string numeroProcesso, int natureza, string secaoJudiciaria, string vara, string descricao, DateTime? dataDecisaoIni, DateTime? dataDecisaoFim, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(numeroProcesso, natureza, secaoJudiciaria, vara, descricao, dataDecisaoIni, dataDecisaoFim) == 0)
                return new ProcessoJudicial[] { new ProcessoJudicial() };

            return LoadDataWithSortExpression(Sql(numeroProcesso, natureza, secaoJudiciaria, vara, descricao, dataDecisaoIni, dataDecisaoFim, true), sortExpression, startRow, pageSize, GetParams(numeroProcesso, natureza, secaoJudiciaria, vara, descricao, dataDecisaoIni, dataDecisaoFim));
        }

        public int GetCount(string numeroProcesso, int natureza, string secaoJudiciaria, string vara, string descricao, DateTime? dataDecisaoIni, DateTime? dataDecisaoFim)
        {
            int count = GetCountReal(numeroProcesso, natureza, secaoJudiciaria, vara, descricao, dataDecisaoIni, dataDecisaoFim);
            return count > 0 ? count : 1;
        }

        public int GetCountReal(string numeroProcesso, int natureza, string secaoJudiciaria, string vara, string descricao, DateTime? dataDecisaoIni, DateTime? dataDecisaoFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(numeroProcesso, natureza, secaoJudiciaria, vara, descricao, dataDecisaoIni, dataDecisaoFim, false), GetParams(numeroProcesso, natureza, secaoJudiciaria, vara, descricao, dataDecisaoIni, dataDecisaoFim));
        }

        public ProcessoJudicial[] GetForEFD(DateTime inicio, DateTime fim)
        {
            return new ProcessoJudicial[0];
        }
    }
}
