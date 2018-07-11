using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class MovEstoqueDAO : BaseDAO<MovEstoque, MovEstoqueDAO>
    {
        //private MovEstoqueDAO() { }

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
            if (string.IsNullOrWhiteSpace(numEtiqueta))
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
            var custoProd = ProdutosPedidoDAO.Instance.ObterCustoProd(sessao, (int)idProdPed);
            var custoBenef = ProdutoPedidoBenefDAO.Instance.ObterCustoTotalPeloIdProdPed(sessao, (int)idProdPed);

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
            var idProdPed = ProdutosLiberarPedidoDAO.Instance.ObterIdProdPed(sessao, (int)idProdLiberarPedido);
            var custoProd = GetTotalProdPed(sessao, idProdPed);
            var qtdeProd = ProdutosPedidoDAO.Instance.ObtemQtde(sessao, (uint)idProdPed);
            var qtdeLib = ProdutosLiberarPedidoDAO.Instance.ObterQtde(sessao, (int)idProdLiberarPedido);
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
            var custoCompra = ProdutoDAO.Instance.ObtemCustoCompra(session, (int)idProd);
            var qtde = ProdutoPedidoInternoDAO.Instance.ObterTotMQtde(session, (int)idProdPedInterno);

            return custoCompra * (decimal)qtde;
        }

        internal decimal GetTotalEstoqueManual(GDASession sessao, int idProd, decimal qtde)
        {
            var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(sessao, (int)idProd);
            var divisor = new List<int> { (int)TipoCalculoGrupoProd.MLAL0, (int)TipoCalculoGrupoProd.MLAL05, (int)TipoCalculoGrupoProd.MLAL1, (int)TipoCalculoGrupoProd.MLAL6 }.Contains(tipoCalculo) ? 6 : 1;
            var custoCompra = ProdutoDAO.Instance.ObtemCustoCompra(sessao, (int)idProd);

            return custoCompra / divisor * qtde;
        }

        internal decimal GetTotalProdPedProducao(GDASession session, int idProdPedProducao)
        {
            var idProdPed = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(null, (uint)idProdPedProducao);
            idProdPed = ProdutosPedidoDAO.Instance.GetIdProdPedByProdPedEsp(null, idProdPed);
            var custoProd = GetTotalProdPed(null, (int)idProdPed);
            var qtdeProd = ProdutosPedidoDAO.Instance.ObtemQtde(null, idProdPed);
            var idProd = (int)ProdutosPedidoDAO.Instance.ObtemIdProd(null, idProdPed);
            // Caso o custo do produto esteja zerado, busca diretamente do cadastro de produto
            custoProd = custoProd > 0 ? custoProd / (decimal)qtdeProd : ProdutoDAO.Instance.ObtemCustoCompra(null, idProd);

            return custoProd;
        }

        #endregion

        #region Baixa Estoque

        public void BaixaEstoquePedido(GDASession sessao, uint idProd, uint idLoja, uint idPedido, uint idProdPed, decimal qtdeBaixa, decimal qtdeBaixaAreaMinima, bool alterarMateriaPrima,
            string observacao, uint? idVolume, uint? idProdImpressaoChapa)
        {
            var totalProdPed = GetTotalProdPed(sessao, (int)idProdPed);
            var pedidoProducao = PedidoDAO.Instance.IsProducao(sessao, idPedido);

            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, idPedido, null, null, null, null, null, null, idProdPed, null, null, null, null, null, null, null, null, null,
                idVolume, null, idProdImpressaoChapa, false, qtdeBaixa, totalProdPed, alterarMateriaPrima, !pedidoProducao, true, DateTime.Now, true, null, observacao);

            if (PedidoDAO.Instance.GetTipoPedido(sessao, idPedido) == Pedido.TipoPedidoEnum.MaoDeObraEspecial)
            {
                var idClientePedido = PedidoDAO.Instance.GetIdCliente(sessao, idPedido);

                MovEstoqueClienteDAO.Instance.BaixaEstoquePedido(sessao, idClientePedido, idProd, idLoja, idPedido, idProdPed, qtdeBaixa, qtdeBaixaAreaMinima);
            }
        }

        public void BaixaEstoqueCompra(GDASession sessao, uint idProd, uint idLoja, uint idCompra, uint idProdCompra, decimal qtdeBaixa)
        {
            var totalProdCompra = GetTotalProdCompra(sessao, (int)idProdCompra);

            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, idCompra, null, null, null, null, null, null, idProdCompra, null, null, null, null, null, null, null, null,
                null, null, null, false, qtdeBaixa, totalProdCompra, true, false, true, DateTime.Now, true, null, null);
        }

        public void BaixaEstoqueLiberacao(GDASession sessao, uint idProd, uint idLoja, uint idLiberarPedido, uint idPedido, uint idProdLiberarPedido, decimal qtdeBaixa, decimal qtdeBaixaAreaMinima)
        {
            var totalProdLiberarPedido = GetTotalProdLiberarPedido(sessao, (int)idProdLiberarPedido);

            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, idPedido, null, idLiberarPedido, null, null, null, null, null, null, idProdLiberarPedido, null, null, null, null,
                null, null, null, null, null, null, false, qtdeBaixa, totalProdLiberarPedido, true, false, true, DateTime.Now, true, null, null);

            if (PedidoDAO.Instance.GetTipoPedido(sessao, idPedido) == Pedido.TipoPedidoEnum.MaoDeObraEspecial)
            {
                var idClientePedido = PedidoDAO.Instance.GetIdCliente(sessao, idPedido);

                MovEstoqueClienteDAO.Instance.BaixaEstoqueLiberacao(sessao, idClientePedido, idProd, idLoja, idLiberarPedido, idPedido, idProdLiberarPedido, qtdeBaixa, qtdeBaixaAreaMinima);
            }
        }

        public void BaixaEstoqueTrocaDevolucao(GDASession session, uint idProd, uint idLoja, uint idTrocaDevolucao, uint? idProdTrocaDev, uint? idProdTrocado, decimal qtdeBaixa)
        {
            var totalProdTrocaDevolucao = GetTotalProdTrocaDevolucao(session, (int?)idProdTrocaDev, (int?)idProdTrocado);

            MovimentaEstoque(session, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, null, idTrocaDevolucao, null, null, null, null, null, idProdTrocaDev, idProdTrocado, null, null,
                null, null, null, null, null, null, false, qtdeBaixa, totalProdTrocaDevolucao, true, false, true, DateTime.Now, true, null, null);
        }

        public void BaixaEstoqueNotaFiscal(GDASession session, uint idProd, uint idLoja, uint idNf, uint idProdNf, decimal qtdeBaixa)
        {
            var totalProdNf = ProdutosNfDAO.Instance.ObterTotal(session, (int)idProdNf);

            MovimentaEstoque(session, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, null, null, idNf, null, null, null, null, null, null,
                idProdNf, null, null, null, null, null, null, null, false, qtdeBaixa, totalProdNf, true, false, true, DateTime.Now, true, null, null);
        }

        public void BaixaEstoquePedidoInterno(GDASession session, uint idProd, uint idLoja, uint idPedidoInterno, uint idProdPedInterno, decimal qtdeBaixa)
        {
            var totalProdPedInterno = GetTotalProdPedInterno(session, (int)idProd, (int)idProdPedInterno);

            MovimentaEstoque(session, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, null, null, null, idPedidoInterno, null, null, null, null, null, null, idProdPedInterno, null,
                null, null, null, null, null, false, qtdeBaixa, totalProdPedInterno, true, false, true, DateTime.Now, true, null, null);
        }

        public void BaixaEstoqueManual(uint idProd, uint idLoja, decimal qtdeBaixa, decimal? valor, DateTime dataMov, string observacao)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var totalEstoqueManual = GetTotalEstoqueManual(transaction, (int)idProd, qtdeBaixa);

                    MovimentaEstoque(transaction, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                        null, null, null, true, qtdeBaixa, valor.GetValueOrDefault(totalEstoqueManual), false, false, true, dataMov, true, null, observacao);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public void BaixaEstoqueProducao(GDASession sessao, uint idProd, uint idLoja, uint idProdPedProducao, decimal qtdeBaixa, decimal qtdeBaixaAreaMinima, bool alterarMateriaPrima,
            bool baixarMesmoProdutoSemMateriaPrima, bool baixarEstoqueCliente)
        {
            var totalProdPedProducao = GetTotalProdPedProducao(sessao, (int)idProdPedProducao);
            var idPedido = ProdutoPedidoProducaoDAO.Instance.ObtemIdPedido(sessao, idProdPedProducao);

            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, idProdPedProducao, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, false, qtdeBaixa, totalProdPedProducao, alterarMateriaPrima, alterarMateriaPrima, baixarMesmoProdutoSemMateriaPrima, DateTime.Now, false, null, null);

            if (baixarEstoqueCliente && PedidoDAO.Instance.GetTipoPedido(sessao, idPedido) == Pedido.TipoPedidoEnum.MaoDeObraEspecial)
            {
                var idClientePedido = PedidoDAO.Instance.ObtemIdCliente(sessao, idPedido);
                MovEstoqueClienteDAO.Instance.BaixaEstoqueProducao(sessao, idClientePedido, idProd, idLoja, idProdPedProducao, qtdeBaixa, qtdeBaixaAreaMinima);
            }
        }

        public void BaixaEstoqueRetalho(GDASession sessao, uint idProd, uint idLoja, uint idRetalhoProducao, decimal qtdeBaixa)
        {
            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, idRetalhoProducao, null, null, null, null, null, false, qtdeBaixa, 0,
                true, false, true, DateTime.Now, false, null, null);
        }

        public void BaixaEstoquePerdaChapa(GDASession sessao, uint idProd, uint idProdNf, uint idLoja, uint idPerdaChapaVidro)
        {
            var totalProdNf = ProdutosNfDAO.Instance.ObterTotal(sessao, (int)idProdNf);

            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, idPerdaChapaVidro, null, null, null, null, false, 1, totalProdNf, false, false, false, DateTime.Now, true, null, null);
        }

        public void BaixaEstoqueInventario(GDASession sessao, uint idProd, uint idLoja, uint idInventarioEstoque, decimal qtdeBaixa)
        {
            var totalEstoqueManual = GetTotalEstoqueManual(sessao, (int)idProd, qtdeBaixa);

            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                idInventarioEstoque, null, true, qtdeBaixa, totalEstoqueManual, false, false, true, DateTime.Now, false, null, null);
        }

        #endregion

        #region Credita Estoque

        public void CreditaEstoquePedido(GDASession session, uint idProd, uint idLoja, uint idPedido, uint idProdPed, decimal qtdeEntrada, bool alterarMateriaPrima, uint? idVolume, uint? idProdImpressaoChapa)
        {
            var totalProdPed = GetTotalProdPed(session, (int)idProdPed);
            var pedidoProducao = PedidoDAO.Instance.IsProducao(session, idPedido);

            MovimentaEstoque(session, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, idPedido, null, null, null, null, null, null, idProdPed, null, null, null, null, null, null, null, null, null,
                idVolume, null, idProdImpressaoChapa, false, qtdeEntrada, totalProdPed, alterarMateriaPrima, !pedidoProducao, true, DateTime.Now, true, null, null);

            if (PedidoDAO.Instance.GetTipoPedido(session, idPedido) == Pedido.TipoPedidoEnum.MaoDeObraEspecial)
            {
                var idClientePedido = PedidoDAO.Instance.GetIdCliente(session, idPedido);

                MovEstoqueClienteDAO.Instance.CreditaEstoquePedido(session, idClientePedido, idProd, idLoja, idPedido, idProdPed, qtdeEntrada);
            }
        }

        public void CreditaEstoqueCompra(GDASession sessao, uint idProd, uint idLoja, uint idCompra, uint idProdCompra, decimal qtdeEntrada)
        {
            var totalProdCompra = GetTotalProdCompra(sessao, (int)idProdCompra);

            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, idCompra, null, null, null, null, null, null, idProdCompra, null, null, null, null, null, null, null, null,
                null, null, null, false, qtdeEntrada, totalProdCompra, true, false, true, DateTime.Now, true, null, null);
        }

        public void CreditaEstoqueLiberacao(GDASession sessao, uint idProd, uint idLoja, uint idLiberarPedido, uint idPedido, uint idProdLiberarPedido, decimal qtdeEntrada)
        {
            var totalProdLiberarPedido = GetTotalProdLiberarPedido(sessao, (int)idProdLiberarPedido);

            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, idPedido, null, idLiberarPedido, null, null, null, null, null, null, idProdLiberarPedido, null, null, null, null,
                null, null, null, null, null, null, false, qtdeEntrada, totalProdLiberarPedido, true, false, true, DateTime.Now, true, null, null);

            if (PedidoDAO.Instance.GetTipoPedido(sessao, idPedido) == Pedido.TipoPedidoEnum.MaoDeObraEspecial)
            {
                var idClientePedido = PedidoDAO.Instance.GetIdCliente(sessao, idPedido);

                MovEstoqueClienteDAO.Instance.CreditaEstoqueLiberacao(sessao, idClientePedido, idProd, idLoja, idLiberarPedido, idPedido, idProdLiberarPedido, qtdeEntrada);
            }
        }

        public void CreditaEstoqueTrocaDevolucao(GDASession session, uint idProd, uint idLoja, uint idTrocaDevolucao, uint? idProdTrocaDev, uint? idProdTrocado, decimal qtdeEntrada)
        {
            var totalProdTrocaDevolucao = GetTotalProdTrocaDevolucao(session, (int?)idProdTrocaDev, (int?)idProdTrocado);

            MovimentaEstoque(session, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, null, null, idTrocaDevolucao, null, null, null, null, null, idProdTrocaDev, idProdTrocado, null,
                null, null, null, null, null, null, null, false, qtdeEntrada, totalProdTrocaDevolucao, true, false, true, DateTime.Now, true, null, null);
        }

        public void CreditaEstoqueNotaFiscal(GDASession sessao, uint idProd, uint idLoja, uint idNf, uint idProdNf, decimal qtdeEntrada)
        {
            DateTime dataMov = DateTime.Now;
            var tipoDoc = (NotaFiscal.TipoDoc)NotaFiscalDAO.Instance.GetTipoDocumento(sessao, idNf);

            /* Chamado 17610. */
            if (tipoDoc != NotaFiscal.TipoDoc.Saída)
            {
                dataMov = NotaFiscalDAO.Instance.ObtemDataEntradaSaida(sessao, idNf).GetValueOrDefault(dataMov);
            }

            var totalProdNf = ProdutosNfDAO.Instance.ObterTotal(sessao, (int)idProdNf);

            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, null, null, null, idNf, null, null, null, null, null, null,
                idProdNf, null, null, null, null, null, null, null, false, qtdeEntrada, totalProdNf, true, false, true, dataMov, true, null, null);
        }

        public void CreditaEstoqueManual(uint idProd, uint idLoja, decimal qtdeEntrada, decimal? valor, DateTime dataMov, string observacao)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var totalEstoqueManual = GetTotalEstoqueManual(transaction, (int)idProd, qtdeEntrada);

                    MovimentaEstoque(transaction, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                        null, null, null, null, true, qtdeEntrada, valor.GetValueOrDefault(totalEstoqueManual), false, false, true, dataMov, true, null, observacao);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public void CreditaEstoqueProducao(GDASession sessao, uint idProd, uint idLoja, uint idProdPedProducao, decimal qtdeEntrada, bool alterarMateriaPrima, bool creditarEstoqueCliente)
        {
            var totalProdPedProducao = GetTotalProdPedProducao(sessao, (int)idProdPedProducao);

            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, null, idProdPedProducao, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, false, qtdeEntrada, totalProdPedProducao, alterarMateriaPrima, alterarMateriaPrima, true, DateTime.Now, false, null, null);

            if (creditarEstoqueCliente)
            {
                var idPedido = ProdutoPedidoProducaoDAO.Instance.ObtemIdPedido(sessao, idProdPedProducao);

                if (PedidoDAO.Instance.GetTipoPedido(sessao, idPedido) == Pedido.TipoPedidoEnum.MaoDeObraEspecial)
                {
                    var idClientePedido = PedidoDAO.Instance.ObtemIdCliente(sessao, idPedido);
                    MovEstoqueClienteDAO.Instance.CreditaEstoqueProducao(sessao, idClientePedido, idProd, idLoja, idProdPedProducao, qtdeEntrada);
                }
            }
        }

        public void CreditaEstoqueRetalho(GDASession session, uint idProd, uint idLoja, uint idRetalhoProducao, decimal qtdeEntrada, LoginUsuario usuario)
        {
            MovimentaEstoque(session, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, idRetalhoProducao, null, null, null, null, null, false, qtdeEntrada, 0, true, false,
                true, DateTime.Now, false, usuario, null);
        }

        public void CreditaEstoquePerdaChapa(GDASession session, uint idProd, uint idProdNf, uint idLoja, uint idPerdaChapaVidro)
        {
            var totalProdNf = ProdutosNfDAO.Instance.ObterTotal(session, (int)idProdNf);

            MovimentaEstoque(session, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, idPerdaChapaVidro, null, null, null, null, false, 1, totalProdNf, false, false, false, DateTime.Now, true, null, null);
        }

        public void CreditaEstoqueInventario(GDASession sessao, uint idProd, uint idLoja, uint idInventarioEstoque, decimal qtdeEntrada)
        {
            var totalEstoqueManual = GetTotalEstoqueManual(sessao, (int)idProd, qtdeEntrada);

            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                idInventarioEstoque, null, true, qtdeEntrada, totalEstoqueManual, false, false, true, DateTime.Now, false, null, null);
        }

        #endregion

        #region Verifica se há uma movimentação posterior

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se há uma movimentação posterior.
        /// </summary>
        /// <param name="idMovEstoque"></param>
        /// <returns></returns>
        public bool TemMovimentacaoPosterior(uint idMovEstoque)
        {
            return TemMovimentacaoPosterior(null, idMovEstoque);
        }

        /// <summary>
        /// Verifica se há uma movimentação posterior.
        /// </summary>
        /// <param name="idMovEstoque"></param>
        /// <returns></returns>
        public bool TemMovimentacaoPosterior(GDASession sessao, uint idMovEstoque)
        {
            DateTime dataMov = ObtemValorCampo<DateTime>(sessao, "dataMov", "idMovEstoque=" + idMovEstoque);
            uint idProd = ObtemValorCampo<uint>(sessao, "idProd", "idMovEstoque=" + idMovEstoque);

            string sql = @"
                select count(*) from mov_estoque 
                where (dataMov>?data or (dataMov=?data and idMovEstoque>" + idMovEstoque + ")) And idProd=" + idProd;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, new GDAParameter("?data", dataMov)) > 0;
        }

        #endregion

        #region Verifica se o estoque deve ser alterado

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se o estoque deve ser alterado
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idCfop"></param>
        /// <returns></returns>
        public bool AlteraEstoque(uint idProd)
        {
            return AlteraEstoque(null, idProd);
        }

        /// <summary>
        /// Verifica se o estoque deve ser alterado
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idCfop"></param>
        /// <returns></returns>
        public bool AlteraEstoque(GDASession sessao, uint idProd)
        {
            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, (int)idProd);
            var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(sessao, (int)idProd);

            // Altera o estoque somente se estiver marcado para alterar no cadastro de subgrupo
            if (Glass.Data.DAL.GrupoProdDAO.Instance.NaoAlterarEstoque(sessao, idGrupoProd, idSubgrupoProd))
                return false;

            return true;
        }

        #endregion

        #region Movimenta Estoque

        /// <summary>
        /// Dá baixa no estoque no produto da loja passados.
        /// </summary>
        private void MovimentaEstoque(GDASession sessao, uint idProd, uint idLoja, MovEstoque.TipoMovEnum tipoMov, uint? idPedido, uint? idCompra, uint? idLiberarPedido, uint? idProdPedProducao,
            uint? idTrocaDevolucao, uint? idNf, uint? idPedidoInterno, uint? idProdPed, uint? idProdCompra, uint? idProdLiberarPedido, uint? idProdTrocaDev, uint? idProdTrocado, uint? idProdNf,
            uint? idProdPedInterno, uint? idRetalhoProducao, uint? idPerdaChapaVidro, uint? idCarregamento, uint? idVolume, uint? idInventarioEstoque, uint? idProdImpressaoChapa, bool lancManual,
            decimal qtdeMov, decimal total, bool alterarMateriaPrima, bool alterarMateriaPrimaProdutoProducao, bool baixarMesmoProdutoSemMateriaPrima, DateTime dataMov, bool alterarProdBase,
            LoginUsuario usuario, string observacao)
        {
            usuario = usuario != null ? usuario : UserInfo.GetUserInfo;

            if (!AlteraEstoque(sessao, idProd) && !lancManual)
            {
                return;
            }

            try
            {
                ProdutoBaixaEstoque[] pbe;

                if (alterarMateriaPrima && (alterarMateriaPrimaProdutoProducao || !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)idProd)))
                {
                    pbe = ProdutoBaixaEstoqueDAO.Instance.GetByProd(sessao, idProd, baixarMesmoProdutoSemMateriaPrima);
                }
                else
                {
                    pbe = new ProdutoBaixaEstoque[] {
                        new ProdutoBaixaEstoque{
                            IdProd = (int)idProd,
                            IdProdBaixa = (int)idProd,
                            Qtde = 1
                        }
                    };
                }

                foreach (var p in pbe)
                {
                    var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, (int)idProd);

                    //Se não for lançamento manual, não for mov. de produção e o produto for chapa de vidro mov. a matéria-prima
                    if (!lancManual && (tipoSubgrupo == TipoSubgrupoProd.ChapasVidro || tipoSubgrupo == TipoSubgrupoProd.ChapasVidroLaminado) && alterarProdBase)
                    {
                        var m2Chapa = ProdutoDAO.Instance.ObtemM2Chapa(sessao, p.IdProdBaixa);
                        var idProdBase = ProdutoDAO.Instance.ObterProdutoBase(sessao, p.IdProdBaixa);

                        if (idProdBase == p.IdProdBaixa)
                        {
                            throw new Exception("O produto base não pode ser o próprio produto.");
                        }

                        if (idProdBase.HasValue)
                        {
                            MovimentaEstoque(sessao, (uint)idProdBase.Value, idLoja, tipoMov, idPedido, idCompra, idLiberarPedido, idProdPedProducao,
                                idTrocaDevolucao, idNf, idPedidoInterno, idProdPed, idProdCompra, idProdLiberarPedido, idProdTrocaDev,
                                idProdTrocado, idProdNf, idProdPedInterno, idRetalhoProducao, idPerdaChapaVidro, idCarregamento, idVolume,
                                idInventarioEstoque, idProdImpressaoChapa, lancManual, qtdeMov * m2Chapa, total, alterarMateriaPrima, alterarMateriaPrimaProdutoProducao,
                                true, dataMov, alterarProdBase, usuario, observacao);
                        }
                    }

                    var qtde = qtdeMov * (decimal)p.Qtde;
                    decimal saldoQtdeAnterior = 0, saldoValorAnterior = 0, saldoQtdeValidar = 0;

                    //Verifica se idloja = 0, caso for tenta recuperar a loja do Funcionario que está realizando a operação.
                    if (idLoja == 0 && usuario?.IdLoja > 0)
                    {
                        idLoja = usuario.IdLoja;
                    }

                    // Registra a alteração do estoque
                    MovEstoque movEstoque = new MovEstoque();
                    movEstoque.IdProd = (uint)p.IdProdBaixa;
                    movEstoque.IdLoja = idLoja;

                    /* Chamado 44947. */
                    if (Configuracoes.Geral.ConsiderarLojaClientePedidoFluxoSistema && (idPedido > 0 || idTrocaDevolucao > 0))
                    {
                        var idPedidoMov = idPedido > 0 ? (int)idPedido : TrocaDevolucaoDAO.Instance.ObterIdPedido(sessao, (int)idTrocaDevolucao);
                        var apenasTransferencia = PedidoDAO.Instance.ObterApenasTransferencia(sessao, (int)idPedidoMov);

                        if (!apenasTransferencia && idPedidoMov > 0)
                        {
                            movEstoque.IdLoja = PedidoDAO.Instance.ObtemIdLoja(sessao, (uint)idPedidoMov);
                        }
                    }

                    ValidarMovimentarEstoque(sessao, p.IdProdBaixa, (int)movEstoque.IdLoja, dataMov, tipoMov, qtde, ref saldoQtdeAnterior, ref saldoValorAnterior, ref saldoQtdeValidar);

                    movEstoque.IdFunc = usuario?.CodUser ?? 0;
                    movEstoque.IdPedido = idPedido;
                    movEstoque.IdCompra = idCompra;
                    movEstoque.IdLiberarPedido = idLiberarPedido;
                    movEstoque.IdProdPedProducao = idProdPedProducao;
                    movEstoque.IdTrocaDevolucao = idTrocaDevolucao;
                    movEstoque.IdNf = idNf;
                    movEstoque.IdPedidoInterno = idPedidoInterno;
                    movEstoque.IdProdPed = idProdPed;
                    movEstoque.IdProdCompra = idProdCompra;
                    movEstoque.IdProdLiberarPedido = idProdLiberarPedido;
                    movEstoque.IdProdTrocaDev = idProdTrocaDev;
                    movEstoque.IdProdTrocado = idProdTrocado;
                    movEstoque.IdProdNf = idProdNf;
                    movEstoque.IdProdPedInterno = idProdPedInterno;
                    movEstoque.IdRetalhoProducao = idRetalhoProducao;
                    movEstoque.IdPerdaChapaVidro = idPerdaChapaVidro;
                    movEstoque.IdCarregamento = idCarregamento;
                    movEstoque.IdVolume = idVolume;
                    movEstoque.IdInventarioEstoque = idInventarioEstoque;
                    movEstoque.IdProdImpressaoChapa = idProdImpressaoChapa;
                    movEstoque.LancManual = lancManual;
                    movEstoque.TipoMov = (int)tipoMov;
                    movEstoque.DataMov = dataMov.AddSeconds(1); // Criado para evitar problemas ao recalcular o saldo
                    movEstoque.QtdeMov = qtde;
                    movEstoque.Obs = observacao;
                    movEstoque.SaldoQtdeMov = Math.Round(saldoQtdeAnterior + (tipoMov == MovEstoque.TipoMovEnum.Entrada ? qtde : -qtde), Configuracoes.Geral.NumeroCasasDecimaisTotM);

                    if (dataMov.Date != DateTime.Now.Date)
                    {
                        movEstoque.DataCad = DateTime.Now;
                    }

                    if (movEstoque.SaldoQtdeMov < 0)
                    {
                        movEstoque.ValorMov = 0;
                        movEstoque.SaldoValorMov = 0;
                    }
                    else if (tipoMov == MovEstoque.TipoMovEnum.Entrada)
                    {
                        var perc = tipoMov == MovEstoque.TipoMovEnum.Entrada && qtdeMov > movEstoque.SaldoQtdeMov ? qtdeMov / (movEstoque.SaldoQtdeMov > 0 ? movEstoque.SaldoQtdeMov : 1) : 1;

                        movEstoque.ValorMov = Math.Abs(total);
                        movEstoque.SaldoValorMov = saldoValorAnterior + (movEstoque.ValorMov * perc);
                    }
                    else
                    {
                        var valorUnit = saldoValorAnterior / (saldoQtdeAnterior > 0 ? saldoQtdeAnterior : 1);

                        movEstoque.ValorMov = Math.Abs(valorUnit * qtde);
                        movEstoque.SaldoValorMov = saldoValorAnterior - (valorUnit * qtde);
                    }

                    movEstoque.IdMovEstoque = Insert(sessao, movEstoque);

                    AtualizaSaldo(sessao, movEstoque.IdMovEstoque);
                    ProdutoLojaDAO.Instance.AtualizarProdutoLoja(sessao, (int)movEstoque.IdProd, (int)movEstoque.IdLoja);

                    // Atualiza o total de m2 dos boxes
                    var m2 = ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)p.IdProdBaixa) ? ProdutoDAO.Instance.ObtemM2BoxPadrao(sessao, (int)p.IdProdBaixa) : 0;

                    if (m2 > 0)
                    {
                        ProdutoLojaDAO.Instance.AtualizarTotalM2(sessao, (int)p.IdProdBaixa, (int)idLoja, m2);
                    }
                }
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("MovEstoque", ex);
                throw ex;
            }
        }

        public void ValidarMovimentarEstoque(GDASession session, int idProd, int idLoja, DateTime dataMovimentacao, MovEstoque.TipoMovEnum tipoMovimentacao, decimal quantidade,
            bool recuperarProdutoBaixaEstoque)
        {
            decimal saldoQtdeAnterior = 0, saldoValorAnterior = 0, saldoQtdeValidar = 0;

            ValidarMovimentarEstoque(session, idProd, idLoja, dataMovimentacao, tipoMovimentacao, quantidade,
                ref saldoQtdeAnterior, ref saldoValorAnterior, ref saldoQtdeValidar, recuperarProdutoBaixaEstoque);
        }

        public void ValidarMovimentarEstoque(GDASession session, int idProd, int idLoja, DateTime dataMovimentacao, MovEstoque.TipoMovEnum tipoMovimentacao, decimal quantidade,
            ref decimal saldoQtdeAnterior, ref decimal saldoValorAnterior, ref decimal saldoQtdeValidar)
        {
            ValidarMovimentarEstoque(session, idProd, idLoja, dataMovimentacao, tipoMovimentacao, quantidade,
                ref saldoQtdeAnterior, ref saldoValorAnterior, ref saldoQtdeValidar, false);
        }

        public void ValidarMovimentarEstoque(GDASession session, int idProd, int idLoja, DateTime dataMovimentacao, MovEstoque.TipoMovEnum tipoMovimentacao, decimal quantidade,
            ref decimal saldoQtdeAnterior, ref decimal saldoValorAnterior, ref decimal saldoQtdeValidar, bool recuperarProdutoBaixaEstoque)
        {
            ProdutoBaixaEstoque[] produtosBaixaEstoque = null;

            //Verifica se o idloja = 0 se for recupera a loja do usuario.
            if (idLoja == 0 && UserInfo.GetUserInfo != null && UserInfo.GetUserInfo.IdLoja > 0)
            {
                idLoja = (int)UserInfo.GetUserInfo.IdLoja;
            }

            // A baixa do produto será recuperada na validação feita ao emitir a nota fiscal.
            if (recuperarProdutoBaixaEstoque)
            {
                produtosBaixaEstoque = ProdutoBaixaEstoqueDAO.Instance.GetByProd(session, (uint)idProd, true);
            }
            // No procedimento de movimentação de estoque este método será chamado com os dados da baixa.
            else
            {
                produtosBaixaEstoque = new ProdutoBaixaEstoque[] {
                        new ProdutoBaixaEstoque
                        {
                            IdProd = idProd, IdProdBaixa = idProd, Qtde = (float)quantidade
                        }
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
                if (tipoMovimentacao == MovEstoque.TipoMovEnum.Saida)
                {
                    if ((saldoQtdeAnterior - quantidade) < 0 || (saldoQtdeValidar - quantidade) < 0)
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
                ValidarMovimentarEstoque(sessao, (int)produtoNotaFiscal.IdProd, (int)idLoja, DateTime.Now, MovEstoque.TipoMovEnum.Saida, (decimal)quantidade, true);
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
                return 0;
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

            /* Chamado 47777 e 48244. */
            if (!mov.DeleteVisible && mov.IdNf == 0)
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
