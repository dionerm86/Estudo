using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class CotacaoCompraDAO : BaseDAO<CotacaoCompra, CotacaoCompraDAO>
    {
        //private CotacaoCompraDAO() { }

        private string Sql(uint idCotacaoCompra, int situacao, string dataCadIni, string dataCadFim, bool selecionar)
        {
            StringBuilder sql = new StringBuilder("select ");

            sql.Append(selecionar ? "cc.*" : "count(*)");

            sql.AppendFormat(@"
                from cotacao_compra cc
                where 1 {0}", FILTRO_ADICIONAL);

            if (idCotacaoCompra > 0)
                sql.AppendFormat(" and cc.idCotacaoCompra={0}", idCotacaoCompra);

            if (situacao > 0)
                sql.AppendFormat(" and cc.situacao={0}", situacao);

            if (!String.IsNullOrEmpty(dataCadIni))
                sql.Append(" and cc.dataCad>=?dataCadIni");

            if (!String.IsNullOrEmpty(dataCadFim))
                sql.Append(" and cc.dataCad<=?dataCadFim");

            return sql.ToString();
        }

        private GDAParameter[] GetParams(string dataCadIni, string dataCadFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataCadIni))
                lst.Add(new GDAParameter("?dataCadIni", DateTime.Parse(dataCadIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataCadFim))
                lst.Add(new GDAParameter("?dataCadFim", DateTime.Parse(dataCadFim + " 23:59:59")));

            return lst.ToArray();
        }

        public IList<CotacaoCompra> ObtemCotacoes(uint idCotacaoCompra, int situacao, string dataCadIni, 
            string dataCadFim, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idCotacaoCompra, situacao, dataCadIni, dataCadFim, true), 
                sortExpression, startRow, pageSize, GetParams(dataCadIni, dataCadFim));
        }

        public int ObtemNumeroCotacoes(uint idCotacaoCompra, int situacao, string dataCadIni, string dataCadFim)
        {
            return GetCountWithInfoPaging(Sql(idCotacaoCompra, situacao, dataCadIni, dataCadFim, true), false,
                GetParams(dataCadIni, dataCadFim));
        }

        public void Finalizar(GDASession session, uint idCotacaoCompra, CotacaoCompra.TipoCalculoCotacao tipoCalculo)
        {
            // Finaliza a cotação de compra
            objPersistence.ExecuteCommand(session, @"update cotacao_compra set situacao=" + (int)CotacaoCompra.SituacaoEnum.Finalizada + @",
                dataFin=?data, idFuncFin=?f, prioridadeCalculoFinalizacao=?p where idCotacaoCompra=" + idCotacaoCompra, 
                new GDAParameter("?data", DateTime.Now), new GDAParameter("?f", UserInfo.GetUserInfo.CodUser), 
                new GDAParameter("?p", (int)tipoCalculo));
        }

        public void Cancelar(uint idCotacaoCompra, string motivo, bool manual)
        {
            CotacaoCompra c = GetElementByPrimaryKey(idCotacaoCompra);
            LogCancelamentoDAO.Instance.LogCotacaoCompras(c, motivo, manual);

            // Cancela a cotação de compra
            objPersistence.ExecuteCommand("update cotacao_compra set situacao=" + (int)CotacaoCompra.SituacaoEnum.Cancelada + @"
                where idCotacaoCompra=" + idCotacaoCompra);
        }

        public void Reabrir(uint idCotacaoCompra)
        {
            // Reabre a cotação de compra
            objPersistence.ExecuteCommand("update cotacao_compra set situacao=" + (int)CotacaoCompra.SituacaoEnum.Aberta + @",
                dataFin=null, idFuncFin=null, prioridadeCalculoFinalizacao=null where idCotacaoCompra=" + idCotacaoCompra);
        }

        public bool PossuiComprasNaoCanceladas(uint idCotacaoCompra)
        {
            return objPersistence.ExecuteSqlQueryCount("select count(*) from compra where idCotacaoCompra=" +
                idCotacaoCompra + " and situacao<>" + (int)Compra.SituacaoEnum.Cancelada) > 0;
        }

        public int ObtemSituacao(uint idCotacaoCompra)
        {
            return ObtemValorCampo<int>("situacao", "idCotacaoCompra=" + idCotacaoCompra);
        }

        public int? ObtemIdFuncFin(uint idCotacaoCompra)
        {
            return ObtemValorCampo<int?>("IdFuncFin", "idCotacaoCompra=" + idCotacaoCompra);
        }
    }
}
