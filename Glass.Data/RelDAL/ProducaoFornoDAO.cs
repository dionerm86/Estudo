using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.Data.RelDAL
{
    public sealed class ProducaoFornoDAO : BaseDAO<ProducaoForno, ProducaoFornoDAO>
    {
        //private ProducaoFornoDAO() { }

        private string Sql(string dataIni, string dataFim, bool selecionar)
        {
            var setor = SetorDAO.Instance.ObterSetorPeloNumSeq(null, 2);
            var idsSetorForno = SetorDAO.Instance.ObtemIdsSetorForno();
            var criterio = $"Data início: { dataIni }    Data fim: { dataFim }";
            var campos = selecionar ? $@"DATE(Data) AS Data,
                CAST(SUM(TotM2PrimSetor) AS DECIMAL(12,2)) AS TotM2PrimSetor,
                CAST(SUM(TotM2FornoProducao) AS DECIMAL(12,2)) AS TotM2FornoProducao,
                CAST(SUM(TotM2FornoPerda) AS DECIMAL(12,2)) AS TotM2FornoPerda, 
                CAST(COALESCE(totM.TotM2Venda, 0) AS DECIMAL (12,2)) AS TotM2PedidoVenda,
                CAST(COALESCE(totM.TotM2Producao, 0) AS DECIMAL (12,2)) AS TotM2PedidoProducao,
                NULL AS Obs, '$$$' AS Criterio, '{ setor.Descricao }' AS NomePrimSetor" : "DISTINCT DATE(Data)";
            
            var campoTotM2 = $"ROUND(SUM(pp.TotM/(pp.Qtde*IF(p.TipoPedido={ (int)Pedido.TipoPedidoEnum.MaoDeObra }, a.Qtde, 1))), 4)";

            var reposicaoPeca = Configuracoes.ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Peca;

            var sql = $@"
                SELECT { campos }
                FROM (
                    SELECT lp.DataLeitura AS Data, { campoTotM2 } AS TotM2PrimSetor, 0 AS TotM2FornoProducao, 0 AS TotM2FornoPerda
                    FROM leitura_producao lp
                        LEFT JOIN produto_pedido_producao ppp ON (lp.IdProdPedProducao=ppp.IdProdPedProducao)
                        LEFT JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed=pp.IdProdPed)
                        LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido=a.IdAmbientePedido)
                        LEFT JOIN pedido p ON (pp.IdPedido=p.IdPedido)
                    WHERE lp.DataLeitura>=?dataIni
                        AND lp.DataLeitura<=?dataFim
                        AND lp.IdSetor={ setor.IdSetor }
                        AND ppp.Situacao={ (int)ProdutoPedidoProducao.SituacaoEnum.Producao }
                        AND p.Situacao<>{ (int)Pedido.SituacaoPedido.Cancelado }
                        AND (pp.InvisivelFluxo=FALSE OR pp.InvisivelFluxo IS NULL)
                    GROUP BY DATE(lp.DataLeitura)
                    
                    UNION ALL SELECT lp.DataLeitura AS Data, 0 AS TotM2PrimSetor, { campoTotM2 } AS TotM2FornoProducao, 0 AS TotM2FornoPerda
                    FROM leitura_producao lp
                        LEFT JOIN produto_pedido_producao ppp ON (lp.IdProdPedProducao=ppp.IdProdPedProducao)
                        LEFT JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed=pp.IdProdPed)
                        LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido=a.IdAmbientePedido)
                        LEFT JOIN pedido p ON (pp.IdPedido=p.IdPedido)
                    WHERE lp.DataLeitura>=?dataIni
                        AND lp.DataLeitura<=?dataFim
                        AND lp.IdSetor IN ({ idsSetorForno })
                        AND ppp.Situacao={ (int)ProdutoPedidoProducao.SituacaoEnum.Producao }
                        AND p.Situacao<>{ (int)Pedido.SituacaoPedido.Cancelado }
                        AND (pp.InvisivelFluxo=FALSE OR pp.InvisivelFluxo IS NULL)
                    GROUP BY DATE(lp.DataLeitura)
                    
                    UNION ALL SELECT { (reposicaoPeca ? "ppp.DataRepos" : "ppp.DataPerda") } AS Data, 0 AS TotM2PrimSetor, 0 AS TotM2FornoProducao, { campoTotM2 } AS TotM2FornoPerda
                    FROM produto_pedido_producao ppp
                        LEFT JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed=pp.IdProdPed)
                        LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido=a.IdAmbientePedido)
                        LEFT JOIN pedido p ON (pp.IdPedido=p.IdPedido)
                    WHERE
                        { (reposicaoPeca ?
                            $"ppp.IdSetorRepos IN ({ idsSetorForno }) AND ppp.DataRepos>=?dataIni AND ppp.DataRepos<=?dataFim" :
                            "ppp.DataPerda>=?dataIni AND ppp.DataPerda<=?dataFim") }
                        AND ppp.Situacao={ (int)ProdutoPedidoProducao.SituacaoEnum.Producao }
                        AND p.Situacao<>{ (int)Pedido.SituacaoPedido.Cancelado }
                        AND (pp.InvisivelFluxo=FALSE OR pp.InvisivelFluxo IS NULL)
                    GROUP BY DATE({ (reposicaoPeca ? "ppp.DataRepos" : "ppp.DataPerda") })
                ) AS producao_forno
                
                LEFT JOIN
                    (SELECT 
                        DataPedido,
                        SUM(TotMVenda) AS TotM2Venda,
                        SUM(TotMProd) AS TotM2Producao
                    FROM
                        (SELECT 
                        DATE(p.DataPedido) AS DataPedido,
                            IF(TipoPedido IN ({ (int)Pedido.TipoPedidoEnum.Revenda },{ (int)Pedido.TipoPedidoEnum.Venda }), pp.TotM, 0) AS TotMVenda,
                            IF(TipoPedido={ (int)Pedido.TipoPedidoEnum.Producao }, pp.TotM, 0) AS totMProd
                        FROM
                            produtos_pedido pp
                        INNER JOIN pedido p ON (pp.IdPedido = p.IdPedido)
                        WHERE p.TipoPedido IN ({ (int)Pedido.TipoPedidoEnum.Revenda },{ (int)Pedido.TipoPedidoEnum.Venda },{ (int)Pedido.TipoPedidoEnum.Producao })
                            AND p.Situacao <> { (int)Pedido.SituacaoPedido.Cancelado }
                            AND !COALESCE(pp.InvisivelFluxo, FALSE) AND p.DataPedido IS NOT NULL
                            AND p.DataPedido >= ?dataIni AND p.DataPedido <= ?dataFim) AS temp
                    GROUP BY DATE(DataPedido)) AS TotM ON (DATE(totM.DataPedido)=DATE(producao_forno.Data))
                GROUP BY DATE(Data)";

            sql = sql.Replace("$$$", criterio);
            return selecionar ? sql : $"SELECT COUNT(*) FROM ({ sql }) AS temp";
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            var lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            return lstParam.ToArray();
        }

        public IList<ProducaoForno> GetForRpt(string dataIni, string dataFim)
        {
            return objPersistence.LoadData(Sql(dataIni, dataFim, true), GetParams(dataIni, dataFim)).ToList();
        }

        public IList<ProducaoForno> GetList(string dataIni, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(dataIni, dataFim, true), sortExpression, startRow, pageSize, GetParams(dataIni, dataFim));
        }

        public int GetCount(string dataIni, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(dataIni, dataFim, false), GetParams(dataIni, dataFim));
        }
    }
}
