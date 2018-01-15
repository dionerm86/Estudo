using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.Data.RelDAL
{
    public sealed class PrevisaoProducaoDAO : BaseDAO<PrevisaoProducao, PrevisaoProducaoDAO>
    {
        #region SQL

        private string SqlPrevisaoProducao(uint idSetor, uint idPedido, uint idPedidoNaoConsiderar, bool incluirPedEspelhoAberto, int idClassificacao)
        {
            string sql = @"
                SELECT " + (idClassificacao > 0 ? "rp.idClassificacaoRoteiroProducao" : "rps.idSetor") + @", ROUND(SUM(COALESCE(ppe.totM2Calc, pp.totM2Calc)), 2) as TotM,
                    CAST(COALESCE(pe.dataFabrica, ?dataFabrica) as DATETIME) as dataFabrica
                FROM pedido p
                    LEFT JOIN pedido_espelho pe ON (p.idPedido = pe.idPedido"
                                               + (!incluirPedEspelhoAberto ? " AND pe.situacao not in (" + (int)PedidoEspelho.SituacaoPedido.Processando + "," +
                                                                                                (int)PedidoEspelho.SituacaoPedido.Aberto + "," +
                                                                                                (int)PedidoEspelho.SituacaoPedido.Cancelado + @")" : "") + @")
                    INNER JOIN produtos_pedido pp ON (p.idPedido = pp.idPedido)
                    LEFT JOIN produtos_pedido_espelho ppe ON (pp.idProdPedEsp = ppe.idProdPed)
                    INNER JOIN roteiro_producao rp ON (rp.idProcesso = COALESCE(ppe.idProcesso, pp.idProcesso))
                    " + (idClassificacao == 0 ? "INNER JOIN roteiro_producao_setor rps ON (rp.idRoteiroProducao = rps.idRoteiroProducao)" : "") + @"
                WHERE p.tipoPedido not in (" +
                                                                                             (int)Pedido.TipoPedidoEnum.MaoDeObra + "," +
                                                                                             (int)Pedido.TipoPedidoEnum.MaoDeObraEspecial + "," +
                                                                                             (int)Pedido.TipoPedidoEnum.Revenda + @")
                    AND COALESCE(pp.invisivelFluxo, pp.invisivelPedido, FALSE) = FALSE  
                    AND IF(pe.dataFabrica is not null, DATE(pe.dataFabrica) = DATE(?dataFabrica), DATE(p.dataEntrega) = DATE(?dataEntrega)) {0}
                GROUP BY " + (idClassificacao > 0 ? "rp.idClassificacaoRoteiroProducao" : "rps.idSetor");

            var filtro = "";

            if (idSetor > 0)
                filtro += " AND rps.idSetor = " + idSetor;

            if (idPedidoNaoConsiderar > 0)
                filtro += " AND p.idPedido <> " + idPedidoNaoConsiderar;

            if (idPedido > 0)
                filtro += " AND p.idPedido = " + idPedido;
            else
                filtro += " AND p.situacao NOT IN (" + (int)Pedido.SituacaoPedido.Ativo + "," + (int)Pedido.SituacaoPedido.Cancelado + ")";

            if (idClassificacao > 0)
                filtro += " AND rp.idClassificacaoRoteiroProducao=" + idClassificacao;

            return sql = string.Format(sql, filtro);
        }

        #endregion

        #region Obtem a previsão de produção

        public IList<PrevisaoProducao> ObtemPrevisaoProducao(uint idSetor, string dataFabrica)
        {
            var dataEntrega = DateTime.Parse(dataFabrica);

            for (int j = 0; j < PCPConfig.Etiqueta.DiasDataFabrica; j++)
            {
                dataEntrega = dataEntrega.AddDays(1);

                while (!FeriadoDAO.Instance.IsDiaUtil(dataEntrega))
                    dataEntrega = dataEntrega.AddDays(1);
            }

            var sql = SqlPrevisaoProducao(idSetor, 0, 0, false, 0);
            return objPersistence.LoadData(sql, GetParams(dataFabrica, dataEntrega.ToShortDateString())).ToList();
        }

        public IList<PrevisaoProducao> ObtemPrevisaoProducao(uint idSetor, string dataFabrica, string dataEntrega)
        {
            string sql = SqlPrevisaoProducao(idSetor, 0, 0, false, 0);
            return objPersistence.LoadData(sql, GetParams(dataFabrica, dataEntrega)).ToList();
        }

        public IList<PrevisaoProducao> ObtemPrevisaoProducao(int idClassificacao, string dataFabrica, string dataEntrega)
        {
            string sql = SqlPrevisaoProducao(0, 0, 0, false, idClassificacao);
            return objPersistence.LoadData(sql, GetParams(dataFabrica, dataEntrega)).ToList();
        }

        public IList<PrevisaoProducao> ObtemPrevisaoProducao(uint idPedido, uint idPedidoNaoConsiderar, string dataFabrica, string dataEntrega)
        {
            return ObtemPrevisaoProducao(null, idPedido, idPedidoNaoConsiderar, dataFabrica, dataEntrega);
        }

        public IList<PrevisaoProducao> ObtemPrevisaoProducao(GDASession session, uint idPedido, uint idPedidoNaoConsiderar, string dataFabrica, string dataEntrega)
        {
            var sql = SqlPrevisaoProducao(0, idPedido, idPedidoNaoConsiderar, false, 0);
            return objPersistence.LoadData(session, sql, GetParams(dataFabrica, dataEntrega)).ToList();
        }

        public IList<PrevisaoProducao> ObtemPrevisaoProducao(uint idPedido, uint idPedidoNaoConsiderar, string dataFabrica, bool incluirPedEspelhoAberto)
        {
            return ObtemPrevisaoProducao(null, idPedido, idPedidoNaoConsiderar, dataFabrica, incluirPedEspelhoAberto);
        }

        public IList<PrevisaoProducao> ObtemPrevisaoProducao(GDASession session, uint idPedido, uint idPedidoNaoConsiderar, string dataFabrica, bool incluirPedEspelhoAberto)
        {
            var dataEntrega = DateTime.Parse(dataFabrica);

            for (int j = 0; j < PCPConfig.Etiqueta.DiasDataFabrica; j++)
            {
                dataEntrega = dataEntrega.AddDays(1);

                while (!FeriadoDAO.Instance.IsDiaUtil(dataEntrega))
                    dataEntrega = dataEntrega.AddDays(1);
            }

            var sql = SqlPrevisaoProducao(0, idPedido, idPedidoNaoConsiderar, incluirPedEspelhoAberto, 0);
            return objPersistence.LoadData(session, sql, GetParams(dataFabrica, dataEntrega.ToShortDateString())).ToList();
        }

        private GDAParameter[] GetParams(string dataFabrica, string dataEntrega)
        {
            List<GDAParameter> lista = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(dataFabrica))
                lista.Add(new GDAParameter("?dataFabrica", DateTime.Parse(dataFabrica)));

            if (!string.IsNullOrEmpty(dataEntrega))
                lista.Add(new GDAParameter("?dataEntrega", DateTime.Parse(dataEntrega)));

            return lista.ToArray();
        }

        #endregion
    }
}
