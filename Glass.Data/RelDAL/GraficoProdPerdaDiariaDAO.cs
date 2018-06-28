using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using Glass.Data.Model;
using GDA;
using System.Linq;
using Glass.Configuracoes;
using Colosoft;

namespace Glass.Data.RelDAL
{
    public sealed class GraficoProdPerdaDiariaDAO : BaseDAO<GraficoProdPerdaDiaria, GraficoProdPerdaDiariaDAO>
    {
        private string Sql(string idsSetor, string mes, string ano, bool selecionar)
        {
            var sqlTotalM2Producao = string.Empty;
            var sqlTotalM2Reposicao = string.Empty;
            var sqlTotalM2DadosReposicao = string.Empty;

            return Sql(idsSetor, mes, ano, selecionar, out sqlTotalM2Producao, out sqlTotalM2Reposicao, out sqlTotalM2DadosReposicao);
        }

        private string Sql(string idsSetor, string mes, string ano, bool selecionar, out string sqlTotalM2Producao, out string sqlTotalM2Reposicao,
            out string sqlTotalM2DadosReposicao)
        {
            var idsProdPed = ExecuteMultipleScalar<int>($@"SELECT DISTINCT(ppp.IdProdPed)
		        FROM produto_pedido_producao ppp
			        INNER JOIN leitura_producao lp ON (ppp.IdProdPedProducao=lp.IdProdPedProducao)
			        AND MONTH(lp.DataLeitura)={ mes }
                    AND YEAR(lp.DataLeitura)={ ano }");

            sqlTotalM2Producao = $@"SELECT DAY(ppp.DataLeitura) AS Dia,
                    ppp.IdSetor,
                    ppp.Descricao,
                    ROUND(SUM(ROUND(IF(TipoPedido = { (int)Pedido.TipoPedidoEnum.MaoDeObra },
                        ((((50 - IF(MOD(Altura, 50) > 0,
                            MOD(Altura, 50),
                            50)) + Altura) * ((50 - IF(MOD(Largura, 50) > 0,
                            MOD(Largura, 50),
                            50)) + Largura)) / 1000000) * a.Qtde,
                        TotM2Calc) / (pp.Qtde * IF(TipoPedido = { (int)Pedido.TipoPedidoEnum.MaoDeObra }, a.Qtde, 1)), 2)), 4) AS TotProdM2,
                    0 AS TotPerdaM2
                FROM pedido ped
	                INNER JOIN
		                (SELECT ppe.IdProdPed,
                            ppe.IdPedido,
                            ppe.IdAmbientePedido,
                            ppe.TotM2Calc,
                            ppe.Qtde
		                FROM produtos_pedido_espelho ppe
                        WHERE ppe.IdProdPed IN ({ string.Join(",", idsProdPed) })) pp ON (ped.IdPedido = pp.IdPedido)
	                INNER JOIN
		                (SELECT ppp.IdProdPedProducao,
                            ppp.IdProdPed,
                            ppp.Situacao,
                            lp.IdSetor,
                            s.Descricao,
                            lp.Dataleitura
		                FROM produto_pedido_producao ppp
			                INNER JOIN leitura_producao lp ON (ppp.IdProdPedProducao = lp.IdProdPedProducao)
			                INNER JOIN setor s ON (lp.IdSetor = s.IdSetor)
                        WHERE ppp.IdProdPed IN ({ string.Join(",", idsProdPed) })) ppp ON (pp.IdProdPed = ppp.IdProdPed)
                        LEFT JOIN
			                (SELECT ape.IdAmbientePedido,
				                ape.Altura,
				                ape.Largura,
				                ape.Qtde
			                FROM ambiente_pedido_espelho ape) a ON (pp.IdAmbientePedido = a.IdAmbientePedido)
                GROUP BY Dia
                ORDER BY DIA;";

            sqlTotalM2Reposicao = $@"SELECT DAY(ppp.DataRepos) AS Dia,
	                ppp.IdSetorRepos AS IdSetor,
	                sr.Descricao,
	                0 AS TotProdM2,
	                ROUND(SUM(ROUND(pp.TotM / (pp.Qtde * IF(ped.TipoPedido = { (int)Pedido.TipoPedidoEnum.MaoDeObra }, a.Qtde, 1)), 4)), 2) AS TotPerdaM2
                FROM produto_pedido_producao ppp
	                LEFT JOIN setor sr ON (ppp.IdSetorRepos = sr.IdSetor)
	                LEFT JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed = pp.IdProdPed)
	                LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido = a.IdAmbientePedido)
	                LEFT JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)
                WHERE ppp.PecaReposta = 1
                    AND MONTH(ppp.DataRepos)={ mes }
                    AND YEAR(ppp.DataRepos)={ ano }
                    AND sr.IdSetor In ({ idsSetor })
                GROUP BY Dia
                ORDER BY Dia;";

