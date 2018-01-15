using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.EFD;

namespace Glass.Data.DAL
{
    public sealed class AjusteApuracaoInfoAdicionalDAO : BaseDAO<AjusteApuracaoInfoAdicional, AjusteApuracaoInfoAdicionalDAO>
    {
        //private AjusteApuracaoInfoAdicionalDAO() { }

        private string Sql(ConfigEFD.TipoImpostoEnum tipoImposto, uint id, uint idABIA, bool selecionar)
        {
            string campos = selecionar ? "ai.*" : "count(*)";
            string sql = "select " + campos + @"
                from sped_ajuste_apuracao_info_ad ai
                    inner join sped_ajuste_beneficio_incentivo_apuracao ap on (ap.Id=ai.IdABIA)
                where 1";

            if ((int)tipoImposto > 0)
                sql += " and ai.TipoImposto=" + (int)tipoImposto;

            if (id > 0)
                sql += " and ai.Id=" + id;

            if (idABIA > 0)
                sql += " and ai.IdABIA = " + idABIA;

            return sql;
        }

        public IList<AjusteApuracaoInfoAdicional> GetList(ConfigEFD.TipoImpostoEnum tipoImposto, uint idABIA, string sortExpression, int startRow, int pageSize)
        {
            if (GetCount(tipoImposto, idABIA) == 0)
                return new AjusteApuracaoInfoAdicional[] { new AjusteApuracaoInfoAdicional() };

            return LoadDataWithSortExpression(Sql(tipoImposto, 0, idABIA, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount(ConfigEFD.TipoImpostoEnum tipoImposto, uint idABIA)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(tipoImposto, 0, idABIA, false));
        }

        public AjusteApuracaoInfoAdicional GetElement(ConfigEFD.TipoImpostoEnum tipoImposto, uint id)
        {
            List<AjusteApuracaoInfoAdicional> item = objPersistence.LoadData(Sql(tipoImposto, id, 0, true));
            return item.Count > 0 ? item[0] : null;
        }

        public List<AjusteApuracaoInfoAdicional> GetList(ConfigEFD.TipoImpostoEnum tipoImposto, uint idABIA)
        {
            return objPersistence.LoadData(Sql(tipoImposto, 0, idABIA, true));
        }

    }
}
