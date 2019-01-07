using GDA;
using Glass.Configuracoes;
using Glass.Data.Helper;
using Glass.Data.Helper.Estoque;
using Glass.Data.Helper.Estoque.Estrategia.Models;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class MovEstoqueDAO : BaseDAO<MovEstoque, MovEstoqueDAO>
    {
        #region Recupera listagem

        private string Sql(uint idLoja, string codInterno, string descricao, string codOtimizacao, string dataIni, string dataFim, int tipoMov, int situacaoProd, string idsGrupoProd,
            string idsSubgrupoProd, uint idCorVidro, uint idCorFerragem, uint idCorAluminio, bool naoBuscarEstoqueZero, bool apenasLancManual, bool selecionar)
        {
            var criterio = string.Empty;
            var campos = selecionar ? @"me.*, p.Descricao as DescrProduto, g.Descricao as DescrGrupo, sg.Descricao as DescrSubgrupo,
                u.codigo as codUnidade, f.nome as nomeFunc,
                (Select Coalesce(fnf.nomeFantasia, fnf.razaoSocial, '') From fornecedor fnf Where fnf.idFornec=nf.idFornec) As nomeFornec, '$$$' as criterio" :
                "Count(*)";

            var sql = $@"
                Select { campos } From mov_estoque me
                    Left Join produto p On (me.idProd=p.idProd)
                    Left Join grupo_prod g on (p.idGrupoProd=g.idGrupoProd)
                    Left Join subgrupo_prod sg on (p.idSubgrupoProd=sg.idSubgrupoProd)
                    Left Join unidade_medida u On (p.idUnidadeMedida=u.idUnidadeMedida)
                    Left Join loja l on (me.idLoja=l.idLoja)
                    Left Join funcionario f On (me.idFunc=f.idFunc)
                    Left Join nota_fiscal nf ON (me.idNf=nf.idNf)
                Where 1";

            // Retorna movimentação apenas se a loja e o produto tiverem sido informados
            if (idLoja == 0 || string.IsNullOrEmpty(codInterno))
            {
                return sql + " And false";
            }

            if (idLoja > 0)
            {
                sql += " And me.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (!string.IsNullOrEmpty(codInterno) || !string.IsNullOrEmpty(descricao) || !string.IsNullOrEmpty(codOtimizacao))
            {
                var ids = ProdutoDAO.Instance.ObtemIds(null, codInterno, descricao, codOtimizacao);
                sql += " And me.idProd In (" + ids + ")";

                if (!string.IsNullOrEmpty(descricao))
                {
                    criterio += "Produto: " + descricao + "    ";
                }
                else
                {
                    criterio += "Produto: " + ProdutoDAO.Instance.GetDescrProduto((GDASession)null, codInterno) + "    ";
                }
            }

            if (!string.IsNullOrEmpty(dataIni))
            {
                sql += " And me.dataMov>=?dataIni";
                criterio += "Período: " + dataIni + "    ";
            }

            if (!string.IsNullOrEmpty(dataFim))
            {
                sql += " And me.dataMov<=?dataFim";
                criterio += " até: " + dataFim + "    ";
            }

            if (tipoMov > 0)
            {
                sql += " And me.tipoMov=" + tipoMov;
                criterio += "Apenas movimentações de " + (tipoMov == (int)MovEstoque.TipoMovEnum.Entrada ? "entrada" : "saída");
            }

            if (situacaoProd > 0)
            {
                sql += " and p.situacao=" + situacaoProd;
                criterio += "Situação: " + (situacaoProd == 1 ? "Ativos" : "Inativos") + "    ";
            }

            if (!string.IsNullOrEmpty(idsGrupoProd))
            {
                sql += " and p.idGrupoProd IN (" + idsGrupoProd + ")";
                var grupos = string.Empty;

                foreach (var id in idsGrupoProd.Split(','))
                {
                    grupos += GrupoProdDAO.Instance.GetDescricao(null, id.StrParaInt()) + ", ";
                }

                criterio += "Grupo(s): " + grupos.TrimEnd(' ', ',') + "    ";
            }

            if (!string.IsNullOrEmpty(idsSubgrupoProd))
            {
                sql += " and p.idSubgrupoProd IN(" + idsSubgrupoProd + ")";

                var subgrupos = string.Empty;

                foreach (var id in idsSubgrupoProd.Split(','))
                {
                    subgrupos += SubgrupoProdDAO.Instance.GetDescricao(null, id.StrParaInt()) + ", ";
                }

                criterio += "Subgrupo(s): " + subgrupos.TrimEnd(' ', ',') + "    ";
            }

            if (idCorVidro > 0)
            {
                sql += " and p.idCorVidro=" + idCorVidro;
                criterio += "Cor Vidro: " + CorVidroDAO.Instance.GetNome(null, idCorVidro) + "    ";
            }

            if (idCorFerragem > 0)
            {
                sql += " and p.idCorFerragem=" + idCorFerragem;
                criterio += "Cor Ferragem: " + CorFerragemDAO.Instance.GetNome(null, idCorFerragem) + "    ";
            }

            if (idCorAluminio > 0)
            {
                sql += " and p.idCorAluminio=" + idCorAluminio;
                criterio += "Cor Alumínio: " + CorAluminioDAO.Instance.GetNome(null, idCorAluminio) + "    ";
            }

            if (apenasLancManual)
            {
                sql += " and me.LancManual=true";
                criterio += "Apenas lançamentos manuais    ";
            }

            if (selecionar)
            {
                sql += " Order By me.dataMov asc, me.idMovEstoque Asc";
            }

            return sql.Replace("$$$", criterio);
        }

        public IList<MovEstoque> GetForRpt(uint idLoja, string codInterno, string descricao, string codOtimizacao, string dataIni, string dataFim, int tipoMov, int situacaoProd,
            string idsGrupoProd, string idsSubgrupoProd, uint idCorVidro, uint idCorFerragem, uint idCorAluminio, bool naoBuscarEstoqueZero, bool apenasLancManual)
        {
            return objPersistence.LoadData(Sql(idLoja, codInterno, descricao, codOtimizacao, dataIni, dataFim, tipoMov, situacaoProd, idsGrupoProd, idsSubgrupoProd, idCorVidro, idCorFerragem,
                idCorAluminio, naoBuscarEstoqueZero, apenasLancManual, true), GetParam(dataIni, dataFim)).ToList();
        }

        public IList<MovEstoque> GetList(uint idLoja, string codInterno, string descricao, string codOtimizacao, string dataIni, string dataFim, int tipoMov, int situacaoProd,
            string idsGrupoProd, string idsSubgrupoProd, uint idCorVidro, uint idCorFerragem, uint idCorAluminio, bool naoBuscarEstoqueZero, bool apenasLancManual)
        {
            return objPersistence.LoadData(Sql(idLoja, codInterno, descricao, codOtimizacao, dataIni, dataFim, tipoMov, situacaoProd, idsGrupoProd, idsSubgrupoProd, idCorVidro, idCorFerragem,
                idCorAluminio, naoBuscarEstoqueZero, apenasLancManual, true), GetParam(dataIni, dataFim)).ToList();
        }

        private GDAParameter[] GetParam(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(dataIni))
            {
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));
            }

            if (!string.IsNullOrEmpty(dataFim))
            {
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));
            }

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        public IList<MovEstoque> GetListByNf(GDASession session, uint idNf)
        {
            return objPersistence.LoadData(session, $"SELECT * FROM mov_estoque WHERE IdNf={ idNf }").ToList();
        }

        #endregion

        #region Obtém o saldo do produto

        private uint? ObtemUltimoIdMovEstoque(GDASession sessao, uint idProd, uint idLoja)
        {
            return ExecuteScalar<uint?>(sessao, @"SELECT IdMovEstoque FROM mov_estoque WHERE IdProd=?idProd
                AND IdLoja=?idLoja ORDER BY DataMov DESC, IdMovEstoque DESC LIMIT 1",
                new GDAParameter("?idProd", idProd), new GDAParameter("?idLoja", idLoja));
        }

        /// <summary>
        /// Obtem o id da movimentação anterior.
        /// </summary>
        private uint? ObtemIdMovAnterior(GDASession sessao, uint? idMovEstoque, uint idProd, uint idLoja, DateTime dataMov)
        {
            // Remove os milisegundos da hora da movimentação
            dataMov = dataMov.AddMilliseconds(-dataMov.Millisecond);

            if (idMovEstoque.GetValueOrDefault() == 0)
            {
                // Adiciona 1 segundo na datamov, para pegar a movimentação correta (Chamado 12177)
                dataMov = dataMov.AddSeconds(1);

                idMovEstoque = ExecuteScalar<uint>(sessao,
                    @"SELECT IdMovEstoque FROM mov_estoque
                    WHERE IdProd=?idProd
                        AND IdLoja=?idLoja
                        AND DataMov<=?data
                    ORDER BY DataMov DESC, IdMovEstoque DESC LIMIT 1",
                    new GDAParameter("?idProd", idProd), new GDAParameter("?idLoja", idLoja),
                    new GDAParameter("?data", dataMov));
            }

            /* Chamado 23697.
             * Criando a tabela temporária ganhamos tempo no fetching. */
            return ExecuteScalar<uint?>(sessao,
                @"SELECT * FROM (SELECT IdMovEstoque FROM mov_estoque
                WHERE IdProd=?idProd
                    AND IdLoja=?idLoja
                    AND (DataMov<?data
                    OR (DataMov=?data AND IdMovEstoque<?idMovEstoque))
                ORDER BY DataMov DESC, IdMovEstoque DESC LIMIT 1) AS temp",
                new GDAParameter("?idProd", idProd),
                new GDAParameter("?idLoja", idLoja),
                new GDAParameter("?data", dataMov),
                new GDAParameter("?idMovEstoque", idMovEstoque));
        }

        /// <summary>
        /// Obtém o saldo em estoque de determinado produto em determinada loja em determinado dia
        /// </summary>
        public decimal ObtemSaldoQtdeMov(GDASession sessao, uint? idMovEstoque, uint idProd, uint idLoja, DateTime? dataMov, bool anterior)
        {
            dataMov = dataMov == null || dataMov == DateTime.MinValue ? DateTime.Now : dataMov;

            if (anterior)
            {
                idMovEstoque = ObtemIdMovAnterior(sessao, idMovEstoque, idProd, idLoja, dataMov.Value);
            }
            else if (idMovEstoque.GetValueOrDefault() == 0)
            {
                idMovEstoque = ObtemUltimoIdMovEstoque(sessao, idProd, idLoja);
            }

            return ObtemValorCampo<decimal>(sessao, "SaldoQtdeMov", $"IdMovEstoque={ idMovEstoque.GetValueOrDefault() }");
        }

        /// <summary>
        /// Obtém o valor total em estoque de determinado produto em determinada loja em determinado dia
        /// </summary>
        public decimal ObtemSaldoValorMov(GDASession sesssao, uint? idMovEstoque, uint idProd, uint idLoja, DateTime dataMov, bool anterior)
        {
            if (anterior)
            {
                idMovEstoque = ObtemIdMovAnterior(sesssao, idMovEstoque, idProd, idLoja, dataMov);
            }
            else if (idMovEstoque.GetValueOrDefault() == 0)
            {
                idMovEstoque = ObtemUltimoIdMovEstoque(sesssao, idProd, idLoja);
            }

            return ObtemValorCampo<decimal>(sesssao, "SaldoValorMov", $"IdMovEstoque={ idMovEstoque.GetValueOrDefault() }");
        }

        #endregion

        #region Recupera mov_estoque chapa

        /// <summary>
        /// Recupera o mov estoque associado a chapa, especificamente para um ProdutoPedidoProducao
        /// </summary>
        public List<int> ObtemMovEstoqueChapaCortePeca(GDASession session, uint idProdPedProducao, string numEtiqueta)
        {
            if (string.IsNullOrWhiteSpace(numEtiqueta) || numEtiqueta.Contains("R"))
            {
                return new List<int>();
            }

            var prodNf = ProdutosNfDAO.Instance.GetProdNfByEtiqueta(session, numEtiqueta);
            var prodImpressao = ProdutoImpressaoDAO.Instance.GetElementByEtiqueta(session, numEtiqueta, ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal);

            return ExecuteMultipleScalar<int>(session,
                $@"SELECT me.idMovEstoque FROM mov_estoque me
                    INNER JOIN produto_pedido_producao ppp ON (ppp.IdProdPedProducao = me.IdProdPedProducao)
                    INNER JOIN produto_impressao pi ON (pi.NumEtiqueta = ppp.NumEtiqueta)
                    INNER JOIN chapa_corte_peca ccp ON (ccp.IdProdImpressaoPeca = pi.IdProdImpressao)
                    INNER JOIN produto_impressao piChapa ON (piChapa.IdProdImpressao = { prodImpressao.IdProdImpressao } And piChapa.IdProdImpressao = ccp.IdProdImpressaoChapa)
                WHERE ppp.IdProdPedProducao = { idProdPedProducao } And me.IdProd = { prodNf.IdProd } group by me.IdMovEstoque");
        }

        #endregion

        #region Recupera o valor total das movimentações

        internal decimal GetTotalProdPed(GDASession sessao, int idProdPed)
        {
            var custoProd = ProdutosPedidoDAO.Instance.ObterCustoProd(sessao, idProdPed);
            var custoBenef = ProdutoPedidoBenefDAO.Instance.ObterCustoTotalPeloIdProdPed(sessao, idProdPed);

            return custoProd + custoBenef;
        }

        private decimal GetTotalProdCompra(GDASession sessao, int idProdCompra)
        {
            var totalProd = ProdutosCompraDAO.Instance.ObterTotal(sessao, idProdCompra);
            var totalBenef = ProdutosCompraBenefDAO.Instance.ObterValorTotalPeloIdProdCompra(sessao, idProdCompra);

            return totalProd + totalBenef;
        }

        internal decimal GetTotalProdLiberarPedido(GDASession sessao, int idProdLiberarPedido)
        {
            var idProdPed = ProdutosLiberarPedidoDAO.Instance.ObterIdProdPed(sessao, idProdLiberarPedido);
            var custoProd = GetTotalProdPed(sessao, idProdPed);
            var qtdeProd = ProdutosPedidoDAO.Instance.ObtemQtde(sessao, (uint)idProdPed);
            var qtdeLib = ProdutosLiberarPedidoDAO.Instance.ObterQtde(sessao, idProdLiberarPedido);
            var qtdTotal = (decimal)qtdeProd * qtdeLib;

            return custoProd / (qtdTotal > 0 ? qtdTotal : 1);
        }

        private decimal GetTotalProdTrocaDevolucao(GDASession session, int? idProdTrocaDev, int? idProdTrocado)
        {
            decimal custo = 0, custoBenef = 0;

            if (idProdTrocaDev > 0)
            {
                custo = ProdutoTrocaDevolucaoDAO.Instance.ObterCustoProd(session, idProdTrocaDev.Value);
                custoBenef = ProdutoTrocaDevolucaoBenefDAO.Instance.ObterCustoTotalPeloIdProdTrocaDev(session, idProdTrocaDev.Value);
            }
            else if (idProdTrocado > 0)
            {
                custo = ProdutoTrocadoDAO.Instance.ObterCustoProd(session, idProdTrocado.Value);
                custoBenef = ProdutoTrocadoBenefDAO.Instance.ObterCustoTotalPeloIdProdTrocado(session, idProdTrocado.Value);
            }

            return custo + custoBenef;
        }

        private decimal GetTotalProdPedInterno(GDASession session, int idProd, int idProdPedInterno)
        {
            var custoCompra = ProdutoDAO.Instance.ObtemCustoCompra(session, idProd);
            var qtde = ProdutoPedidoInternoDAO.Instance.ObterTotMQtde(session, idProdPedInterno);

            return custoCompra * (decimal)qtde;
        }

        internal decimal GetTotalEstoqueManual(GDASession sessao, int idProd, decimal qtde)
        {
            var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(sessao, idProd, false);
            var divisor = new List<int> { (int)TipoCalculoGrupoProd.MLAL0, (int)TipoCalculoGrupoProd.MLAL05, (int)TipoCalculoGrupoProd.MLAL1, (int)TipoCalculoGrupoProd.MLAL6 }.Contains(tipoCalculo) ? 6 : 1;
            var custoCompra = ProdutoDAO.Instance.ObtemCustoCompra(sessao, idProd);

            return custoCompra / divisor * qtde;
        }

        internal decimal GetTotalProdPedProducao(GDASession session, int idProdPedProducao)
        {
            var idProdPed = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(session, (uint)idProdPedProducao);
            idProdPed = ProdutosPedidoDAO.Instance.GetIdProdPedByProdPedEsp(session, idProdPed);
            var custoProd = GetTotalProdPed(session, (int)idProdPed);
            var qtdeProd = ProdutosPedidoDAO.Instance.ObtemQtde(session, idProdPed);
            var idProd = (int)ProdutosPedidoDAO.Instance.ObtemIdProd(session, idProdPed);
            // Caso o custo do produto esteja zerado, busca diretamente do cadastro de produto
            custoProd = custoProd > 0 ? custoProd / (decimal)qtdeProd : ProdutoDAO.Instance.ObtemCustoCompra(session, idProd);

            return custoProd;
        }

        #endregion

        #region Métodos privados controle estoque

        private decimal CalcularQuantidadeEstoque(TipoCalculoGrupoProd tipoCalculo, float quantidadeMovimentacao, float quantidadeProduto, float metroQuadrado, float altura)
        {
            var calculaMetroQuadrado = tipoCalculo == TipoCalculoGrupoProd.M2 || tipoCalculo == TipoCalculoGrupoProd.M2Direto;

            if (tipoCalculo == TipoCalculoGrupoProd.MLAL0
                || tipoCalculo == TipoCalculoGrupoProd.MLAL05
                || tipoCalculo == TipoCalculoGrupoProd.MLAL1
                || tipoCalculo == TipoCalculoGrupoProd.MLAL6
                || tipoCalculo == TipoCalculoGrupoProd.ML)
            {
                quantidadeMovimentacao *= altura;
            }

            return (decimal)(calculaMetroQuadrado ? (metroQuadrado / quantidadeProduto) * quantidadeMovimentacao : quantidadeMovimentacao);
        }

        private bool VerificarAlterarMateriaPrima(GDASession sessao, int idGrupoProduto, int idSubgrupoProduto, TipoCalculoGrupoProd tipoCalculo, Pedido.TipoPedidoEnum tipoPedido, int idProduto)
        {
            var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupoPorSubgrupo(sessao, idSubgrupoProduto);

            return GrupoProdDAO.Instance.IsVidro(idGrupoProduto)
                && tipoCalculo != TipoCalculoGrupoProd.Qtd
                && tipoSubgrupo != TipoSubgrupoProd.ChapasVidro
                && tipoSubgrupo != TipoSubgrupoProd.ChapasVidroLaminado
                && (tipoPedido != Pedido.TipoPedidoEnum.Producao || !ProdutoDAO.Instance.IsProdutoProducao(sessao, idProduto));
        }

        private int ObterIdLojaTrocaDevolucao(GDASession sessao, int idTrocaDevolucao, int idProd)
        {
            var idPedido = TrocaDevolucaoDAO.Instance.ObterIdPedido(sessao, idTrocaDevolucao);
            var idLoja = Geral.ConsiderarLojaClientePedidoFluxoSistema && idPedido > 0
                ? (int)PedidoDAO.Instance.ObtemIdLoja(sessao, (uint)idPedido)
                : (int)UserInfo.GetUserInfo.IdLoja;

            var etiquetasTrocadas = ChapaTrocadaDevolvidaDAO.Instance.BuscarEtiquetasJaEntreguesPelaTrocaDevolucao(sessao, idTrocaDevolucao)
                .Split(',')
                .Where(f => !string.IsNullOrWhiteSpace(f.ToString()))
                .ToList();

            foreach (var etiqueta in etiquetasTrocadas)
            {
                var idProdNf = ProdutoImpressaoDAO.Instance.ObtemIdProdNf(sessao, etiqueta, ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal);
                var idProdEtiqueta = ProdutosNfDAO.Instance.GetIdProdByEtiqueta(sessao, etiqueta);
                if (idProdEtiqueta == idProd && idProdNf > 0)
                {
                    idLoja = (int)NotaFiscalDAO.Instance.ObtemIdLoja(sessao, ProdutosNfDAO.Instance.ObtemIdNf(sessao, idProdNf));
                }
            }

            return idLoja;
        }

        private int ObterIdLojaProdutoPedidoProducao(GDASession sessao, int idProdutoPedidoProducao, int idPedido, int idProduto)
        {
            var idLoja = Geral.ConsiderarLojaClientePedidoFluxoSistema && idPedido > 0
                ? (int)PedidoDAO.Instance.ObtemIdLoja(sessao, (uint)idPedido)
                : (int)UserInfo.GetUserInfo.IdLoja;

            var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, idProduto);

            if (tipoSubgrupo != TipoSubgrupoProd.ChapasVidro && tipoSubgrupo != TipoSubgrupoProd.ChapasVidroLaminado)
            {
                return idLoja;
            }

            var numEtiqueta = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<string>(sessao, "NumEtiqueta", $"IdProdPedProducao={idProdutoPedidoProducao}");
            var idProdImpressaoPeca = ProdutoImpressaoDAO.Instance.ObtemIdProdImpressao(sessao, numEtiqueta, ProdutoImpressaoDAO.TipoEtiqueta.Pedido);
            var idProdImpressaoChapa = ChapaCortePecaDAO.Instance.ObtemIdProdImpressaoChapa(sessao, (int)idProdImpressaoPeca);
            var idNotaFiscal = ProdutoImpressaoDAO.Instance.ObtemIdNf(sessao, (uint)idProdImpressaoChapa) ?? 0;

            if (idNotaFiscal > 0)
            {
                idLoja = (int)NotaFiscalDAO.Instance.ObtemIdLoja(sessao, idNotaFiscal);
            }

            return idLoja;
        }

        private int ObterIdLojaPeloIdProdImpressaoChapa(GDASession sessao, int idPedido, int idProduto, int idProdutoImpressaoChapa)
        {
            var idLoja = Geral.ConsiderarLojaClientePedidoFluxoSistema && idPedido > 0
                ? (int)PedidoDAO.Instance.ObtemIdLoja(sessao, (uint)idPedido)
                : (int)UserInfo.GetUserInfo.IdLoja;

            var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, idProduto);

            if (tipoSubgrupo != TipoSubgrupoProd.ChapasVidro && tipoSubgrupo != TipoSubgrupoProd.ChapasVidroLaminado)
            {
                return idLoja;
            }

            var idNotaFiscal = ProdutoImpressaoDAO.Instance.ObtemIdNf(sessao, (uint)idProdutoImpressaoChapa) ?? 0;

            if (idNotaFiscal > 0)
            {
                idLoja = (int)NotaFiscalDAO.Instance.ObtemIdLoja(sessao, idNotaFiscal);
            }

            return idLoja;
        }

        #endregion

        #region Baixa Estoque

        public void BaixaEstoqueConfirmacaoPedido(GDASession sessao, uint idPedido, IEnumerable<ProdutosPedido> produtosPedido)
        {
            if (!FinanceiroConfig.Estoque.SaidaEstoqueAutomaticaAoConfirmar)
            {
                return;
            }

            var tipoPedido = PedidoDAO.Instance.GetTipoPedido(sessao, idPedido);
            var idLoja = PedidoDAO.Instance.ObtemIdLoja(sessao, idPedido);
            var idSaidaEstoque = SaidaEstoqueDAO.Instance.GetNewSaidaEstoque(sessao, idLoja, idPedido, null, null, false);

            foreach (var item in produtosPedido)
            {
                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)item.IdGrupoProd, (int)item.IdSubgrupoProd, false);
                var quantidadeBaixa = CalcularQuantidadeEstoque(tipoCalculo, item.Qtde - item.QtdSaida, item.Qtde, item.TotM, item.Altura);

                if ((decimal)item.Qtde <= (decimal)item.QtdSaida)
                {
                    throw new InvalidOperationException($"Operação cancelada. O produto {item.DescrProduto} teve uma saída maior do que sua quantidade.");
                }

                if (quantidadeBaixa > 0)
                {
                    ProdutosPedidoDAO.Instance.MarcarSaida(
                        sessao,
                        item.IdProdPed,
                        item.Qtde - item.QtdSaida,
                        idSaidaEstoque,
                        System.Reflection.MethodBase.GetCurrentMethod().Name,
                        string.Empty,
                        saidaConfirmacao: true);

                    new EstoqueStrategyFactory()
                        .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                        .Baixar(sessao, new MovimentacaoDto
                        {
                            IdProduto = item.IdProd,
                            IdLoja = idLoja,
                            IdPedido = idPedido,
                            IdProdPed = item.IdProdPed,
                            Quantidade = quantidadeBaixa,
                            Total = GetTotalProdPed(sessao, (int)item.IdProdPed),
                            AlterarMateriaPrima = VerificarAlterarMateriaPrima(sessao, (int)item.IdGrupoProd, (int)item.IdSubgrupoProd, tipoCalculo, tipoPedido, (int)item.IdProd),
                            BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                            AlterarProdutoBase = true,
                        });
                }
            }

            if (tipoPedido != Pedido.TipoPedidoEnum.Producao && !FinanceiroConfig.Estoque.SaidaEstoqueAutomaticaAoConfirmar)
            {
                ProdutoLojaDAO.Instance.RecalcularReserva(sessao, (int)idLoja, produtosPedido.Select(f => (int)f.IdProd).Distinct());
            }
        }

        public void BaixaEstoqueChapa(GDASession sessao, int idProdPed, uint? idProdImpressaoChapa)
        {
            var produtoPedido = ProdutosPedidoDAO.Instance.GetElementFluxoLite(sessao, (uint)idProdPed);

            if (produtoPedido.Qtde <= produtoPedido.QtdSaida)
            {
                throw new InvalidOperationException($"Operação cancelada. O produto {produtoPedido.DescrProduto} teve uma saída maior do que sua quantidade.");
            }

            var idLoja = ObterIdLojaPeloIdProdImpressaoChapa(sessao, (int)produtoPedido.IdPedido, (int)produtoPedido.IdProd, (int)idProdImpressaoChapa);

            var idSaidaEstoque = SaidaEstoqueDAO.Instance.GetNewSaidaEstoque(sessao, (uint)idLoja, produtoPedido.IdPedido, null, null, false);

            ProdutosPedidoDAO.Instance.MarcarSaida(sessao, produtoPedido.IdProdPed, 1, idSaidaEstoque, System.Reflection.MethodBase.GetCurrentMethod().Name, string.Empty);

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Baixar(sessao, new MovimentacaoDto
                {
                    IdProduto = produtoPedido.IdProd,
                    IdLoja = (uint)idLoja,
                    IdPedido = produtoPedido.IdPedido,
                    IdProdPed = produtoPedido.IdProdPed,
                    IdProdImpressaoChapa = idProdImpressaoChapa,
                    Total = GetTotalProdPed(sessao, (int)produtoPedido.IdProdPed),
                    AlterarProdutoBase = true,
                });
        }

        public void BaixaEstoqueCorteChapa(GDASession sessao, uint idProdPedProducao, int idProdImpressaoChapa)
        {
            uint? idProd = ProdutoImpressaoDAO.Instance.GetIdProd(sessao, (uint)idProdImpressaoChapa);

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Baixar(sessao, new MovimentacaoDto
                {
                    IdProduto = idProd.Value,
                    IdLoja = (uint)ObterIdLojaPeloIdProdImpressaoChapa(sessao, 0, (int)idProd.Value, idProdImpressaoChapa),
                    IdProdPedProducao = idProdPedProducao,
                    Total = GetTotalProdPedProducao(sessao, (int)idProdPedProducao),
                });
        }

        public void BaixaEstoqueVolume(GDASession sessao, uint idPedido, IEnumerable<VolumeProdutosPedido> volumes)
        {
            var idLoja = PedidoDAO.Instance.ObtemIdLoja(sessao, idPedido);
            var idSaidaEstoque = SaidaEstoqueDAO.Instance.GetNewSaidaEstoque(sessao, idLoja, idPedido, null, null, false);
            var tipoPedido = PedidoDAO.Instance.GetTipoPedido(sessao, idPedido);

            foreach (var item in volumes)
            {
                var produtoPedido = ProdutosPedidoDAO.Instance.GetElementFluxoLite(sessao, item.IdProdPed);

                if ((decimal)item.Qtde > ((decimal)produtoPedido.Qtde - (decimal)produtoPedido.QtdSaida))
                {
                    throw new InvalidOperationException($"Operação cancelada. O produto {produtoPedido.DescrProduto} teve uma saída maior do que sua quantidade.");
                }

                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)produtoPedido.IdGrupoProd, (int)produtoPedido.IdSubgrupoProd, false);
                var quantidadeBaixa = CalcularQuantidadeEstoque(tipoCalculo, item.Qtde, produtoPedido.Qtde, produtoPedido.TotM, produtoPedido.Altura);

                ProdutosPedidoDAO.Instance.MarcarSaida(sessao, produtoPedido.IdProdPed, item.Qtde, idSaidaEstoque, System.Reflection.MethodBase.GetCurrentMethod().Name, string.Empty);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Baixar(sessao, new MovimentacaoDto
                    {
                        IdProduto = produtoPedido.IdProd,
                        IdLoja = idLoja,
                        IdPedido = produtoPedido.IdPedido,
                        IdProdPed = produtoPedido.IdProdPed,
                        IdVolume = item.IdVolume,
                        Quantidade = quantidadeBaixa,
                        Total = GetTotalProdPed(sessao, (int)produtoPedido.IdProdPed),
                        AlterarMateriaPrima = VerificarAlterarMateriaPrima(sessao, (int)produtoPedido.IdGrupoProd, (int)produtoPedido.IdSubgrupoProd, tipoCalculo, tipoPedido, (int)produtoPedido.IdProd),
                        BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                        AlterarProdutoBase = true,
                    });
            }

            PedidoDAO.Instance.MarcaPedidoEntregue(sessao, idPedido);

            if (tipoPedido != Pedido.TipoPedidoEnum.Producao)
            {
                var idsProduto = volumes.Select(f => (int)f.IdProd).Distinct();

                ProdutoLojaDAO.Instance.RecalcularReserva(sessao, (int)idLoja, idsProduto);
                ProdutoLojaDAO.Instance.RecalcularLiberacao(sessao, (int)idLoja, idsProduto);
            }
        }

        public void BaixaEstoqueManual(GDASession sessao, int idLoja, int idPedido, IEnumerable<KeyValuePair<int, float>> produtosPedido, string observacao)
        {
            var idSaidaEstoque = SaidaEstoqueDAO.Instance.GetNewSaidaEstoque(sessao, (uint)idLoja, (uint)idPedido, null, null, true);
            var tipoPedido = PedidoDAO.Instance.GetTipoPedido(sessao, (uint)idPedido);
            var idsProduto = new List<int>();

            foreach (var item in produtosPedido)
            {
                var produtoPedido = ProdutosPedidoDAO.Instance.GetElement((uint)item.Key);
                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)produtoPedido.IdGrupoProd, (int)produtoPedido.IdSubgrupoProd, false);
                var quantidadeBaixa = CalcularQuantidadeEstoque(tipoCalculo, item.Value, produtoPedido.Qtde, produtoPedido.TotM, produtoPedido.Altura);

                ProdutosPedidoDAO.Instance.MarcarSaida(sessao, produtoPedido.IdProdPed, item.Value, idSaidaEstoque, System.Reflection.MethodBase.GetCurrentMethod().Name, string.Empty);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Baixar(sessao, new MovimentacaoDto
                    {
                        IdProduto = produtoPedido.IdProd,
                        IdLoja = (uint)idLoja,
                        IdPedido = produtoPedido.IdPedido,
                        IdProdPed = produtoPedido.IdProdPed,
                        Quantidade = quantidadeBaixa,
                        LancamentoManual = true,
                        Total = GetTotalProdPed(sessao, (int)produtoPedido.IdProdPed),
                        AlterarMateriaPrima = VerificarAlterarMateriaPrima(sessao, (int)produtoPedido.IdGrupoProd, (int)produtoPedido.IdSubgrupoProd, tipoCalculo, tipoPedido, (int)produtoPedido.IdProd),
                        BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                        AlterarProdutoBase = true,
                        Observacao = observacao,
                    });

                idsProduto.Add((int)produtoPedido.IdProd);
            }

            PedidoDAO.Instance.MarcaPedidoEntregue(sessao, (uint)idPedido);

            if (tipoPedido != Pedido.TipoPedidoEnum.Producao)
            {
                ProdutoLojaDAO.Instance.RecalcularReserva(sessao, idLoja, idsProduto.Distinct());
                ProdutoLojaDAO.Instance.RecalcularLiberacao(sessao, idLoja, idsProduto.Distinct());
            }
        }

        public void BaixaEstoqueCancelamentoCompra(GDASession sessao, int idCompra, IEnumerable<ProdutosCompra> produtosCompra)
        {
            foreach (var item in produtosCompra)
            {
                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)item.IdGrupoProd, (int)item.IdSubgrupoProd, false);
                var metroQuadrado = Global.CalculosFluxo.ArredondaM2Compra(item.Largura, (int)item.Altura, (int)item.QtdeEntrada);
                var quantidadeBaixa = CalcularQuantidadeEstoque(tipoCalculo, item.QtdeEntrada, item.Qtde, metroQuadrado, item.Altura);

                if (quantidadeBaixa > 0)
                {
                    var idLoja = ObterIdLojaPeloIdCompra(sessao, idCompra).GetValueOrDefault((int)UserInfo.GetUserInfo.IdLoja);

                    new EstoqueStrategyFactory()
                        .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                        .Baixar(sessao, new MovimentacaoDto
                        {
                            IdProduto = item.IdProd,
                            IdLoja = (uint)idLoja,
                            IdCompra = (uint)idCompra,
                            IdProdCompra = item.IdProdCompra,
                            Quantidade = quantidadeBaixa,
                            Total = GetTotalProdCompra(sessao, (int)item.IdProdCompra),
                            AlterarMateriaPrima = !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)item.IdProd),
                            BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                            AlterarProdutoBase = true,
                        });
                }

                objPersistence.ExecuteCommand(sessao, "update produtos_compra set qtdeEntrada=0 where idProdCompra=" + item.IdProdCompra);
            }

            objPersistence.ExecuteCommand(sessao, "update compra set estoqueBaixado=false where idCompra=" + idCompra);
        }

        public void BaixaEstoqueCancelamentoEntradaEstoqueCompra(GDASession sessao, int idLoja, int idCompra, int idEntradaEstoque, IEnumerable<ProdutoEntradaEstoque> produtosEntradaEstoque)
        {
            foreach (var item in produtosEntradaEstoque)
            {
                var produtoCompra = ProdutosCompraDAO.Instance.GetElementByPrimaryKey(sessao, item.IdProdCompra.Value);

                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)produtoCompra.IdGrupoProd, (int)produtoCompra.IdSubgrupoProd, false);
                var metroQuadrado = Global.CalculosFluxo.ArredondaM2Compra(produtoCompra.Largura, (int)produtoCompra.Altura, (int)item.QtdeEntrada);
                var quantidadeBaixa = CalcularQuantidadeEstoque(tipoCalculo, item.QtdeEntrada, produtoCompra.Qtde, metroQuadrado, produtoCompra.Altura);

                ProdutosCompraDAO.Instance.MarcarEntrada(sessao, item.IdProdCompra.Value, -item.QtdeEntrada, (uint)idEntradaEstoque);

                if (quantidadeBaixa > 0)
                {
                    new EstoqueStrategyFactory()
                        .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                        .Baixar(sessao, new MovimentacaoDto
                        {
                            IdProduto = produtoCompra.IdProd,
                            IdLoja = (uint)idLoja,
                            IdCompra = (uint)idCompra,
                            IdProdCompra = item.IdProdCompra,
                            Quantidade = quantidadeBaixa,
                            Total = GetTotalProdCompra(sessao, (int)item.IdProdCompra),
                            AlterarMateriaPrima = !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)produtoCompra.IdProd),
                            BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                            AlterarProdutoBase = true,
                        });
                }
            }
        }

        public void BaixaEstoqueCancelamentoEntradaEstoqueNotaFiscal(GDASession sessao, int idLoja, int idNotaFiscal, int idEntradaEstoque, IEnumerable<ProdutoEntradaEstoque> produtosEntradaEstoque)
        {
            foreach (var item in produtosEntradaEstoque)
            {
                var produtoNotaFiscal = ProdutosNfDAO.Instance.GetElementByPrimaryKey(sessao, item.IdProdNf.Value);

                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)produtoNotaFiscal.IdGrupoProd, (int)produtoNotaFiscal.IdSubgrupoProd, false);
                var metroQuadrado = Global.CalculosFluxo.ArredondaM2Compra(produtoNotaFiscal.Largura, (int)produtoNotaFiscal.Altura, (int)item.QtdeEntrada);
                var quantidadeBaixa = CalcularQuantidadeEstoque(tipoCalculo, item.QtdeEntrada, produtoNotaFiscal.Qtde, metroQuadrado, produtoNotaFiscal.Altura);

                ProdutosNfDAO.Instance.MarcarEntrada(sessao, item.IdProdNf.Value, -item.QtdeEntrada, (uint)idEntradaEstoque);

                if (quantidadeBaixa > 0)
                {
                    new EstoqueStrategyFactory()
                        .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                        .Baixar(sessao, new MovimentacaoDto
                        {
                            IdProduto = produtoNotaFiscal.IdProd,
                            IdLoja = (uint)idLoja,
                            IdNf = (uint)idNotaFiscal,
                            IdProdNf = produtoNotaFiscal.IdProdNf,
                            Quantidade = quantidadeBaixa,
                            Total = ProdutosNfDAO.Instance.ObterTotal(sessao, (int)produtoNotaFiscal.IdProdNf),
                            AlterarMateriaPrima = !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)produtoNotaFiscal.IdProd),
                            BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                            AlterarProdutoBase = true,
                        });
                }
            }
        }

        public void BaixaEstoqueLiberacao(GDASession sessao, uint idLiberarPedido, uint idCliente, IEnumerable<KeyValuePair<int, float>> produtosPedido)
        {
            var idsProdutoReservaLiberacao = new Dictionary<int, List<int>>();

            foreach (var item in produtosPedido)
            {
                var produtoPedido = ProdutosPedidoDAO.Instance.GetElementFluxoLite(sessao, (uint)item.Key);
                var idLojaPedido = PedidoDAO.Instance.ObtemIdLoja(sessao, produtoPedido.IdPedido);
                var idLoja = idLojaPedido > 0 ? idLojaPedido : UserInfo.GetUserInfo.IdLoja;

                var saidaNaoVidro = Liberacao.Estoque.SaidaEstoqueAoLiberarPedido && (!GrupoProdDAO.Instance.IsVidro((int)produtoPedido.IdGrupoProd) || !PCPConfig.ControlarProducao);
                var saidaBox = Liberacao.Estoque.SaidaEstoqueBoxLiberar && GrupoProdDAO.Instance.IsVidro((int)produtoPedido.IdGrupoProd) && SubgrupoProdDAO.Instance.IsSubgrupoProducao(sessao, (int)produtoPedido.IdGrupoProd, (int?)produtoPedido.IdSubgrupoProd);
                var subGrupoVolume = OrdemCargaConfig.UsarControleOrdemCarga && SubgrupoProdDAO.Instance.IsSubgrupoGeraVolume(sessao, produtoPedido.IdGrupoProd, produtoPedido.IdSubgrupoProd);
                var entregaBalcao = PedidoDAO.Instance.ObtemTipoEntrega(sessao, produtoPedido.IdPedido) == (int)Pedido.TipoEntregaPedido.Balcao;
                var naoVolume = OrdemCargaConfig.GerarVolumeApenasDePedidosEntrega ? entregaBalcao || !subGrupoVolume : !subGrupoVolume;
                var pedidoGerarProducaoParaCorte = PedidoDAO.Instance.GerarPedidoProducaoCorte(sessao, produtoPedido.IdPedido);
                var pedidoPossuiVolumeExpedido = false;

                foreach (var volume in VolumeDAO.Instance.ObterPeloPedido(sessao, (int)produtoPedido.IdPedido))
                {
                    if (VolumeDAO.Instance.TemExpedicao(sessao, volume.IdVolume))
                    {
                        pedidoPossuiVolumeExpedido = true;
                        break;
                    }
                }

                if (!idsProdutoReservaLiberacao.ContainsKey((int)idLoja))
                {
                    idsProdutoReservaLiberacao.Add((int)idLoja, new List<int> { (int)produtoPedido.IdProd });
                }
                else if (!idsProdutoReservaLiberacao[(int)idLoja].Contains((int)produtoPedido.IdProd))
                {
                    idsProdutoReservaLiberacao[(int)idLoja].Add((int)produtoPedido.IdProd);
                }

                if (pedidoPossuiVolumeExpedido || pedidoGerarProducaoParaCorte || !naoVolume || (!saidaNaoVidro && !saidaBox))
                {
                    continue;
                }

                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)produtoPedido.IdGrupoProd, (int?)produtoPedido.IdSubgrupoProd, false);
                var m2Calc = Global.CalculosFluxo.ArredondaM2(sessao, produtoPedido.Largura, (int)produtoPedido.Altura, item.Value, 0, produtoPedido.Redondo, 0, tipoCalculo != TipoCalculoGrupoProd.M2Direto);
                var qtdeBaixa = CalcularQuantidadeEstoque(tipoCalculo, item.Value, produtoPedido.Qtde, m2Calc, produtoPedido.Altura);

                // Marca quantos produtos do pedido foi marcado como saída, se o pedido não tiver que transferir
                var idSaidaEstoque = SaidaEstoqueDAO.Instance.GetNewSaidaEstoque(sessao, idLoja, null, idLiberarPedido, null, false);
                ProdutosPedidoDAO.Instance.MarcarSaida(sessao, produtoPedido.IdProdPed, item.Value, idSaidaEstoque, System.Reflection.MethodBase.GetCurrentMethod().Name, string.Empty);

                var idProdutoLiberarPedido = ProdutosLiberarPedidoDAO.Instance.ObtemIdProdLiberarPedido(sessao, idLiberarPedido, produtoPedido.IdProdPed);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Baixar(sessao, new MovimentacaoDto
                    {
                        IdProduto = produtoPedido.IdProd,
                        IdLoja = idLoja,
                        IdPedido = produtoPedido.IdPedido,
                        IdLiberarPedido = idLiberarPedido,
                        IdProdLiberarPedido = idProdutoLiberarPedido,
                        Quantidade = qtdeBaixa,
                        Total = GetTotalProdLiberarPedido(sessao, (int)idProdutoLiberarPedido),
                        AlterarMateriaPrima = !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)produtoPedido.IdProd),
                        BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                        AlterarProdutoBase = true,
                    });
            }

            foreach (var idLoja in idsProdutoReservaLiberacao.Keys)
            {
                if (idsProdutoReservaLiberacao[idLoja].Count > 0)
                {
                    ProdutoLojaDAO.Instance.RecalcularReserva(sessao, idLoja, idsProdutoReservaLiberacao[idLoja]);
                    ProdutoLojaDAO.Instance.RecalcularLiberacao(sessao, idLoja, idsProdutoReservaLiberacao[idLoja]);
                }
            }
        }

        public void BaixaEstoqueProdutoDevolvidoTrocaDevolucao(GDASession sessao, int idTrocaDevolucao, IEnumerable<ProdutoTrocaDevolucao> produtosDevolvidos)
        {
            foreach (var item in produtosDevolvidos.Where(f => f.AlterarEstoque))
            {
                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)item.IdGrupoProd, (int?)item.IdSubgrupoProd, false);
                var qtdeBaixa = CalcularQuantidadeEstoque(tipoCalculo, item.Qtde, item.Qtde, item.TotM, item.Altura);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Baixar(sessao, new MovimentacaoDto
                    {
                        IdProduto = item.IdProd,
                        IdLoja = (uint)ObterIdLojaTrocaDevolucao(sessao, idTrocaDevolucao, (int)item.IdProd),
                        IdTrocaDevolucao = item.IdTrocaDevolucao,
                        IdProdTrocaDev = item.IdProdTrocaDev,
                        Quantidade = qtdeBaixa,
                        Total = GetTotalProdTrocaDevolucao(sessao, (int?)item.IdProdTrocaDev, null),
                        AlterarMateriaPrima = !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)item.IdProd),
                        BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                        AlterarProdutoBase = true,
                    });
            }
        }

        public void BaixaEstoqueProdutoTrocadoTrocaDevolucao(GDASession sessao, int idTrocaDevolucao, IEnumerable<ProdutoTrocado> produtosTrocados)
        {
            foreach (var item in produtosTrocados)
            {
                var idLoja = (uint)ObterIdLojaTrocaDevolucao(sessao, idTrocaDevolucao, (int)item.IdProd);
                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)item.IdGrupoProd, (int?)item.IdSubgrupoProd, false);
                var qtdeBaixa = CalcularQuantidadeEstoque(tipoCalculo, item.Qtde, item.Qtde, item.TotM, item.Altura);

                if (item.ComDefeito)
                {
                    ProdutoLojaDAO.Instance.BaixaDefeito(sessao, item.IdProd, idLoja, (float)qtdeBaixa);
                }
                else if (item.AlterarEstoque)
                {
                    new EstoqueStrategyFactory()
                        .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                        .Baixar(sessao, new MovimentacaoDto
                        {
                            IdProduto = item.IdProd,
                            IdLoja = idLoja,
                            IdTrocaDevolucao = item.IdTrocaDevolucao,
                            IdProdTrocado = item.IdProdTrocado,
                            Quantidade = qtdeBaixa,
                            Total = GetTotalProdTrocaDevolucao(sessao, null, (int?)item.IdProdTrocado),
                            AlterarMateriaPrima = !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)item.IdProd),
                            BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                            AlterarProdutoBase = true,
                        });
                }
            }
        }

        public void BaixaEstoqueRealNotaFiscal(GDASession sessao, NotaFiscal notaFiscal, IEnumerable<ProdutosNf> produtosNotaFiscal)
        {
            if (notaFiscal.SaiuEstoque || !notaFiscal.GerarEstoqueReal)
            {
                return;
            }

            foreach (var item in produtosNotaFiscal)
            {
                var quantidadeBaixa = (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(sessao, item.IdProd, item.TotM, item.Qtde, item.Altura, item.Largura, false, false);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Baixar(sessao, new MovimentacaoDto
                    {
                        IdProduto = item.IdProd,
                        IdLoja = notaFiscal.IdLoja.Value,
                        IdNf = item.IdNf,
                        IdProdNf = item.IdProdNf,
                        Quantidade = quantidadeBaixa,
                        Total = ProdutosNfDAO.Instance.ObterTotal(sessao, (int)item.IdProdNf),
                        AlterarMateriaPrima = !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)item.IdProd),
                        BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                        AlterarProdutoBase = true,
                    });

                objPersistence.ExecuteCommand(sessao, $"UPDATE produtos_nf SET qtdeSaida={ProdutosNfDAO.Instance.ObtemQtdDanfe(sessao, item, false).ToString().Replace(",", ".")} Where idProdNf={item.IdProdNf}");
            }
        }

        public void BaixaEstoquePedidoInterno(GDASession sessao, int idLoja, Dictionary<int, float> saidasProduto, IEnumerable<ProdutoPedidoInterno> produtosPedidoInterno)
        {
            foreach (var item in produtosPedidoInterno)
            {
                saidasProduto[(int)item.IdProdPedInterno] = Math.Min(saidasProduto[(int)item.IdProdPedInterno], item.QtdeConfirmar);

                if (saidasProduto[(int)item.IdProdPedInterno] <= 0)
                {
                    continue;
                }

                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)item.IdGrupoProd, (int?)item.IdSubgrupoProd, false);

                var quantidadeBaixa = CalcularQuantidadeEstoque(
                    tipoCalculo,
                    item.ConfirmarQtde ? saidasProduto[(int)item.IdProdPedInterno] : 1,
                    item.Qtde,
                    item.TotM,
                    item.Altura);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Baixar(sessao, new MovimentacaoDto
                    {
                        IdProduto = item.IdProd,
                        IdLoja = (uint)idLoja,
                        IdPedidoInterno = item.IdPedidoInterno,
                        IdProdPedInterno = item.IdProdPedInterno,
                        Quantidade = quantidadeBaixa,
                        Total = GetTotalProdPedInterno(sessao, (int)item.IdProd, (int)item.IdProdPedInterno),
                        AlterarMateriaPrima = !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)item.IdProd),
                        BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                        AlterarProdutoBase = true,
                    });

                objPersistence.ExecuteCommand(sessao, "UPDATE produto_pedido_interno SET qtdeConfirmada=COALESCE(qtdeConfirmada,0)+?qtde WHERE idProdPedInterno=" + item.IdProdPedInterno,
                    new GDAParameter("?qtde", saidasProduto[(int)item.IdProdPedInterno]));

                var idCentroCusto = PedidoInternoDAO.Instance.ObtemIdCentroCusto(sessao, (int)item.IdPedidoInterno);

                if (FiscalConfig.UsarControleCentroCusto && CentroCustoDAO.Instance.GetCountReal(sessao) > 0 && idCentroCusto.GetValueOrDefault(0) > 0)
                {
                    CentroCustoAssociadoDAO.Instance.Insert(sessao, new CentroCustoAssociado()
                    {
                        IdCentroCusto = idCentroCusto.Value,
                        IdPedidoInterno = (int)item.IdPedidoInterno,
                        IdConta = MovEstoqueCentroCustoDAO.Instance.ObtemUltimoIdConta(sessao, idLoja, (int)item.IdProd).GetValueOrDefault(),
                        Valor = quantidadeBaixa * MovEstoqueCentroCustoDAO.Instance.ObtemValorUnitarioProd(sessao, idLoja, (int)item.IdProd)
                    });

                    MovEstoqueCentroCustoDAO.Instance.BaixaEstoquePedidoInterno(sessao, (int)item.IdPedidoInterno, (int)item.IdProd, idLoja, quantidadeBaixa);
                }
            }
        }

        public void BaixaEstoqueManual(GDASession sessao, uint idProd, uint idLoja, decimal qtdeBaixa, decimal? valor, DateTime dataMov, string observacao)
        {
            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Baixar(sessao, new MovimentacaoDto
                {
                    IdProduto = idProd,
                    IdLoja = idLoja,
                    LancamentoManual = true,
                    Quantidade = qtdeBaixa,
                    Total = valor.GetValueOrDefault(GetTotalEstoqueManual(sessao, (int)idProd, qtdeBaixa)),
                    Data = dataMov,
                    AlterarProdutoBase = true,
                    Observacao = observacao,
                });
        }

        public void BaixaEstoquePedidoProducao(GDASession sessao, IEnumerable<int> idsProdutoPedidoProducao)
        {
            foreach (var item in ProdutoPedidoProducaoDAO.Instance.ObterParaBaixaEstoqueProducao(sessao, idsProdutoPedidoProducao).Where(f => f.IdProdPed > 0))
            {
                var passouSetorLaminado = !ProdutoPedidoProducaoDAO.Instance.PecaPassouSetorLaminado(sessao, (int)item.IdProdPedProducao);
                var produtoPedidoEspelho = ProdutosPedidoEspelhoDAO.Instance.GetProdPedByEtiqueta(sessao, null, item.IdProdPed, true);
                var m2Calc = produtoPedidoEspelho.TotM2Calc / produtoPedidoEspelho.Qtde;

                var idLoja = ObterIdLojaProdutoPedidoProducao(
                    sessao,
                    (int)item.IdProdPedProducao,
                    (int)produtoPedidoEspelho.IdPedido,
                    (int)produtoPedidoEspelho.IdProd);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Baixar(sessao, new MovimentacaoDto
                    {
                        IdProduto = produtoPedidoEspelho.IdProd,
                        IdLoja = (uint)idLoja,
                        IdProdPedProducao = item.IdProdPedProducao,
                        Total = GetTotalProdPedProducao(sessao, (int)item.IdProdPedProducao),
                    });

                CreditaEstoqueProducao(sessao, (int)produtoPedidoEspelho.IdProd, idLoja, (int)item.IdProdPedProducao, (decimal)(m2Calc > 0 && passouSetorLaminado ? m2Calc : 1));

                // Marca que este produto entrou em estoque
                objPersistence.ExecuteCommand(sessao, $"UPDATE produto_pedido_producao SET EntrouEstoque = 0 WHERE IdProdPedProducao = {item.IdProdPedProducao}");
            }
        }

        public void BaixaEstoquePedidoProducaoPerda(GDASession sessao, int idProdutoPedidoProducao, Pedido.TipoPedidoEnum tipoPedido, uint idFuncionario)
        {
            if (tipoPedido != Pedido.TipoPedidoEnum.Producao
                || !ProdutoPedidoProducaoDAO.Instance.EntrouEmEstoque(sessao, idProdutoPedidoProducao))
            {
                return;
            }

            var produtoPedidoEspelho = ProdutosPedidoEspelhoDAO.Instance.GetElementByPrimaryKey(
                sessao,
                ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(sessao, (uint)idProdutoPedidoProducao));

            var idLoja = ObterIdLojaProdutoPedidoProducao(
                sessao,
                idProdutoPedidoProducao,
                (int)produtoPedidoEspelho.IdPedido,
                (int)produtoPedidoEspelho.IdProd);

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Baixar(sessao, new MovimentacaoDto
                {
                    IdProduto = produtoPedidoEspelho.IdProd,
                    IdLoja = (uint)idLoja,
                    IdProdPedProducao = (uint)idProdutoPedidoProducao,
                    Total = GetTotalProdPedProducao(sessao, idProdutoPedidoProducao),
                    BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                });

            // Credita a matéria-prima do box
            CreditaEstoqueProducao(sessao, (int)produtoPedidoEspelho.IdProd, idLoja, idProdutoPedidoProducao, (decimal)(produtoPedidoEspelho.TotM2Calc / produtoPedidoEspelho.Qtde));

            // Marca que este produto entrou em estoque
            objPersistence.ExecuteCommand(sessao, $"UPDATE produto_pedido_producao SET entrouEstoque=true WHERE idProdPedProducao={idProdutoPedidoProducao}");
        }

        public void BaixaEstoqueVoltarPecaProducao(GDASession sessao, int idProdutoPedidoProducao, int idPedido, Pedido.TipoPedidoEnum tipoPedido, Setor setorAtual, Setor setorNovo)
        {
            if (tipoPedido != Pedido.TipoPedidoEnum.Producao
                || !setorAtual.EntradaEstoque
                || setorNovo.EntradaEstoque)
            {
                return;
            }

            var idProdutoPedido = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(sessao, (uint)idProdutoPedidoProducao);
            var pecaPassouSetorLaminado = ProdutoPedidoProducaoDAO.Instance.PecaPassouSetorLaminado(sessao, idProdutoPedidoProducao);
            var produtoPedidoEspelho = ProdutosPedidoEspelhoDAO.Instance.GetProdPedByEtiqueta(sessao, null, idProdutoPedido, true);
            float m2Calc = produtoPedidoEspelho.TotM / produtoPedidoEspelho.Qtde;

            var idLoja = ObterIdLojaProdutoPedidoProducao(
                sessao,
                idProdutoPedidoProducao,
                idPedido,
                (int)produtoPedidoEspelho.IdProd);

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Baixar(sessao, new MovimentacaoDto
                {
                    IdProduto = produtoPedidoEspelho.IdProd,
                    IdLoja = (uint)idLoja,
                    IdProdPedProducao = (uint)idProdutoPedidoProducao,
                    Total = GetTotalProdPedProducao(sessao, idProdutoPedidoProducao),
                });

            CreditaEstoqueProducao(sessao, (int)produtoPedidoEspelho.IdProd, idLoja, idProdutoPedidoProducao, (decimal)(m2Calc > 0 && !pecaPassouSetorLaminado ? m2Calc : 1));

            // Marca que este produto entrou em estoque
            objPersistence.ExecuteCommand(sessao, $"UPDATE produto_pedido_producao SET entrouEstoque=false WHERE idProdPedProducao={idProdutoPedidoProducao}");
        }

        public void BaixaEstoqueEntregaExpedicao(GDASession sessao, int idProdutoPedidoProducao, int idPedido, int? idProdutoPedidoRevenda, ProdutosPedido produtoPedido, ProdutosPedidoEspelho produtoPedidoEspelho)
        {
            var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)produtoPedidoEspelho.IdGrupoProd, (int)produtoPedidoEspelho.IdSubgrupoProd, false);
            var quantidadeBaixa = CalcularQuantidadeEstoque(tipoCalculo, 1, produtoPedidoEspelho.Qtde, produtoPedidoEspelho.TotM, produtoPedidoEspelho.Altura);
            var idLoja = ObterIdLojaProdutoPedidoProducao(
                sessao,
                idProdutoPedidoProducao,
                idPedido,
                (int)produtoPedidoEspelho.IdProd);

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Baixar(sessao, new MovimentacaoDto
                {
                    IdProduto = produtoPedidoEspelho.IdProd,
                    IdLoja = (uint)idLoja,
                    IdProdPedProducao = (uint)idProdutoPedidoProducao,
                    Quantidade = quantidadeBaixa,
                    Total = GetTotalProdPedProducao(sessao, idProdutoPedidoProducao),
                    AlterarMateriaPrima = !SubgrupoProdDAO.Instance.IsSubgrupoProducao(sessao, (int)produtoPedido.IdGrupoProd, (int)produtoPedido.IdSubgrupoProd),
                    BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                });

            var codigoEtiqueta = ProdutoPedidoProducaoDAO.Instance.ObtemEtiqueta(sessao, (uint)idProdutoPedidoProducao);

            // Marca saída desta peça no ProdutosPedido do pedido de PRODUÇÃO
            ProdutosPedidoDAO.Instance.MarcarSaida(sessao, produtoPedido.IdProdPed, 1, 0, System.Reflection.MethodBase.GetCurrentMethod().Name, codigoEtiqueta);

            // Marca saída desta peça no ProdutosPedido do pedido de REVENDA desde que o pedido produção não seja para corte.
            if (idProdutoPedidoRevenda > 0 && !PedidoDAO.Instance.IsPedidoProducaoCorte(sessao, (uint)idPedido))
            {
                ProdutosPedidoDAO.Instance.MarcarSaida(sessao, (uint)idProdutoPedidoRevenda.Value, 1, 0, System.Reflection.MethodBase.GetCurrentMethod().Name, codigoEtiqueta);
            }
        }

        public void BaixaEstoquePecaRepostaPedidoProducao(GDASession sessao, int idProdutoPedidoProducao, ProdutosPedidoEspelho produtoPedidoEspelho)
        {
            if (!ProdutoPedidoProducaoDAO.Instance.EntrouEmEstoque(sessao, idProdutoPedidoProducao))
            {
                return;
            }

            var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)produtoPedidoEspelho.IdGrupoProd, (int)produtoPedidoEspelho.IdSubgrupoProd, false);
            var quantidadeBaixa = CalcularQuantidadeEstoque(tipoCalculo, 1, produtoPedidoEspelho.Qtde, produtoPedidoEspelho.TotM, produtoPedidoEspelho.Altura);

            var idLoja = ObterIdLojaProdutoPedidoProducao(
                sessao,
                idProdutoPedidoProducao,
                (int)produtoPedidoEspelho.IdPedido,
                (int)produtoPedidoEspelho.IdProd);

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Baixar(sessao, new MovimentacaoDto
                {
                    IdProduto = produtoPedidoEspelho.IdProd,
                    IdLoja = (uint)idLoja,
                    IdProdPedProducao = (uint)idProdutoPedidoProducao,
                    Quantidade = quantidadeBaixa,
                    Total = GetTotalProdPedProducao(sessao, idProdutoPedidoProducao),
                });

            // Só baixa apenas se a peça possuir produto para baixa associado
            CreditaEstoqueProducao(sessao, (int)produtoPedidoEspelho.IdProd, idLoja, idProdutoPedidoProducao, quantidadeBaixa);

            // Marca que este produto não entrou em estoque
            objPersistence.ExecuteCommand(sessao, $"UPDATE produto_pedido_producao SET entrouEstoque = FALSE WHERE idProdPedProducao = {idProdutoPedidoProducao}");
        }

        public void BaixaEstoquePerda(GDASession sessao, int idProdutoPedidoProducao, ProdutosPedidoEspelho produtoPedidoEspelho)
        {
            var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)produtoPedidoEspelho.IdGrupoProd, (int)produtoPedidoEspelho.IdSubgrupoProd, false);
            var quantidadeBaixa = CalcularQuantidadeEstoque(tipoCalculo, 1, produtoPedidoEspelho.Qtde, produtoPedidoEspelho.TotM, produtoPedidoEspelho.Altura);

            var idLoja = ObterIdLojaProdutoPedidoProducao(
                sessao,
                idProdutoPedidoProducao,
                (int)produtoPedidoEspelho.IdPedido,
                (int)produtoPedidoEspelho.IdProd);

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Baixar(sessao, new MovimentacaoDto
                {
                    IdProduto = produtoPedidoEspelho.IdProd,
                    IdLoja = (uint)idLoja,
                    IdProdPedProducao = (uint)idProdutoPedidoProducao,
                    Quantidade = quantidadeBaixa,
                    Total = GetTotalProdPedProducao(sessao, idProdutoPedidoProducao),
                    AlterarMateriaPrima = true,
                    BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                });
        }

        public void BaixaEstoqueVoltarPecaProducaoRetalho(GDASession sessao, int idProdutoPedidoProducao, IEnumerable<RetalhoProducao> retalhos)
        {
            var idPedido = ProdutoPedidoProducaoDAO.Instance.ObtemIdPedido(sessao, (uint)idProdutoPedidoProducao);

            foreach (var item in retalhos)
            {
                var idLoja = ObterIdLojaProdutoPedidoProducao(
                    sessao,
                    idProdutoPedidoProducao,
                    (int)idPedido,
                    item.IdProd);

                RetalhoProducaoDAO.Instance.AlteraSituacao(sessao, (uint)item.IdRetalhoProducao, SituacaoRetalhoProducao.Cancelado);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Baixar(sessao, new MovimentacaoDto
                    {
                        IdProduto = (uint)item.IdProd,
                        IdLoja = (uint)idLoja,
                        IdProdPedProducao = item.IdProdPedProducaoOrig.GetValueOrDefault(),
                        Total = GetTotalProdPedProducao(sessao, idProdutoPedidoProducao),
                    });
            }
        }

        public void BaixaEstoqueMateriaPrimaPedidoProducao(GDASession sessao, int idProd, int idPedido, int idProdutoPedidoProducao, decimal quantidadeBaixa, Setor setor)
        {
            if (!setor.EntradaEstoque
                || !PedidoDAO.Instance.IsProducao(sessao, (uint)idPedido)
                || ProdutoPedidoProducaoDAO.Instance.EntrouEmEstoque(sessao, idProdutoPedidoProducao))
            {
                return;
            }

            var idLoja = ObterIdLojaProdutoPedidoProducao(
                sessao,
                idProdutoPedidoProducao,
                idPedido,
                idProd);

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Baixar(sessao, new MovimentacaoDto
                {
                    IdProduto = (uint)idProd,
                    IdLoja = (uint)idLoja,
                    IdProdPedProducao = (uint)idProdutoPedidoProducao,
                    Quantidade = quantidadeBaixa,
                    Total = GetTotalProdPedProducao(sessao, (int)idProdutoPedidoProducao),
                    AlterarMateriaPrima = true,
                });
        }

        public void BaixaEstoqueProducao(GDASession sessao, int idProduto, int idPedido, int idProdutoPedidoProducao, decimal quantidadeBaixa)
        {
            var idLoja = ObterIdLojaProdutoPedidoProducao(
                sessao,
                idProdutoPedidoProducao,
                idPedido,
                idProduto);

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Baixar(sessao, new MovimentacaoDto
                {
                    IdProduto = (uint)idProduto,
                    IdLoja = (uint)idLoja,
                    IdProdPedProducao = (uint)idProdutoPedidoProducao,
                    Quantidade = quantidadeBaixa,
                    Total = GetTotalProdPedProducao(sessao, idProdutoPedidoProducao),
                    AlterarMateriaPrima = true,
                });
        }

        public void BaixaEstoqueChapaRetalho(GDASession sessao, uint idRetalhoProducao, bool chapaPossuiLeitura)
        {
            if (chapaPossuiLeitura)
            {
                return;
            }

            var idProduto = RetalhoProducaoDAO.Instance.ObtemValorCampo<uint>(sessao, "IdProd", $"IdRetalhoProducao={idRetalhoProducao}");

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Baixar(sessao, new MovimentacaoDto
                {
                    IdProduto = idProduto,
                    IdLoja = UserInfo.GetUserInfo.IdLoja,
                    IdRetalhoProducao = idRetalhoProducao,
                    AlterarMateriaPrima = !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)idProduto),
                    BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                });
        }

        public void BaixaEstoqueRetalho(GDASession sessao, RetalhoProducao retalho)
        {
            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Baixar(sessao, new MovimentacaoDto
                {
                    IdProduto = (uint)retalho.IdProd,
                    IdLoja = UserInfo.GetUserInfo.IdLoja,
                    IdRetalhoProducao = (uint)retalho.IdRetalhoProducao,
                });

            var produtoRetalho = ProdutoDAO.Instance.GetElementByPrimaryKey(sessao, retalho.IdProd);
            var quantidadeBaixaProdutoOriginal = Math.Round((decimal)(produtoRetalho.Altura * produtoRetalho.Largura) / 1000000, 2);

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Baixar(sessao, new MovimentacaoDto
                {
                    IdProduto = (uint)produtoRetalho.IdProdOrig,
                    IdLoja = UserInfo.GetUserInfo.IdLoja,
                    IdRetalhoProducao = (uint)retalho.IdRetalhoProducao,
                    Quantidade = quantidadeBaixaProdutoOriginal,
                });
        }

        public void BaixaEstoquePerdaChapa(GDASession sessao, PerdaChapaVidro perdaChapaVidro)
        {
            var idNotaFiscal = ProdutosNfDAO.Instance.ObtemIdNf(sessao, perdaChapaVidro.IdProdNf.Value);

            if (idNotaFiscal == 0)
            {
                throw new InvalidOperationException("Não foi possível recuperar a nota fiscal.");
            }

            var idLoja = NotaFiscalDAO.Instance.ObtemIdLoja(sessao, idNotaFiscal);

            if (idLoja == 0)
            {
                throw new InvalidOperationException("Não foi possível recuperar a loja da nota fiscal.");
            }

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Baixar(sessao, new MovimentacaoDto
                {
                    IdProduto = perdaChapaVidro.IdProd,
                    IdLoja = idLoja,
                    IdPerdaChapaVidro = perdaChapaVidro.IdPerdaChapaVidro,
                    Total = ProdutosNfDAO.Instance.ObterTotal(sessao, (int)perdaChapaVidro.IdProdNf.GetValueOrDefault()),
                    AlterarProdutoBase = true,
                });
        }

        public void BaixaEstoqueInventario(GDASession sessao, uint idLoja, IEnumerable<ProdutoInventarioEstoque> produtosInventarioEstoque)
        {
            foreach (var item in produtosInventarioEstoque)
            {
                var quantidadeBaixa = (decimal)(item.QtdeIni - item.QtdeFim.GetValueOrDefault());

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Baixar(sessao, new MovimentacaoDto
                    {
                        IdProduto = item.IdProd,
                        IdLoja = idLoja,
                        IdInventarioEstoque = item.IdInventarioEstoque,
                        LancamentoManual = true,
                        Quantidade = quantidadeBaixa,
                        Total = GetTotalEstoqueManual(sessao, (int)item.IdProd, quantidadeBaixa),
                        BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                    });
            }
        }

        #endregion

        #region Credita Estoque

        public void CreditaEstoqueCancelamentoSaidaEstoque(GDASession sessao, SaidaEstoque saidaEstoque)
        {
            if (!saidaEstoque.PodeCancelar)
            {
                throw new InvalidOperationException("Não é possível cancelar essa saída de estoque.");
            }

            SaidaEstoqueDAO.Instance.MarcaEstorno(sessao, saidaEstoque.IdPedido, saidaEstoque.IdLiberarPedido, saidaEstoque.IdVolume);

            var produtosSaidaEstoque = ProdutoSaidaEstoqueDAO.Instance.GetForRpt(sessao, saidaEstoque.IdSaidaEstoque);
            var idsProduto = new List<int>();
            var idsPedido = new List<int>();

            foreach (var item in produtosSaidaEstoque)
            {
                var produtoPedido = ProdutosPedidoDAO.Instance.GetElementFluxoLite(sessao, item.IdProdPed);
                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)produtoPedido.IdGrupoProd, (int?)produtoPedido.IdSubgrupoProd, false);
                var tipoPedido = PedidoDAO.Instance.GetTipoPedido(sessao, produtoPedido.IdPedido);
                var quantidadeEntrada = CalcularQuantidadeEstoque(tipoCalculo, item.QtdeSaida, produtoPedido.Qtde, produtoPedido.TotM, produtoPedido.Altura);

                var idProdutoLiberarPedido = saidaEstoque.IdLiberarPedido > 0
                    ? (int?)ProdutosLiberarPedidoDAO.Instance.ObtemIdProdLiberarPedido(sessao, saidaEstoque.IdLiberarPedido.Value, produtoPedido.IdProdPed)
                    : null;

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Creditar(sessao, new MovimentacaoDto
                    {
                        IdProduto = produtoPedido.IdProd,
                        IdLoja = saidaEstoque.IdLoja,
                        IdPedido = saidaEstoque.IdPedido,
                        IdLiberarPedido = saidaEstoque.IdLiberarPedido,
                        IdProdLiberarPedido = (uint?)idProdutoLiberarPedido,
                        IdProdPed = produtoPedido.IdProdPed,
                        Quantidade = quantidadeEntrada,
                        Total = GetTotalProdPed(sessao, (int)produtoPedido.IdProdPed),
                        AlterarMateriaPrima = VerificarAlterarMateriaPrima(sessao, (int)produtoPedido.IdGrupoProd, (int)produtoPedido.IdSubgrupoProd, tipoCalculo, tipoPedido, (int)produtoPedido.IdProd),
                        BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                        AlterarProdutoBase = true,
                    });

                ProdutosPedidoDAO.Instance.MarcarSaida(sessao, item.IdProdPed, -item.QtdeSaida, saidaEstoque.IdSaidaEstoque, System.Reflection.MethodBase.GetCurrentMethod().Name, string.Empty);

                if (!idsProduto.Contains((int)produtoPedido.IdProd))
                {
                    idsProduto.Add((int)produtoPedido.IdProd);
                }

                if (!idsPedido.Contains((int)produtoPedido.IdPedido))
                {
                    idsPedido.Add((int)produtoPedido.IdPedido);
                }
            }

            foreach (var id in idsPedido)
            {
                PedidoDAO.Instance.AtualizaSituacaoProducao(sessao, (uint)id, null, DateTime.Now);
            }

            ProdutoLojaDAO.Instance.RecalcularReserva(sessao, (int)saidaEstoque.IdLoja, idsProduto);
            ProdutoLojaDAO.Instance.RecalcularLiberacao(sessao, (int)saidaEstoque.IdLoja, idsProduto);

        }

        public void CreditaEstoqueCancelamentoPedido(GDASession sessao, uint idLoja, uint idPedido, IEnumerable<ProdutosPedido> produtosPedido)
        {
            var tipoPedido = PedidoDAO.Instance.GetTipoPedido(sessao, idPedido);

            foreach (var item in produtosPedido.Where(f => f.QtdSaida > 0))
            {
                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)item.IdGrupoProd, (int)item.IdSubgrupoProd, false);
                var quantidadeEntrada = CalcularQuantidadeEstoque(tipoCalculo, item.QtdSaida, item.Qtde, item.TotM, item.Altura);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Creditar(sessao, new MovimentacaoDto
                    {
                        IdProduto = item.IdProd,
                        IdLoja = idLoja,
                        IdPedido = idPedido,
                        IdProdPed = item.IdProdPed,
                        Quantidade = quantidadeEntrada,
                        Total = GetTotalProdPed(sessao, (int)item.IdProdPed),
                        AlterarMateriaPrima = VerificarAlterarMateriaPrima(sessao, (int)item.IdGrupoProd, (int)item.IdSubgrupoProd, tipoCalculo, tipoPedido, (int)item.IdProd),
                        BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                        AlterarProdutoBase = true,
                    });
            }

            if (tipoPedido != Pedido.TipoPedidoEnum.Producao)
            {
                var idsProduto = produtosPedido.Select(f => (int)f.IdProd).Distinct();
                ProdutoLojaDAO.Instance.RecalcularLiberacao(sessao, (int)idLoja, idsProduto);
                ProdutoLojaDAO.Instance.RecalcularReserva(sessao, (int)idLoja, idsProduto);
            }
        }

        public void CreditaEstoqueEstornoVolume(GDASession sessao, int idPedido, int idVolume, IEnumerable<VolumeProdutosPedido> volumes)
        {
            var tipoPedido = PedidoDAO.Instance.GetTipoPedido(sessao, (uint)idPedido);
            var idLoja = PedidoDAO.Instance.ObtemIdLoja(sessao, (uint)idPedido);

            SaidaEstoqueDAO.Instance.MarcaEstorno(sessao, (uint)idPedido, null, (uint?)idVolume);

            foreach (var item in volumes)
            {
                var produtoPedido = ProdutosPedidoDAO.Instance.GetElementFluxoLite(sessao, item.IdProdPed);

                if (item.Qtde > produtoPedido.QtdSaida)
                {
                    throw new InvalidOperationException($"Operação cancelada. O produto {produtoPedido.DescrProduto} está tendo um estono maior do que a quantidade que já joi dado saída.");
                }

                ProdutosPedidoDAO.Instance.EstornoSaida(sessao, item.IdProdPed, item.Qtde, System.Reflection.MethodBase.GetCurrentMethod().Name, string.Empty);

                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)produtoPedido.IdGrupoProd, (int)produtoPedido.IdSubgrupoProd, false);
                var quantidadeEntrada = CalcularQuantidadeEstoque(tipoCalculo, item.Qtde, item.Qtde, produtoPedido.TotM, produtoPedido.Altura);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Creditar(sessao, new MovimentacaoDto
                    {
                        IdProduto = item.IdProd,
                        IdLoja = idLoja,
                        IdPedido = (uint)idPedido,
                        IdProdPed = item.IdProdPed,
                        IdVolume = (uint)idVolume,
                        Quantidade = quantidadeEntrada,
                        Total = GetTotalProdPed(sessao, (int)item.IdProdPed),
                        AlterarMateriaPrima = tipoPedido != Pedido.TipoPedidoEnum.Producao || !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)item.IdProd),
                        BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                        AlterarProdutoBase = true,
                    });
            }

            PedidoDAO.Instance.AtualizaSituacaoProducao(sessao, (uint)idPedido, null, DateTime.Now);

            if (tipoPedido != Pedido.TipoPedidoEnum.Producao)
            {
                var idsProduto = volumes.Select(f => (int)f.IdProd).Distinct();
                ProdutoLojaDAO.Instance.RecalcularReserva(sessao, (int)idLoja, idsProduto);
                ProdutoLojaDAO.Instance.RecalcularLiberacao(sessao, (int)idLoja, idsProduto);
            }
        }

        public void CreditaEstoqueEstornoCarregamentoExpedicaoChapa(GDASession sessao, int idPedido, int? idProdImpressaoChapa, IEnumerable<ProdutosPedido> produtosPedido)
        {
            var idLoja = PedidoDAO.Instance.ObtemIdLoja(sessao, (uint)idPedido);
            var idNotaFiscal = ProdutoImpressaoDAO.Instance.ObtemIdNf(sessao, (uint)idProdImpressaoChapa);
            if (idNotaFiscal > 0)
            {
                idLoja = NotaFiscalDAO.Instance.ObtemIdLoja(sessao, (uint)idNotaFiscal);
            }

            var tipoPedido = PedidoDAO.Instance.GetTipoPedido(sessao, (uint)idPedido);

            SaidaEstoqueDAO.Instance.MarcaEstorno(sessao, (uint)idPedido, null, null);

            foreach (var item in produtosPedido.Where(f => f.QtdMarcadaSaida > 0))
            {
                if (item.QtdMarcadaSaida > item.QtdSaida)
                {
                    throw new InvalidOperationException($"Operação cancelada. O produto {item.DescrProduto} está tendo um estono maior do que a quantidade que já joi dado saída.");
                }

                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)item.IdGrupoProd, (int)item.IdSubgrupoProd, false);
                var quantidadeItem = ProdutosPedidoDAO.Instance.ObtemQtde(sessao, item.IdProdPed);
                var metroQuadradoItem = ProdutosPedidoDAO.Instance.ObtemTotM(sessao, item.IdProdPed);
                var quantidadeEntrada = CalcularQuantidadeEstoque(tipoCalculo, item.QtdMarcadaSaida, quantidadeItem, metroQuadradoItem, item.Altura);

                ProdutosPedidoDAO.Instance.EstornoSaida(sessao, item.IdProdPed, item.QtdMarcadaSaida, System.Reflection.MethodBase.GetCurrentMethod().Name, string.Empty);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Creditar(sessao, new MovimentacaoDto
                    {
                        IdProduto = item.IdProd,
                        IdLoja = idLoja,
                        IdPedido = (uint)idPedido,
                        IdProdPed = item.IdProdPed,
                        IdProdImpressaoChapa = (uint)idProdImpressaoChapa,
                        Quantidade = quantidadeEntrada,
                        Total = GetTotalProdPed(sessao, (int)item.IdProdPed),
                        AlterarMateriaPrima = tipoPedido != Pedido.TipoPedidoEnum.Producao || !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)item.IdProd),
                        BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                        AlterarProdutoBase = true,
                    });
            }

            PedidoDAO.Instance.AtualizaSituacaoProducao(sessao, (uint)idPedido, null, DateTime.Now);

            if (tipoPedido != Pedido.TipoPedidoEnum.Producao)
            {
                var idsProduto = produtosPedido.Select(f => (int)f.IdProd).Distinct();
                ProdutoLojaDAO.Instance.RecalcularReserva(sessao, (int)idLoja, idsProduto);
                ProdutoLojaDAO.Instance.RecalcularLiberacao(sessao, (int)idLoja, idsProduto);
            }
        }

        public void CreditaEstoqueCompra(GDASession sessao, Compra compra)
        {
            if (EstoqueConfig.EntradaEstoqueManual || compra.EstoqueBaixado)
            {
                return;
            }

            var produtosCompra = ProdutosCompraDAO.Instance.GetByCompra(sessao, compra.IdCompra)
                .Where(f => f.QtdeEntrada > 0);

            foreach (var item in produtosCompra)
            {
                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)item.IdGrupoProd, (int)item.IdSubgrupoProd, false);
                var quantidadeEntrada = CalcularQuantidadeEstoque(tipoCalculo, item.Qtde - item.QtdeEntrada, item.Qtde, item.TotM, item.Altura);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Creditar(sessao, new MovimentacaoDto
                    {
                        IdProduto = item.IdProd,
                        IdLoja = compra.IdLoja,
                        IdCompra = item.IdCompra,
                        IdProdCompra = item.IdProdCompra,
                        Quantidade = quantidadeEntrada,
                        Total = GetTotalProdCompra(sessao, (int)item.IdProdCompra),
                        AlterarMateriaPrima = !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)item.IdProd),
                        BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                        AlterarProdutoBase = true,
                    });

                objPersistence.ExecuteCommand(sessao, $"UPDATE produtos_compra SET qtdeEntrada={item.Qtde.ToString().Replace(",", ".")} WHERE idProdCompra={item.IdProdCompra}");
            }

            objPersistence.ExecuteCommand(sessao, "UPDATE compra SET EstoqueBaixado=true WHERE idCompra=" + compra.IdCompra);
        }

        public void CreditaEstoqueManualCompra(GDASession sessao, uint idLoja, uint idCompra, IEnumerable<ProdutosCompra> produtosCompra)
        {
            var idEntradaEstoque = EntradaEstoqueDAO.Instance.GetNewEntradaEstoque(sessao, idLoja, idCompra, null, true, (int)UserInfo.GetUserInfo.CodUser);

            foreach (var item in produtosCompra.Where(f => f.QtdMarcadaEntrada > 0))
            {
                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)item.IdGrupoProd, (int)item.IdSubgrupoProd, false);
                var quantidadeEntrada = CalcularQuantidadeEstoque(tipoCalculo, item.QtdMarcadaEntrada, item.Qtde, item.TotM, item.Altura);

                ProdutosCompraDAO.Instance.MarcarEntrada(sessao, item.IdProdCompra, item.QtdMarcadaEntrada, idEntradaEstoque);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Creditar(sessao, new MovimentacaoDto
                    {
                        IdProduto = item.IdProd,
                        IdLoja = idLoja,
                        IdCompra = idCompra,
                        IdProdCompra = item.IdProdCompra,
                        Quantidade = quantidadeEntrada,
                        Total = GetTotalProdCompra(sessao, (int)item.IdProdCompra),
                        AlterarMateriaPrima = !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)item.IdProd),
                        BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                        AlterarProdutoBase = true,
                    });
            }

            CompraDAO.Instance.MarcaEstoqueBaixado(sessao, idCompra);
        }

        public void CreditaEstoqueLiberacao(GDASession sessao, uint idLiberarPedido, IEnumerable<ProdutosLiberarPedido> produtosLiberarPedido)
        {
            var saidaEstoque = SaidaEstoqueDAO.Instance.GetByLiberacao(sessao, idLiberarPedido);
            var produtosSaidaEstoque = saidaEstoque != null ? ProdutoSaidaEstoqueDAO.Instance.GetForRpt(sessao, saidaEstoque.IdSaidaEstoque).ToArray() : null;
            var idsProdutoReservaLiberacao = new Dictionary<int, List<int>>();

            foreach (var item in produtosLiberarPedido)
            {
                var idLoja = PedidoDAO.Instance.ObtemIdLoja(sessao, item.IdPedido);

                var subGrupoVolume = SubgrupoProdDAO.Instance.IsSubgrupoGeraVolume(sessao, item.IdGrupoProd, item.IdSubgrupoProd.GetValueOrDefault());
                var entregaBalcao = PedidoDAO.Instance.ObtemTipoEntrega(sessao, item.IdPedido) == (uint)Pedido.TipoEntregaPedido.Balcao;
                var naoVolume = OrdemCargaConfig.GerarVolumeApenasDePedidosEntrega ? entregaBalcao || !subGrupoVolume : !subGrupoVolume;
                var produtoVidro = GrupoProdDAO.Instance.IsVidro((int)item.IdGrupoProd);
                var saidaNaoVidro = Liberacao.Estoque.SaidaEstoqueAoLiberarPedido && (!produtoVidro || !PCPConfig.ControlarProducao);
                var saidaBox = Liberacao.Estoque.SaidaEstoqueBoxLiberar && produtoVidro && SubgrupoProdDAO.Instance.IsSubgrupoProducao(sessao, (int)item.IdGrupoProd, (int?)item.IdSubgrupoProd);
                var pedidoGerarProducaoParaCorte = PedidoDAO.Instance.GerarPedidoProducaoCorte(sessao, item.IdPedido);
                var pedidoPossuiVolumeExpedido = false;

                foreach (var volume in VolumeDAO.Instance.ObterPeloPedido(sessao, (int)item.IdPedido))
                {
                    if (VolumeDAO.Instance.TemExpedicao(sessao, volume.IdVolume))
                    {
                        pedidoPossuiVolumeExpedido = true;
                        break;
                    }
                }

                if (!idsProdutoReservaLiberacao.ContainsKey((int)idLoja))
                {
                    idsProdutoReservaLiberacao.Add((int)idLoja, new List<int> { (int)item.IdProd });
                }
                else if (!idsProdutoReservaLiberacao[(int)idLoja].Contains((int)item.IdProd))
                {
                    idsProdutoReservaLiberacao[(int)idLoja].Add((int)item.IdProd);
                }

                if (pedidoPossuiVolumeExpedido || pedidoGerarProducaoParaCorte || !naoVolume || (!saidaNaoVidro && !saidaBox))
                {
                    continue;
                }

                var produtoSaidaEstoque = produtosSaidaEstoque.Any()
                    ? produtosSaidaEstoque.FirstOrDefault(f => f.IdProdPed == item.IdProdPed)
                    : null;

                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)item.IdProd, false);
                var quantidadeEntrada = CalcularQuantidadeEstoque(tipoCalculo, produtoSaidaEstoque?.QtdeSaida ?? item.Qtde, item.Qtde, (float)item.TotM2, item.Altura);

                ProdutosPedidoDAO.Instance.MarcarSaida(sessao, item.IdProdPed, -(float)quantidadeEntrada, 0, System.Reflection.MethodBase.GetCurrentMethod().Name, string.Empty);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Creditar(sessao, new MovimentacaoDto
                    {
                        IdProduto = item.IdProd,
                        IdLoja = idLoja,
                        IdPedido = item.IdPedido,
                        IdLiberarPedido = idLiberarPedido,
                        IdProdLiberarPedido = item.IdProdLiberarPedido,
                        Quantidade = quantidadeEntrada,
                        Total = GetTotalProdLiberarPedido(sessao, (int)item.IdProdLiberarPedido),
                        AlterarMateriaPrima = !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)item.IdProd),
                        BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                        AlterarProdutoBase = true,
                    });
            }

            foreach (var idLoja in idsProdutoReservaLiberacao.Keys)
            {
                if (idsProdutoReservaLiberacao[idLoja].Count > 0)
                {
                    ProdutoLojaDAO.Instance.RecalcularReserva(sessao, idLoja, idsProdutoReservaLiberacao[idLoja]);
                    ProdutoLojaDAO.Instance.RecalcularLiberacao(sessao, idLoja, idsProdutoReservaLiberacao[idLoja]);
                }
            }
        }

        public void CreditaEstoqueProdutoDevolvidoTrocaDevolucao(GDASession sessao, int idTrocaDevolucao, IEnumerable<ProdutoTrocaDevolucao> produtosTrocaDevolucao)
        {
            foreach (var item in produtosTrocaDevolucao.Where(f => f.AlterarEstoque))
            {
                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)item.IdGrupoProd, (int)item.IdSubgrupoProd, false);
                var quantidadeEntrada = CalcularQuantidadeEstoque(tipoCalculo, item.Qtde, item.Qtde, item.TotM, item.Altura);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Creditar(sessao, new MovimentacaoDto
                    {
                        IdProduto = item.IdProd,
                        IdLoja = (uint)ObterIdLojaTrocaDevolucao(sessao, idTrocaDevolucao, (int)item.IdProd),
                        IdTrocaDevolucao = item.IdTrocaDevolucao,
                        IdProdTrocaDev = item.IdProdTrocaDev,
                        Quantidade = quantidadeEntrada,
                        Total = GetTotalProdTrocaDevolucao(sessao, (int?)item.IdProdTrocaDev, null),
                        AlterarMateriaPrima = !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)item.IdProd),
                        BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                        AlterarProdutoBase = true,
                    });
            }
        }

        public void CreditaEstoqueProdutoTrocadoTrocaDevolucao(GDASession sessao, int idTrocaDevolucao, IEnumerable<ProdutoTrocado> produtosTrocados)
        {
            foreach (var item in produtosTrocados)
            {
                var idLoja = (uint)ObterIdLojaTrocaDevolucao(sessao, idTrocaDevolucao, (int)item.IdProd);
                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)item.IdGrupoProd, (int)item.IdSubgrupoProd, false);
                var quantidadeEntrada = CalcularQuantidadeEstoque(tipoCalculo, item.Qtde, item.Qtde, item.TotM, item.Altura);

                if (item.ComDefeito)
                {
                    ProdutoLojaDAO.Instance.CreditaDefeito(sessao, item.IdProd, idLoja, (float)quantidadeEntrada);
                }
                else if (item.AlterarEstoque)
                {
                    new EstoqueStrategyFactory()
                        .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                        .Creditar(sessao, new MovimentacaoDto
                        {
                            IdProduto = item.IdProd,
                            IdLoja = (uint)ObterIdLojaTrocaDevolucao(sessao, idTrocaDevolucao, (int)item.IdProd),
                            IdTrocaDevolucao = item.IdTrocaDevolucao,
                            IdProdTrocado = item.IdProdTrocado,
                            Quantidade = quantidadeEntrada,
                            Total = GetTotalProdTrocaDevolucao(sessao, null, (int?)item.IdProdTrocado),
                            AlterarMateriaPrima = !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)item.IdProd),
                            BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                            AlterarProdutoBase = true,
                        });
                }
            }
        }

        public void CreditaEstoqueNotaFiscalManual(GDASession sessao, int idNotaFiscal, int idLoja, IEnumerable<ProdutosNf> produtosNotaFiscal)
        {
            var dataMov = DateTime.Now;
            var tipoDoc = (NotaFiscal.TipoDoc)NotaFiscalDAO.Instance.GetTipoDocumento(sessao, (uint)idNotaFiscal);

            if (tipoDoc != NotaFiscal.TipoDoc.Saída)
            {
                dataMov = NotaFiscalDAO.Instance.ObtemDataEntradaSaida(sessao, (uint)idNotaFiscal).GetValueOrDefault(dataMov);
            }

            var numeroNFe = NotaFiscalDAO.Instance.ObtemNumerosNFePeloIdNf(sessao, idNotaFiscal.ToString()).StrParaUint();
            var idEntradaEstoque = EntradaEstoqueDAO.Instance.GetNewEntradaEstoque(sessao, (uint)idLoja, null, numeroNFe, true,
                (int)UserInfo.GetUserInfo.CodUser);

            foreach (var item in produtosNotaFiscal.Where(f => f.QtdMarcadaEntrada > 0))
            {
                var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)item.IdGrupoProd, (int)item.IdSubgrupoProd, false);
                var quantidadeEntrada = CalcularQuantidadeEstoque(tipoCalculo, item.QtdMarcadaEntrada, item.Qtde, item.TotM, item.Altura);

                ProdutosNfDAO.Instance.MarcarEntrada(sessao, item.IdProdNf, item.QtdMarcadaEntrada, idEntradaEstoque);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Creditar(sessao, new MovimentacaoDto
                    {
                        IdProduto = item.IdProd,
                        IdLoja = (uint)idLoja,
                        IdNf = item.IdNf,
                        IdProdNf = item.IdProdNf,
                        Quantidade = quantidadeEntrada,
                        Total = ProdutosNfDAO.Instance.ObterTotal(sessao, (int)item.IdProdNf),
                        AlterarMateriaPrima = !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)item.IdProd),
                        BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                        Data = dataMov,
                        AlterarProdutoBase = true,
                    });
            }
        }

        public void CreditaEstoqueNotaFiscal(GDASession sessao, NotaFiscal notaFiscal, IEnumerable<ProdutosNf> produtosNotaFiscal)
        {
            if (!notaFiscal.GerarEstoqueReal)
            {
                return;
            }

            var dataMov = DateTime.Now;

            if (notaFiscal.TipoDocumento != (int)NotaFiscal.TipoDoc.Saída)
            {
                dataMov = notaFiscal.DataSaidaEnt.GetValueOrDefault(dataMov);
            }

            foreach (var item in produtosNotaFiscal)
            {
                var quantidadeEntrada = (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(sessao, item.IdProd, item.TotM, item.Qtde, item.Altura, item.Largura, false, false);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Creditar(sessao, new MovimentacaoDto
                    {
                        IdProduto = item.IdProd,
                        IdLoja = notaFiscal.IdLoja.Value,
                        IdNf = item.IdNf,
                        IdProdNf = item.IdProdNf,
                        Quantidade = quantidadeEntrada,
                        Total = ProdutosNfDAO.Instance.ObterTotal(sessao, (int)item.IdProdNf),
                        AlterarMateriaPrima = !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)item.IdProd),
                        BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                        Data = dataMov,
                        AlterarProdutoBase = true,
                    });
            }
        }

        public void CreditaEstoqueManual(GDASession sessao, uint idProd, uint idLoja, decimal qtdeEntrada, decimal? valor, DateTime dataMov, string observacao)
        {
            var totalEstoqueManual = GetTotalEstoqueManual(sessao, (int)idProd, qtdeEntrada);

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Creditar(sessao, new MovimentacaoDto
                {
                    IdProduto = idProd,
                    IdLoja = idLoja,
                    LancamentoManual = true,
                    Quantidade = qtdeEntrada,
                    Total = valor.GetValueOrDefault(totalEstoqueManual),
                    Data = dataMov,
                    AlterarProdutoBase = true,
                    Observacao = observacao,
                });
        }

        public void CreditaEstoquePedidoProducao(GDASession sessao, int idPedido, int idProdutoPedidoProducao, ProdutosPedidoEspelho produtoPedidoEspelho, Setor setor)
        {
            if (!setor.EntradaEstoque
                || !PedidoDAO.Instance.IsProducao(sessao, (uint)idPedido)
                || ProdutoPedidoProducaoDAO.Instance.EntrouEmEstoque(sessao, idProdutoPedidoProducao))
            {
                return;
            }

            var idLoja = ObterIdLojaProdutoPedidoProducao(
                sessao,
                idProdutoPedidoProducao,
                idPedido,
                (int)produtoPedidoEspelho.IdProd);

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Creditar(sessao, new MovimentacaoDto
                {
                    IdProduto = produtoPedidoEspelho.IdProd,
                    IdLoja = (uint)idLoja,
                    IdProdPedProducao = (uint)idProdutoPedidoProducao,
                    Total = GetTotalProdPedProducao(sessao, idProdutoPedidoProducao),
                });

            var quantidadeBaixaMateriaPrima = produtoPedidoEspelho.TotM / produtoPedidoEspelho.Qtde;
            BaixaEstoqueMateriaPrimaPedidoProducao(
                sessao,
                (int)produtoPedidoEspelho.IdProd,
                idPedido,
                idProdutoPedidoProducao,
                (decimal)(quantidadeBaixaMateriaPrima > 0 && !ProdutoPedidoProducaoDAO.Instance.PecaPassouSetorLaminado(sessao, idProdutoPedidoProducao) ? quantidadeBaixaMateriaPrima : 1),
                setor);

            objPersistence.ExecuteCommand(sessao, $"UPDATE produto_pedido_producao SET entrouEstoque=true WHERE idProdPedProducao={idProdutoPedidoProducao}");
        }

        public void CreditaEstoqueVoltarPecaProducao(GDASession sessao, int idProduto, int idProdutoPedidoProducao, ProdutosPedidoEspelho produtoPedidoEspelho)
        {
            var idLoja = ObterIdLojaProdutoPedidoProducao(
                sessao,
                idProdutoPedidoProducao,
                (int)produtoPedidoEspelho.IdPedido,
                (int)produtoPedidoEspelho.IdProd);

            var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)produtoPedidoEspelho.IdGrupoProd, (int)produtoPedidoEspelho.IdSubgrupoProd, false);
            var quantidadeEntrada = CalcularQuantidadeEstoque(tipoCalculo, 1, produtoPedidoEspelho.Qtde, produtoPedidoEspelho.TotM, produtoPedidoEspelho.Altura);

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Creditar(sessao, new MovimentacaoDto
                {
                    IdProduto = (uint)idProduto,
                    IdLoja = (uint)idLoja,
                    IdProdPedProducao = (uint)idProdutoPedidoProducao,
                    Quantidade = quantidadeEntrada,
                    Total = GetTotalProdPedProducao(sessao, idProdutoPedidoProducao),
                    AlterarMateriaPrima = !SubgrupoProdDAO.Instance.IsSubgrupoProducao(sessao, (int)produtoPedidoEspelho.IdGrupoProd, (int)produtoPedidoEspelho.IdSubgrupoProd),
                    BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                });

            var idProdutoPedido = ProdutosPedidoDAO.Instance.ObterIdProdPed(sessao, (int)produtoPedidoEspelho.IdProdPed);
            var codigoEtiqueta = ProdutoPedidoProducaoDAO.Instance.ObtemEtiqueta(sessao, (uint)idProdutoPedidoProducao);
            if (idProdutoPedido.GetValueOrDefault() == 0)
            {
                throw new InvalidOperationException($"Não foi possível recuperar o produto do pedido. Etiqueta: {codigoEtiqueta}.");
            }

            ProdutosPedidoDAO.Instance.EstornoSaida(sessao, (uint)idProdutoPedido, 1, System.Reflection.MethodBase.GetCurrentMethod().Name, codigoEtiqueta);
        }

        public void CreditaEstoqueChapaVoltarPecaProducao(GDASession sessao, int idPedido, int idProdutoPedidoProducao, int idProdutoImpressaoChapa)
        {
            var idProduto = ProdutoImpressaoDAO.Instance.GetIdProd(sessao, (uint)idProdutoImpressaoChapa);

            if (idProduto.GetValueOrDefault() == 0)
            {
                return;
            }

            var idLoja = Geral.ConsiderarLojaClientePedidoFluxoSistema && idPedido > 0
                ? PedidoDAO.Instance.ObtemIdLoja(sessao, (uint)idPedido)
                : UserInfo.GetUserInfo.IdLoja;

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Creditar(sessao, new MovimentacaoDto
                {
                    IdProduto = idProduto.GetValueOrDefault(),
                    IdLoja = (uint)ObterIdLojaPeloIdProdImpressaoChapa(sessao, idPedido, (int)idProduto, idProdutoImpressaoChapa),
                    IdProdPedProducao = (uint)idProdutoPedidoProducao,
                    Total = GetTotalProdPedProducao(sessao, idProdutoPedidoProducao),
                });
        }

        public void CreditaEstoqueVoltarPecaPedidoProducao(GDASession sessao, int idPedido, int idProdutoPedidoProducao)
        {
            if (PedidoDAO.Instance.GetTipoPedido(sessao, (uint)idPedido) != Pedido.TipoPedidoEnum.Producao
                || !ProdutoPedidoProducaoDAO.Instance.EntrouEmEstoque(sessao, idProdutoPedidoProducao))
            {
                return;
            }

            var produtoPedidoEspelho = ProdutosPedidoEspelhoDAO.Instance.GetProdPedByEtiqueta(
                sessao,
                null,
                ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(sessao, (uint)idProdutoPedidoProducao),
                true);

            var idLoja = ObterIdLojaProdutoPedidoProducao(
                sessao,
                idProdutoPedidoProducao,
                idPedido,
                (int)produtoPedidoEspelho.IdProd);

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Creditar(sessao, new MovimentacaoDto
                {
                    IdProduto = produtoPedidoEspelho.IdProd,
                    IdLoja = (uint)idLoja,
                    IdProdPedProducao = (uint)idProdutoPedidoProducao,
                    Total = GetTotalProdPedProducao(sessao, idProdutoPedidoProducao),
                });

            var tipoCalculo = (TipoCalculoGrupoProd)GrupoProdDAO.Instance.TipoCalculo(sessao, (int)produtoPedidoEspelho.IdGrupoProd, (int)produtoPedidoEspelho.IdSubgrupoProd, false);
            var quantidadeBaixa = CalcularQuantidadeEstoque(tipoCalculo, 1, produtoPedidoEspelho.Qtde, produtoPedidoEspelho.TotM, produtoPedidoEspelho.Altura);
            BaixaEstoqueProducao(sessao, (int)produtoPedidoEspelho.IdProd, idPedido, idProdutoPedidoProducao, quantidadeBaixa);

            objPersistence.ExecuteCommand(sessao, $"UPDATE produto_pedido_producao SET entrouEstoque=true WHERE idProdPedProducao={idProdutoPedidoProducao}");
        }

        public void CreditaEstoqueProducao(GDASession sessao, int idProduto, int idLoja, int idProdutoPedidoProducao, decimal quantidadeEntrada)
        {
            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Creditar(sessao, new MovimentacaoDto
                {
                    IdProduto = (uint)idProduto,
                    IdLoja = (uint)idLoja,
                    IdProdPedProducao = (uint)idProdutoPedidoProducao,
                    Quantidade = quantidadeEntrada,
                    Total = GetTotalProdPedProducao(sessao, idProdutoPedidoProducao),
                    AlterarMateriaPrima = true,
                    BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                });
        }

        public void CreditaEstoqueRetalho(GDASession sessao, int idProd, RetalhoProducao retalho, LoginUsuario usuario)
        {
            var idLoja = usuario != null
                ? usuario.IdLoja
                : UserInfo.GetUserInfo.IdLoja;

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Creditar(sessao, new MovimentacaoDto
                {
                    IdProduto = (uint)idProd,
                    IdLoja = idLoja,
                    IdRetalhoProducao = (uint)retalho.IdRetalhoProducao,
                    AlterarMateriaPrima = !ProdutoDAO.Instance.IsProdutoProducao(sessao, idProd),
                    BaixarProprioProdutoSeNaoTiverMateriaPrima = true,
                    Usuario = usuario,
                });

            var produtoRetalho = ProdutoDAO.Instance.GetElementByPrimaryKey(sessao, retalho.IdProd);
            var quantidadeEntradaProdutoOriginal = Math.Round((decimal)(produtoRetalho.Altura * produtoRetalho.Largura) / 1000000, 2);

            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Creditar(sessao, new MovimentacaoDto
                {
                    IdProduto = (uint)produtoRetalho.IdProdOrig,
                    IdLoja = UserInfo.GetUserInfo.IdLoja,
                    IdRetalhoProducao = (uint)retalho.IdRetalhoProducao,
                    Quantidade = quantidadeEntradaProdutoOriginal,
                });
        }

        public void CreditaEstoquePerdaChapa(GDASession sessao, uint idProd, uint idProdNf, uint idLoja, uint idPerdaChapaVidro)
        {
            new EstoqueStrategyFactory()
                .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                .Creditar(sessao, new MovimentacaoDto
                {
                    IdProduto = idProd,
                    IdLoja = idLoja,
                    IdPerdaChapaVidro = idPerdaChapaVidro,
                    Total = ProdutosNfDAO.Instance.ObterTotal(sessao, (int)idProdNf),
                    AlterarProdutoBase = true,
                });
        }

        public void CreditaEstoqueInventario(GDASession sessao, uint idLoja, IEnumerable<ProdutoInventarioEstoque> produtosInventarioEstoque)
        {
            foreach (var item in produtosInventarioEstoque)
            {
                var quantidadeEntrada = (decimal)(item.QtdeFim.GetValueOrDefault() - item.QtdeIni);

                new EstoqueStrategyFactory()
                    .RecuperaEstrategia(Helper.Estoque.Estrategia.Cenario.Generica)
                    .Creditar(sessao, new MovimentacaoDto
                    {
                        IdProduto = item.IdProd,
                        IdLoja = idLoja,
                        IdInventarioEstoque = item.IdInventarioEstoque,
                        LancamentoManual = true,
                        Quantidade = quantidadeEntrada,
                        Total = this.GetTotalEstoqueManual(sessao, (int)item.IdProd, quantidadeEntrada),
                    });
            }
        }

        #endregion

        #region Validar estoque

        public void ValidarMovimentarEstoque(GDASession session, int idProd, int idLoja, DateTime dataMovimentacao, MovEstoque.TipoMovEnum tipoMovimentacao, decimal quantidade,
            ref decimal saldoQtdeAnterior, ref decimal saldoValorAnterior, ref decimal saldoQtdeValidar, bool recuperarProdutoBaixaEstoque)
        {
            ProdutoBaixaEstoque[] produtosBaixaEstoque = null;

            if (recuperarProdutoBaixaEstoque)
            {
                produtosBaixaEstoque = ProdutoBaixaEstoqueDAO.Instance.GetByProd(session, (uint)idProd, true);
            }
            else
            {
                produtosBaixaEstoque = new ProdutoBaixaEstoque[]
                {
                    new ProdutoBaixaEstoque
                    {
                        IdProd = idProd,
                        IdProdBaixa = idProd,
                        Qtde = (float)quantidade,
                    },
                };
            }

            foreach (var produtoBaixaEstoque in produtosBaixaEstoque)
            {
                // No procedimento de movimentação de estoque este método será chamado com os dados da baixa.
                // Já na tela de nota fiscal a baixa do produto será recuperada e a quantidade da movimentação deve ser calculada.
                if (recuperarProdutoBaixaEstoque)
                {
                    quantidade *= (decimal)produtoBaixaEstoque.Qtde;
                }

                // Recupera os dados da movimentação anterior.
                saldoQtdeAnterior = ObtemSaldoQtdeMov(session, null, (uint)produtoBaixaEstoque.IdProdBaixa, (uint)idLoja, dataMovimentacao, true);
                saldoValorAnterior = ObtemSaldoValorMov(session, null, (uint)produtoBaixaEstoque.IdProdBaixa, (uint)idLoja, dataMovimentacao, true);
                saldoQtdeValidar = ObtemSaldoQtdeMov(session, null, (uint)produtoBaixaEstoque.IdProdBaixa, (uint)idLoja, dataMovimentacao, false);

                // Verifica se, ao registrar a movimentação, o saldo em estoque do produto ficará negativo.
                if (tipoMovimentacao == MovEstoque.TipoMovEnum.Saida
                    && ((saldoQtdeAnterior - quantidade) < 0 || (saldoQtdeValidar - quantidade) < 0))
                {
                    var idGrupoProdBaixa = ProdutoDAO.Instance.ObtemIdGrupoProd(session, produtoBaixaEstoque.IdProdBaixa);
                    var idSubgrupoProdBaixa = ProdutoDAO.Instance.ObtemIdSubgrupoProd(session, produtoBaixaEstoque.IdProdBaixa);

                    // Verifica se o subgrupo ou o grupo do produto estão marcados para bloquear estoque.
                    if (GrupoProdDAO.Instance.BloquearEstoque(session, idGrupoProdBaixa, idSubgrupoProdBaixa))
                    {
                        var descricaoGrupo = GrupoProdDAO.Instance.GetDescricao(session, idGrupoProdBaixa);
                        var descricaoSubgrupo = SubgrupoProdDAO.Instance.GetDescricao(session, idSubgrupoProdBaixa > 0 ? idSubgrupoProdBaixa.Value : 0);
                        var codInternoProduto = ProdutoDAO.Instance.GetCodInterno(session, produtoBaixaEstoque.IdProdBaixa);
                        var descricaoProduto = ProdutoDAO.Instance.GetDescrProduto(session, produtoBaixaEstoque.IdProdBaixa);

                        throw new Exception(MensagemAlerta.FormatErrorMsg($@"O Grupo: { descricaoGrupo } ou o Subgrupo: { descricaoSubgrupo } do produto { codInternoProduto } -
                            { descricaoProduto } está marcado para bloquear estoque, portanto, o estoque (Disponível) não pode ser negativo (verificar o extrato de estoque deste produto).", null));
                    }
                }
            }
        }

        #endregion

        #region Atualiza o saldo de estoque

        private void AtualizaSaldoQtd(GDASession sessao, uint idMovEstoque)
        {
            var idProd = ObtemValorCampo<uint>(sessao, "IdProd", $"IdMovEstoque={ idMovEstoque }");
            var idLoja = ObtemValorCampo<uint>(sessao, "IdLoja", $"IdMovEstoque={ idMovEstoque }");
            var dataMov = ObtemValorCampo<DateTime>(sessao, "DataMov", $"IdMovEstoque={ idMovEstoque }");

            var sql = @"
                SET @saldo := COALESCE((SELECT SaldoQtdeMov FROM mov_estoque
                    WHERE (DataMov<?data OR (DataMov=?data AND IdMovEstoque<?id)) AND IdProd=?idProd AND IdLoja=?idLoja
                    ORDER BY DataMov DESC, IdMovEstoque DESC LIMIT 1), 0);

                UPDATE mov_estoque SET SaldoQtdeMov=(@saldo := @saldo + IF(TipoMov=1, QtdeMov, -QtdeMov))
                WHERE (DataMov>?data OR (DataMov=?data AND IdMovEstoque>=?id)) AND IdProd=?idProd AND IdLoja=?idLoja
                ORDER BY DataMov ASC, IdMovEstoque ASC";

            objPersistence.ExecuteCommand(sessao, sql,
                new GDAParameter("?data", dataMov), new GDAParameter("?id", idMovEstoque),
                new GDAParameter("?idProd", idProd), new GDAParameter("?idLoja", idLoja));
        }

        private void AtualizaSaldoTotal(GDASession sessao, uint idMovEstoque)
        {
            var idProd = ObtemValorCampo<uint>(sessao, "IdProd", $"IdMovEstoque={ idMovEstoque }");
            var idLoja = ObtemValorCampo<uint>(sessao, "IdLoja", $"IdMovEstoque={ idMovEstoque }");
            var dataMov = ObtemValorCampo<DateTime>(sessao, "DataMov", $"IdMovEstoque={ idMovEstoque }");
            var idMovEstoqueAnt = ObtemIdMovAnterior(sessao, idMovEstoque, idProd, idLoja, dataMov).GetValueOrDefault();

            var sql = @"
                /**
                 * Recupera algumas variáveis para uso durante o cálculo dos valores das movimentações:
                 * @saldo - o saldo de valor da movimentação anterior à movimentação que está sendo alterada
                 * @valorUnit - o valor unitário da movimentação anterior à movimentação que está sendo alterada
                 * @valorProd - o maior valor salvo para o produto (para normalização de valores, ver abaixo)
                 */
                set @saldo := coalesce((
                    select if(saldoQtdeMov<0, 0, Coalesce(saldoValorMov, 0))
                    from mov_estoque where idMovEstoque=?idAnt
                ), 0),

                @valorUnit := coalesce((
                    select if(saldoQtdeMov<0, 0, abs(coalesce(saldoValorMov/if(saldoQtdeMov<>0, saldoQtdeMov, 1), 0)))
                    from mov_estoque where idMovEstoque=?idAnt
                ), 0),

                @valorProd := 0 /* Removido - erro no cálculo de produtos com valor muito baixo - coalesce((
                    select greatest(valorAtacado, valorBalcao, valorObra, custoCompra, custoFabBase)
                    from produto where idProd=?idProd
                ), 0) */;

                update mov_estoque
                set valorMov=abs(

                    /**
                     * Verifica se a movimentação é de entrada: se for, o valor da movimentação é o próprio valor (calcula o
                     * valor unitário com base no saldo anterior, no valor da movimentação e no saldo de quantidade);
                     * caso não seja, o valor é calculado com base no valor unitário anterior * a quantidade movimentada -
                     * esse valor é armazenado para o próximo cálculo
                     */
                    if(tipoMov=1, (@valorUnit := (@saldo + coalesce(valorMov, 0)) / if(saldoQtdeMov <> 0, saldoQtdeMov, 1)) * 0 + coalesce(valorMov, 0), (@valorUnit :=

                        /**
                         * Verifica se o saldo da movimentação atual é negativo: caso seja, o valor fica zerado (evita saldo negativo);
                         * se não for, o valor unitário é calculado com base no saldo de valor e no saldo de quantidade
                         */
                        if(saldoQtdeMov<0, 0,

                            /**
                             * Verifica se o saldo da movimentação é menor que a quantidade da movimentação (apenas para movimentações
                             * de entrada): calcula o valor da movimentação apenas do percentual que ficou positivo, desprezando o
                             * restante (o saldo anterior era negativo); caso contrário, calcula o valor integral da movimentação
                             */
                            if(saldoQtdeMov<qtdeMov and tipoMov=1,

                                /**
                                 * Soma o saldo anterior (variável @saldo) ao valor da movimentação atual, dividindo pelo saldo de
                                 * quantidade (valor vendido), dividindo novamente pela quantidade da movimentação (valor unitário) e
                                 * multiplicando pelo saldo de quantidade
                                 */
                                ((@saldo+coalesce(valorMov,0)) / if(saldoQtdeMov<>0, saldoQtdeMov, 1)) / if(qtdeMov<>0, qtdeMov, 1) * saldoQtdeMov,

                                /**
                                 * Calcula um valor unitário (temporário) para uso no cálculo
                                 */
                                if((@valorUnit := abs(coalesce((

                                        /**
                                         * Ao calcular o valor da movimentação, é verificado o seu tipo: caso seja movimentação de entrada
                                         * é somado o saldo anterior (variável @saldo) com o valor da movimentação atual, dividindo pelo
                                         * saldo de quantidade (valor vendido); mas se a movimentação for de saída, utiliza-se o valor
                                         * unitário calculado anteriormente
                                         */
                                        if(tipoMov=1, (@saldo+coalesce(valorMov,0))/if(saldoQtdeMov<>0, saldoQtdeMov, 1), @valorUnit)

                                    ), 0))) >

                                    /**
                                     * Garante que o valor calculado seja, no máximo, 5 vezes o maior valor de tabela do produto
                                     * (tentativa de que não haja valores exorbitantes durante o cálculo - apenas se houver algum valor
                                     * de tabela recuperado para o produto)
                                     */
                                    if(@valorProd>0, @valorProd*5, @valorUnit), @valorProd, @valorUnit)
                                )
                            )

                        /**
                         * Multiplica o valor calculado (unitário) pela quantidade movimentada
                         */
                        ) * qtdeMov
                    )),

                    /**
                     * Atualiza o novo saldo na variável @saldo para que esse valor seja usado para o cálculo do valor da
                     * próxima movimentação
                     */
                    saldoValorMov=(@saldo :=

                        /**
                         * Verifica se o saldo da movimentação ficou negativo, alterando para 0.
                         */
                        if(saldoQtdeMov<0, 0,

                            /**
                             * Verifica se o saldo da movimentação é menor que a quantidade movimentada (apenas para
                             * movimentações de entrada): caso seja, calcula o novo valor com base no percentual da movimentação
                             * que ficou positiva; se não, apenas soma/subtrai o valor da movimentação ao saldo
                             */
                            if(saldoQtdeMov<qtdeMov and tipoMov=1,

                                /**
                                 * Divide o valor da movimentação pela quantidade (valor unitário) e então multiplica
                                 * pelo saldo atual de quantidade (apenas a parte positiva)
                                 */
                                valorMov / if(qtdeMov<>0, qtdeMov, 1) * saldoQtdeMov,

                                /**
                                 * Soma ou subtrai o valor da movimentação ao saldo anterior, com base no tipo de movimentação
                                 * realizada (soma para movimentação de entrada, subtrai para movimentação de saída)
                                 */
                                @saldo + if(tipoMov=1, valorMov, -valorMov)
                            )
                        )
                    )

                where (dataMov>?data or (dataMov=?data and idMovEstoque>=?id)) and idProd=?idProd and idLoja=?idLoja
                order by dataMov asc, idMovEstoque asc";

            objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?idAnt", idMovEstoqueAnt),
                new GDAParameter("?data", dataMov), new GDAParameter("?id", idMovEstoque),
                new GDAParameter("?idProd", idProd), new GDAParameter("?idLoja", idLoja));

            if (idMovEstoqueAnt == 0)
            {
                idMovEstoque = ExecuteScalar<uint>(sessao, @"SELECT IdMovEstoque FROM mov_estoque WHERE IdProd=?idProd
                    AND IdLoja=?idLoja AND IdMovEstoque<>?id ORDER BY DataMov ASC, IdMovEstoque ASC LIMIT 1",
                    new GDAParameter("?id", idMovEstoque), new GDAParameter("?idProd", idProd),
                    new GDAParameter("?idLoja", idLoja));

                if (idMovEstoque > 0)
                {
                    AtualizaSaldoTotal(sessao, idMovEstoque);
                }
            }
        }

        /// <summary>
        /// Atualiza o saldo a partir de uma movimentação.
        /// </summary>
        public void AtualizaSaldo(GDASession sessao, uint idMovEstoque)
        {
            AtualizaSaldoQtd(sessao, idMovEstoque);
            AtualizaSaldoTotal(sessao, idMovEstoque);
        }

        #endregion

        #region Apaga as movimentações de NF-e

        /// <summary>
        /// Apaga as movimentações de NF-e.
        /// </summary>
        internal void DeleteByNf(GDASession sessao, uint idNf)
        {
            var idLoja = NotaFiscalDAO.Instance.ObtemIdLoja(sessao, idNf);

            foreach (var produtoNotaFiscal in ProdutosNfDAO.Instance.GetByNf(sessao, idNf))
            {
                var quantidade = ProdutosNfDAO.Instance.ObtemQtdDanfe(sessao, produtoNotaFiscal.IdProd, produtoNotaFiscal.TotM, produtoNotaFiscal.Qtde, produtoNotaFiscal.Altura, produtoNotaFiscal.Largura, false, false);

                decimal saldoQtdeAnterior = 0, saldoValorAnterior = 0, saldoQtdeValidar = 0;
                ValidarMovimentarEstoque(sessao, (int)produtoNotaFiscal.IdProd, (int)idLoja, DateTime.Now, MovEstoque.TipoMovEnum.Saida, (decimal)quantidade, ref saldoQtdeAnterior, ref saldoValorAnterior, ref saldoQtdeValidar, true);
            }

            foreach (var movEstoque in ExecuteMultipleScalar<uint>(sessao, $"SELECT IdMovEstoque FROM mov_estoque WHERE IdNf={ idNf }"))
            {
                DeleteByPrimaryKey(sessao, movEstoque);
            }
        }

        #endregion

        #region Atualiza dados da tabela mov_estoque

        public void AtualizarIdProdPedProducao(GDASession session, int idProdPedProducao, List<int> idsMovEstoque)
        {
            if (idProdPedProducao > 0 && idsMovEstoque.Any(f => f > 0))
            {
                objPersistence.ExecuteCommand(session, $"UPDATE mov_estoque SET IdProdPedProducao={ idProdPedProducao } WHERE IdMovEstoque IN ({ string.Join(",", idsMovEstoque) })");
            }
        }

        public void AtualizarMovimentacaoChapaCortePeca(GDASession session, int idProdPedProducao, List<int> idsMovEstoque, string numEtiquetaChapa, string planoCorteVinculado)
        {
            if (idProdPedProducao > 0 && idsMovEstoque.Any(f => f > 0))
            {
                var numEtiqueta = ProdutoImpressaoDAO.Instance.ObtemNumEtiqueta(session, (uint)idProdPedProducao);
                var obs = ($"Etiqueta: { numEtiquetaChapa }|{ planoCorteVinculado ?? numEtiqueta ?? string.Empty }.").Replace("|.", ".");

                objPersistence.ExecuteCommand(session, $"UPDATE mov_estoque SET IdProdPedProducao=NULL, Obs=?obs WHERE IdMovEstoque IN ({ string.Join(", ", idsMovEstoque) })", new GDAParameter("?obs", obs));
            }
        }

        #endregion

        #region Obtém dados da tabela mov_estoque

        public int? ObterIdLojaPeloIdCompra(GDASession session, int idCompra)
        {
            if (idCompra == 0)
            {
                return null;
            }

            return ObtemValorCampo<int?>(session, "IdLoja", $"IdCompra={ idCompra } LIMIT 1");
        }

        public int? ObterIdLojaPeloIdNf(GDASession session, int idNf, int idProd, MovEstoque.TipoMovEnum tipoMovimentacao)
        {
            if (idNf == 0 || idProd == 0)
            {
                return 0;
            }

            var idLoja = objPersistence.ExecuteScalar(session, $@"SELECT IdLoja FROM mov_estoque
                WHERE IdNf={ idNf } AND IdProd={ idProd } AND TipoMov={ (int)tipoMovimentacao }
                ORDER BY IdMovEstoque DESC LIMIT 1");

            return idLoja?.ToString()?.StrParaInt();
        }

        public bool VerificarIdProdIdLojaPossuiMovimentacao(GDASession session, int idProd, int idLoja)
        {
            if (idProd == 0 || idLoja == 0)
            {
                return false;
            }

            return ObtemValorCampo<bool>(session, "COUNT(*)>0", $"IdProd={ idProd } AND IdLoja={ idLoja }");
        }

        #endregion

        #region Métodos sobrescritos

        public override int Update(MovEstoque objUpdate)
        {
            LogAlteracaoDAO.Instance.LogMovEstoque(objUpdate);
            return base.Update(objUpdate);
        }

        public int DeleteComTransacao(MovEstoque objDelete)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = DeleteByPrimaryKey(transaction, objDelete.IdMovEstoque);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public override int Delete(MovEstoque objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdMovEstoque);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return DeleteByPrimaryKey(null, Key);
        }

        public override int DeleteByPrimaryKey(GDASession session, uint idMovEstoque)
        {
            MovEstoque mov = GetElementByPrimaryKey(session, idMovEstoque);

            if (!mov.DeleteVisible && mov.IdNf.GetValueOrDefault() == 0)
            {
                throw new Exception("Esta movimentação não foi gerada a partir de um lançamento manual, portanto, não é possível excluí-la.");
            }

            LogCancelamentoDAO.Instance.LogMovEstoque(session, mov, null, true);

            // Zera a movimentação para recalcular o saldo
            objPersistence.ExecuteCommand(session, $"UPDATE mov_estoque SET QtdeMov=0, ValorMov=0 WHERE IdMovEstoque={ idMovEstoque }");
            AtualizaSaldo(session, idMovEstoque);

            // Atualiza o saldo na tabela produto_loja
            ProdutoLojaDAO.Instance.AtualizarProdutoLoja(session, (int)mov.IdProd, (int)mov.IdLoja);

            return base.DeleteByPrimaryKey(session, idMovEstoque);
        }

        #endregion
    }
}
