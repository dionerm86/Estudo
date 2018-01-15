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
            var sqlNomePrimSetor = @"(
                select descricao
                from setor
                where numSeq=2) as nomePrimSetor";

            var criterio = "Data início: " + dataIni + "    Data fim: " + dataFim;
            var campos = selecionar ? @"Date(Data) As Data,
                Cast(Sum(TotM2PrimSetor) As decimal(12,2)) As TotM2PrimSetor,
                Cast(Sum(TotM2FornoProducao) As decimal(12,2)) As TotM2FornoProducao,
                Cast(Sum(TotM2FornoPerda) As decimal(12,2)) As TotM2FornoPerda, 
                Cast(Coalesce(totM.TotM2Venda, 0) As Decimal (12,2)) As TotM2PedidoVenda,
                Cast(Coalesce(totM.TotM2Producao, 0) As Decimal (12,2)) As TotM2PedidoProducao,
                Null As Obs, '$$$' As Criterio, " + sqlNomePrimSetor : "Distinct Date(Data)";

            var campoTotM2 = "round(sum(pp.totM/(pp.Qtde*if(p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.qtde, 1))), 4)";

            var reposicaoPeca = Glass.Configuracoes.ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Peca;

            var sql = @"
                select " + campos + @"
                from (
                    select lp.DataLeitura as Data, " + campoTotM2 + @" as TotM2PrimSetor, 0 as TotM2FornoProducao, 0 as TotM2FornoPerda
                    from leitura_producao lp
                        left join produto_pedido_producao ppp on (lp.idProdPedProducao=ppp.idProdPedProducao)
                        left join produtos_pedido_espelho pp on (ppp.idProdPed=pp.idProdPed)
                        left join ambiente_pedido_espelho a on (pp.idAmbientePedido=a.idAmbientePedido)
                        left join pedido p on (pp.idPedido=p.idPedido)
                    where lp.DataLeitura>=?dataIni
                        and lp.DataLeitura<=?dataFim
                        and lp.idSetor=(select idSetor from setor where numSeq=2)
                        and ppp.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @"
                        and p.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + @"
                        and (pp.InvisivelFluxo=false or pp.InvisivelFluxo is null)
                    group by date(lp.DataLeitura)
                    
                    union all select lp.DataLeitura as Data, 0 as TotM2PrimSetor, " + campoTotM2 + @" as TotM2FornoProducao, 0 as TotM2FornoPerda
                    from leitura_producao lp
                        left join produto_pedido_producao ppp on (lp.idProdPedProducao=ppp.idProdPedProducao)
                        left join produtos_pedido_espelho pp on (ppp.idProdPed=pp.idProdPed)
                        left join ambiente_pedido_espelho a on (pp.idAmbientePedido=a.idAmbientePedido)
                        left join pedido p on (pp.idPedido=p.idPedido)
                    where lp.DataLeitura>=?dataIni
                        and lp.DataLeitura<=?dataFim
                        and lp.idSetor in (select idSetor from setor where forno=true)
                        and ppp.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @"
                        and p.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + @"
                        and (pp.InvisivelFluxo=false or pp.InvisivelFluxo is null)
                    group by date(lp.DataLeitura)
                    
                    union all select " + (reposicaoPeca ? "ppp.dataRepos" : "ppp.dataPerda") +
                        @" as Data, 0 as TotM2PrimSetor, 0 as TotM2FornoProducao, " + campoTotM2 + @" as TotM2FornoPerda
                    from produto_pedido_producao ppp
                        left join produtos_pedido_espelho pp on (ppp.idProdPed=pp.idProdPed)
                        left join ambiente_pedido_espelho a on (pp.idAmbientePedido=a.idAmbientePedido)
                        left join pedido p on (pp.idPedido=p.idPedido)
                    where
                        " + (reposicaoPeca ?
                        "ppp.idSetorRepos In (select idSetor from setor where forno=true) And ppp.dataRepos>=?dataIni And ppp.dataRepos<=?dataFim" :
                        "ppp.dataPerda>=?dataIni And ppp.dataPerda<=?dataFim") + @"
                        and ppp.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @"
                        and p.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + @"
                        and (pp.InvisivelFluxo=false or pp.InvisivelFluxo is null)
                    group by date(" + (reposicaoPeca ? "ppp.dataRepos" : "ppp.dataPerda") + @")
                ) as producao_forno

                Left Join
                    (Select 
                        dataPedido,
                        SUM(TotMVenda) AS TotM2Venda,
                        SUM(TotMProd) AS TotM2Producao
                    From
                        (Select 
                        Date(p.datapedido) As dataPedido,
                            If(tipoPedido In (" + (int)Pedido.TipoPedidoEnum.Revenda + "," + (int)Pedido.TipoPedidoEnum.Venda + @"), pp.totM, 0) As totMVenda,
                            If(tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @", pp.totM, 0) As totMProd
                        From
                            produtos_pedido pp
                        Inner Join pedido p ON (pp.idPedido = p.idPedido)
                        Where p.tipoPedido In (" + (int)Pedido.TipoPedidoEnum.Revenda + "," + (int)Pedido.TipoPedidoEnum.Venda + "," +
                            (int)Pedido.TipoPedidoEnum.Producao + @") And p.situacao <> " + (int)Pedido.SituacaoPedido.Cancelado + @" And
                            !Coalesce(pp.InvisivelFluxo, False) And p.dataPedido Is Not Null
                            And p.dataPedido >= ?dataIni and p.dataPedido <= ?dataFim) As temp
                    Group By DATE(DataPedido)) As totM ON (DATE(totM.DataPedido)=Date(producao_forno.data))
                Group By Date(Data)";

            sql = sql.Replace("$$$", criterio);
            return selecionar ? sql : "Select Count(*) From (" + sql + ") As temp";
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
