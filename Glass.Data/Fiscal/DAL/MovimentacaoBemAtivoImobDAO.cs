using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class MovimentacaoBemAtivoImobDAO : BaseDAO<MovimentacaoBemAtivoImob, MovimentacaoBemAtivoImobDAO>
    {
        //private MovimentacaoBemAtivoImobDAO() { }

        #region Busca padrão

        private string Sql(uint idProdNf, string idsLojas, string dataIni, string dataFim, bool apenasCadastrados, bool forEfd, bool selecionar)
        {
            string campoData = "coalesce(if(nf.tipoDocumento<>" + (int)NotaFiscal.TipoDoc.Saída + ", nf.dataSaidaEnt, null), nf.dataEmissao)";
            string campos = selecionar ? "pnf.idProdNf, bai.idBemAtivoImobilizado, cast(max(" + campoData + @") as datetime) as data, mbai.tipo, 
                pnf.valorIcms, pnf.valorIcmsSt, cast(sum(mbai.valorIcmsFrete) as decimal(12,2)) as valorIcmsFrete, cast(sum(mbai.valorIcmsDif) as decimal(12,2)) as valorIcmsDif, 
                cast(sum(mbai.numParcIcms) as signed) as numParcIcms, cast(sum(mbai.valorParcIcms) as decimal(12,2)) as valorParcIcms,
                nf.tipoDocumento as tipoNFe, nf.numeroNFe, p.descricao as descrProd, cast(group_concat(distinct pnf.idNf) as char) as idNf" : "count(*)";

            string sql = "select " + campos + @"
                from produtos_nf pnf
                    inner join nota_fiscal nf on (pnf.idNf=nf.idNf)
                    inner join bem_ativo_imobilizado bai on (pnf.idProd=bai.idProd)
                    inner join produto p on (pnf.idProd=p.idProd)
                    " + (apenasCadastrados ? "inner" : "left") + @" join movimentacao_bem_ativo_imob mbai on (pnf.idProdNf=mbai.idProdNf)
                where nf.situacao in (" + (int)NotaFiscal.SituacaoEnum.Autorizada + "," + (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros + @")
                    and nf.tipoDocumento<>" + (int)NotaFiscal.TipoDoc.NotaCliente;

            if (idProdNf > 0)
                sql += " and pnf.idProdNf=" + idProdNf;

            if (!String.IsNullOrEmpty(idsLojas) && idsLojas != "0")
                sql += " and nf.idLoja in (" + idsLojas + ")";

            if (!String.IsNullOrEmpty(dataIni))
                sql += " and " + campoData + ">=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sql += " and " + campoData + "<=?dataFim";

            sql += " group by " + (!forEfd ? "pnf.idProdNf" : "bai.idBemAtivoImobilizado, mbai.tipo");

            if (!selecionar)
                sql = "select count(*) from (" + sql + ") as temp";

            return sql;
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lst.ToArray();
        }

        public IList<MovimentacaoBemAtivoImob> GetList(string dataIni, string dataFim, bool apenasCadastrados, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "data desc";
            return LoadDataWithSortExpression(Sql(0, null, dataIni, dataFim, apenasCadastrados, false, true),
                sortExpression, startRow, pageSize, GetParams(dataIni, dataFim));
        }

        public int GetCount(string dataIni, string dataFim, bool apenasCadastrados)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, null, dataIni, dataFim, apenasCadastrados, false, 
                false), GetParams(dataIni, dataFim));
        }

        public MovimentacaoBemAtivoImob GetElement(uint idProdNf)
        {
            return GetElement(null, idProdNf);
        }

        public MovimentacaoBemAtivoImob GetElement(GDASession session, uint idProdNf)
        {
            var prod = objPersistence.LoadData(session, Sql(idProdNf, null, null, null, true, false, true)).ToList();
            return prod.Count> 0 ? prod[0] : null;
        }

        public IList<MovimentacaoBemAtivoImob> GetForEFD(string idsLojas, DateTime dataIni, DateTime dataFim)
        {
            string dtIni = dataIni.ToString("dd/MM/yyyy"), dtFim = dataFim.ToString("dd/MM/yyyy");
            return objPersistence.LoadData(Sql(0, idsLojas, dtIni, dtFim, true, true, true), GetParams(dtIni, dtFim)).ToList();
        }

        public int GetCountForEFD(string idsLojas, DateTime dataIni, DateTime dataFim)
        {
            string dtIni = dataIni.ToString("dd/MM/yyyy"), dtFim = dataFim.ToString("dd/MM/yyyy");
            return objPersistence.ExecuteSqlQueryCount(Sql(0, idsLojas, dtIni, dtFim, true, true, false), GetParams(dtIni, dtFim));
        }

        #endregion

        #region Tipos de movimentação

        public GenericModel[] GetTipos()
        {
            List<GenericModel> retorno = new List<GenericModel>();

            MovimentacaoBemAtivoImob temp = new MovimentacaoBemAtivoImob();
            foreach (int n in Enum.GetValues(typeof(MovimentacaoBemAtivoImob.TipoEnum)))
            {
                temp.Tipo = n;
                retorno.Add(new GenericModel((uint)n, temp.DescricaoTipo));
            }

            retorno.Sort(new Comparison<GenericModel>(
                delegate(GenericModel x, GenericModel y)
                {
                    return x.Descr.CompareTo(y.Descr);
                }
            ));

            return retorno.ToArray();
        }

        #endregion

        #region Métodos sobrescritos

        public override void InsertOrUpdate(MovimentacaoBemAtivoImob objUpdate)
        {
            LogAlteracaoDAO.Instance.LogMovimentacaoBemAtivoImob(objUpdate);
            base.InsertOrUpdate(objUpdate);
        }

        #endregion
    }
}
