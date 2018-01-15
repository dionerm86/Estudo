using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;
using GDA;
using Glass.Data.EFD;

namespace Glass.Data.DAL
{
    public sealed class AjusteContribuicaoDAO : BaseDAO<AjusteContribuicao, AjusteContribuicaoDAO>
    {
        //private AjusteContribuicaoDAO() { }

        private string Sql(int codCredCont, string dataIni, string dataFim, int fonteAjuste, int tipoImposto,
            float? aliqImposto, bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            StringBuilder sb = new StringBuilder();

            StringBuilder sql = new StringBuilder("select ");
            sql.Append(selecionar ? "ac.*" : "count(*)");
            sql.AppendFormat(@"
                from ajuste_contribuicao ac
                where 1 {0}", FILTRO_ADICIONAL);

            if (codCredCont > 0)
                sb.AppendFormat(" and ac.codCredCont={0}", codCredCont);

            if (!String.IsNullOrEmpty(dataIni))
                sb.Append(" and ac.dataAjuste>=?dataIni");

            if (!String.IsNullOrEmpty(dataFim))
                sb.Append(" and ac.dataAjuste<=?dataFim");

            if (fonteAjuste > 0)
                sb.AppendFormat(" and ac.fonteAjuste={0}", fonteAjuste);

            if (tipoImposto > 0)
                sb.AppendFormat(" and ac.tipoImposto={0}", tipoImposto);

            if (aliqImposto != null)
                sb.Append(" and ac.aliqImposto=?aliqImposto");

            filtroAdicional = sb.ToString();
            return sql.ToString();
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim, float? aliqImposto)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            if (aliqImposto != null)
                lst.Add(new GDAParameter("?aliqImposto", aliqImposto));

            return lst.ToArray();
        }

        public IList<AjusteContribuicao> GetList(int codCredCont, string dataIni, string dataFim, int fonteAjuste, int tipoImposto, 
            string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(codCredCont, dataIni, dataFim, fonteAjuste, tipoImposto) == 0)
                return new AjusteContribuicao[] { new AjusteContribuicao() };

            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "dataAjuste desc, idAjusteCont desc";

            bool temFiltro;
            string filtroAdicional;

            return LoadDataWithSortExpression(Sql(codCredCont, dataIni, dataFim, fonteAjuste, tipoImposto, null, true, out temFiltro,
                out filtroAdicional), sortExpression, startRow, pageSize, temFiltro, filtroAdicional, GetParams(dataIni, dataFim, null));
        }

        public int GetCount(int codCredCont, string dataIni, string dataFim, int fonteAjuste, int tipoImposto)
        {
            int retorno = GetCountReal(codCredCont, dataIni, dataFim, fonteAjuste, tipoImposto);
            return retorno > 0 ? retorno : 1;
        }

        public int GetCountReal(int codCredCont, string dataIni, string dataFim, int fonteAjuste, int tipoImposto)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = Sql(codCredCont, dataIni, dataFim, fonteAjuste, tipoImposto, null, false, out temFiltro, out filtroAdicional).
                Replace(FILTRO_ADICIONAL, filtroAdicional);

            return objPersistence.ExecuteSqlQueryCount(sql, GetParams(dataIni, dataFim, null));
        }

        public IList<AjusteContribuicao> GetForEFD(int codCredCont, DateTime inicio, DateTime fim, Sync.Fiscal.Enumeracao.AjusteContribuicao.FonteAjuste fonteAjuste,
            DataSourcesEFD.TipoImpostoEnum tipoImposto, float aliqImposto)
        {
            string di = inicio.ToString("dd/MM/yyyy"), df = fim.ToString("dd/MM/yyyy");

            bool temFiltro;
            string filtroAdicional;

            string sql = Sql(codCredCont, di, df, (int)fonteAjuste, (int)tipoImposto, aliqImposto, true, 
                out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional);

            return objPersistence.LoadData(sql, GetParams(di, df, aliqImposto)).ToList();
        }
    }
}
