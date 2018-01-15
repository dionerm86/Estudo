using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.Helper;
using GDA;

namespace Glass.Data.RelDAL
{
    public sealed class CaixaDiarioDAO : Glass.Data.DAL.BaseDAO<CaixaDiario, CaixaDiarioDAO>
    {
        //private CaixaDiarioDAO() { }

        public CaixaDiario[] GetMovimentacoes(int tipoMov, string dataIni, string dataFim)
        {
            string contasQueNaoEntram = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfCaixaGeral).ToString();

            string sql = "Select Concat('Cx. Diário ', l.NomeFantasia) as DescrCxDiario, cast(coalesce(Sum(c.Valor), 0) as decimal(12,2)) as Total From caixa_diario c " +
                "Left Join loja l On (c.idLoja=l.idLoja) Where c.TipoMov=" + tipoMov + " And c.idConta Not In (" + contasQueNaoEntram + ") " + 
                "And (c.IdPedido Not In (Select IdPedido From pedido Where situacao=" + (int)Glass.Data.Model.Pedido.SituacaoPedido.Cancelado + ") Or c.idPedido is null) " +
                "And c.DataCad>=?dataIni And c.DataCad<=?dataFim Group By c.idLoja Order By l.NomeFantasia";

            List<GDAParameter> lstParam = new List<GDAParameter>();
            lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));
            lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return CurrentPersistenceObject.LoadData(sql, lstParam.ToArray()).ToList().ToArray();
        }

        /// <summary>
        /// Sql para 
        /// </summary>
        /// <param name="tipoMov"></param>
        /// <returns></returns>
        internal string SqlValorMovimentacoes(string idLoja, int tipoMov, bool agrupar)
        {
            string contasQueNaoEntram = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TransfCaixaGeral).ToString();

            string sql = "Select " + (agrupar ? "l.idLoja, " : "") + "cast(coalesce(Sum(c.Valor), 0) as decimal(12,2)) as valor " +
                "From caixa_diario c Left Join loja l On (c.idLoja=l.idLoja) Where c.TipoMov=" + tipoMov;

            if (!String.IsNullOrEmpty(idLoja) && idLoja != "0")
                sql += " and c.idLoja=" + idLoja;

            sql += " And c.idConta Not In (" + contasQueNaoEntram + ") " +
                "And (c.IdPedido Not In (Select IdPedido From pedido Where situacao=" + (int)Glass.Data.Model.Pedido.SituacaoPedido.Cancelado + ") Or c.idPedido is null) " +
                "And c.DataCad>=?dataIni And c.DataCad<=?dataFim";

            if (agrupar)
                sql += " group by c.idLoja";

            sql += " Order By l.NomeFantasia";
            return sql;
        }

        /// <summary>
        /// Retorna o valor das movimentações de todos os caixas diários, sendo apenas entrada ou apenas saida, 
        /// de acordo com o que for passado e por período
        /// </summary>
        /// <param name="tipoMov"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public Single GetValorMovimentacoes(int tipoMov, string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();
            lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));
            lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return Single.Parse(CurrentPersistenceObject.ExecuteScalar(SqlValorMovimentacoes(null, tipoMov, false), lstParam.ToArray()).ToString());
        }
    }
}
