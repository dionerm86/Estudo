using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class OrcamentoSiteDAO : BaseDAO<OrcamentoSite, OrcamentoSiteDAO>
    {
        //private OrcamentoSiteDAO() { }

        #region Busca orçamentos

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idOrca"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <param name="emitido">0-Todos, 1-Emitido, 2-Não emitido</param>
        /// <param name="selecionar"></param>
        /// <returns></returns>
        private string Sql(uint idOrca, string dataIni, string dataFim, int emitido, bool selecionar)
        {
            string campos = selecionar ? "*, '$$$' as criterio" : "Count(*)";
            string criterio = String.Empty;

            string sql = "Select " + campos + " From orcamento_site Where 1 ";

            if (idOrca > 0)
                sql += " And CodOrcamento=" + idOrca;

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " And DataPedido>=?dataIni";
                criterio += "Data Início: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " And DataPedido<=?dataFim";
                criterio += "Data Fim: " + dataFim + "    ";
            }

            if (emitido > 0)
            {
                sql += emitido == 1 ? " And Emitido=1" : " And Emitido=0";
                criterio += emitido == 1 ? "Emitidos    " : "Não Emitidos    ";
            }

            return sql.Replace("$$$", criterio);
        }

        public OrcamentoSite GetElement(uint idOrca)
        {
            return objPersistence.LoadOneData(Sql(idOrca, null, null, 0, true));
        }

        public IList<OrcamentoSite> GetForRpt(uint idOrca, string dataIni, string dataFim, int emitido)
        {
            return objPersistence.LoadData(Sql(idOrca, dataIni, dataFim, emitido, true) + " Order By DataPedido Desc", GetParameters(dataIni, dataFim)).ToList();
        }

        public IList<OrcamentoSite> GetList(uint idOrca, string dataIni, string dataFim, int emitido, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "DataPedido Desc" : sortExpression;
            return LoadDataWithSortExpression(Sql(idOrca, dataIni, dataFim, emitido, true), sort, startRow, pageSize, GetParameters(dataIni, dataFim));
        }

        public int GetCount(uint idOrca, string dataIni, string dataFim, int emitido)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idOrca, dataIni, dataFim, emitido, false), GetParameters(dataIni, dataFim));
        }

        private GDAParameter[] GetParameters(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count == 0 ? null : lstParam.ToArray();
        }

        #endregion

        public void SetAsEmitido(uint codOrcamento)
        {
            string sql;

            if (!ObtemValorCampo<bool>("emitido", "codOrcamento=" + codOrcamento))
                sql = "Update orcamento_site Set Emitido=1 Where codOrcamento=" + codOrcamento;
            else
                sql = "Update orcamento_site Set Emitido=0 Where codOrcamento=" + codOrcamento;

            objPersistence.ExecuteCommand(sql);
        }

        public override uint Insert(OrcamentoSite objInsert)
        {
            objInsert.DataPedido = DateTime.Now;
            objInsert.Emitido = false;

            return base.Insert(objInsert);
        }
    }
}
