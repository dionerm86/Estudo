using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.Model;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class PosicaoMateriaPrimaDAO : BaseDAO<PosicaoMateriaPrima, PosicaoMateriaPrimaDAO>
    {
        /// <summary>
        /// Obtém os produtos de pedido que estão em produção ou serão produzidos, obtém também a metragem quadrada da chapa de vidro,
        /// comparando somente a cor e espessura do produto a ser produzido com a chapa.
        /// </summary>
        /// <param name="idsRota"></param>
        /// <param name="tipoPedido"></param>
        /// <param name="situacaoPedido"></param>
        /// <param name="etiquetas"></param>
        /// <param name="dataIniEnt"></param>
        /// <param name="dataFimEnt"></param>
        /// <param name="idsCorVidro"></param>
        /// <param name="espessura"></param>
        /// <returns></returns>
        private string SqlPosMateriaPrima(string idsRota, string tipoPedido, string situacaoPedido, string dataIniEnt, string dataFimEnt,
            string idsCorVidro, float? espessura)
        {
            string sql = @"
                SELECT p.espessura, p.idCorVidro, cv.descricao as DescrCorVidro,
                    CAST(ROUND(SUM(pp.totM2Calc), 2) as decimal(12,2)) as TotM2, 
                    CAST(ROUND(SUM(pp.TotM2ComEtiqueta), 2) as decimal(12,2)) as TotM2ComEtiqueta,
                    CAST(ROUND(SUM(pp.TotM2SemEtiqueta), 2) as decimal(12,2)) as TotM2SemEtiqueta,
                    CAST(ROUND(SUM(pp.TotM2Producao), 2) as decimal(12,2)) as TotM2Producao,
                    CAST(ROUND(SUM(pp.TotM2Venda), 2) as decimal(12,2)) as TotM2Venda
                FROM produto p
	                INNER JOIN subgrupo_prod sgp ON (p.idSubgrupoProd = sgp.idSubgrupoProd)
	                INNER JOIN cor_vidro cv ON (p.idCorVidro = cv.idCorVidro)
                    LEFT JOIN
                    (
    	                SELECT pp.idProd, ((pp.totm2calc / pp.qtde) * SUM(IF(ppp.idSetor IS NULL OR ppp.idSetor = 1, 1, 0))) as TotM2Calc,
        	                ((pp.totm2calc / pp.qtde) * SUM(IF((ppp.idSetor IS NULL OR ppp.idSetor = 1) AND ppp.TemLeitura IS NOT NULL AND ppp.TemLeitura = 1, 1, 0))) as TotM2ComEtiqueta,
                            ((pp.totm2calc / pp.qtde) * SUM(IF((ppp.idSetor IS NULL OR ppp.idSetor = 1) AND (ppp.TemLeitura IS NULL OR ppp.TemLeitura = 0), 1, 0))) as TotM2SemEtiqueta,
                            IF(ped.TipoPedido= " + (int)Pedido.TipoPedidoEnum.Producao + @", ((pp.totm2calc / pp.qtde) * SUM(IF(ppp.idSetor IS NULL OR ppp.idSetor = 1, 1, 0))), 0) as TotM2Producao,
                            IF(ped.TipoPedido= " + (int)Pedido.TipoPedidoEnum.Venda + @", ((pp.totm2calc / pp.qtde) * SUM(IF(ppp.idSetor IS NULL OR ppp.idSetor = 1, 1, 0))), 0) as TotM2Venda
                        FROM pedido ped
        	                INNER JOIN produtos_pedido pp ON (ped.idPedido = pp.idPedido AND (InvisivelFluxo IS NULL OR InvisivelFluxo=0))
                            INNER JOIN produto p ON (pp.idProd = p.idProd)
                            LEFT JOIN
                            (
            	                SELECT ppp.idProdPed, ppp.idSetor, ppp.IdImpressao IS NOT NULL AS TemLeitura
                                FROM produto_pedido_producao ppp
                                WHERE ppp.situacao = " + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @"
                            ) as ppp ON (pp.idProdPedEsp = ppp.idProdPed)
                            {0}
                        WHERE 1 
        	                {1}
                        GROUP by pp.idProdPed HAVING TotM2Calc > 0
                    ) as pp ON (pp.idProd = p.idProd)
                WHERE 1 {2}
                GROUP BY cv.idCorVidro, p.espessura
                ORDER BY cv.descricao Asc, p.espessura Asc";

            var sqlFiltroRota = @"
                INNER JOIN cliente c ON (ped.idCli = c.id_cli)
			    LEFT JOIN rota_cliente rc ON (c.id_cli = rc.idCliente)
			    LEFT JOIN rota r ON (rc.idRota = r.idRota)";

            string filtro = "", filtroPedido = "";
            var filtroRota = false;

            if (!String.IsNullOrEmpty(idsRota))
            {
                filtroPedido += " And r.idRota IS NOT NULL AND r.idRota In (" + idsRota + ")";
                filtroRota = true;
            }

            // Se o tipo de pedido não tiver sido informado é necessário buscar somente pedidos do tipo Produção e Venda.
            if (!String.IsNullOrEmpty(tipoPedido))
                filtroPedido += " And ped.tipoPedido In (" + tipoPedido + ")";
            else
                filtroPedido += " And ped.tipoPedido In (" + (int)Pedido.TipoPedidoEnum.Producao + "," + (int)Pedido.TipoPedidoEnum.Venda + @")";

            // Caso a situação do pedido não tenha sido informada é necessário buscar somente os pedidos nas situações Conferido COM e Confirmado PCP.
            if (!String.IsNullOrEmpty(situacaoPedido))
                filtroPedido += " And ped.situacao In (" + situacaoPedido + ")";
            else
                filtroPedido += " And ped.situacao In (" + (int)Pedido.SituacaoPedido.Conferido + "," + (int)Pedido.SituacaoPedido.Confirmado + "," + (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + ")";

            if (!String.IsNullOrEmpty(dataIniEnt))
                filtroPedido += " And ped.dataEntrega >= ?dataIniEnt";

            if (!String.IsNullOrEmpty(dataFimEnt))
                filtroPedido += " And ped.dataEntrega <= ?dataFimEnt";

            if (!String.IsNullOrEmpty(idsCorVidro))
                filtro += " And p.idCorVidro IS NOT NULL AND p.idCorVidro In (" + idsCorVidro + ")";

            if (espessura.GetValueOrDefault() > 0)
                filtro += " And p.espessura=" + espessura.ToString().Replace(",", ".");

            sql = string.Format(sql, filtroRota ? sqlFiltroRota : "", filtroPedido, filtro);


            return sql;
        }

        /// <summary>
        /// Retorna uma lista de produtos com o total de m2 do produto e a matéria prima disponível.
        /// </summary>
        /// <param name="idsRota"></param>
        /// <param name="tipoPedido"></param>
        /// <param name="situacaoPedido"></param>
        /// <param name="etiquetas"></param>
        /// <param name="dataIniEnt"></param>
        /// <param name="dataFimEnt"></param>
        /// <param name="idsCorVidro"></param>
        /// <param name="espessura"></param>
        /// <returns></returns>
        public IList<PosicaoMateriaPrima> GetPosMateriaPrima(string idsRota, string tipoPedido, string situacaoPedido, string dataIniEnt,
            string dataFimEnt, string idsCorVidro, float? espessura, bool totM2DisponivelNegativo)
        {
            var sql = SqlPosMateriaPrima(idsRota, tipoPedido, situacaoPedido, dataIniEnt, dataFimEnt, idsCorVidro, espessura);

            var prodPed = objPersistence.LoadData(sql, GetParamsPosMatPrima(dataIniEnt, dataFimEnt)).ToList();

            if (totM2DisponivelNegativo)
                prodPed = prodPed.Where(f => f.TotM2Disponivel < 0).ToList();

            var corEspessura = prodPed
                .GroupBy(f => string.Format("{0}|{1}", f.IdCorVidro, f.Espessura))
                .Select(f => new { IdCorVidro = f.Key.Split('|')[0], Espessura = f.Key.Split('|')[1].Replace(",", ".") }).ToList();

            var idsCor = string.Join(",", corEspessura.GroupBy(f => f.IdCorVidro).Select(f => f.Key).ToArray());
            var espessuras = string.Join(",", corEspessura.GroupBy(f => f.Espessura).Select(f => f.Key).ToArray());

            var chapas = PosicaoMateriaPrimaChapaDAO.Instance.GetChapaByCorEsp(0, 0, idsCor, espessuras);

            foreach (var p in prodPed)
                p.Chapas = chapas.Where(f => f.IdCorVidro == p.IdCorVidro && f.Espessura == p.Espessura).ToList();

            return prodPed;
        }

        /// <summary>
        /// Retorna uma lista de parametros.
        /// </summary>
        /// <param name="dataIniEnt"></param>
        /// <param name="dataFimEnt"></param>
        /// <returns></returns>
        public GDAParameter[] GetParamsPosMatPrima(string dataIniEnt, string dataFimEnt)
        {
            var lstParametros = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIniEnt))
                lstParametros.Add(new GDAParameter("?dataIniEnt", Glass.Conversoes.StrParaDate(dataIniEnt + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimEnt))
                lstParametros.Add(new GDAParameter("?dataFimEnt", Glass.Conversoes.StrParaDate(dataFimEnt + " 23:59")));

            return lstParametros.ToArray();
        }
    }
}