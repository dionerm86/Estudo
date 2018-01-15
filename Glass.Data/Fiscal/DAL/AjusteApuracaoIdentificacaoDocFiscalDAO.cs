using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.EFD;

namespace Glass.Data.DAL
{
    public sealed class AjusteApuracaoIdentificacaoDocFiscalDAO : BaseDAO<AjusteApuracaoIdentificacaoDocFiscal, AjusteApuracaoIdentificacaoDocFiscalDAO>
    {
        //private AjusteApuracaoIdentificacaoDocFiscalDAO() { }

        private GDAParameter[] GetParams(uint id, uint idNf, uint idProd, uint idABIA)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (id > 0)
                lst.Add(new GDAParameter("?id", id));

            if (idNf > 0)
                lst.Add(new GDAParameter("?idNf", idNf));

            if (idProd > 0)
                lst.Add(new GDAParameter("?idProd", idProd));

            if (idABIA > 0)
                lst.Add(new GDAParameter("?idABIA", idABIA));

            return lst.ToArray();
        }

        private string Sql(ConfigEFD.TipoImpostoEnum tipoImposto, uint id, uint idNf, uint idProd, uint idABIA, bool selecionar)
        {
            string campos = selecionar ? @"af.*, p.Descricao as DescricaoProduto, p.CodInterno as CodInternoProduto, n.NumeroNFE as NumeroNFE, 
                                        n.Serie as Serie, n.SubSerie as SubSerie, n.DataEmissao as DataEmissao,
                                        a.Codigo as CodigoAjuste, ap.Data as DataAjuste, ap.Obs as ObservacaoAjuste" : "count(*)";

            string sql = "select " + campos + @"
                from sped_ajuste_apuracao_ident_doc_fiscal af
                    inner join sped_ajuste_beneficio_incentivo_apuracao ap on (ap.Id=af.IdABIA)
                    inner join nota_fiscal n on(n.IdNf=af.IdNf)
                    inner join produtos_nf pnf on(pnf.IdNf=n.IdNf)
                    inner join produto p on(pnf.IdProd=p.IdProd and p.IdProd=af.IdProd)
                    inner join ajuste_beneficio_incentivo a on(a.IdAjBenInc=af.IdABIA)
                where 1";

            if ((int)tipoImposto > 0)
                sql += " and af.TipoImposto=" + (int)tipoImposto;

            if (id > 0)
                sql += " and af.Id=?id";

            if (idNf > 0)
                sql += " and n.IdNf=?idNf";

            if (idProd > 0)
                sql += " and p.IdProd=?idProd";

            if (idABIA > 0)
                sql += " and af.IdABIA = ?idABIA";

            return sql;
        }

        public IList<AjusteApuracaoIdentificacaoDocFiscal> GetList(ConfigEFD.TipoImpostoEnum tipoImposto, uint idNf, uint idProd, uint idABIA, string sortExpression, int startRow, int pageSize)
        {
            if (GetCount(tipoImposto, idNf, idProd, idABIA) == 0)
                return new AjusteApuracaoIdentificacaoDocFiscal[] { new AjusteApuracaoIdentificacaoDocFiscal() };

            return LoadDataWithSortExpression(Sql(tipoImposto, 0, idNf, idProd, idABIA, true), sortExpression, startRow, pageSize, GetParams(0, idNf, idProd, idABIA));
        }

        public int GetCount(ConfigEFD.TipoImpostoEnum tipoImposto, uint idNf, uint idProd, uint idABIA)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(tipoImposto, 0, idNf, idProd, idABIA, false), GetParams(0, idNf, idProd, idABIA));
        }

        public AjusteApuracaoIdentificacaoDocFiscal GetElement(ConfigEFD.TipoImpostoEnum tipoImposto, uint id)
        {
            List<AjusteApuracaoIdentificacaoDocFiscal> item = objPersistence.LoadData(Sql(tipoImposto, id, 0, 0, 0, true), GetParams(id, 0,0,0));
            return item.Count > 0 ? item[0] : null;
        }

        public override int Update(AjusteApuracaoIdentificacaoDocFiscal objUpdate)
        {
            int retorno = base.Update(objUpdate);

            return retorno;
        }

        public List<AjusteApuracaoIdentificacaoDocFiscal> GetList(ConfigEFD.TipoImpostoEnum tipoImposto, uint idABIA)
        {
            return objPersistence.LoadData(Sql(tipoImposto, 0, 0, 0, idABIA, true), GetParams(0, 0, 0, idABIA));
        }

    }
}
