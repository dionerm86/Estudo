using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.EFD;

namespace Glass.Data.DAL
{
    public sealed class ObrigacaoRecolhidoRecolherDAO : BaseDAO<ObrigacaoRecolhidoRecolher, ObrigacaoRecolhidoRecolherDAO>
    {
        //private ObrigacaoRecolhidoRecolherDAO() { }

        private GDAParameter[] GetParam(uint id, string dataIni, string dataFim)
        {
            var lstParam = new List<GDAParameter>();

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
            string campos = selecionar ? "o.*, t.Descricao as DescricaoCodigoReceita" : "count(*)";
            string sql = "select " + campos + @"
                from sped_obrigacoes_recolhido_recolher o
                left join sped_tabela_codigos_receita t on(t.Codigo=o.Cod_Rec)
                where 1";

            if ((int)tipoImposto > 0)
                sql += " and o.TipoImposto=" + (int)tipoImposto;

            if (!string.IsNullOrEmpty(dataInicio))
                sql += " and o.DT_VCTO >= ?dataIni";

            if (!string.IsNullOrEmpty(dataFim))
                sql += " and o.DT_VCTO <= ?dataFim";

            if (id > 0)
                sql += " and o.Id=?id";

            return sql;
        }

        public int GetCount(ConfigEFD.TipoImpostoEnum tipoImposto, string dataInicio, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(tipoImposto, 0, dataInicio, dataFim, false), GetParam(0, dataInicio, dataFim));
        }

        public IList<ObrigacaoRecolhidoRecolher> GetList(ConfigEFD.TipoImpostoEnum tipoImposto, string dataInicio, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            if (GetCount(tipoImposto, dataInicio, dataFim) == 0)
                return new List<ObrigacaoRecolhidoRecolher>();

            return LoadDataWithSortExpression(Sql(tipoImposto, 0, dataInicio, dataFim, true), sortExpression, startRow, pageSize, GetParam(0, dataInicio, dataFim));
        }

        public ObrigacaoRecolhidoRecolher GetElement(uint id)
        {
            List<ObrigacaoRecolhidoRecolher> item = objPersistence.LoadData(Sql(0, id, null, null, true), GetParam(id, null, null));
            return item.Count > 0 ? item[0] : null;
        }

        public List<ObrigacaoRecolhidoRecolher> GetList(ConfigEFD.TipoImpostoEnum tipoImposto, string dataInicio, string dataFim)
        {
            return objPersistence.LoadData(Sql(tipoImposto, 0, dataInicio, dataFim, true), GetParam(0, dataInicio, dataFim));
        }
    }
}
