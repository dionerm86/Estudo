using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.RelDAL
{
    public class DefinicaoCargaRotaDAO : BaseDAO<DefinicaoCargaRota, DefinicaoCargaRotaDAO>
    {
        //private DefinicaoCargaRotaDAO() { }

        private GDAParameter[] GetParam(int rota, string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if(rota > 0)
                lstParam.Add(new GDAParameter("?rota", rota));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        public List<DefinicaoCargaRota> ObterDados(int rota, string dataIni, string dataFim)
        {
            var where = String.Empty;

            if (rota > 0)
                where += " AND rc.IdRota=?rota";

            var sql =
                string.Format(@"
                SET @i = 0;
                SELECT CAST(FLOOR(((@i := @i + 1))) AS SIGNED INTEGER) AS Indice, temp.*
                FROM (
                    SELECT c.Id_Cli AS IdCliente, CAST(GROUP_CONCAT(DISTINCT p.IdPedido) AS CHAR) AS Pedidos,
                        COALESCE(c.Nome, c.NomeFantasia) AS NomeCliente, 
                        CAST(SUM(pp.TotM / pp.Qtde * IF(ppp.IdProdPedProducao IS NULL, pp.Qtde, 1)) AS DECIMAL(11,2)) AS TotalM2,
                        CAST(SUM(pp.Peso / pp.Qtde * IF(ppp.IdProdPedProducao IS NULL, pp.Qtde, 1)) AS DECIMAL(11,2)) AS Peso,
                        CAST(SUM(IF(s.NumSeq IN (SELECT NumSeq FROM setor s1 WHERE s1.Tipo IN ({0})),
                            pp.TotM / pp.Qtde, 0)) AS DECIMAL(11,2)) AS Pronto,
                        CAST(SUM(IF(s.NumSeq IN (SELECT NumSeq FROM setor s1 WHERE s1.Tipo IN ({1})),
                            pp.TotM / pp.Qtde, 0)) AS DECIMAL(11,2)) AS Entregue,
                        CAST(SUM(IF(s.NumSeq IN (SELECT s1.NumSeq FROM setor s1 WHERE s1.Tipo={2}) 
                            AND ppp.IdProdPedProducao IN (SELECT lp1.IdProdPedProducao FROM leitura_producao lp1 WHERE lp1.DataLeitura IS NOT NULL),
                        pp.TotM / pp.Qtde,0)) AS DECIMAL(11,2)) AS Pendente
                    FROM produtos_pedido pp
                        LEFT JOIN produto_pedido_producao ppp ON (ppp.IdProdPed=pp.IdProdPedEsp)
                        INNER JOIN pedido p ON (p.IdPedido=pp.IdPedido)
                        INNER JOIN cliente c ON (c.Id_Cli=p.IdCli)
                        INNER JOIN rota_cliente rc ON (c.Id_Cli=rc.IdCliente)
                        LEFT JOIN setor s ON (ppp.IdSetor=s.IdSetor)
                    WHERE (pp.InvisivelFluxo IS NULL OR pp.InvisivelFluxo=FALSE) {3}
                        AND p.DataEntrega >= ?dataIni AND p.DataEntrega <= ?dataFim
                        AND p.Situacao NOT IN ({4}, {5}, {6})
                        /* Chamado 13021. Estavam sendo buscadas peças canceladas e por isso o m² estava ficando incorreto. */
                        AND (ppp.Situacao IS NULL OR ppp.Situacao = {7})
                    GROUP BY NomeCliente) AS temp",
                    (int)TipoSetor.Pronto, (int)TipoSetor.Entregue, (int)TipoSetor.Pendente, where, (int)Pedido.SituacaoPedido.Cancelado,
                    (int)Pedido.SituacaoPedido.Ativo, (int)Pedido.SituacaoPedido.AtivoConferencia,
                    (int)ProdutoPedidoProducao.SituacaoEnum.Producao);

            return objPersistence.LoadData(sql, GetParam(rota, dataIni, dataFim));
        }
    }
}
