using GDA;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.RelModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class ItemProduzidoEFDDAO : BaseDAO<ItemProduzidoEFD, ItemProduzidoEFDDAO>
    {
        /// <summary>
        /// Obtém os itens produzidos de uma determinada loja, com base no período informado.
        /// </summary>
        /// <param name="idLoja">idLoja.</param>
        /// <param name="dataInicio">dataInicio.</param>
        /// <param name="dataFim">dataFim.</param>
        /// <returns>Retorna os itens produzidos de uma determinada loja, com base no período informado.</returns>
        public IEnumerable<Sync.Fiscal.EFD.Entidade.IItemProduzido> ObtemItensProduzidosParaEFD(
            int idLoja,
            DateTime dataInicio,
            DateTime dataFim)
        {
            if (idLoja == 0 || (dataInicio == DateTime.MinValue && dataFim == DateTime.MinValue))
            {
                return null;
            }

            var setoresPronto = Utils.GetSetores.Where(f => f.Tipo == TipoSetor.Pronto)?.ToList();
            var setoresCorteLaminado = Utils.GetSetores.Where(f => f.Corte || f.Laminado);

            var idsPecasIniciadasNoPeriodo = this.ObterIdsProdutoProducaoIniciadosNoPeriodo(dataInicio, dataFim);
            var idsPecasConcluidasNoPeriodo = this.ObterIdsProdutoProducaoConcluidosNoPeriodo(
                setoresPronto,
                dataInicio,
                dataFim);
            var idsPecasIniciadasNoPeriodoConcluidasNoPeriodo = this.ObterIdsProdutoProducaoIniciadosNoPeriodoConcluidosNoPeriodo(
                idsPecasIniciadasNoPeriodo,
                idsPecasConcluidasNoPeriodo);
            var idsPecasIniciadasNoPeriodoNaoConcluidasNoPeriodo = this.ObterIdsProdutoProducaoIniciadosNoPeriodoNaoConcluidosNoPeriodo(
                idsPecasIniciadasNoPeriodo,
                idsPecasIniciadasNoPeriodoConcluidasNoPeriodo);
            var idsPecasIniciadasAntesPeriodoConcluidasNoPeriodo = this.ObterIdsProdutoProducaoIniciadosAntesPeriodoConcluidosNoPeriodo(
                idsPecasConcluidasNoPeriodo,
                idsPecasIniciadasNoPeriodo);
            var idsPecasIniciadasForaPeriodoNaoConcluidasPeriodo = this.ObterIdsProdutoProducaoIniciadosAntesPeriodoNaoConcluidosNoPeriodo(
                setoresPronto,
                dataInicio,
                dataFim);

            var idsPecasConsultar = new List<int>();
            idsPecasConsultar.AddRange(idsPecasIniciadasNoPeriodoConcluidasNoPeriodo);
            idsPecasConsultar.AddRange(idsPecasIniciadasNoPeriodoNaoConcluidasNoPeriodo);
            idsPecasConsultar.AddRange(idsPecasIniciadasAntesPeriodoConcluidasNoPeriodo);
            idsPecasConsultar.AddRange(idsPecasIniciadasForaPeriodoNaoConcluidasPeriodo);

            if (!idsPecasConsultar.Any(f => f > 0))
            {
                return null;
            }

            var filtroSetoresPronto = setoresPronto.Any(f => f.IdSetor > 0)
                ? $", {string.Join(",", setoresPronto.Select(f => f.IdSetor))}"
                : string.Empty;

            var filtroSetoresCorteLaminado = setoresCorteLaminado.Any(f => f.IdSetor > 0)
                ? $", {string.Join(",", setoresCorteLaminado.Select(f => f.IdSetor))}"
                : string.Empty;

            var sqlDadosProducao = $@"SELECT ppp.IdProdPedProducao,
                    ppp.IdProdPed,
                    ppe.IdProd,
                    ppe.Altura,
                    ppe.Largura,
                    ppe.Qtde AS Quantidade,
                    ppe.TotM2Calc,
                    ppp.NumEtiqueta,
                    lp.DataLeitura,
		            lp.IdSetor,
                    lp.ProntoRoteiro,
                    ppe.IdGrupoProd,
                    ppe.IdSubgrupoProd,
                    ppe.TipoCalculoGrupo,
                    ppe.TipoCalculoNfGrupo,
                    ppe.TipoCalculoSubgrupo,
                    ppe.TipoCalculoNfSubgrupo
                FROM produto_pedido_producao ppp
                    INNER JOIN (
                        SELECT lp.IdProdPedProducao, lp.DataLeitura, lp.IdSetor, lp.ProntoRoteiro
                        FROM leitura_producao lp
                        WHERE lp.IdProdPedProducao IN ({string.Join(",", idsPecasConsultar)})
                            AND (lp.ProntoRoteiro
                            OR lp.IdSetor IN (1{filtroSetoresPronto}{filtroSetoresCorteLaminado}))
                    ) lp ON (lp.IdProdPedProducao = ppp.IdProdPedProducao)
                    INNER JOIN (
                        SELECT ppe.IdProdPed,
                            ppe.IdProd,
                            ppe.Altura,
                            ppe.Largura,
                            ppe.Qtde,
                            ppe.TotM2Calc,
                            gp.IdGrupoProd,
                            sp.IdSubgrupoProd,
                            gp.TipoCalculo AS TipoCalculoGrupo,
                            gp.TipoCalculoNf AS TipoCalculoNfGrupo,
                            sp.TipoCalculo AS TipoCalculoSubgrupo,
                            sp.TipoCalculoNf AS TipoCalculoNfSubgrupo
                        FROM produtos_pedido_espelho ppe
                            INNER JOIN pedido p ON (ppe.IdPedido = p.IdPedido)
                            INNER JOIN produto prod ON (ppe.IdProd = prod.IdProd)
                            INNER JOIN grupo_prod gp ON (prod.IdGrupoProd = gp.IdGrupoProd)
                            INNER JOIN subgrupo_prod sp ON (prod.IdSubgrupoProd = sp.IdSubgrupoProd)
                        WHERE p.IdLoja = {idLoja}
                            AND (p.DataPronto IS NULL OR p.DataPronto > ?dataFim)
                            AND prod.TipoMercadoria IN ({(int)TipoMercadoria.ProdutoEmProcesso}, {(int)TipoMercadoria.ProdutoAcabado})
                    ) ppe ON (ppp.IdProdPed = ppe.IdProdPed)";

            var itens = this.objPersistence.LoadData(sqlDadosProducao, ObterParametrosConsulta(null, dataFim)).ToList();
            var itensProcessados = new List<Sync.Fiscal.EFD.Entidade.IItemProduzido>();
            var idProdProdutosBaixaEstoqueFiscal = new Dictionary<int, List<ProdutoBaixaEstoqueFiscal>>();

            foreach (var item in itens.GroupBy(f => f.IdProdPedProducao))
            {
                var itemProcessado = new ItemProduzidoEFD();

                var aux = item.FirstOrDefault();

                if (aux == null)
                {
                    continue;
                }

                itemProcessado.IdProdPed = aux.IdProdPed;
                itemProcessado.CodigoProduto = aux.CodigoProduto;
                itemProcessado.Altura = aux.Altura;
                itemProcessado.Largura = aux.Largura;
                itemProcessado.Quantidade = aux.Quantidade;
                itemProcessado.TotM2Calc = aux.TotM2Calc;
                itemProcessado.QtdeProduzida = Math.Round((decimal)(itemProcessado.TotM2Calc / itemProcessado.Quantidade), 2);
                itemProcessado.NumEtiqueta = aux.NumEtiqueta;
                itemProcessado.IdSetor = aux.IdSetor;
                itemProcessado.ProntoRoteiro = aux.ProntoRoteiro;
                itemProcessado.IdGrupoProd = aux.IdGrupoProd;
                itemProcessado.IdSubgrupoProd = aux.IdSubgrupoProd;
                itemProcessado.TipoCalculoGrupo = aux.TipoCalculoGrupo;
                itemProcessado.TipoCalculoNfGrupo = aux.TipoCalculoNfGrupo;
                itemProcessado.TipoCalculoSubgrupo = aux.TipoCalculoSubgrupo;
                itemProcessado.TipoCalculoNfSubgrupo = aux.TipoCalculoNfSubgrupo;

                if (itemProcessado.QtdeProduzida == 0)
                {
                    continue;
                }

                var inicioProd = item.FirstOrDefault(f => f.IdSetor == 1);
                if (inicioProd != null)
                {
                    itemProcessado.InicioProducao = inicioProd.DataLeitura;
                }

                var finalProd = item.FirstOrDefault(f => (f.ProntoRoteiro || setoresPronto.Any(g => g.IdSetor == f.IdSetor)) && f.DataLeitura >= dataInicio && f.DataLeitura <= dataFim);
                if (finalProd != null)
                {
                    itemProcessado.FinalProducao = finalProd.DataLeitura;
                }

                var insumo = item.FirstOrDefault(f => setoresCorteLaminado.Any(g => g.IdSetor == f.IdSetor) && f.DataLeitura >= dataInicio && f.DataLeitura <= dataFim);
                if (insumo != null)
                {
                    if (!idProdProdutosBaixaEstoqueFiscal.ContainsKey(itemProcessado.CodigoProduto))
                    {
                        var produtosBaixaEstoqueFiscal = ProdutoBaixaEstoqueFiscalDAO.Instance.ObterParaItemProduzidoEfd(null, itemProcessado.CodigoProduto);

                        if (produtosBaixaEstoqueFiscal.Any(f => f.IdProd > 0))
                        {
                            idProdProdutosBaixaEstoqueFiscal.Add(itemProcessado.CodigoProduto, produtosBaixaEstoqueFiscal);
                        }
                    }

                    if (idProdProdutosBaixaEstoqueFiscal?[itemProcessado.CodigoProduto]?.Any(f => f.IdProd > 0) ?? false)
                    {
                        itemProcessado.Insumos = idProdProdutosBaixaEstoqueFiscal[itemProcessado.CodigoProduto]
                            .Select(f => new InsumoConsumidoEFD
                            {
                                CodigoProduto = f.IdProdBaixa,
                                DataSaidaEstoque = insumo.DataLeitura,
                                QtdeConsumida = (decimal)(ProdutosNfDAO.Instance.ObtemQtdDanfe(
                                    null,
                                    (uint)f.IdProdBaixa,
                                    (float)insumo.QtdeProduzida,
                                    1,
                                    insumo.Altura,
                                    insumo.Largura,
                                    true,
                                    true,
                                    itemProcessado.IdGrupoProd,
                                    itemProcessado.IdSubgrupoProd,
                                    null,
                                    (TipoCalculoGrupoProd?)itemProcessado.TipoCalculoGrupo,
                                    (TipoCalculoGrupoProd?)itemProcessado.TipoCalculoNfGrupo,
                                    (TipoCalculoGrupoProd?)itemProcessado.TipoCalculoSubgrupo,
                                    (TipoCalculoGrupoProd?)itemProcessado.TipoCalculoNfSubgrupo)
                                    * f.Qtde),
                            }).ToList();
                    }
                }

                itensProcessados.Add(itemProcessado);
            }

            return itensProcessados;
        }

        private List<int> ObterIdsProdutoProducaoIniciadosNoPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            if (dataInicio == DateTime.MinValue && dataFim == DateTime.MinValue)
            {
                return new List<int>();
            }

            var sqlPecasIniciadasNoPeriodo = $@"SELECT DISTINCT(lp.IdProdPedProducao)
                FROM leitura_producao lp
                WHERE lp.IdSetor = 1";

            if (dataInicio != DateTime.MinValue)
            {
                sqlPecasIniciadasNoPeriodo += " AND lp.DataLeitura >= ?dataInicio";
            }

            if (dataFim != DateTime.MinValue)
            {
                sqlPecasIniciadasNoPeriodo += " AND lp.DataLeitura <= ?dataFim";
            }

            return this.ExecuteMultipleScalar<int>(sqlPecasIniciadasNoPeriodo, this.ObterParametrosConsulta(dataInicio, dataFim));
        }

        private List<int> ObterIdsProdutoProducaoConcluidosNoPeriodo(
            List<Setor> setoresPronto,
            DateTime dataInicio,
            DateTime dataFim)
        {
            if (dataInicio == DateTime.MinValue && dataFim == DateTime.MinValue)
            {
                return new List<int>();
            }

            var sqlPecasConcluidasNoPeriodo = $@"SELECT DISTINCT(lp.IdProdPedProducao)
                FROM leitura_producao lp
                WHERE 1";

            if (dataInicio != DateTime.MinValue)
            {
                sqlPecasConcluidasNoPeriodo += " AND lp.DataLeitura >= ?dataInicio";
            }

            if (dataFim != DateTime.MinValue)
            {
                sqlPecasConcluidasNoPeriodo += " AND lp.DataLeitura <= ?dataFim";
            }

            if (setoresPronto?.Any() ?? false)
            {
                sqlPecasConcluidasNoPeriodo += $@" AND (lp.IdSetor IN ({string.Join(",", setoresPronto.Select(f => f.IdSetor))})
                    OR lp.ProntoRoteiro = 1)";
            }
            else
            {
                sqlPecasConcluidasNoPeriodo += " AND lp.ProntoRoteiro = 1";
            }

            return this.ExecuteMultipleScalar<int>(sqlPecasConcluidasNoPeriodo, this.ObterParametrosConsulta(dataInicio, dataFim));
        }

        private List<int> ObterIdsProdutoProducaoIniciadosNoPeriodoConcluidosNoPeriodo(
            List<int> idsPecasIniciadasNoPeriodo,
            List<int> idsPecasConcluidasNoPeriodo)
        {
            if (!idsPecasIniciadasNoPeriodo.Any(f => f > 0) && !idsPecasConcluidasNoPeriodo.Any(f => f > 0))
            {
                return new List<int>();
            }

            var sqlPecasIniciadasNoPeriodoConcluidasNoPeriodo = $@"SELECT DISTINCT(ppp.IdProdPedProducao)
                FROM produto_pedido_producao ppp
                WHERE 1";

            if (idsPecasIniciadasNoPeriodo.Any(f => f > 0))
            {
                sqlPecasIniciadasNoPeriodoConcluidasNoPeriodo += $" AND ppp.IdProdPedProducao IN ({string.Join(",", idsPecasIniciadasNoPeriodo)})";
            }

            if (idsPecasConcluidasNoPeriodo.Any(f => f > 0))
            {
                sqlPecasIniciadasNoPeriodoConcluidasNoPeriodo += $"AND ppp.IdProdPedProducao IN ({string.Join(",", idsPecasConcluidasNoPeriodo)})";
            }

            return this.ExecuteMultipleScalar<int>(sqlPecasIniciadasNoPeriodoConcluidasNoPeriodo);
        }

        private List<int> ObterIdsProdutoProducaoIniciadosNoPeriodoNaoConcluidosNoPeriodo(
            List<int> idsPecasIniciadasNoPeriodo,
            List<int> idsPecasIniciadasNoPeriodoConcluidasNoPeriodo)
        {
            if (!idsPecasIniciadasNoPeriodo.Any(f => f > 0) && !idsPecasIniciadasNoPeriodoConcluidasNoPeriodo.Any(f => f > 0))
            {
                return new List<int>();
            }

            var sqlPecasIniciadasNoPeriodoNaoConcluidasNoPeriodo = $@"SELECT DISTINCT(ppp.IdProdPedProducao)
                FROM produto_pedido_producao ppp
                WHERE 1";

            if (idsPecasIniciadasNoPeriodo.Any(f => f > 0))
            {
                sqlPecasIniciadasNoPeriodoNaoConcluidasNoPeriodo += $" AND ppp.IdProdPedProducao IN ({string.Join(",", idsPecasIniciadasNoPeriodo)})";
            }

            if (idsPecasIniciadasNoPeriodoConcluidasNoPeriodo.Any(f => f > 0))
            {
                sqlPecasIniciadasNoPeriodoNaoConcluidasNoPeriodo += $" AND ppp.IdProdPedProducao NOT IN ({string.Join(",", idsPecasIniciadasNoPeriodoConcluidasNoPeriodo)})";
            }

            return this.ExecuteMultipleScalar<int>(sqlPecasIniciadasNoPeriodoNaoConcluidasNoPeriodo);
        }

        private List<int> ObterIdsProdutoProducaoIniciadosAntesPeriodoConcluidosNoPeriodo(
            List<int> idsPecasConcluidasNoPeriodo,
            List<int> idsPecasIniciadasNoPeriodo)
        {
            if (!idsPecasConcluidasNoPeriodo.Any(f => f > 0) && !idsPecasIniciadasNoPeriodo.Any(f => f > 0))
            {
                return new List<int>();
            }

            var sqlPecasIniciadasAntesPeriodoConcluidasNoPeriodo = $@"SELECT DISTINCT(ppp.IdProdPedProducao)
                FROM produto_pedido_producao ppp
                WHERE 1";

            if (idsPecasConcluidasNoPeriodo.Any(f => f > 0))
            {
                sqlPecasIniciadasAntesPeriodoConcluidasNoPeriodo += $" AND ppp.IdProdPedProducao IN ({string.Join(",", idsPecasConcluidasNoPeriodo)})";
            }

            if (idsPecasIniciadasNoPeriodo.Any(f => f > 0))
            {
                sqlPecasIniciadasAntesPeriodoConcluidasNoPeriodo += $" AND ppp.IdProdPedProducao NOT IN ({string.Join(",", idsPecasIniciadasNoPeriodo)})";
            }

            return this.ExecuteMultipleScalar<int>(sqlPecasIniciadasAntesPeriodoConcluidasNoPeriodo);
        }

        private List<int> ObterIdsProdutoProducaoIniciadosAntesPeriodoNaoConcluidosNoPeriodo(
            List<Setor> setoresPronto,
            DateTime dataInicio,
            DateTime dataFim)
        {
            var filtroPecasProntas = string.Empty;

            if (setoresPronto?.Any() ?? false)
            {
                filtroPecasProntas += $@" AND (lp.IdSetor IN ({string.Join(",", setoresPronto.Select(f => f.IdSetor))})
                    OR lp.ProntoRoteiro = 1)";
            }
            else
            {
                filtroPecasProntas += " AND lp.ProntoRoteiro = 1";
            }

            var sqlPecasIniciadasAntesPeriodoNaoConcluidasNoPeriodo = $@"SELECT ppp.IdProdPedProducao
                FROM produto_pedido_producao ppp
	                INNER JOIN (
		                SELECT lp.IdProdPedProducao
		                FROM leitura_producao lp
		                WHERE (lp.DataLeitura IS NULL OR lp.DataLeitura > ?dataFim)
			                {filtroPecasProntas}
	                ) peca_pronta ON (ppp.IdProdPedProducao = peca_pronta.IdProdPedProducao)
	                INNER JOIN (
		                SELECT lp.IdProdPedProducao
		                FROM leitura_producao lp
		                WHERE lp.DataLeitura < ?dataInicio
			                AND lp.IdSetor = 1
	                ) peca_pendente ON (ppp.IdProdPedProducao = peca_pendente.IdProdPedProducao);";

            return this.ExecuteMultipleScalar<int>(
                sqlPecasIniciadasAntesPeriodoNaoConcluidasNoPeriodo,
                this.ObterParametrosConsulta(dataInicio, dataFim));
        }

        private GDAParameter[] ObterParametrosConsulta(DateTime? dataInicio, DateTime? dataFim)
        {
            var parameters = new List<GDAParameter>();

            if (dataInicio.HasValue && dataInicio != DateTime.MinValue)
            {
                parameters.Add(new GDAParameter("?dataInicio", DateTime.Parse($"{dataInicio.Value.ToShortDateString()} 00:00:00")));
            }

            if (dataFim.HasValue && dataFim != DateTime.MinValue)
            {
                parameters.Add(new GDAParameter("?dataFim", DateTime.Parse($"{dataFim.Value.ToShortDateString()} 23:59:59")));
            }

            return parameters.ToArray();
        }
    }
}
