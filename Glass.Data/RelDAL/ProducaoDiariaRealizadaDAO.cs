using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class ProducaoDiariaRealizadaDAO : BaseDAO<ProducaoDiariaRealizada, ProducaoDiariaRealizadaDAO>
    {
        //private ProducaoDiariaRealizadaDAO() { }

        private string Sql(uint idSetor, DateTime dataProducao)
        {
            string idsProdPed = CapacidadeProducaoDAO.Instance.ObtemIdsProdPed(0, dataProducao, true, false);
            string idsProdPedProducao = CapacidadeProducaoDAO.Instance.ObtemIdsProdPedProducao(0, idsProdPed);

            return String.Format(@"
                select idSetor, cast(0 as decimal(12,2)) as totMPrevisto, cast(sum(totM) as decimal(12,2)) as totMRealizado, 
                    cast(sum(if(hoje, totM, 0)) as decimal(12,2)) as totMHoje
                from (
                    select rps.idSetor, if(p.tipoPedido<>{1}, ppe.totM2Calc/ppe.qtde*count(distinct ppp.idProdPedProducao),
                        ape.altura*ape.largura*ape.qtde/1000000*count(distinct ppp.idProdPedProducao)) as totM,
                        date(lp.dataLeitura)=date(?data) as hoje
                    from roteiro_producao_etiqueta rps
                        inner join leitura_producao lp on (rps.idProdPedProducao=lp.idProdPedProducao
                            and rps.idSetor=lp.idSetor and lp.dataLeitura is not null)
                        inner join produto_pedido_producao ppp on (rps.idProdPedProducao=ppp.idProdPedProducao {3})
                        inner join produtos_pedido_espelho ppe on (ppp.idProdPed=ppe.idProdPed {2})
                        inner join pedido_espelho pe on (ppe.idPedido=pe.idPedido)
                        inner join pedido p on (pe.idPedido=p.idPedido)
                        left join ambiente_pedido_espelho ape on (ppe.idAmbientePedido=ape.idAmbientePedido)
                    where ppp.situacao={0} {2} {3} {4}
                    group by ppp.idProdPed, rps.idSetor
                ) as temp
                group by idSetor",
                                 
                (int)ProdutoPedidoProducao.SituacaoEnum.Producao,
                (int)Pedido.TipoPedidoEnum.MaoDeObra,
                
                idsProdPed,
                idsProdPedProducao,
                idSetor > 0 ? " and rps.idSetor=" + idSetor : String.Empty);
        }

        private string SqlProducaoRealizada(int idSetor, List<DateTime> datasFabrica, bool agruparSetor)
        {
            string campos = @"
                CAST(ROUND(SUM(IF(p.tipoPedido = 3,
                            ((((50 - IF(MOD(a.altura, 50) > 0, MOD(a.altura, 50), 50)) + a.altura) *
                            ((50 - IF(MOD(a.largura, 50) > 0, MOD(a.largura, 50), 50)) + a.largura)) / 1000000) *
                            a.qtde, ppo.TotM2Calc) / (ppo.qtde * IF(p.tipoPedido = 3, a.qtde, 1))), 2) as decimal(12, 2)) as TotMPrevisto,
                CAST(ROUND(SUM(IF(" + (agruparSetor ? "lp.dataLeitura is not null" : "ppp.situacaoProducao <> 1") + @", IF(p.tipoPedido = 3,
                            ((((50 - IF(MOD(a.altura, 50) > 0, MOD(a.altura, 50), 50)) + a.altura) *
                            ((50 - IF(MOD(a.largura, 50) > 0, MOD(a.largura, 50), 50)) + a.largura)) / 1000000) *
                            a.qtde, ppo.TotM2Calc) / (ppo.qtde * IF(p.tipoPedido = 3, a.qtde, 1)), 0)), 2) as decimal(12,2)) as TotMRealizado,
                pe.dataFabrica" + (agruparSetor ? ", rpe.idSetor" : "");

            string sql = @"
                SELECT " + campos + @"
                FROM pedido p
                    INNER JOIN pedido_espelho pe ON (p.idPedido = pe.idPedido)
                    INNER JOIN produtos_pedido_espelho pp ON (p.idPedido = pp.idPedido)
                    INNER JOIN produtos_pedido ppo ON (ppo.idProdPedEsp = pp.idProdPed)
                    INNER JOIN produto_pedido_producao ppp ON (pp.idProdPed = ppp.idProdPed)
                    LEFT JOIN ambiente_pedido_espelho a on (pp.idAmbientePedido = a.idAmbientePedido)";

            if (agruparSetor)
                sql += @"
                    INNER JOIN roteiro_producao_etiqueta rpe ON (ppp.idProdPedProducao = rpe.idProdPedProducao)
                    LEFT JOIN leitura_producao lp on (rpe.idProdPedProducao = lp.idProdPedProducao and rpe.idSetor = lp.idSetor)
                    LEFT JOIN setor s ON (rpe.idSetor=s.idSetor)";

            sql += @"
                WHERE 1
                    AND ppp.situacao = " + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;

            if (datasFabrica != null && datasFabrica.Count > 0)
            {
                sql += " AND date(pe.dataFabrica) IN (";

                for (var i = 0; i < datasFabrica.Count; i++)
                {
                    sql += $"?data{i},";
                }

                sql = sql.TrimEnd(',') + ")";
            }

            if (agruparSetor && idSetor > 0)
                sql += " AND rpe.idSetor = " + idSetor;

            sql += " GROUP BY DATE(pe.dataFabrica)";

            if (agruparSetor)
                sql += ", rpe.idSetor ORDER BY s.numseq";

            return sql;
        }

        /// <summary>
        /// Recupera a producao realizada de todos os setores.
        /// </summary>
        /// <returns></returns>
        public IList<ProducaoDiariaRealizadaSetor> ObtemProducaoRealizadaSetoresClassificacao(int idClassificacao)
        {
            var resultado = new List<ProducaoDiariaRealizadaSetor>();
            var setores = new List<Setor>();

            setores = SetorDAO.Instance.GetSetoresClassificacao(idClassificacao);            

            // Informa as mensagens que serão passadas no rodapé da página            
            foreach (var s in setores)
            {
                if (s.ExibirRelatorio && s.Tipo != TipoSetor.Entregue && s.Tipo != TipoSetor.ExpCarregamento)
                {
                    var prodDiaRealSet = new ProducaoDiariaRealizadaSetor(s.Descricao);

                    var mensagem = new System.Text.StringBuilder()
                        .AppendFormat("{0}: ", s.Descricao);

                    var pedidoProducaoDAO = ProdutoPedidoProducaoDAO.Instance;

                    // Verifica quais dados serão exibidos no rodapé do painel da produção, de acordo com a empresa.
                    if (PCPConfig.PainelProducao.ExibirTotalM2LidoNoDia)
                        prodDiaRealSet.TotRealizado = pedidoProducaoDAO
                            .ObtemTotM2LidoSetor(s.IdSetor, idClassificacao, DateTime.Today, DateTime.Today.AddDays(1), true);

                    else if (PCPConfig.PainelProducao.ExibirTotalQtdeLidoNoDia)
                        prodDiaRealSet.TotRealizado = pedidoProducaoDAO
                            .ObtemTotM2LidoSetor(s.IdSetor, idClassificacao, DateTime.Today, DateTime.Today.AddDays(1), false);

                    else if (OrdemCargaConfig.DataEntregaBaseConsiderarPedidoParaOC != null)
                        prodDiaRealSet.TotRealizado = pedidoProducaoDAO
                           .ObtemTotM2Setor(s.IdSetor, idClassificacao, OrdemCargaConfig.DataEntregaBaseConsiderarPedidoParaOC,
                            DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd 23:59:59"), true);

                    else
                        prodDiaRealSet.TotRealizado = ProdutoPedidoProducaoDAO.Instance.ObtemTotM2Setor(s.IdSetor, idClassificacao, null, null, true);

                    resultado.Add(prodDiaRealSet);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Recupera a producao realizada de todos os setores.
        /// </summary>
        /// <returns></returns>
        public IList<ProducaoDiariaRealizadaSetor> ObtemProducaoRealizadaTodosSetores()
        {
            var resultado = new List<ProducaoDiariaRealizadaSetor>();

            var setores = new List<Setor>();

            // Informa as mensagens que serão passadas no rodapé da página.
            if (!ProducaoConfig.ExibirTotalEtiquetaNaoImpressaPainel)
                setores = Data.Helper.Utils.GetSetores.ToList();
            // Na MB Temper as etiquetas não impressas devem ser mostradas no rodapé;
            else
            {
                var setorEtiquetaNaoImpressa = new Setor();
                setorEtiquetaNaoImpressa.IdSetor = -1;
                setorEtiquetaNaoImpressa.Descricao = "Etiqueta Não Impressa";
                setorEtiquetaNaoImpressa.ExibirRelatorio = true;
                setorEtiquetaNaoImpressa.Tipo = TipoSetor.Pendente;

                setores.Add(setorEtiquetaNaoImpressa);
                setores.AddRange(SetorDAO.Instance.GetOrdered());
            }

            // Informa as mensagens que serão passadas no rodapé da página            
            foreach (var s in setores)
            {
                if (s.ExibirRelatorio && s.Tipo != TipoSetor.Entregue && s.Tipo != TipoSetor.ExpCarregamento)
                {
                    var prodDiaRealSet = new ProducaoDiariaRealizadaSetor(s.Descricao);

                    var mensagem = new System.Text.StringBuilder()
                        .AppendFormat("{0}: ", s.Descricao);

                    var pedidoProducaoDAO = ProdutoPedidoProducaoDAO.Instance;

                    // Verifica quais dados serão exibidos no rodapé do painel da produção, de acordo com a empresa.
                    if (PCPConfig.PainelProducao.ExibirTotalM2LidoNoDia)
                        prodDiaRealSet.TotRealizado = pedidoProducaoDAO
                            .ObtemTotM2LidoSetor(s.IdSetor, null, DateTime.Today, DateTime.Today.AddDays(1), true);

                    else if (PCPConfig.PainelProducao.ExibirTotalQtdeLidoNoDia)
                        prodDiaRealSet.TotRealizado = pedidoProducaoDAO
                            .ObtemTotM2LidoSetor(s.IdSetor, null, DateTime.Today, DateTime.Today.AddDays(1), false);

                    else if (OrdemCargaConfig.DataEntregaBaseConsiderarPedidoParaOC != null)
                        prodDiaRealSet.TotRealizado = pedidoProducaoDAO
                           .ObtemTotM2Setor(s.IdSetor, null, OrdemCargaConfig.DataEntregaBaseConsiderarPedidoParaOC,
                            DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd 23:59:59"), true);

                    else
                        prodDiaRealSet.TotRealizado = ProdutoPedidoProducaoDAO.Instance.ObtemTotM2Setor(s.IdSetor);

                    resultado.Add(prodDiaRealSet);
                }
            }

            return resultado;
        }

        public IList<ProducaoDiariaRealizada> ObtemDadosProducao(DateTime dataProducao)
        {
            return ObtemDadosProducao(0, dataProducao);
        }

        public IList<ProducaoDiariaRealizada> ObtemDadosProducao(uint idSetor, DateTime dataProducao)
        {
            string sql = Sql(idSetor, dataProducao);
            return objPersistence.LoadData(sql, new GDAParameter("?data", dataProducao)).ToList();
        }

        public IList<ProducaoDiariaRealizada> ObtemProducaoRealizada(List<DateTime> dataFabrica)
        {
            return objPersistence.LoadData(SqlProducaoRealizada(0, dataFabrica, false), GetParams(dataFabrica)).ToList();
        }

        public IList<ProducaoDiariaRealizada> ObtemProducaoRealizadaPorSetor(int idSetor, DateTime dataFabrica)
        {
            return objPersistence.LoadData(SqlProducaoRealizada(idSetor, new List<DateTime>() { dataFabrica }, true), GetParams(new List<DateTime>() { dataFabrica })).ToList();
        }

        private GDAParameter[] GetParams(List<DateTime> datasFabrica)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (datasFabrica != null && datasFabrica.Count > 0)
            {
                for (var i = 0; i < datasFabrica.Count; i++)
                {
                    lstParam.Add(new GDAParameter($"?data{i}", datasFabrica[i]));
                }
            }

            return lstParam.Count == 0 ? null : lstParam.ToArray();
        }
    }
}