            sqlTotalM2DadosReposicao = $@"SELECT DAY(ppp.DataRepos) AS Dia,
	                dr.IdSetorRepos AS IdSetor,
	                sr.Descricao,
	                0 AS TotProdM2,
	                ROUND(SUM(ROUND(pp.TotM / (pp.Qtde * IF(ped.TipoPedido = { (int)Pedido.TipoPedidoEnum.MaoDeObra }, a.Qtde, 1)), 4)), 2) AS TotPerdaM2
                FROM produto_pedido_producao ppp
	                LEFT JOIN dados_reposicao dr ON (ppp.IdProdPedProducao = dr.IdProdPedProducao)
	                LEFT JOIN setor sr ON (dr.IdSetorRepos = sr.IdSetor)
	                LEFT JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed = pp.IdProdPed)
	                LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido = a.IdAmbientePedido)
	                LEFT JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)
                WHERE ppp.PecaReposta = 1
                    AND MONTH(ppp.DataRepos)={ mes }
                    AND YEAR(ppp.DataRepos)={ ano }
                    AND sr.IdSetor IN ({ idsSetor })
                GROUP BY Dia
                ORDER BY Dia;";

            var criterio = ObterCriterioProducaoPerdaDiaria(idsSetor, mes, ano);

            var retorno = $@"SELECT Dia, IdSetor, Descricao, SUM(TotProdM2), SUM(TotPerdaM2), { criterio } AS Criterio
                FROM ({ sqlTotalM2Producao } UNION { sqlTotalM2Reposicao } UNION { sqlTotalM2DadosReposicao }) AS temp
                GROUP BY Dia
                ORDER BY Dia;";

            if (!selecionar)
            {
                retorno = $"SELECT COUNT(*) FROM ({ retorno }) AS count";
            }

            return retorno;
        }

        private string ObterCriterioProducaoPerdaDiaria(string idsSetor, string mes, string ano)
        {
            return $"Setor: { SetorDAO.Instance.GetNomeSetores(idsSetor) }. Mês Referência: { mes.PadLeft(2, '0') }/{ ano }";
        }

        private string SqlProducao(DateTime? dataIni, DateTime? dataFim)
        {
            return SqlProducao(null, dataIni, dataFim);
        }

        private string SqlProducao(int? idClassificacao, DateTime? dataIni, DateTime? dataFim)
        {
            // Chamado 12993.
            // Caso um ou mais setores estejam configurados com o tipo "Por Roteiro" então será considerado que a empresa trabalha com a produção por roteiro.
            var producaoPorRoteiro = objPersistence.ExecuteSqlQueryCount(string.Format("SELECT COUNT(*) FROM setor s WHERE s.Tipo={0}", (int)TipoSetor.PorRoteiro)) > 0;

            if (producaoPorRoteiro)
            {
                return SqlProducaoPorRoteiro(idClassificacao, dataIni, dataFim);
            }
            else
            {
                return SqlProducaoIdSetorPronto(idClassificacao, dataIni, dataFim);
            }
        }

        private string SqlProducaoIdSetorPronto(int? idClassificacao, DateTime? dataIni, DateTime? dataFim)
        {
            // Verifica se a empresa considera como pronto o setor "Depósito".
            var idsSetorPronto = ExecuteScalar<string>(string.Format("SELECT GROUP_CONCAT(s.IdSetor) FROM setor s WHERE s.Tipo={0}", (int)TipoSetor.Pronto));
            var where = string.Empty;

            var sql = string.Format(@"SELECT Dia, IdSetor, '' AS Descricao, CAST(ROUND(SUM(TotM2), 2) AS DECIMAL(12, 2)) AS TotProdM2, 0 AS TotPerdaM2, '' AS Criterio, 0 AS DesafioPerda,
                    0 AS MetaPerda, 0 AS Espessura, '' AS CorVidro
                FROM (
                    SELECT ROUND(IF(ped.TipoPedido={0}, ((((50 - IF(MOD(a.Altura, 50) > 0, MOD(a.Altura, 50), 50)) + a.Altura) *
                        ((50 - IF(MOD(a.Largura, 50) > 0, MOD(a.Largura, 50), 50)) + a.Largura)) / 1000000) * a.Qtde, ppo.TotM2Calc) / (pp.Qtde * IF(ped.TipoPedido={0}, a.Qtde, 1)), 4) AS TotM2,
                        s.IdSetor, DAY(lp.DataLeitura) AS Dia
                FROM pedido ped
                    INNER JOIN produtos_pedido_espelho pp ON (ped.IdPedido = pp.IdPedido)
	                INNER JOIN produtos_pedido ppo On (pp.IdProdPed=ppo.IdProdPedEsp)
                    INNER JOIN produto_pedido_producao ppp ON (pp.IdProdPed = ppp.IdProdPed)
                    INNER JOIN setor s ON (ppp.IdSetor = s.IdSetor)
                    LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido = a.IdAmbientePedido)
                    LEFT JOIN leitura_producao lp ON (ppp.IdProdPedProducao=lp.IdProdPedProducao)
                WHERE ppp.Situacao IN ({2},{3}) {4} {5} {1}
                GROUP BY ppp.IdProdPedProducao) AS temp;",
                // Posição 7.
                (int)Pedido.TipoPedidoEnum.MaoDeObra,
                // Posição 1.
                "{0}",
                // Posição 2.
                (int)ProdutoPedidoProducao.SituacaoEnum.Producao,
                // Posição 3.
                (int)ProdutoPedidoProducao.SituacaoEnum.Perda,
                // Posição 4.
                idsSetorPronto.IsNullOrEmpty() ? string.Format(" AND ppp.SituacaoProducao = {0}", (int)SituacaoProdutoProducao.Pronto) : string.Format(" AND lp.IdSetor IN ({0}) ", idsSetorPronto),
                // Posição 5.
                idClassificacao > 0 ? string.Format(" AND pp.IdProcesso IN (SELECT IdProcesso FROM roteiro_producao WHERE IdClassificacaoRoteiroProducao={0}) ", idClassificacao.Value) : string.Empty);

            if (dataIni > DateTime.MinValue)
            {
                where += " AND lp.DataLeitura>=?dataIni";
            }

            if (dataFim > DateTime.MinValue)
            {
                where += " AND lp.DataLeitura<=?dataFim";
            }

            return string.Format(sql, where);
        }

        private string SqlProducaoPorRoteiro(int? idClassificacao, DateTime? dataIni, DateTime? dataFim)
        {
            var filtro = string.Empty;

            var sql = string.Format(@"SELECT Dia, IdSetor, '' AS Descricao, CAST(ROUND(SUM(TotM2), 2) AS DECIMAL(12, 2)) AS TotProdM2, 0 AS TotPerdaM2, '' AS Criterio, 0 AS DesafioPerda,
                    0 AS MetaPerda, 0 AS Espessura, '' AS CorVidro
                FROM (
                    SELECT ROUND(IF(ped.TipoPedido={0}, ((((50 - IF(MOD(a.Altura, 50) > 0, MOD(a.Altura, 50), 50)) + a.Altura) *
                        ((50 - IF(MOD(a.Largura, 50) > 0, MOD(a.Largura, 50), 50)) + a.Largura)) / 1000000) * a.Qtde, ppo.TotM2Calc) / (pp.Qtde * IF(ped.TipoPedido={0}, a.Qtde, 1)), 4) AS TotM2,
                        lpr.IdSetor, DAY(lpr.DataLeitura) AS Dia
                FROM pedido ped
                    INNER JOIN produtos_pedido_espelho pp ON (ped.IdPedido=pp.IdPedido)
	                INNER JOIN produtos_pedido ppo On (pp.IdProdPed=ppo.IdProdPedEsp)
                    INNER JOIN
                        (
                            SELECT ppp.IdProdPedProducao, ppp.IdProdPed
                            FROM produto_pedido_producao ppp
                                INNER JOIN leitura_producao lp ON (ppp.IdProdPedProducao=lp.IdProdPedProducao)
                            WHERE ppp.Situacao IN ({2},{3}) AND lp.ProntoRoteiro IS NOT NULL AND lp.ProntoRoteiro=1 {1}
                        ) ppp ON (pp.IdProdPed=ppp.IdProdPed)
                    LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido=a.IdAmbientePedido)
                    INNER JOIN leitura_producao lpr ON (ppp.IdProdPedProducao=lpr.IdProdPedProducao )
                WHERE 1 {4}
                GROUP BY ppp.IdProdPedProducao) AS temp;",
                // Posição 0.
                (int)Pedido.TipoPedidoEnum.MaoDeObra,
                // Posição 1.
                "{0}",
                // Posição 2.
                (int)ProdutoPedidoProducao.SituacaoEnum.Producao,
                // Posição 3.
                (int)ProdutoPedidoProducao.SituacaoEnum.Perda,
                // Posição 4.
                idClassificacao > 0 ? string.Format(" AND pp.IdProcesso IN (SELECT IdProcesso FROM roteiro_producao WHERE IdClassificacaoRoteiroProducao={0}) ", idClassificacao.Value) : string.Empty);

            if (dataIni > DateTime.MinValue)
            {
                filtro += " AND lp.DataLeitura>=?dataIni";
            }

            if (dataFim > DateTime.MinValue)
            {
                filtro += " AND lp.DataLeitura<=?dataFim";
            }

            return string.Format(sql, filtro);
        }

        private string SqlPerdaSetores(string mes, string ano, bool selecionar)
        {
            // ATENÇÃO: Ao alterar este sql, alterar também o sql do método Sql(), pois devem buscar dados idênticos, por menor que seja a alteração,
            // modificar os dois sqls.

            string sql = @"
                Select idSetor,
	                descricao,
	                Cast(0 as decimal(12, 2)) As TotProdM2,
	                Round(Sum(TotM2), 2) As TotPerdaM2,
		            desafioPerda,
		            metaPerda,
	                1 As Dia,
                    0 As espessura,
                    '' As corVidro
                From (Select Day(ppp.dataRepos) As Dia,
		                ppp.idSetorRepos As idSetor,
		                sr.descricao As descricao,
		                sr.desafioPerda,
		                sr.metaPerda,
		                Round(pp.TotM/(pp.qtde*If(ped.tipoPedido=3, a.qtde, 1)), 4) As TotM2
	                From produto_pedido_producao ppp
                    /*Alterei de Inner Join para Left Join e o tempo de execução do sql diminuiu considerávelmente, o resultado foi o mesmo*/
		                Left Join setor sr ON (ppp.idSetorRepos=sr.idSetor)
		                Left Join produtos_pedido_espelho pp ON (ppp.idProdPed=pp.idProdPed) 
		                Left Join ambiente_pedido_espelho a ON (pp.idAmbientePedido=a.idAmbientePedido) 
		                Left Join pedido ped ON (pp.idPedido=ped.idPedido)
                    /*Ao alterar de Inner Join para Left Join estava sendo buscado um setor nulo, devido à isso eu coloquei esta condição para que sejam buscados somente setores existentes*/
	                Where sr.idSetor > 0
                        And ppp.pecaReposta = True And Month(ppp.dataRepos) = " + mes + @" And Year(ppp.dataRepos) = " + ano + @"
	            Union All /* Incluímos o All para resolver o chamado 21143, no qual perdas de peças diferentes mas com metragem igual estava retornando apenas uma delas */
	            Select Day(ppp.dataRepos) As Dia,
		            dr.idSetorRepos as idSetor,
		            sr.descricao as descricao,
		            sr.desafioPerda,
		            sr.metaPerda,
		            Round(pp.TotM/(pp.qtde*If(ped.tipoPedido=3, a.qtde, 1)), 4) As TotM2
	            From produto_pedido_producao ppp
                    /*Alterei de Inner Join para Left Join e o tempo de execução do sql diminuiu considerávelmente, o resultado foi o mesmo*/
		            Left Join dados_reposicao dr ON (ppp.idProdPedProducao=dr.idProdPedProducao)
		            Left Join setor sr ON (dr.idSetorRepos=sr.idSetor)
		            Left Join produtos_pedido_espelho pp ON (ppp.idProdPed=pp.idProdPed) 
		            Left Join ambiente_pedido_espelho a ON (pp.idAmbientePedido=a.idAmbientePedido) 
		            Left Join pedido ped ON (pp.idPedido=ped.idPedido) 
                /*Ao alterar de Inner Join para Left Join estava sendo buscado um setor nulo, devido à isso eu coloquei esta condição para que sejam buscados somente setores existentes*/
	            Where sr.idSetor > 0
                    And ppp.pecaReposta=True And Month(dr.dataRepos) = " + mes + @" And Year(dr.dataRepos) = " + ano + @"
	            ) As tb_result
            Group By 1
            Order By 1";

            if (!selecionar)
                return "Select Count(*) From (" + sql + ") As tbResultCount";

            return sql;
        }

        private string SqlPerdaPorProduto(int idSetor, string mes, string ano, bool selecionar, bool incluirTotM2, bool incluirTrocaDevolucao, bool somenteTrocaDevolucao)
        {
            var sqlTotM2 = incluirTotM2 ? @"
                    select idSetor, descricao, desafioPerda, metaPerda, TotProdM2, 0 as TotPerdaM2, espessura, corVidro
                    from (
                        select ltpd.idSetor, stor.descricao, stor.desafioPerda, stor.metaPerda,
	                        sum(ltpd.totProdM2) as TotProdM2, 0 as TotPerdaM2, prod.espessura, crvd.descricao as corVidro
                        from (
	                        select pdpe.idProd, temp.idSetor, sum(pdpe.TotM / pdpe.qtde) as totProdM2
	                        from (
		                        select idProdPedProducao, idSetor
		                        from leitura_producao
                                where dataLeitura >= ?dataIni 
                                    and dataLeitura <= ?dataFim " +
                                    (idSetor > 0 ? " and idSetor = " + idSetor.ToString() : "").ToString() + @"
                            ) as temp
		                        inner join produto_pedido_producao pdpp on (temp.idProdPedProducao = pdpp.idProdPedProducao)
		                        inner join produtos_pedido_espelho pdpe on (pdpe.idProdPed = pdpp.idProdPed)
	                        group by pdpe.idProd, temp.idSetor
                        ) as ltpd
	                        inner join setor stor on (stor.idSetor = ltpd.idSetor)
	                        inner join produto prod on (ltpd.idProd = prod.idProd)
	                        left join cor_vidro crvd on (prod.idCorVidro = crvd.idCorVidro)
                        group by prod.espessura, crvd.descricao
                        having corVidro is not null
                        order by prod.espessura
                    ) as tbResultProd

                    UNION ALL" : "";

            var sqlTrocaDevolucao = incluirTrocaDevolucao || somenteTrocaDevolucao ? @"
		                        select * from
		                        (select if(td.tipo=1, 1, 2) as idSetor
			                         , if(td.tipo=1, 'Troca', 'Devolução') as descricao
			                         , 0 as desafioPerda
			                         , 0 as metaPerda
			                         , sum(pt.TotM) as TotM2
			                         , p.espessura
			                         , crvd.descricao as corVidro
			                        from produto_trocado pt
				                        inner join troca_devolucao td on (pt.idTrocaDevolucao=td.idTrocaDevolucao)
				                        left join pedido ped On (td.idPedido=ped.idPedido) 
				                        left join produtos_pedido pp on (pt.idProdPed=pp.idProdPed)
				                        left Join ambiente_pedido a On (pp.idAmbientePedido=a.idAmbientePedido) 
				                        left Join liberarpedido lp On (lp.idLiberarPedido=ped.idLiberarPedido) 
				                        left Join cliente cli On (ped.idCli=cli.id_Cli) 
				                        left Join produto p On (pt.idProd=p.idProd)
				                        left Join etiqueta_aplicacao apl On (pt.idAplicacao=apl.idAplicacao) 
				                        left Join etiqueta_processo prc On (pt.idProcesso=prc.idProcesso) 
				                        left Join funcionario f on (td.idFunc=f.idFunc)
				                        left join cor_vidro crvd on(crvd.idCorVidro = p.idCorVidro)
			                        where td.situacao=2 And td.idTipoPerda is not null
			                          And td.dataTroca >= ?dataIni
			                          And td.dataTroca <= ?dataFim " +
                                      (idSetor > 0 ? " And td.tipo = " + idSetor.ToString() : "").ToString() + @"
			                        group by p.espessura, crvd.descricao
			                        order by p.espessura) as tbPerda1" : "";

            var sqlPerda = @"select * from
		                        (select dr.idSetorRepos as IdSetor
			                         , sr.descricao as descricao
			                         , sr.desafioPerda
			                         , sr.metaPerda
			                         , sum(round(pp.TotM/(pp.qtde*if(ped.tipoPedido=3, a.qtde, 1)), 4)) as TotM2
			                         , p.espessura
			                         , crvd.descricao as corVidro
			                        from produto_pedido_producao ppp
				                        Inner Join dados_reposicao dr on (ppp.idProdPedProducao=dr.idProdPedProducao)
				                        Left Join setor s On (ppp.idSetor=s.idSetor)
				                        Left Join setor sr On (dr.idSetorRepos=sr.idSetor)
				                        Inner Join produtos_pedido_espelho pp On (ppp.idProdPed=pp.idProdPed) 
				                        Left Join ambiente_pedido_espelho a On (pp.idAmbientePedido=a.idAmbientePedido) 
				                        Inner Join pedido ped On (pp.idPedido=ped.idPedido) 
				                        Left Join liberarpedido lp On (lp.idLiberarPedido=ped.idLiberarPedido) 
				                        Inner Join cliente cli On (ped.idCli=cli.id_Cli) 
				                        Inner Join produto p On (pp.idProd=p.idProd)
				                        Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao) 
				                        Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso) 
				                        Inner Join funcionario f on (dr.idFuncRepos=f.idFunc)
				                        left join cor_vidro crvd on (crvd.idCorVidro = p.idCorVidro)
			                        where ppp.pecaReposta=true
			                          and dr.dataRepos >= ?dataIni
			                          and dr.dataRepos <= ?dataFim " +
                                      (idSetor > 0 ? " and dr.idSetorRepos = " + idSetor.ToString() : "").ToString() + @"
		                        group by p.espessura, crvd.idCorVidro
		                        order by p.espessura) as tbPerda2

		                        UNION ALL

		                        select * from
		                        (select ppp.idSetorRepos
			                         , sr.descricao as descricao
			                         , sr.desafioPerda
			                         , sr.metaPerda
			                         , sum(round(pp.TotM/(pp.qtde*if(ped.tipoPedido=3, a.qtde, 1)), 4)) as TotM2
			                         , p.espessura
			                         , crvd.descricao as CorVidro
			                        from produto_pedido_producao ppp
				                        Left Join setor s On (ppp.idSetor=s.idSetor)
				                        Left Join setor sr On (ppp.idSetorRepos=sr.idSetor)
				                        Inner Join produtos_pedido_espelho pp On (ppp.idProdPed=pp.idProdPed) 
				                        Left Join ambiente_pedido_espelho a On (pp.idAmbientePedido=a.idAmbientePedido) 
				                        Inner Join pedido ped On (pp.idPedido=ped.idPedido) 
				                        Left Join liberarpedido lp On (lp.idLiberarPedido=ped.idLiberarPedido) 
				                        Inner Join cliente cli On (ped.idCli=cli.id_Cli) 
				                        Inner Join produto p On (pp.idProd=p.idProd)
				                        Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao) 
				                        Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso) 
				                        Inner Join funcionario f on (ppp.idFuncRepos=f.idFunc)
				                        left join cor_vidro crvd on (crvd.idCorVidro = p.idCorVidro)
			                        where ppp.pecaReposta=true
			                          And ppp.dataRepos >= ?dataIni
			                          And ppp.dataRepos <= ?dataFim " +
                                      (idSetor > 0 ? " and ppp.idSetorRepos = " + idSetor.ToString() : "").ToString() + @"
			                        group by p.espessura, crvd.idCorVidro
			                        order by p.espessura) as tbPerda3";

            string sql = @"
                select cast(idSetor as unsigned) as idSetor, descricao, desafioPerda, metaPerda, 1 as Dia, 
                    Cast(round(sum(TotProdM2), 2) as decimal(12,2)) as TotProdM2, sum(TotPerdaM2) as TotPerdaM2, espessura, corVidro 
                from(
                    " + (!somenteTrocaDevolucao ? sqlTotM2 : "") + @"

                    select * from (
                        select idSetor, descricao, desafioPerda, metaPerda, 0 as TotProdM2, sum(TotM2) as TotPerdaM2, espessura, corVidro
	                        from( " + sqlTrocaDevolucao + (!somenteTrocaDevolucao ? (!string.IsNullOrEmpty(sqlTrocaDevolucao) ? " UNION ALL " : "") + sqlPerda : "") + @" ) as tbPerdaTotal
	                        group by espessura, corVidro
	                        order by espessura
                        ) as tbResultPerda
                    ) as ResultFinal
                    group by espessura, corVidro
                    order by espessura;";

            if (!selecionar)
                return "select count(*) from (" + sql + ")";

            return sql;
        }

        private string SqlTotalPerda(bool incluirTrocaDevolucao, bool somenteTrocaDevolucao)
        {
            return SqlTotalPerda(null, incluirTrocaDevolucao, somenteTrocaDevolucao);
        }

        private string SqlTotalPerda(int? idClassificacao, bool incluirTrocaDevolucao, bool somenteTrocaDevolucao)
        {
            var sqlTrocaDevolucao =
                incluirTrocaDevolucao || somenteTrocaDevolucao ?
                    string.Format(
                        @"SELECT * FROM
                            (SELECT SUM(pt.TotM) AS TotM2, YEAR(td.DataTroca) AS Ano, MONTH(td.DataTroca) AS Mes
                            FROM produto_trocado pt
                                INNER JOIN troca_devolucao td ON(pt.IdTrocaDevolucao = td.IdTrocaDevolucao)
                                LEFT JOIN tipo_perda tp ON (td.IdTipoPerda=tp.IdTipoPerda)
                            WHERE td.Situacao={0}
                                AND td.IdTipoPerda IS NOT NULL
                                AND (td.IdTipoPerda IS NULL OR tp.ExibirPainelProducao)
                                AND td.DataTroca>=?dataIni
                                AND td.DataTroca<=?dataFim " +

                                /* Chamado 45622. */
                                (idClassificacao > 0 ? string.Format(" AND pt.IdProcesso IN (SELECT IdProcesso FROM roteiro_producao WHERE IdClassificacaoRoteiroProducao={0}) ", idClassificacao.Value) : string.Empty) +

                            @" GROUP BY YEAR(td.DataTroca), MONTH(td.DataTroca))
                        AS tbPerda3", (int)TrocaDevolucao.SituacaoTrocaDev.Finalizada) :
                    string.Empty;

            var sqlPerdaChapaVidro =
                PCPConfig.PainelProducao.ContabilizarPerdaChapaVidroNoTotalDePerdaPainel ?
                    @"UNION ALL

                    SELECT * FROM
                        (SELECT SUM(ROUND(pnf.TotM / pnf.Qtde, 4)) AS TotM2, YEAR(pcv.DataPerda) AS Ano, MONTH(pcv.DataPerda) AS Mes
                        FROM perda_chapa_vidro pcv
                            INNER JOIN produto_impressao pi ON (pcv.IdProdImpressao=pi.IdProdImpressao)
                            INNER JOIN produtos_nf pnf ON (pi.IdProdNf=pnf.IdProdNf)
                            LEFT JOIN tipo_perda tp ON (pcv.IdTipoPerda=tp.IdTipoPerda)
                        WHERE !pcv.Cancelado
                            AND (pcv.IdTipoPerda IS NULL OR tp.ExibirPainelProducao)
                            AND pcv.DataPerda>=?dataIni
                            AND pcv.DataPerda<=?dataFim " +

                            /* Chamado 45622. */
                            (idClassificacao > 0 ? string.Format(@" AND pi.IdProdPed IN
                                (SELECT IdProdPed FROM produtos_pedido_espelho WHERE IdProcesso IN (SELECT IdProcesso FROM roteiro_producao WHERE IdClassificacaoRoteiroProducao={0})) ", idClassificacao.Value) : string.Empty) +

                        @" GROUP BY YEAR(pcv.DataPerda), MONTH(pcv.DataPerda))
                    AS tbPerda4" : string.Empty;

            var sqlPerda =
                string.Format(
                    @"SELECT * FROM 
                        (SELECT SUM(ROUND(pp.TotM/(pp.Qtde*IF(ped.TipoPedido={0}, a.Qtde, 1)), 4)) AS TotM2, YEAR(ppp.DataRepos) AS Ano, MONTH(ppp.DataRepos) AS Mes
                        FROM produto_pedido_producao ppp
                            INNER JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed=pp.IdProdPed) 
                            LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido=a.IdAmbientePedido) 
                            INNER JOIN pedido ped ON (pp.IdPedido=ped.IdPedido)
                            LEFT JOIN tipo_perda tp ON (ppp.TipoPerda=tp.IdTipoPerda)
                        WHERE ppp.PecaReposta=TRUE
                            /* Necessário porque nem todas as perdas tem tipo */
                            AND (ppp.tipoperda IS NULL OR tp.ExibirPainelProducao)
                            AND ppp.DataRepos>=?dataIni
                            AND ppp.DataRepos<=?dataFim " +

                            /* Chamado 45622. */
                            (idClassificacao > 0 ? string.Format(" AND pp.IdProcesso IN (SELECT IdProcesso FROM roteiro_producao WHERE IdClassificacaoRoteiroProducao={0}) ", idClassificacao.Value) : string.Empty) +

                        @" GROUP BY YEAR(ppp.DataRepos), MONTH(ppp.DataRepos))
                    AS tbPerda1
                
                    UNION ALL

                    SELECT * FROM
                        (SELECT SUM(ROUND(pp.TotM/(pp.Qtde*IF(ped.TipoPedido={0}, a.Qtde, 1)), 4)) AS TotM2, YEAR(dr.DataRepos) AS Ano, MONTH(dr.DataRepos) AS Mes
                        FROM produto_pedido_producao ppp
                            INNER JOIN dados_reposicao dr ON (ppp.IdProdPedProducao=dr.IdProdPedProducao)
                            INNER JOIN produtos_pedido_espelho pp ON (ppp.IdProdPed=pp.IdProdPed)
                            LEFT JOIN ambiente_pedido_espelho a ON (pp.IdAmbientePedido=a.IdAmbientePedido)
                            INNER JOIN pedido ped ON (pp.IdPedido=ped.IdPedido)
                            LEFT JOIN tipo_perda tp ON (dr.TipoPerdaRepos=tp.IdTipoPerda)
                        WHERE ppp.PecaReposta=TRUE
                            AND (dr.TipoPerdaRepos IS NULL OR tp.ExibirPainelProducao)
                            AND dr.DataRepos>=?dataIni
                            AND dr.DataRepos<=?dataFim " +

                            /* Chamado 45622. */
                            (idClassificacao > 0 ? string.Format(" AND pp.IdProcesso IN (SELECT IdProcesso FROM roteiro_producao WHERE IdClassificacaoRoteiroProducao={0}) ", idClassificacao.Value) : string.Empty) +

                        @" GROUP BY YEAR(ppp.DataRepos), MONTH(ppp.DataRepos))
                    AS tbPerda2 {1} {2}",
                    (int)Pedido.TipoPedidoEnum.MaoDeObra, (sqlTrocaDevolucao.IsNullOrEmpty() ? string.Empty : (" UNION ALL " + sqlTrocaDevolucao)), sqlPerdaChapaVidro);
            
            var sql = string.Format("SELECT SUM(TotM2) AS TotPerdaM2, Ano, Mes FROM ({0}) AS ResultFinal GROUP BY Ano, Mes",
                somenteTrocaDevolucao ? sqlTrocaDevolucao : sqlPerda);

            return sql;
        }

        public IList<GraficoProdPerdaDiaria> GetList(string idsSetor, string mes, string ano, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idsSetor, mes, ano, true), sortExpression, startRow, pageSize).ToList();
        }

        public int GetCount(string idsSetor, string mes, string ano)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idsSetor, mes, ano, false));
        }

        public GraficoProdPerdaDiaria[] GetForRpt(string idsSetor, string mes, string ano)
        {
            var sqlTotalM2Producao = string.Empty;
            var sqlTotalM2Reposicao = string.Empty;
            var sqlTotalM2DadosReposicao = string.Empty;

            Sql(idsSetor, mes, ano, true, out sqlTotalM2Producao, out sqlTotalM2Reposicao, out sqlTotalM2DadosReposicao);

            var totalM2Producao = objPersistence.LoadData(sqlTotalM2Producao)?.ToList() ?? new List<GraficoProdPerdaDiaria>();
            var totalM2Reposicao = objPersistence.LoadData(sqlTotalM2Reposicao)?.ToList() ?? new List<GraficoProdPerdaDiaria>();
            var totalM2DadosReposicao = objPersistence.LoadData(sqlTotalM2DadosReposicao)?.ToList() ?? new List<GraficoProdPerdaDiaria>();
            var diasBuscados = new List<int>();
            var retorno = new List<GraficoProdPerdaDiaria>();

            if (totalM2Producao.Any(f => f.Dia > 0))
            {
                diasBuscados.AddRange(totalM2Producao.Where(f => f.Dia > 0).Select(f => (int)f.Dia).Distinct().ToList());
            }

            if (totalM2Reposicao.Any(f => f.Dia > 0))
            {
                diasBuscados.AddRange(totalM2Reposicao.Where(f => f.Dia > 0).Select(f => (int)f.Dia).Distinct().ToList());
            }

            if (totalM2DadosReposicao.Any(f => f.Dia > 0))
            {
                diasBuscados.AddRange(totalM2DadosReposicao.Where(f => f.Dia > 0).Select(f => (int)f.Dia).Distinct().ToList());
            }

            foreach (var dia in diasBuscados.Distinct())
            {
                var dadosDia = new GraficoProdPerdaDiaria();
                dadosDia.Dia = dia;

                if (totalM2Producao.Any(f => f.Dia == dia))
                {
                    dadosDia.TotProdM2 = totalM2Producao.FirstOrDefault(f => f.Dia == dia)?.TotProdM2 ?? 0;
                    dadosDia.TotPerdaM2 = totalM2Producao.FirstOrDefault(f => f.Dia == dia)?.TotPerdaM2 ?? 0;
                    dadosDia.DescricaoSetor = totalM2Producao.FirstOrDefault(f => f.Dia == dia)?.DescricaoSetor ?? dadosDia.DescricaoSetor;
                }

                if (totalM2Reposicao.Any(f => f.Dia == dia))
                {
                    dadosDia.TotProdM2 = totalM2Reposicao.FirstOrDefault(f => f.Dia == dia)?.TotProdM2 ?? 0;
                    dadosDia.TotPerdaM2 = totalM2Reposicao.FirstOrDefault(f => f.Dia == dia)?.TotPerdaM2 ?? 0;
                    dadosDia.DescricaoSetor = totalM2Reposicao.FirstOrDefault(f => f.Dia == dia)?.DescricaoSetor ?? dadosDia.DescricaoSetor;
                }

                if (totalM2DadosReposicao.Any(f => f.Dia == dia))
                {
                    dadosDia.TotProdM2 = totalM2DadosReposicao.FirstOrDefault(f => f.Dia == dia)?.TotProdM2 ?? 0;
                    dadosDia.TotPerdaM2 = totalM2DadosReposicao.FirstOrDefault(f => f.Dia == dia)?.TotPerdaM2 ?? 0;
                    dadosDia.DescricaoSetor = totalM2DadosReposicao.FirstOrDefault(f => f.Dia == dia)?.DescricaoSetor ?? dadosDia.DescricaoSetor;
                }

                retorno.Add(dadosDia);
            }

            int diasDecorridos = 0;
            double prodAcumulada = 0;
            double perdaAcumulada = 0;

            if (diasBuscados.Count > 0)
            {
                for (var i = 0; i < diasBuscados.Count; i++)
                {
                    diasDecorridos++;
                    prodAcumulada += retorno[i].TotProdM2;
                    perdaAcumulada += retorno[i].TotPerdaM2;

                    retorno[i].ProducaoAcumulada = prodAcumulada;
                    retorno[i].PerdaAcumulada = perdaAcumulada;
                    retorno[i].MediaDiariaProducao = prodAcumulada / diasDecorridos;
                }
            }

            return retorno.ToArray();
        }

        public GraficoProdPerdaDiaria GetProdPerdaDiaClassificacao(int idClassificacao)
        {
            var retorno = new GraficoProdPerdaDiaria();

            if (idClassificacao == 0)
                return retorno;

            var dados = objPersistence.LoadData(SqlProducao(idClassificacao, DateTime.Today, DateTime.Today),
                GetParams(DateTime.Today.ToShortDateString(), DateTime.Today.ToShortDateString())).ToList();
            var prodAcumulada = 0.0;

            if (dados.Count > 0)
            {
                for (var i = 0; i < dados.Count; i++)
                {
                    prodAcumulada += dados[i].TotProdM2;
                    dados[i].ProducaoAcumulada = prodAcumulada;
                    retorno = dados[i];
                }
            }

            return retorno;
        }

        public GraficoProdPerdaDiaria GetProdPerdaDia()
        {
            var retorno = new GraficoProdPerdaDiaria();
            var dados = objPersistence.LoadData(SqlProducao(DateTime.Today, DateTime.Today), GetParams(DateTime.Today.ToShortDateString(), DateTime.Today.ToShortDateString())).ToList();
            var prodAcumulada = 0.0;

            if (dados.Count > 0)
            {
                for (var i = 0; i < dados.Count; i++)
                {
                    prodAcumulada += dados[i].TotProdM2;
                    dados[i].ProducaoAcumulada = prodAcumulada;
                    retorno = dados[i];
                }
            }

            return retorno;
        }

        public decimal GetTotM2ProducaoMensalClassificacao(int mes, int ano, int idClassificacao)
        {
            if (idClassificacao == 0)
                return 0;

            var dtini = new DateTime(ano, mes, 1);
            var dtFim = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));

            return Convert.ToDecimal(objPersistence.LoadData(SqlProducao(idClassificacao, dtini, dtFim),
                GetParams(dtini.ToShortDateString(), dtFim.ToShortDateString())).ToList()
                .Sum(f => f.TotProdM2));
        }

        public decimal GetTotM2ProducaoMensal(int mes, int ano)
        {
            var dtini = new DateTime(ano, mes, 1);
            var dtFim = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes));

            var sql = SqlProducao(dtini, dtFim);

            return Convert.ToDecimal(objPersistence.LoadData(SqlProducao(dtini, dtFim), GetParams(dtini.ToShortDateString(), dtFim.ToShortDateString())).ToList()
                .Sum(f => f.TotProdM2));
        }

        public GraficoProdPerdaDiaria[] GetPerdaSetores(int setor, string mes, string ano)
        {
            var retorno = new List<GraficoProdPerdaDiaria>(objPersistence.LoadData(SqlPerdaSetores(mes, ano, true)).ToList());
            return retorno.ToArray();
        }

        public Dictionary<string, double> GetPerdaMensal(string mes, string ano)
        {
            Dictionary<string, double> dctGrafico = new Dictionary<string, double>();

            double perdaAcumulada = 0;
            List<GraficoProdPerdaDiaria> retorno;

            DateTime mesAtual = new DateTime(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes), 5);

            for (int i = 3; i >= 0; i--)
            {
                DateTime mesRel = mesAtual.AddMonths(-i);
                perdaAcumulada = 0;
                retorno = objPersistence.LoadData(SqlPerdaSetores(mesRel.Month.ToString(), mesRel.Year.ToString(), true)).ToList();

                foreach (GraficoProdPerdaDiaria objPerda in retorno)
                    perdaAcumulada += objPerda.TotPerdaM2;

                dctGrafico.Add(mesRel.ToString("MMMM").ToUpper(), Math.Round(perdaAcumulada, 4));
            }

            return dctGrafico;
        }

        /// <summary>
        /// Obtém a quantidade de peças perdidas dos últimos 4 meses (Criado para o Painel de produção classificação).
        /// </summary>
        /// <param name="mes"> Último mês a ser considerado do período de 4 meses de perda, ex.: para saber a perda de Julho, Agosto, Setembro e Outubro somente o mês de Outubro deve ser passado como parâmetro</param>
        /// <param name="ano"> Ano do último mês a ser considerado.</param>
        /// <returns></returns>
        public Dictionary<string, double> GetPerdaclassificacao(string mes, string ano, bool incluirTrocaDevolucao, bool somenteTrocaDevolucao, int idClassificacao)
        {
            var retorno = new Dictionary<string, double>();

            if (idClassificacao == 0)
                return retorno;

            var sql = SqlTotalPerda(idClassificacao, incluirTrocaDevolucao, somenteTrocaDevolucao);
            
            for (var i = 2; i >= 0; i--)
            {
                var dataInicial = new DateTime(ano.StrParaInt(), mes.StrParaInt(), 1).AddMonths(-i);
                var dataFinal =
                    new DateTime(dataInicial.Year, dataInicial.Month,
                        GetLastDay(dataInicial.Month, dataInicial.Year));

                retorno.Add(
                    dataInicial.ToString("MMMM").ToUpper(),
                    objPersistence.LoadData(sql,
                        GetParams(dataInicial.ToString("yyyy-MM-dd"), dataFinal.ToString("yyyy-MM-dd"))).ToList()
                        .Sum(f => f.TotPerdaM2));
            }

            return retorno;
        }

        /// <summary>
        /// Obtém a quantidade de peças perdidas dos últimos 4 meses (Criado para o Painel de produção).
        /// </summary>
        /// <param name="mes"> Último mês a ser considerado do período de 4 meses de perda, ex.: para saber a perda de Julho, Agosto, Setembro e Outubro somente o mês de Outubro deve ser passado como parâmetro</param>
        /// <param name="ano"> Ano do último mês a ser considerado.</param>
        /// <param name="incluirTrocaDevolucao"></param>
        /// <param name="somenteTrocaDevolucao"></param>
        /// <returns></returns>
        public Dictionary<string, double> GetPerda(string mes, string ano, bool incluirTrocaDevolucao, bool somenteTrocaDevolucao)
        {
            var sql = SqlTotalPerda(incluirTrocaDevolucao, somenteTrocaDevolucao);

            var retorno = new Dictionary<string, double>();

            for (var i = 2; i >= 0; i--)
            {
                var dataInicial = new DateTime(ano.StrParaInt(), mes.StrParaInt(), 1).AddMonths(-i);
                var dataFinal =
                    new DateTime(dataInicial.Year, dataInicial.Month,
                        GetLastDay(dataInicial.Month, dataInicial.Year));

                retorno.Add(
                    dataInicial.ToString("MMMM").ToUpper(),
                    objPersistence.LoadData(sql,
                        GetParams(dataInicial.ToString("yyyy-MM-dd"), dataFinal.ToString("yyyy-MM-dd"))).ToList()
                        .Sum(f => f.TotPerdaM2));
            }

            return retorno;
        }

        public void GetPerdaProduto(int setor, bool incluirTrocaDevolucao, string mes, string ano, out GraficoProdPerdaDiaria[] lst6mm, out GraficoProdPerdaDiaria[] lst8mm,
            out GraficoProdPerdaDiaria[] lst10mm)
        {
            string dataIni = new DateTime(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes), 1).ToString("yyyy-MM-dd");
            string dataFim = new DateTime(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes), this.GetLastDay(Glass.Conversoes.StrParaInt(mes), Glass.Conversoes.StrParaInt(ano))).ToString("yyyy-MM-dd");

            var retorno = new List<GraficoProdPerdaDiaria>(objPersistence.LoadData(SqlPerdaPorProduto(setor, mes, ano, true, true, incluirTrocaDevolucao, false), GetParams(dataIni, dataFim)).ToList());

            var lstAux6mm = new List<GraficoProdPerdaDiaria>();
            var lstAux8mm = new List<GraficoProdPerdaDiaria>();
            var lstAux10mm = new List<GraficoProdPerdaDiaria>();

            foreach (GraficoProdPerdaDiaria objPerda in retorno)
            {
                if (objPerda.EspessuraVidro == 6)
                    lstAux6mm.Add(objPerda);
                else if (objPerda.EspessuraVidro == 8)
                    lstAux8mm.Add(objPerda);
                else if (objPerda.EspessuraVidro == 10)
                    lstAux10mm.Add(objPerda);
            }

            lst6mm = lstAux6mm.ToArray();
            lst8mm = lstAux8mm.ToArray();
            lst10mm = lstAux10mm.ToArray();
        }

        public IList<GraficoProdPerdaDiaria> GetPerdaProduto(int setor, bool incluirTrocaDevolucao, string mes, string ano)
        {
            string dataIni = new DateTime(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes), 1).ToString("yyyy-MM-dd");
            string dataFim = new DateTime(Glass.Conversoes.StrParaInt(ano), Glass.Conversoes.StrParaInt(mes), this.GetLastDay(Glass.Conversoes.StrParaInt(mes), Glass.Conversoes.StrParaInt(ano))).ToString("yyyy-MM-dd");

            return objPersistence.LoadData(SqlPerdaPorProduto(setor, mes, ano, true, true, incluirTrocaDevolucao, false), GetParams(dataIni, dataFim)).ToList();
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

        private int GetLastDay(int mes, int ano)
        {
            if (mes < 8)
            {
                if (mes == 2)
                {
                    if (ano % 4 == 0)
                        return 29;
                    else
                        return 28;
                }

                if (mes % 2 == 0)
                    return 30;
                else
                    return 31;
            }
            else if (mes >= 8)
            {
                if (mes % 2 == 0)
                    return 31;
                else
                    return 30;
            }
            else
                return 1;
        }
    }
}