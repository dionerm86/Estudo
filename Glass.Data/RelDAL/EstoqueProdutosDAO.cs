using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using GDA;
using Glass.Data.Model;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class EstoqueProdutosDAO : BaseDAO<EstoqueProdutos, EstoqueProdutosDAO>
    {
        private string Sql(string idsProd, int idLoja, uint idProd, string codInterno, string descricao, int idGrupo, int idSubgrupo, 
            int ordenar, string dataIni, string dataFim, string dataIniLib, string dataFimLib, bool forRpt, bool selecionar, bool atualizar, 
            bool incluirLiberacao, bool forAtualizacao, int tipoColunas, out GDAParameter[] parametros)
        {
            List<GDAParameter> lstParams = new List<GDAParameter>();
            string criterio = "";

            string sqlProd = "";
            bool semProduto = false;

            if (!string.IsNullOrEmpty(idsProd))
                sqlProd += " and pp.idProd in (" + idsProd + ")";
            else if (idProd > 0)
            {
                sqlProd += " and pp.idProd=" + idProd;
                criterio += "Produto: " + ProdutoDAO.Instance.GetCodInterno((int)idProd) + " - " + ProdutoDAO.Instance.GetDescrProduto((int)idProd) + "    ";
            }
            else if (!string.IsNullOrEmpty(codInterno))
            {
                sqlProd += " and pp.idProd=" + ProdutoDAO.Instance.ObtemIdProd(codInterno);
                criterio += "Produto: " + codInterno + " - " + ProdutoDAO.Instance.GetDescrProduto(codInterno) + "    ";
            }
            else if (idGrupo == 0 && idSubgrupo == 0)
            {
                //sqlProd = " and false";
                semProduto = true;
            }

            string tipoCalculoM2 = "coalesce(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + ") in (" +
                (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto + ")";

            string tipoCalculoMLAL = "coalesce(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + ") in (" +
                (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 + "," +
                (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 + ")";

            string tipoCalculoMLAL6 = "coalesce(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + ") in (" +
                (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 + ")";

            string where = "";

            if (!semProduto)
            {
                if (idLoja > 0)
                {
                    where += " and p.idLoja=" + idLoja;
                    criterio += "Loja: " + LojaDAO.Instance.GetNome((uint)idLoja) + "    ";
                }

                if (!String.IsNullOrEmpty(descricao))
                {
                    string ids = ProdutoDAO.Instance.ObtemIds(null, descricao);
                    where += " And prod.idProd In (" + ids + ")";
                    criterio += "Produtos: " + descricao + "    ";
                    lstParams.Add(new GDAParameter("?descricao", "%" + descricao + "%"));
                }

                if (idGrupo > 0)
                {
                    where += " and prod.idGrupoProd=" + idGrupo;
                    criterio += "Grupo: " + GrupoProdDAO.Instance.GetDescricao((int)idGrupo) + "    ";
                }

                if (idSubgrupo > 0)
                {
                    where += " and prod.idSubgrupoProd=" + idSubgrupo;
                    criterio += "Subgrupo: " + SubgrupoProdDAO.Instance.GetDescricao((int)idSubgrupo) + "    ";
                }

                if (!string.IsNullOrEmpty(dataIni))
                {
                    where += " and p.dataConf>=?dataIni";
                    criterio += "Data de início: " + dataIni + "    ";
                    lstParams.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));
                }

                if (!String.IsNullOrEmpty(dataFim))
                {
                    where += " and p.dataConf<=?dataFim";
                    criterio += "Data de término: " + dataFim + "    ";
                    lstParams.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));
                }

                if (!String.IsNullOrEmpty(dataIniLib))
                {
                    where += " and lp.dataLiberacao>=?dataIniLib";
                    criterio += "Data de início liberação: " + dataIniLib + "    ";
                    lstParams.Add(new GDAParameter("?dataIniLib", DateTime.Parse(dataIniLib + " 00:00:00")));
                }

                if (!String.IsNullOrEmpty(dataFimLib))
                {
                    where += " and lp.dataLiberacao<=?dataFimLib";
                    criterio += "Data de término liberação: " + dataFimLib + "    ";
                    lstParams.Add(new GDAParameter("?dataFimLib", DateTime.Parse(dataFimLib + " 23:59:59")));
                }
            }

            string entradaEstoqueQtde = @"(select count(*) from produto_pedido_producao ppp1 where ppp1.idProdPed=pp.idProdPedEsp
                and coalesce(ppp1.entrouEstoque,false)=false and ppp1.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + ")";

            string entradaEstoque = string.Format(@"
                round(if(p.tipoPedido<>" + (int)Pedido.TipoPedidoEnum.Producao + @", 0, 
                    if(" + tipoCalculoM2 + ", coalesce(pp.totM / {0}, 0), coalesce({0}, 0))),2)", entradaEstoqueQtde);

            string sql = string.Format(@"
                SELECT p.idPedido, p.idCli as idCliente, p.dataEntrega, pp.idProd, (pp.Qtde-pp.QtdSaida) as QtdeReserva, 
                    0 as QtdeLiberacao, {0} as QtdeEntradaEstoque, c.Nome as NomeCliente, p.dataConf, 
                    lp.dataLiberacao as dataLiberacao, '$$$' as Criterio, prod.codInterno, prod.descricao, 
                    p.idProjeto, p.dataPedido, prod.idGrupoProd, prod.idSubgrupoProd, g.descricao as NomeGrupo, s.descricao as NomeSubgrupo,
                    round((Select Sum(QtdEstoque) From produto_loja pl Where pl.idProd=prod.idProd),2) as qtdeEstoque, 
                    round((Select Sum(M2) From produto_loja pl Where pl.idProd=prod.idProd),2) as m2Estoque, p.idLoja
                FROM produtos_pedido pp 
                    LEFT JOIN pedido p on (p.idPedido=pp.idPedido) 
                    LEFT JOIN cliente c on (p.idCli=c.id_cli)
                    LEFT JOIN produto prod on (pp.idProd=prod.idProd)
                    LEFT JOIN grupo_prod g on (prod.idGrupoProd=g.idGrupoProd)
                    LEFT JOIN subgrupo_prod s on (prod.idSubgrupoProd=s.idSubgrupoProd)
                    LEFT JOIN produtos_liberar_pedido plp on (pp.idProdPed=plp.idProdPed)
                    LEFT JOIN liberarpedido lp on (plp.idLiberarPedido=lp.idLiberarPedido)
                WHERE pp.Qtde<>pp.QtdSaida 
                    AND pp.Invisivel{1}=0
		            AND p.Situacao={2}
                    AND p.TipoPedido<>{3}
		            {4}{5}
                GROUP BY pp.idProdPed, p.idPedido
                HAVING {6}{7}",

                entradaEstoque,
                PCPConfig.UsarConferenciaFluxo ? "Fluxo" : "Pedido",
                (int)Pedido.SituacaoPedido.ConfirmadoLiberacao,
                (int)Pedido.TipoPedidoEnum.Producao,
                sqlProd,
                where,
                (tipoColunas == 1 ? "qtdeReserva>0" : tipoColunas == 2 ? "qtdeLiberacao>0" : "qtdeReserva>0 or qtdeLiberacao>0"),
                (forRpt && tipoColunas == 0 ? " or qtdeEntradaEstoque>0" : ""));

            sql += string.Format(@"
                UNION ALL
                SELECT p.idPedido, p.idCli as idCliente, p.dataEntrega, pp.idProd, 0 as QtdeReserva, 
                    (pp.Qtde-pp.QtdSaida) as QtdeLiberacao, {0} as QtdeEntradaEstoque, c.Nome as NomeCliente, p.dataConf, 
                    lp.dataLiberacao as dataLiberacao, '$$$' as Criterio, prod.codInterno, prod.descricao, 
                    p.idProjeto, p.dataPedido, prod.idGrupoProd, prod.idSubgrupoProd, g.descricao as NomeGrupo, s.descricao as NomeSubgrupo,
                    ROUND((Select SUM(QtdEstoque) FROM produto_loja pl WHERE pl.idProd=prod.idProd),2) as qtdeEstoque, 
                    ROUND((Select SUM(M2) FROM produto_loja pl WHERE pl.idProd=prod.idProd),2) as m2Estoque, p.idLoja
                FROM produtos_pedido pp 
                    LEFT JOIN pedido p on (p.idPedido=pp.idPedido) 
                    LEFT JOIN cliente c on (p.idCli=c.id_cli)
                    LEFT JOIN produto prod on (pp.idProd=prod.idProd)
                    LEFT JOIN grupo_prod g on (prod.idGrupoProd=g.idGrupoProd)
                    LEFT JOIN subgrupo_prod s on (prod.idSubgrupoProd=s.idSubgrupoProd)
                    LEFT JOIN produtos_liberar_pedido plp on (pp.idProdPed=plp.idProdPed)
                    LEFT JOIN liberarpedido lp on (plp.idLiberarPedido=lp.idLiberarPedido)
                WHERE pp.Qtde<>pp.QtdSaida 
                    AND pp.Invisivel{1}=0
		            AND p.Situacao={2}
                    AND p.SituacaoProducao<>{3}
                    AND p.TipoPedido<>{4}
		            {5}{6}
                GROUP BY pp.idProdPed, p.idPedido
                HAVING {7}{8}",

                entradaEstoque,
                PCPConfig.UsarConferenciaFluxo ? "Fluxo" : "Pedido",
                (int)Pedido.SituacaoPedido.Confirmado,
                (int)Pedido.SituacaoProducaoEnum.Entregue,
                (int)Pedido.TipoPedidoEnum.Producao,
                sqlProd,
                where,
                (tipoColunas == 1 ? "qtdeReserva>0" : tipoColunas == 2 ? "qtdeLiberacao>0" : "qtdeReserva>0 or qtdeLiberacao>0"),
                (forRpt && tipoColunas == 0 ? " or qtdeEntradaEstoque>0" : ""));

            sql = @"
                SELECT idPedido, idCliente, dataEntrega, idProd, ROUND(SUM(qtdeReserva),2) as QtdeReserva, ROUND(SUM(qtdeLiberacao),2) as QtdeLiberacao, 
                    ROUND(SUM(qtdeEntradaEstoque),2) as QtdeEntradaEstoque, NomeCliente, dataConf, dataLiberacao, Criterio, codInterno, descricao, 
                    idProjeto, dataPedido, idGrupoProd, idSubgrupoProd, NomeGrupo, NomeSubgrupo, qtdeEstoque, m2Estoque
                FROM (
                    " + sql + @"
                ) as temp
                GROUP BY " + (!forRpt ? "idProd" : "idProd, idPedido");

            switch (ordenar)
            {
                case 1:
                    sql += " ORDER BY prod.codInterno";
                    criterio += "Ordenado pelo código do produto    ";
                    break;
                case 2:
                    sql += " ORDER BY prod.descricao";
                    criterio += "Ordenado pela descrição do produto    ";
                    break;
            }

            parametros = lstParams.ToArray();
            return sql.Replace("$$$", criterio);
        }

        public string SqlRelatorioNovo(uint idProd, string codInterno, string descricao, int idGrupo, int idSubgrupo, int ordenar,
            string dataIni, string dataFim, string dataIniLib, string dataFimLib, bool selecionar, ref List<GDAParameter> lstParams)
        {
            var campos = @"prod.codInterno, prod.descricao, prod.idGrupoProd, prod.idSubgrupoProd,
                            g.descricao AS NomeGrupo, s.descricao AS NomeSubgrupo, pl.IdProd, pl.Idloja,
                            SUM(pl.qtdEstoque) AS QtdeEstoque, SUM(pl.m2) AS M2Estoque, 
                            SUM(pl.reserva) AS QtdeReserva, SUM(pl.liberacao) AS QtdeLiberacao";

            var sql = @"
                SELECT " + campos + @"
                FROM produto_loja pl
	                INNER JOIN produto prod ON (pl.IdProd = prod.IdProd)
                    LEFT JOIN grupo_prod g ON (prod.idGrupoProd = g.idGrupoProd)
                    LEFT JOIN subgrupo_prod s ON (prod.idSubgrupoProd = s.idSubgrupoProd)
                WHERE 1";

            var sqlPedido = @"
                SELECT DISTINCT(pp.IdProd) as IdProd
                FROM produtos_pedido pp
	                INNER JOIN pedido P ON (pp.idPedido = p.IdPedido)
                    LEFT JOIN produtos_liberar_pedido plp ON (pp.idProdPed = plp.idProdPed)
                    LEFT JOIN liberarpedido lp ON (plp.idLiberarPedido = lp.idLiberarPedido)
                WHERE p.situacao IN (5,7) 
	                AND COALESCE(pp.invisivelFluxo, FALSE) = FALSE 
                    {0}";


            string where = "", wherePedido = "", criterio = "";

            if (!string.IsNullOrEmpty(codInterno))
            {
                where += " AND prod.CodInterno = ?codInterno";
                criterio += "Produtos: " + codInterno + "    ";
                lstParams.Add(new GDAParameter("?codInterno", codInterno));
            }
            else if (!String.IsNullOrEmpty(descricao))
            {
                var ids = ProdutoDAO.Instance.ObtemIds(null, descricao);
                where += " AND prod.idProd IN (" + ids + ")";
                criterio += "Produtos: " + descricao + "    ";
                lstParams.Add(new GDAParameter("?descricao", "%" + descricao + "%"));
            }

            if (idGrupo > 0)
            {
                where += " AND prod.idGrupoProd=" + idGrupo;
                criterio += "Grupo: " + GrupoProdDAO.Instance.GetDescricao((int)idGrupo) + "    ";
            }

            if (idSubgrupo > 0)
            {
                where += " AND prod.idSubgrupoProd=" + idSubgrupo;
                criterio += "Subgrupo: " + SubgrupoProdDAO.Instance.GetDescricao((int)idSubgrupo) + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                wherePedido += " AND p.dataConf>=?dataIni";
                criterio += "Data de início: " + dataIni + "    ";
                lstParams.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                wherePedido += " AND p.dataConf<=?dataFim";
                criterio += "Data de término: " + dataFim + "    ";
                lstParams.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));
            }

            if (!String.IsNullOrEmpty(dataIniLib))
            {
                wherePedido += " AND lp.dataLiberacao>=?dataIniLib";
                criterio += "Data de início liberação: " + dataIniLib + "    ";
                lstParams.Add(new GDAParameter("?dataIniLib", DateTime.Parse(dataIniLib + " 00:00:00")));
            }

            if (!String.IsNullOrEmpty(dataFimLib))
            {
                wherePedido += " AND lp.dataLiberacao<=?dataFimLib";
                criterio += "Data de término liberação: " + dataFimLib + "    ";
                lstParams.Add(new GDAParameter("?dataFimLib", DateTime.Parse(dataFimLib + " 23:59:59")));
            }

            if (!string.IsNullOrEmpty(wherePedido))
            {
                sqlPedido = string.Format(sqlPedido, wherePedido);
                sql += " AND prod.IdProd IN (" + sqlPedido + ")";
            }

            sql += where;
            sql += @" GROUP BY pl.IdProd HAVING QtdeReserva > 0 OR QtdeLiberacao > 0";

            switch (ordenar)
            {
                case 1:
                    sql += " ORDER BY prod.codInterno";
                    criterio += "Ordenado pelo código do produto    ";
                    break;
                case 2:
                    sql += " ORDER BY prod.descricao";
                    criterio += "Ordenado pela descrição do produto    ";
                    break;
            }

            if(!selecionar)
                return "SELECT COUNT(*) FROM (" + sql + ") AS temp1";

            return sql;
        }

        public string SqlRelatorio(uint idProd, string codInterno, string descricao, int idGrupo, int idSubgrupo, int ordenar,
            string dataIni, string dataFim, string dataIniLib, string dataFimLib, bool selecionar, ref List<GDAParameter> lstParams)
        {
            var campos = String.Empty;
            var tabelas = String.Empty;
            var where = String.Empty;
            var groupBy = String.Empty;
            var criterio = String.Empty;
            var sql1 = String.Empty;
            var sql2 = String.Empty;
            var sqlUnion = String.Empty;

            campos = @"p.idPedido, p.idCli AS IdCliente, p.dataEntrega, pp.idProd, /*c.nome*/ NULL AS NomeCliente, p.dataConf, lp.dataLiberacao AS DataLiberacao,
                prod.codInterno, prod.descricao, p.idProjeto, p.dataPedido, prod.idGrupoProd, prod.idSubgrupoProd, g.descricao AS NomeGrupo,
                s.descricao AS NomeSubgrupo, p.idLoja, pl.qtdEstoque AS QtdeEstoque, pl.m2 AS M2Estoque,

                /* Chamado 11316, estava ocorrendo time out ao abrir a tela, alteramos para buscar a reserva e liberação direto do banco de dados. */
                pl.reserva AS QtdeReserva,
                pl.liberacao AS QtdeLiberacao,

                '$$$' as Criterio ";

            tabelas = @"
                /* Força a utilização do campo idProdPed como Index para que a consulta fique mais rápida */
                produtos_pedido pp FORCE INDEX(PRIMARY)
                    LEFT JOIN pedido p ON (p.idPedido=pp.idPedido)
                    /*LEFT JOIN cliente c ON (p.idCli=c.id_cli)*/
                    LEFT JOIN produto prod ON (pp.idProd=prod.idProd)
                    LEFT JOIN produto_loja pl ON (prod.idProd=pl.idProd And p.idLoja=pl.idLoja)
                    LEFT JOIN grupo_prod g ON (prod.idGrupoProd=g.idGrupoProd)
                    LEFT JOIN subgrupo_prod s ON (prod.idSubgrupoProd=s.idSubgrupoProd)
                    LEFT JOIN produtos_liberar_pedido plp ON (pp.idProdPed=plp.idProdPed)
                    LEFT JOIN liberarpedido lp ON (plp.idLiberarPedido=lp.idLiberarPedido)
                    
                    /*LEFT JOIN (
                        SELECT ppe1.idProd, ppp2.idPedidoExpedicao, COUNT(*) AS Num
                        FROM produto_pedido_producao ppp2
                            INNER JOIN produtos_pedido_espelho ppe1 ON (ppp2.idProdPed=ppe1.idProdPed)
                        WHERE ppp2.idPedidoExpedicao IS NOT NULL
                        GROUP BY ppe1.idProd, ppp2.idPedidoExpedicao)
                    AS producao ON (pp.idProd=producao.idProd AND pp.idPedido=producao.idPedidoExpedicao)*/

                    /*LEFT JOIN (
                        SELECT pp1.idPedido, plp1.idProdPed, SUM(plp1.qtdeCalc) AS Qtde
                        FROM produtos_liberar_pedido plp1
                            INNER JOIN produtos_pedido pp1 ON (plp1.idProdPed=pp1.idProdPed)
                            INNER JOIN liberarpedido lp1 ON (plp1.idLiberarPedido=lp1.idLiberarPedido)
                            LEFT JOIN produto_pedido_producao ppp1 ON
                                (plp1.idProdPed=ppp1.idProdPed AND COALESCE(plp1.idProdPedProducao, ppp1.idProdPedProducao) = ppp1.idProdPedProducao)
                            LEFT JOIN setor s1 ON (ppp1.idSetor=s1.idSetor)
                        WHERE (ppp1.idProdPedProducao IS NULL OR ppp1.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + ") " +
                            "AND lp1.situacao<>" + (int)LiberarPedido.SituacaoLiberarPedido.Cancelado + @"
                        GROUP BY pp1.idPedido, plp1.idProdPed)
                    AS qtdeLib ON (p.idPedido=qtdeLib.idPedido AND pp.idProdPed=qtdeLib.idProdPed)*/";

            where = " AND COALESCE(pp.invisivel" + (PCPConfig.UsarConferenciaFluxo ? "Fluxo" : "Pedido") + ", FALSE)=FALSE";

            groupBy = @"
                GROUP BY pp.idProdPed, p.idPedido
                HAVING QtdeReserva > 0 OR QtdeLiberacao > 0";

            sql1 = @"
                SELECT " + campos + @"

                    /* Reserva */
                    /*ROUND(
                        IF(p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + " OR p.situacao<>" + (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + @",
                            0,
                            IF(COALESCE(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @")
                            IN (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto + @"),
                                COALESCE((pp.totM / pp.qtde) * (pp.qtde - pp.qtdSaida - COALESCE(producao.num, 0)), 0),
                                IF(COALESCE(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @")
                                IN (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 + "," +
                                (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 + @"),
                                    COALESCE(pp.qtde - pp.qtdSaida - COALESCE(producao.num, 0), 0) *
                                    IF(COALESCE(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @")
                                    IN (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 + @"),
                                    " + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 + @",
                                    pp.altura),
                                COALESCE(pp.qtde - pp.qtdSaida - COALESCE(producao.num, 0), 0)))), 2) AS QtdeReserva,

                    /* Liberação */
                    /*ROUND(
                        IF(p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + " OR p.situacao<>" + (int)Pedido.SituacaoPedido.Confirmado + @",
                            0,
                            IF(COALESCE(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @")
                            IN (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto + @"),
                                COALESCE((pp.totM / pp.qtde) * (pp.qtde - pp.qtdSaida - COALESCE(producao.num, 0)), 0),
                                IF(COALESCE(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @")
                                IN (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 + "," +
                                (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 + @"),
                                    COALESCE(pp.qtde - pp.qtdSaida - COALESCE(producao.num, 0), 0) *
                                    IF(COALESCE(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @")
                                    IN (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 + @"),
                                    " + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 + @",
                                    pp.altura),
                                COALESCE(pp.qtde - pp.qtdSaida - COALESCE(producao.num, 0), 0)))), 2) AS QtdeLiberacao*/

                FROM " + tabelas + @"

                WHERE p.situacao IN (" + (int)Pedido.SituacaoPedido.Confirmado + "," + (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + ")";

            sql2 = @"
                SELECT " + campos + @"

                    /* Reserva */
                    /*ROUND(IF(p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @",
                        0,
                        IF(COALESCE(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @")
                        IN (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto + @"),
                            COALESCE((pp.totM / (pp.qtde - COALESCE(qtdeLib.qtde, 0))) * (pp.qtde - pp.qtdSaida - COALESCE(producao.num, 0)), 0),
                            IF(COALESCE(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @")
                            IN (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 + "," +
                            (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 + @"),
                                COALESCE((pp.qtde - COALESCE(qtdeLib.qtde, 0)) - pp.qtdSaida - COALESCE(producao.num, 0), 0) *
                                IF(COALESCE(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @")
                                IN (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 + @"),
                                    " + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 + @",
                                    pp.altura),
                                COALESCE((pp.qtde - COALESCE(qtdeLib.qtde, 0)) - pp.qtdSaida - COALESCE(producao.num, 0), 0)))), 2) AS QtdeReserva,
                    
                    /* Liberação */
                    /*ROUND(IF(p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @",
                        0,
                        IF(COALESCE(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @")
                        IN (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto + @"),
                            COALESCE((pp.totM / (COALESCE(qtdeLib.qtde, 0))) * (pp.qtde - pp.qtdSaida - COALESCE(producao.num, 0)), 0),
                            IF(COALESCE(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @")
                            IN (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 + "," +
                            (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 + @"),
                                COALESCE((COALESCE(qtdeLib.qtde, 0)) - pp.qtdSaida - COALESCE(producao.num, 0), 0) *
                                IF(COALESCE(s.tipoCalculo, g.tipoCalculo, " + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @")
                                IN (" + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 + @"),
                                    " + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 + @",
                                    pp.altura),
                                COALESCE((COALESCE(qtdeLib.qtde, 0)) - pp.qtdSaida - COALESCE(producao.num, 0), 0)))), 2) AS QtdeLiberacao*/

                FROM " + tabelas + @"

                WHERE p.situacao=" + (int)Pedido.SituacaoPedido.LiberadoParcialmente;

            if (!String.IsNullOrEmpty(descricao))
            {
                var ids = ProdutoDAO.Instance.ObtemIds(null, descricao);
                where += " AND prod.idProd IN (" + ids + ")";
                criterio += "Produtos: " + descricao + "    ";
                lstParams.Add(new GDAParameter("?descricao", "%" + descricao + "%"));
            }

            if (idGrupo > 0)
            {
                where += " AND prod.idGrupoProd=" + idGrupo;
                criterio += "Grupo: " + GrupoProdDAO.Instance.GetDescricao((int)idGrupo) + "    ";
            }

            if (idSubgrupo > 0)
            {
                where += " AND prod.idSubgrupoProd=" + idSubgrupo;
                criterio += "Subgrupo: " + SubgrupoProdDAO.Instance.GetDescricao((int)idSubgrupo) + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                where += " AND p.dataConf>=?dataIni";
                criterio += "Data de início: " + dataIni + "    ";
                lstParams.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                where += " AND p.dataConf<=?dataFim";
                criterio += "Data de término: " + dataFim + "    ";
                lstParams.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));
            }

            if (!String.IsNullOrEmpty(dataIniLib))
            {
                where += " AND lp.dataLiberacao>=?dataIniLib";
                criterio += "Data de início liberação: " + dataIniLib + "    ";
                lstParams.Add(new GDAParameter("?dataIniLib", DateTime.Parse(dataIniLib + " 00:00:00")));
            }

            if (!String.IsNullOrEmpty(dataFimLib))
            {
                where += " AND lp.dataLiberacao<=?dataFimLib";
                criterio += "Data de término liberação: " + dataFimLib + "    ";
                lstParams.Add(new GDAParameter("?dataFimLib", DateTime.Parse(dataFimLib + " 23:59:59")));
            }

            //sqlUnion = sql1 + where + groupBy + " UNION ALL " + sql2 + where + groupBy;
            sqlUnion = sql1 + where + groupBy;

            if (selecionar)
            {
                sqlUnion = @"
                    SELECT IdPedido, IdCliente, DataEntrega, IdProd, ROUND(SUM(QtdeReserva), 2) as QtdeReserva, ROUND(SUM(QtdeLiberacao), 2) AS QtdeLiberacao,
                        0 AS QtdeEntradaEstoque, NomeCliente, DataConf, DataLiberacao, CodInterno, Descricao, IdProjeto,
                        DataPedido, IdGrupoProd, IdSubgrupoProd, NomeGrupo, NomeSubgrupo, QtdeEstoque, M2Estoque, Criterio
                    FROM ( " + sqlUnion + @" ) AS temp
                    GROUP BY IdProd";

                switch (ordenar)
                {
                    case 1:
                        sqlUnion += " ORDER BY prod.codInterno";
                        criterio += "Ordenado pelo código do produto    ";
                        break;
                    case 2:
                        sqlUnion += " ORDER BY prod.descricao";
                        criterio += "Ordenado pela descrição do produto    ";
                        break;
                }
            }
            else if (!selecionar)
                sqlUnion = "SELECT COUNT(*) FROM (" + sqlUnion + ") AS temp1 GROUP BY IdProd";

            return sqlUnion.Replace("$$$", criterio);            
        }

        public IList<EstoqueProdutos> GetList(uint idProd, string codInterno, string descricao, int idGrupo, int idSubgrupo, int ordenar,
            string dataIni, string dataFim, string dataIniLib, string dataFimLib, string sortExpression, int startRow, int pageSize)
        {
            var parametros = new List<GDA.GDAParameter>();

            string sql = SqlRelatorioNovo(idProd, codInterno, descricao, idGrupo, idSubgrupo, ordenar, dataIni, dataFim, dataIniLib,
                dataFimLib, true, ref parametros);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, parametros.ToArray());
        }

        public int GetCount(uint idProd, string codInterno, string descricao, int idGrupo, int idSubgrupo, int ordenar,
            string dataIni, string dataFim, string dataIniLib, string dataFimLib)
        {
            var parametros = new List<GDA.GDAParameter>();
            string sql = SqlRelatorioNovo(idProd, codInterno, descricao, idGrupo, idSubgrupo, ordenar, dataIni, dataFim, dataIniLib,
                dataFimLib, false, ref parametros);

            return objPersistence.ExecuteSqlQueryCount(sql, parametros.ToArray());
        }

        public IList<EstoqueProdutos> GetForRpt(uint idLoja, uint idProd, string codInterno, string descricao, int idGrupo, int idSubgrupo, int ordenar,
            string dataIni, string dataFim, string dataIniLib, string dataFimLib, int tipoColunas)
        {
            GDAParameter[] p;
            string sql = Sql(null, (int)idLoja, idProd, codInterno, descricao, idGrupo, idSubgrupo, ordenar, dataIni, dataFim, dataIniLib,
                dataFimLib, true, true, false, PedidoConfig.LiberarPedido && (tipoColunas == 0 || tipoColunas == 2), false, tipoColunas, out p);

            return objPersistence.LoadData(sql, p).ToList();
        }

        public IList<EstoqueProdutos> GetByProd(uint idProd)
        {
            if (idProd == 0)
                return new List<EstoqueProdutos>();

            GDAParameter[] p;
            string sql = Sql(null, 0, idProd, null, null, 0, 0, 0, null, null, null,
                null, false, true, false, PedidoConfig.LiberarPedido, false, 0, out p) + ", IdPedido";
            
            return objPersistence.LoadData(sql, p).ToList();
        }

        public void CancelarPedido(uint idPedido)
        {
            PedidoDAO.Instance.CancelarPedidoComTransacao(idPedido, "Disponibilização de estoque", false, true, DateTime.Now);
        }
    }
}
