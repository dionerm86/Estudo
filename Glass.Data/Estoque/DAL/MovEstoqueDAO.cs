using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class MovEstoqueDAO : BaseDAO<MovEstoque, MovEstoqueDAO>
    {
        //private MovEstoqueDAO() { }

        #region Recupera listagem

        private string Sql(uint idLoja, string codInterno, string descricao, string codOtimizacao, string dataIni, string dataFim, int tipoMov,
            int situacaoProd, string idsGrupoProd, string idsSubgrupoProd, uint idCorVidro, uint idCorFerragem, uint idCorAluminio, bool naoBuscarEstoqueZero,
            bool apenasLancManual, bool selecionar)
        {
            var criterio = String.Empty;
            var campos = selecionar ? @"me.*, p.Descricao as DescrProduto, g.Descricao as DescrGrupo, sg.Descricao as DescrSubgrupo, 
                u.codigo as codUnidade, f.nome as nomeFunc,
                (Select Coalesce(fnf.nomeFantasia, fnf.razaoSocial, '') From fornecedor fnf Where fnf.idFornec=nf.idFornec) As nomeFornec, '$$$' as criterio" :
                "Count(*)";

            var sql = @"
                Select " + campos + @" From mov_estoque me
                    Left Join produto p On (me.idProd=p.idProd) 
                    Left Join grupo_prod g on (p.idGrupoProd=g.idGrupoProd) 
                    Left Join subgrupo_prod sg on (p.idSubgrupoProd=sg.idSubgrupoProd) 
                    Left Join unidade_medida u On (p.idUnidadeMedida=u.idUnidadeMedida)
                    Left Join loja l on (me.idLoja=l.idLoja)
                    Left Join funcionario f On (me.idFunc=f.idFunc)
                    Left Join nota_fiscal nf ON (me.idNf=nf.idNf)
                Where 1";

            // Retorna movimentação apenas se a loja e o produto tiverem sido informados
            if (idLoja == 0 || String.IsNullOrEmpty(codInterno))
                return sql + " And false";

            if (idLoja > 0)
            {
                sql += " And me.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (!String.IsNullOrEmpty(codInterno) || !String.IsNullOrEmpty(descricao) || !String.IsNullOrEmpty(codOtimizacao))
            {
                var ids = ProdutoDAO.Instance.ObtemIds(codInterno, descricao, codOtimizacao);
                sql += " And me.idProd In (" + ids + ")";

                if (!String.IsNullOrEmpty(descricao))
                    criterio += "Produto: " + descricao + "    ";
                else
                    criterio += "Produto: " + ProdutoDAO.Instance.GetDescrProduto(codInterno) + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " And me.dataMov>=?dataIni";
                criterio += "Período: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
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

            if (!String.IsNullOrEmpty(idsGrupoProd))
            {
                sql += " and p.idGrupoProd IN (" + idsGrupoProd + ")";
                var grupos = String.Empty;

                foreach (var id in idsGrupoProd.Split(','))
                    grupos += GrupoProdDAO.Instance.GetDescricao(Conversoes.StrParaInt(id)) + ", ";

                criterio += "Grupo(s): " + grupos.TrimEnd(' ', ',') + "    ";
            }

            if (!string.IsNullOrEmpty(idsSubgrupoProd))
            {
                sql += " and p.idSubgrupoProd IN(" + idsSubgrupoProd + ")";

                var subgrupos = string.Empty;

                foreach (var id in idsSubgrupoProd.Split(','))
                    subgrupos += SubgrupoProdDAO.Instance.GetDescricao(Conversoes.StrParaInt(id)) + ", ";

                criterio += "Subgrupo(s): " + subgrupos.TrimEnd(' ', ',') + "    ";
            }

            if (idCorVidro > 0)
            {
                sql += " and p.idCorVidro=" + idCorVidro;
                criterio += "Cor Vidro: " + CorVidroDAO.Instance.GetNome(idCorVidro) + "    ";
            }

            if (idCorFerragem > 0)
            {
                sql += " and p.idCorFerragem=" + idCorFerragem;
                criterio += "Cor Ferragem: " + CorFerragemDAO.Instance.GetNome(idCorFerragem) + "    ";
            }

            if (idCorAluminio > 0)
            {
                sql += " and p.idCorAluminio=" + idCorAluminio;
                criterio += "Cor Alumínio: " + CorAluminioDAO.Instance.GetNome(idCorAluminio) + "    ";
            }

            if (apenasLancManual)
            {
                sql += " and me.LancManual=true";
                criterio += "Apenas lançamentos manuais    ";
            }

            if (selecionar)
                sql += " Order By me.dataMov asc, me.idMovEstoque Asc";

            return sql.Replace("$$$", criterio);
        }

        public IList<MovEstoque> GetForRpt(uint idLoja, string codInterno, string descricao, string codOtimizacao, string dataIni, string dataFim, int tipoMov,
            int situacaoProd, string idsGrupoProd, string idsSubgrupoProd, uint idCorVidro, uint idCorFerragem, uint idCorAluminio, bool naoBuscarEstoqueZero, 
            bool apenasLancManual)
        {
            return objPersistence.LoadData(Sql(idLoja, codInterno, descricao, codOtimizacao, dataIni, dataFim, tipoMov, situacaoProd,
                idsGrupoProd, idsSubgrupoProd, idCorVidro, idCorFerragem, idCorAluminio, naoBuscarEstoqueZero, apenasLancManual, true),
                GetParam(dataIni, dataFim)).ToList();
        }

        public IList<MovEstoque> GetList(uint idLoja, string codInterno, string descricao, string codOtimizacao, string dataIni, string dataFim, int tipoMov,
            int situacaoProd, string idsGrupoProd, string idsSubgrupoProd, uint idCorVidro, uint idCorFerragem, uint idCorAluminio, bool naoBuscarEstoqueZero,
            bool apenasLancManual)
        {
            return objPersistence.LoadData(Sql(idLoja, codInterno, descricao, codOtimizacao, dataIni, dataFim, tipoMov, situacaoProd,
                idsGrupoProd, idsSubgrupoProd, idCorVidro, idCorFerragem, idCorAluminio, naoBuscarEstoqueZero, apenasLancManual, true),
                GetParam(dataIni, dataFim)).ToList();
        }

        private GDAParameter[] GetParam(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        public IList<MovEstoque> GetListByNf(GDASession session, uint idNf)
        {
            return objPersistence.LoadData(session, "SELECT * FROM mov_estoque WHERE idNf=" + idNf).ToList();
        }

        #endregion

        #region Obtém o saldo do produto

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        private uint? ObtemUltimoIdMovEstoque(uint idProd, uint idLoja)
        {
            return ObtemUltimoIdMovEstoque(null, idProd, idLoja);
        }

        private uint? ObtemUltimoIdMovEstoque(GDASession sessao, uint idProd, uint idLoja)
        {
            return ExecuteScalar<uint?>(sessao, @"select idMovEstoque from mov_estoque FORCE INDEX (INDEX_DATAMOV) where idProd=?idProd
                and idLoja=?idLoja order by dataMov desc, idMovEstoque desc limit 1",
                new GDAParameter("?idProd", idProd), new GDAParameter("?idLoja", idLoja));
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtem o id da movimentação anterior.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <param name="dataMov"></param>
        /// <returns></returns>
        private uint? ObtemIdMovAnterior(uint? idMovEstoque, uint idProd, uint idLoja, DateTime dataMov)
        {
            return ObtemIdMovAnterior(null, idMovEstoque, idProd, idLoja, dataMov);
        }

        /// <summary>
        /// Obtem o id da movimentação anterior.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <param name="dataMov"></param>
        /// <returns></returns>
        private uint? ObtemIdMovAnterior(GDASession sessao, uint? idMovEstoque, uint idProd, uint idLoja, DateTime dataMov)
        {
            // Remove os milisegundos da hora da movimentação
            dataMov = dataMov.AddMilliseconds(-dataMov.Millisecond);

            if (idMovEstoque.GetValueOrDefault() == 0)
            {
                // Adiciona 1 segundo na datamov, para pegar a movimentação correta (Chamado 12177)
                dataMov = dataMov.AddSeconds(1);

                idMovEstoque = ExecuteScalar<uint>(sessao,
                    @"SELECT IdMovEstoque FROM mov_estoque FORCE INDEX (INDEX_DATAMOV)
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
                @"SELECT * FROM (SELECT IdMovEstoque FROM mov_estoque FORCE INDEX (INDEX_DATAMOV)
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
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém o saldo em estoque de determinado produto em determinada loja
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal ObtemSaldoQtdeMov(uint? idMovEstoque, uint idProd, uint idLoja, bool anterior)
        {
            return ObtemSaldoQtdeMov(null, idMovEstoque, idProd, idLoja, anterior);
        }

        /// <summary>
        /// Obtém o saldo em estoque de determinado produto em determinada loja
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal ObtemSaldoQtdeMov(GDASession sessao, uint? idMovEstoque, uint idProd, uint idLoja, bool anterior)
        {
            return ObtemSaldoQtdeMov(sessao, idMovEstoque, idProd, idLoja, DateTime.Now, anterior);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém o saldo em estoque de determinado produto em determinada loja em determinado dia
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <param name="dataMov"></param>
        /// <returns></returns>
        public decimal ObtemSaldoQtdeMov(uint? idMovEstoque, uint idProd, uint idLoja, DateTime dataMov, bool anterior)
        {
            return ObtemSaldoQtdeMov(null, idMovEstoque, idProd, idLoja, dataMov, anterior);
        }

        /// <summary>
        /// Obtém o saldo em estoque de determinado produto em determinada loja em determinado dia
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <param name="dataMov"></param>
        /// <returns></returns>
        public decimal ObtemSaldoQtdeMov(GDASession sessao, uint? idMovEstoque, uint idProd, uint idLoja, DateTime dataMov, bool anterior)
        {
            if (anterior)
                idMovEstoque = ObtemIdMovAnterior(sessao, idMovEstoque, idProd, idLoja, dataMov);
            else if (idMovEstoque.GetValueOrDefault() == 0)
                idMovEstoque = ObtemUltimoIdMovEstoque(sessao, idProd, idLoja);

            return ObtemValorCampo<decimal>(sessao, "saldoQtdeMov", "idMovEstoque=" + idMovEstoque.GetValueOrDefault());
        }

        /// <summary>
        /// Obtém o valor total em estoque de determinado produto em determinada loja
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal ObtemSaldoValorMov(uint? idMovEstoque, uint idProd, uint idLoja, bool anterior)
        {
            return ObtemSaldoValorMov(idMovEstoque, idProd, idLoja, DateTime.Now, anterior);
        }

        #endregion

        #region Recupera mov_estoque chapa
        /// <summary>
        /// Recupera o mov estoque associado a chapa, especificamente para um ProdutoPedidoProducao
        /// </summary>
        public List<int> ObtemMovEstoqueChapaCortePeca(GDASession session, uint idProdPedProducao, string numEtiqueta)
        {
             if(string.IsNullOrWhiteSpace(numEtiqueta))
                return new List<int>();

            ProdutoImpressao produtoimpressao = null;
            var idProd = 0;

            if (numEtiqueta.Contains("N"))
            {
                produtoimpressao = ProdutoImpressaoDAO.Instance.GetElementByEtiqueta(numEtiqueta, ProdutoImpressaoDAO.TipoEtiqueta.NotaFiscal);
                idProd = ProdutosNfDAO.Instance.ObtemValorCampo<int>("IdProd", $"IdProdNf={ produtoimpressao.IdProdNf }");
            }
            else if (numEtiqueta.Contains("R"))
            {
                produtoimpressao = ProdutoImpressaoDAO.Instance.GetElementByEtiqueta(numEtiqueta, ProdutoImpressaoDAO.TipoEtiqueta.Retalho);
                idProd = ((int?)RetalhoProducaoDAO.Instance.ObtemIdProd(session, produtoimpressao.IdRetalhoProducao.GetValueOrDefault())).GetValueOrDefault();
            }

            if (idProd == 0)
            {
                return new List<int>();
            }

            return ExecuteMultipleScalar<int>(session,
                string.Format(@"SELECT me.idMovEstoque FROM mov_estoque me
                    INNER JOIN produto_impressao pi ON (pi.NumEtiqueta = ppp.NumEtiqueta)
                    INNER JOIN chapa_corte_peca ccp ON (ccp.IdProdImpressaoPeca = pi.IdProdImpressao)
                    INNER JOIN produto_impressao piChapa ON (piChapa.IdProdImpressao = {0} And piChapa.IdProdImpressao = ccp.IdProdImpressaoChapa)
                    WHERE ppp.IdProdPedProducao = {1} And me.IdProd = {2} group by me.IdMovEstoque", produtoimpressao.IdProdImpressao, idProdPedProducao, idProd));
        }

        #endregion

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém o valor total em estoque de determinado produto em determinada loja em determinado dia
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal ObtemSaldoValorMov(uint? idMovEstoque, uint idProd, uint idLoja, DateTime dataMov, bool anterior)
        {
            return ObtemSaldoQtdeMov(null, idMovEstoque, idProd, idLoja, dataMov, anterior);
        }

        /// <summary>
        /// Obtém o valor total em estoque de determinado produto em determinada loja em determinado dia
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal ObtemSaldoValorMov(GDASession sesssao, uint? idMovEstoque, uint idProd, uint idLoja, DateTime dataMov, bool anterior)
        {
            if (anterior)
                idMovEstoque = ObtemIdMovAnterior(sesssao, idMovEstoque, idProd, idLoja, dataMov);
            else if (idMovEstoque.GetValueOrDefault() == 0)
                idMovEstoque = ObtemUltimoIdMovEstoque(sesssao, idProd, idLoja);

            return ObtemValorCampo<decimal>(sesssao, "saldoValorMov", "idMovEstoque=" + idMovEstoque.GetValueOrDefault());
        }

        #region Recupera o valor total das movimentações

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idProdPed"></param>
        /// <returns></returns>
        internal decimal GetTotalProdPed(uint idProdPed)
        {
            return GetTotalProdPed(null, idProdPed);
        }

        internal decimal GetTotalProdPed(GDASession sessao, uint idProdPed)
        {   
            decimal custoProd = ProdutosPedidoDAO.Instance.ObtemValorCampo<decimal>(sessao, "custoProd", "idProdPed=" + idProdPed);
            decimal custoBenef = ProdutoPedidoBenefDAO.Instance.ObtemValorCampo<decimal>(sessao, "sum(custo)", "idProdPed=" + idProdPed);
            return custoProd + custoBenef;
        }

        private decimal GetTotalProdCompra(uint idProdCompra)
        {
            return GetTotalProdCompra(null, idProdCompra);
        }

        private decimal GetTotalProdCompra(GDASession sessao, uint idProdCompra)
        {
            decimal totalProd = ProdutosCompraDAO.Instance.ObtemValorCampo<decimal>(sessao, "total", "idProdCompra=" + idProdCompra);
            decimal totalBenef = ProdutosCompraBenefDAO.Instance.ObtemValorCampo<decimal>(sessao, "sum(valor)", "idProdCompra=" + idProdCompra);
            return totalProd + totalBenef;
        }

        internal decimal GetTotalProdLiberarPedido(GDASession sessao, uint idProdLiberarPedido)
        {
            uint idProdPed = ProdutosLiberarPedidoDAO.Instance.ObtemValorCampo<uint>(sessao, "idProdPed", "idProdLiberarPedido=" + idProdLiberarPedido);
            
            decimal custoProd = GetTotalProdPed(sessao, idProdPed);
            float qtdeProd = ProdutosPedidoDAO.Instance.ExecuteScalar<float>(sessao, "Select coalesce(qtde,0) From produtos_pedido Where idProdPed=" + idProdPed);
            float qtdeLib = ProdutosLiberarPedidoDAO.Instance.ObtemValorCampo<float>(sessao, "qtde", "idProdLiberarPedido=" + idProdLiberarPedido);

            float qtdTotal = qtdeProd * qtdeLib;

            return custoProd / (qtdTotal > 0 ? (decimal)qtdTotal : 1);
        }

        private decimal GetTotalProdTrocaDevolucao(uint? idProdTrocaDev, uint? idProdTrocado)
        {
            return GetTotalProdTrocaDevolucao(null, idProdTrocaDev, idProdTrocado);
        }

        private decimal GetTotalProdTrocaDevolucao(GDASession session, uint? idProdTrocaDev, uint? idProdTrocado)
        {
            decimal custo = 0, custoBenef = 0;

            if (idProdTrocaDev > 0)
            {
                custo = ProdutoTrocaDevolucaoDAO.Instance.ObtemValorCampo<decimal>(session, "custoProd", "idProdTrocaDev=" + idProdTrocaDev);
                custoBenef = ProdutoTrocaDevolucaoBenefDAO.Instance.ObtemValorCampo<decimal>(session, "sum(custo)", "idProdTrocaDev=" + idProdTrocaDev);
            }
            else if (idProdTrocado > 0)
            {
                custo = ProdutoTrocadoDAO.Instance.ObtemValorCampo<decimal>(session, "custoProd", "idProdTrocado=" + idProdTrocado);
                custoBenef = ProdutoTrocadoBenefDAO.Instance.ObtemValorCampo<decimal>(session, "sum(custo)", "idProdTrocado=" + idProdTrocado);
            }

            return custo + custoBenef;
        }

        internal decimal GetTotalProdNF(GDASession sessao, uint idProdNf)
        {
            return ProdutosNfDAO.Instance.ObtemValorCampo<decimal>(sessao, "total", "idProdNf=" + idProdNf);
        }

        private decimal GetTotalProdPedInterno(GDASession session, uint idProd, uint idProdPedInterno)
        {
            decimal custoCompra = ProdutoDAO.Instance.ObtemCustoCompra(session, (int)idProd);
            float qtde = ProdutoPedidoInternoDAO.Instance.ObtemValorCampo<float>(session, "coalesce(TotM, Qtde, 0)", 
                "idProdPedInterno=" + idProdPedInterno);

            return custoCompra * (decimal)qtde;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="qtde"></param>
        /// <returns></returns>
        internal decimal GetTotalEstoqueManual(uint idProd, decimal qtde)
        {
            return GetTotalEstoqueManual(null, idProd, qtde);
        }

        internal decimal GetTotalEstoqueManual(GDASession sessao, uint idProd, decimal qtde)
        {
            int divisor = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(sessao, (int)idProd);
            divisor = new List<int> { (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0, (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05,
                (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1, (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 }.Contains(divisor) ? 6 : 1;

            decimal custoCompra = ProdutoDAO.Instance.ObtemCustoCompra(sessao, (int)idProd);
            return custoCompra / divisor * qtde;
        }

        internal decimal GetTotalProdPedProducao(uint idProdPedProducao)
        {
            uint idProdPed = ProdutoPedidoProducaoDAO.Instance.ObtemValorCampo<uint>("idProdPed", "idProdPedProducao=" + idProdPedProducao);
            idProdPed = ProdutosPedidoDAO.Instance.ObtemValorCampo<uint>("idProdPed", "idProdPedEsp=" + idProdPed);

            decimal custoProd = GetTotalProdPed(idProdPed);
            float qtdeProd = ProdutosPedidoDAO.Instance.ObtemQtde(idProdPed);

            // Caso o custo do produto esteja zerado, busca diretamente do cadastro de produto
            custoProd = custoProd > 0 ? custoProd / (decimal)qtdeProd :
                ProdutoDAO.Instance.ObtemCustoCompra((int)ProdutosPedidoDAO.Instance.ObtemIdProd(null, idProdPed));

            return custoProd;
        }

        private decimal GetTotalCarregamento(uint? idProdPed, uint? idProdPedProducao)
        {
            if (idProdPed.HasValue)
            {
                decimal custoProd = GetTotalProdPed(idProdPed.Value);
                float qtdeProd = ProdutosPedidoDAO.Instance.ObtemQtde(idProdPed.Value);

                return custoProd / (decimal)qtdeProd;
            }
            else
            {
                return GetTotalProdPedProducao(idProdPedProducao.Value);
            }
        }

        #endregion

        #region Baixa Estoque

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        public void BaixaEstoquePedido(uint idProd, uint idLoja, uint idPedido, uint idProdPed, decimal qtdeBaixa,
            decimal qtdeBaixaAreaMinima, bool alterarMateriaPrima, string observacao)
        {
            BaixaEstoquePedido(null, idProd, idLoja, idPedido, idProdPed, qtdeBaixa, qtdeBaixaAreaMinima, alterarMateriaPrima, observacao);
        }

        public void BaixaEstoquePedido(GDASession sessao, uint idProd, uint idLoja, uint idPedido, uint idProdPed, decimal qtdeBaixa,
            decimal qtdeBaixaAreaMinima, bool alterarMateriaPrima, string observacao)
        {
            BaixaEstoquePedido(sessao, idProd, idLoja, idPedido, idProdPed, qtdeBaixa, qtdeBaixaAreaMinima, alterarMateriaPrima, observacao, null, null);
        }

        public void BaixaEstoquePedido(GDASession sessao, uint idProd, uint idLoja, uint idPedido, uint idProdPed, decimal qtdeBaixa,
           decimal qtdeBaixaAreaMinima, bool alterarMateriaPrima, string observacao, uint? idVolume, uint? idProdImpressaoChapa)
        {
            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, idPedido, null, null, null, null, null, null, idProdPed, null, null, null,
                null, null, null, null, null, null, idVolume, null, idProdImpressaoChapa, false, qtdeBaixa, GetTotalProdPed(idProdPed), alterarMateriaPrima,
                !PedidoDAO.Instance.IsProducao(idPedido), true, DateTime.Now, true, observacao);

            if (PedidoDAO.Instance.GetTipoPedido(sessao, idPedido) == Pedido.TipoPedidoEnum.MaoDeObraEspecial)
                MovEstoqueClienteDAO.Instance.BaixaEstoquePedido(sessao, PedidoDAO.Instance.GetIdCliente(sessao, idPedido), idProd, idLoja, idPedido, idProdPed, qtdeBaixa, qtdeBaixaAreaMinima);
        }

        public void BaixaEstoqueCompra(uint idProd, uint idLoja, uint idCompra, uint idProdCompra, decimal qtdeBaixa)
        {
            BaixaEstoqueCompra(null, idProd, idLoja, idCompra, idProdCompra, qtdeBaixa);
        }

        public void BaixaEstoqueCompra(GDASession sessao, uint idProd, uint idLoja, uint idCompra, uint idProdCompra, decimal qtdeBaixa)
        {
            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, idCompra, null, null, null, null, null, null, idProdCompra, null,
                null, null, null, null, null, null, null, null, null, null, false, qtdeBaixa, GetTotalProdCompra(sessao, idProdCompra), true, false, true, DateTime.Now, true, null);
        }

        public void BaixaEstoqueLiberacao(GDASession sessao, uint idProd, uint idLoja, uint idLiberarPedido, uint idPedido, uint idProdLiberarPedido,
            decimal qtdeBaixa, decimal qtdeBaixaAreaMinima)
        {
            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, idPedido, null, idLiberarPedido, null, null, null, null, null, null,
                idProdLiberarPedido, null, null, null, null, null, null, null, null, null, null, false, qtdeBaixa,
                GetTotalProdLiberarPedido(sessao, idProdLiberarPedido), true, false, true, DateTime.Now, true, null);

            if (PedidoDAO.Instance.GetTipoPedido(idPedido) == Pedido.TipoPedidoEnum.MaoDeObraEspecial)
                MovEstoqueClienteDAO.Instance.BaixaEstoqueLiberacao(sessao, PedidoDAO.Instance.GetIdCliente(idPedido), idProd, idLoja, 
                    idLiberarPedido, idPedido, idProdLiberarPedido, qtdeBaixa, qtdeBaixaAreaMinima);
        }

        public void BaixaEstoqueTrocaDevolucao(uint idProd, uint idLoja, uint idTrocaDevolucao, uint? idProdTrocaDev, uint? idProdTrocado, decimal qtdeBaixa)
        {
            BaixaEstoqueTrocaDevolucao(null, idProd, idLoja, idTrocaDevolucao, idProdTrocaDev, idProdTrocado, qtdeBaixa);
        }

        public void BaixaEstoqueTrocaDevolucao(GDASession session, uint idProd, uint idLoja, uint idTrocaDevolucao, uint? idProdTrocaDev, uint? idProdTrocado, decimal qtdeBaixa)
        {
            MovimentaEstoque(session, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, null, idTrocaDevolucao, null, null, null, null, null,
                idProdTrocaDev, idProdTrocado, null, null, null, null, null, null, null, null, false, qtdeBaixa, GetTotalProdTrocaDevolucao(session, idProdTrocaDev, idProdTrocado),
                true, false, true, DateTime.Now, true, null);
        }

        public void BaixaEstoqueNotaFiscal(uint idProd, uint idLoja, uint idNf, uint idProdNf, decimal qtdeBaixa)
        {
            BaixaEstoqueNotaFiscal(null, idProd, idLoja, idNf, idProdNf, qtdeBaixa);
        }

        public void BaixaEstoqueNotaFiscal(GDASession session, uint idProd, uint idLoja, uint idNf, uint idProdNf, decimal qtdeBaixa)
        {
            MovimentaEstoque(session, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, null, null, idNf, null, null, null, null, null, null,
                idProdNf, null, null, null, null, null, null, null, false, qtdeBaixa, GetTotalProdNF(null, idProdNf), true, false, true, DateTime.Now, true, null);
        }

        public void BaixaEstoquePedidoInterno(GDASession session, uint idProd, uint idLoja, uint idPedidoInterno, uint idProdPedInterno, decimal qtdeBaixa)
        {
            MovimentaEstoque(session, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, null, null, null, idPedidoInterno,
                null, null, null, null, null, null, idProdPedInterno, null, null, null, null, null, null, false, qtdeBaixa,
                GetTotalProdPedInterno(session, idProd, idProdPedInterno), true, false, true, DateTime.Now, true, null);
        }

        public void BaixaEstoqueManual(uint idProd, uint idLoja, decimal qtdeBaixa, decimal? valor, DateTime dataMov, string observacao)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    MovimentaEstoque(transaction, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, null, null, null, null, null, null, null, null, null,
                        null, null, null, null, null, null, null, null, true, qtdeBaixa, valor.GetValueOrDefault(GetTotalEstoqueManual(idProd, qtdeBaixa)),
                        false, false, true, dataMov, true, observacao);

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

        public void BaixaEstoqueProducao(GDASession sessao, uint idProd, uint idLoja, uint idProdPedProducao, decimal qtdeBaixa, decimal qtdeBaixaAreaMinima,
            bool alterarMateriaPrima, bool baixarMesmoProdutoSemMateriaPrima, bool baixarEstoqueCliente)
        {
            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, idProdPedProducao, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, false, qtdeBaixa, GetTotalProdPedProducao(idProdPedProducao), alterarMateriaPrima, 
                alterarMateriaPrima, baixarMesmoProdutoSemMateriaPrima, DateTime.Now, false, null);

            uint idPedido = ProdutoPedidoProducaoDAO.Instance.ObtemIdPedido(sessao, idProdPedProducao);

            if (baixarEstoqueCliente && PedidoDAO.Instance.GetTipoPedido(sessao, idPedido) == Pedido.TipoPedidoEnum.MaoDeObraEspecial)
            {
                uint idCliente = PedidoDAO.Instance.ObtemIdCliente(sessao, idPedido);
                MovEstoqueClienteDAO.Instance.BaixaEstoqueProducao(sessao, idCliente, idProd, idLoja, idProdPedProducao, qtdeBaixa, qtdeBaixaAreaMinima);
            }
        }

        public void BaixaEstoqueProducao(uint idProd, uint idLoja, uint idProdPedProducao, decimal qtdeBaixa, decimal qtdeBaixaAreaMinima,
            bool alterarMateriaPrima, bool baixarMesmoProdutoSemMateriaPrima, bool baixarEstoqueCliente)
        {
            BaixaEstoqueProducao(null, idProd, idLoja, idProdPedProducao, qtdeBaixa, qtdeBaixaAreaMinima, alterarMateriaPrima, baixarMesmoProdutoSemMateriaPrima, baixarEstoqueCliente);
        }

        public void BaixaEstoqueRetalho(GDASession sessao, uint idProd, uint idLoja, uint idRetalhoProducao, decimal qtdeBaixa)
        {
            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, idRetalhoProducao, null, null, null, null, null, false, qtdeBaixa, 0, 
                true, false, true, DateTime.Now, false, null);
        }

        //public void BaixaEstoqueRetalho(uint idProd, uint idLoja, uint idRetalhoProducao, decimal qtdeBaixa)
        //{
        //    BaixaEstoqueRetalho(null, idProd, idLoja, idRetalhoProducao, qtdeBaixa);
        //}

        public void BaixaEstoquePerdaChapa(GDASession sessao, uint idProd, uint idProdNf, uint idLoja, uint idPerdaChapaVidro)
        {
            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, idPerdaChapaVidro, null, null, null, null, false, 1, GetTotalProdNF(sessao, idProdNf), false, false, false, DateTime.Now, true, null);
        }

        public void BaixaEstoqueCarregamento(uint idLoja, uint idCarregamento, uint idPedido, uint? idVolume, uint idProd,
            uint? idProdPedProducao, decimal qtdeBaixa)
        {
            MovimentaEstoque(idProd, idLoja, MovEstoque.TipoMovEnum.Saida, idPedido, null, null, idProdPedProducao, null, null, null, null,
                null, null, null, null, null, null, null, null, idCarregamento, idVolume, null, null, false, qtdeBaixa, 0, true, false, true, DateTime.Now, false, null);
        }

        public void BaixaEstoqueInventario(GDASession sessao, uint idProd, uint idLoja, uint idInventarioEstoque, decimal qtdeBaixa)
        {
            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Saida, null, null, null, null, null, null, null, null, null, null, null, null, 
                null, null, null, null, null, null, idInventarioEstoque, null, true, qtdeBaixa, GetTotalEstoqueManual(sessao, idProd, qtdeBaixa), false, 
                false, true, DateTime.Now, false, null);
        }

        #endregion

        #region Credita Estoque

        public void CreditaEstoquePedido(uint idProd, uint idLoja, uint idPedido, uint idProdPed, decimal qtdeEntrada, bool alterarMateriaPrima)
        {
            CreditaEstoquePedido(null, idProd, idLoja, idPedido, idProdPed, qtdeEntrada, alterarMateriaPrima);
        }

        public void CreditaEstoquePedido(GDASession session, uint idProd, uint idLoja, uint idPedido, uint idProdPed, decimal qtdeEntrada, bool alterarMateriaPrima)
        {
            CreditaEstoquePedido(session, idProd, idLoja, idPedido, idProdPed, qtdeEntrada, alterarMateriaPrima, null, null);
        }

        public void CreditaEstoquePedido(GDASession session, uint idProd, uint idLoja, uint idPedido, uint idProdPed, decimal qtdeEntrada, bool alterarMateriaPrima, uint? idVolume, uint? idProdImpressaoChapa)
        {
            MovimentaEstoque(session, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, idPedido, null, null, null, null, null, null, idProdPed, null, null,
                null, null, null, null, null, null, null, idVolume, null, idProdImpressaoChapa, false, qtdeEntrada, GetTotalProdPed(session, idProdPed), alterarMateriaPrima,
                !PedidoDAO.Instance.IsProducao(session, idPedido), true, DateTime.Now, true, null);

            if (PedidoDAO.Instance.GetTipoPedido(session, idPedido) == Pedido.TipoPedidoEnum.MaoDeObraEspecial)
                MovEstoqueClienteDAO.Instance.CreditaEstoquePedido(session, PedidoDAO.Instance.GetIdCliente(session, idPedido), idProd, idLoja, idPedido, idProdPed, qtdeEntrada);
        }

        public void CreditaEstoqueCompra(GDASession sessao, uint idProd, uint idLoja, uint idCompra, uint idProdCompra, decimal qtdeEntrada)
        {
            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, idCompra, null, null, null, null, null, null, idProdCompra,
                null, null, null, null, null, null, null, null, null, null, null, false, qtdeEntrada, GetTotalProdCompra(sessao, idProdCompra), true, false, 
                true, DateTime.Now, true, null);
        }

        public void CreditaEstoqueLiberacao(GDASession sessao, uint idProd, uint idLoja, uint idLiberarPedido, uint idPedido, uint idProdLiberarPedido, decimal qtdeEntrada)
        {
            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, idPedido, null, idLiberarPedido, null, null, null, null, null, null,
                idProdLiberarPedido, null, null, null, null, null, null, null, null, null, null, false, qtdeEntrada, GetTotalProdLiberarPedido(sessao, idProdLiberarPedido), 
                true, false, true, DateTime.Now, true, null);

            if (PedidoDAO.Instance.GetTipoPedido(idPedido) == Pedido.TipoPedidoEnum.MaoDeObraEspecial)
                MovEstoqueClienteDAO.Instance.CreditaEstoqueLiberacao(sessao, PedidoDAO.Instance.GetIdCliente(sessao, idPedido), idProd, idLoja, idLiberarPedido, idPedido,
                    idProdLiberarPedido, qtdeEntrada);
        }

        public void CreditaEstoqueTrocaDevolucao(uint idProd, uint idLoja, uint idTrocaDevolucao, uint? idProdTrocaDev, uint? idProdTrocado, decimal qtdeEntrada)
        {
            CreditaEstoqueTrocaDevolucao(null, idProd, idLoja, idTrocaDevolucao, idProdTrocaDev, idProdTrocado, qtdeEntrada);
        }

        public void CreditaEstoqueTrocaDevolucao(GDASession session, uint idProd, uint idLoja, uint idTrocaDevolucao, uint? idProdTrocaDev, uint? idProdTrocado, decimal qtdeEntrada)
        {
            MovimentaEstoque(session, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, null, null, idTrocaDevolucao, null, null, null, null, null,
                idProdTrocaDev, idProdTrocado, null, null, null, null, null, null, null, null, false, qtdeEntrada, GetTotalProdTrocaDevolucao(session, idProdTrocaDev, idProdTrocado), 
                true, false, true, DateTime.Now, true, null);
        }

        public void CreditaEstoqueNotaFiscal(GDASession sessao, uint idProd, uint idLoja, uint idNf, uint idProdNf, decimal qtdeEntrada)
        {
            DateTime dataMov = DateTime.Now;
            var tipoDoc = (NotaFiscal.TipoDoc)NotaFiscalDAO.Instance.GetTipoDocumento(sessao, idNf);

            /* Chamado 17610. */
            if (tipoDoc != NotaFiscal.TipoDoc.Saída)
                dataMov = NotaFiscalDAO.Instance.ObtemValorCampo<DateTime?>(sessao, "dataSaidaEnt", "idNf=" + idNf).GetValueOrDefault(dataMov);

            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, null, null, null, idNf, null, null, null, null, null, null,
                idProdNf, null, null, null, null, null, null, null, false, qtdeEntrada, GetTotalProdNF(sessao, idProdNf), true, false, true, dataMov, true, null);
        }

        public void CreditaEstoquePedidoInterno(uint idProd, uint idLoja, uint idPedidoInterno, uint idProdPedInterno, decimal qtdeEntrada)
        {
            MovimentaEstoque(idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, null, null, null, null, idPedidoInterno, null, null, null,
                null, null, null, idProdPedInterno, null, null, null, null, null, null, false, qtdeEntrada, GetTotalProdPedInterno(null, idProd, idProdPedInterno), 
                true, false, true, DateTime.Now, true, null);
        }

        public void CreditaEstoqueManual(uint idProd, uint idLoja, decimal qtdeEntrada, decimal? valor, DateTime dataMov, string observacao)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    MovimentaEstoque(transaction, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, null, null, null, null, null, null, null, null, null, null,
                        null, null, null, null, null, null, null, null, true, qtdeEntrada, valor.GetValueOrDefault(GetTotalEstoqueManual(transaction, idProd, qtdeEntrada)), false,
                        false, true, dataMov, true, observacao);
                    
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

        public void CreditaEstoqueProducao(uint idProd, uint idLoja, uint idProdPedProducao, decimal qtdeEntrada, bool alterarMateriaPrima, bool creditarEstoqueCliente)
        {
            CreditaEstoqueProducao(null, idProd, idLoja, idProdPedProducao, qtdeEntrada, alterarMateriaPrima, creditarEstoqueCliente);
        }

        public void CreditaEstoqueProducao(GDASession sessao, uint idProd, uint idLoja, uint idProdPedProducao, decimal qtdeEntrada, bool alterarMateriaPrima, bool creditarEstoqueCliente)
        {
            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, null, idProdPedProducao, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, false, qtdeEntrada, GetTotalProdPedProducao(idProdPedProducao), alterarMateriaPrima,
                alterarMateriaPrima, true, DateTime.Now, false, null);

            if (creditarEstoqueCliente)
            {
                uint idPedido = ProdutoPedidoProducaoDAO.Instance.ObtemIdPedido(sessao, idProdPedProducao);

                if (PedidoDAO.Instance.GetTipoPedido(idPedido) == Pedido.TipoPedidoEnum.MaoDeObraEspecial)
                {
                    uint idCliente = PedidoDAO.Instance.ObtemIdCliente(sessao, idPedido);
                    MovEstoqueClienteDAO.Instance.CreditaEstoqueProducao(sessao, idCliente, idProd, idLoja, idProdPedProducao, qtdeEntrada);
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
            MovimentaEstoque(session, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, idPerdaChapaVidro, null, null, null, null, false, 1, GetTotalProdNF(session, idProdNf), false, false, false, DateTime.Now, true, null);
        }

        public void CreditaEstoqueCarregamento(uint idLoja, uint idCarregamento, uint idPedido, uint? idVolume, uint idProd,
            uint? idProdPedProducao, uint? idProdPed, decimal qtdeEntrada)
        {
            MovimentaEstoque(idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, idPedido, null, null, idProdPedProducao, null, null, null, null,
                null, null, null, null, null, null, null, null, idCarregamento, idVolume, null, null, false,
                qtdeEntrada, GetTotalCarregamento(idProdPed, idProdPedProducao), true, false, true, DateTime.Now, false, null);
        }

        public void CreditaEstoqueInventario(GDASession sessao, uint idProd, uint idLoja, uint idInventarioEstoque, decimal qtdeEntrada)
        {
            MovimentaEstoque(sessao, idProd, idLoja, MovEstoque.TipoMovEnum.Entrada, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, idInventarioEstoque, null, true, qtdeEntrada, GetTotalEstoqueManual(sessao, idProd, qtdeEntrada), false,
                false, true, DateTime.Now, false, null);
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
        private void MovimentaEstoque(uint idProd, uint idLoja, MovEstoque.TipoMovEnum tipoMov, uint? idPedido, uint? idCompra, uint? idLiberarPedido,
            uint? idProdPedProducao, uint? idTrocaDevolucao, uint? idNf, uint? idPedidoInterno, uint? idProdPed, uint? idProdCompra,
            uint? idProdLiberarPedido, uint? idProdTrocaDev, uint? idProdTrocado, uint? idProdNf, uint? idProdPedInterno, uint? idRetalhoProducao,
            uint? idPerdaChapaVidro, uint? idCarregamento, uint? idVolume, uint? idInventarioEstoque, uint? idProdImpressaoChapa, bool lancManual, decimal qtdeMov, decimal total,
            bool alterarMateriaPrima, bool alterarMateriaPrimaProdutoProducao, bool baixarMesmoProdutoSemMateriaPrima, DateTime dataMov, bool alterarProdBase,
            string observacao)
        {
            MovimentaEstoque(null, idProd, idLoja, tipoMov, idPedido, idCompra, idLiberarPedido, idProdPedProducao, idTrocaDevolucao,
                idNf, idPedidoInterno, idProdPed, idProdCompra, idProdLiberarPedido, idProdTrocaDev, idProdTrocado, idProdNf, idProdPedInterno,
                idRetalhoProducao, idPerdaChapaVidro, idCarregamento, idVolume, idInventarioEstoque, idProdImpressaoChapa, lancManual, qtdeMov, total, alterarMateriaPrima,
                alterarMateriaPrimaProdutoProducao, baixarMesmoProdutoSemMateriaPrima, dataMov, alterarProdBase, null, observacao);
        }

        /// <summary>
        /// Dá baixa no estoque no produto da loja passados.
        /// </summary>
        private void MovimentaEstoque(GDASession sessao, uint idProd, uint idLoja, MovEstoque.TipoMovEnum tipoMov, uint? idPedido, uint? idCompra, uint? idLiberarPedido,
            uint? idProdPedProducao, uint? idTrocaDevolucao, uint? idNf, uint? idPedidoInterno, uint? idProdPed, uint? idProdCompra,
            uint? idProdLiberarPedido, uint? idProdTrocaDev, uint? idProdTrocado, uint? idProdNf, uint? idProdPedInterno, uint? idRetalhoProducao,
            uint? idPerdaChapaVidro, uint? idCarregamento, uint? idVolume, uint? idInventarioEstoque, uint? idProdImpressaoChapa, bool lancManual, decimal qtdeMov, decimal total,
            bool alterarMateriaPrima, bool alterarMateriaPrimaProdutoProducao, bool baixarMesmoProdutoSemMateriaPrima, DateTime dataMov, bool alterarProdBase,
            string observacao)
        {
            MovimentaEstoque(sessao, idProd, idLoja, tipoMov, idPedido, idCompra, idLiberarPedido, idProdPedProducao, idTrocaDevolucao,
                idNf, idPedidoInterno, idProdPed, idProdCompra, idProdLiberarPedido, idProdTrocaDev, idProdTrocado, idProdNf, idProdPedInterno,
                idRetalhoProducao, idPerdaChapaVidro, idCarregamento, idVolume, idInventarioEstoque, idProdImpressaoChapa, lancManual, qtdeMov, total, alterarMateriaPrima,
                alterarMateriaPrimaProdutoProducao, baixarMesmoProdutoSemMateriaPrima, dataMov, alterarProdBase, null, observacao);
        }

        /// <summary>
        /// Dá baixa no estoque no produto da loja passados.
        /// </summary>
        private void MovimentaEstoque(GDASession sessao, uint idProd, uint idLoja, MovEstoque.TipoMovEnum tipoMov, uint? idPedido, uint? idCompra, uint? idLiberarPedido,
            uint? idProdPedProducao, uint? idTrocaDevolucao, uint? idNf, uint? idPedidoInterno, uint? idProdPed, uint? idProdCompra,
            uint? idProdLiberarPedido, uint? idProdTrocaDev, uint? idProdTrocado, uint? idProdNf, uint? idProdPedInterno, uint? idRetalhoProducao,
            uint? idPerdaChapaVidro, uint? idCarregamento, uint? idVolume, uint? idInventarioEstoque, uint? idProdImpressaoChapa, bool lancManual, decimal qtdeMov, decimal total,
            bool alterarMateriaPrima, bool alterarMateriaPrimaProdutoProducao, bool baixarMesmoProdutoSemMateriaPrima, DateTime dataMov, bool alterarProdBase,
            LoginUsuario usuario, string observacao)
        {
            if (!AlteraEstoque(sessao, idProd) && !lancManual)
                return;

            try
            {
                ProdutoBaixaEstoque[] pbe;

                if (alterarMateriaPrima && (alterarMateriaPrimaProdutoProducao || !ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)idProd)))
                    pbe = ProdutoBaixaEstoqueDAO.Instance.GetByProd(sessao, idProd, baixarMesmoProdutoSemMateriaPrima);
                else
                    pbe = new ProdutoBaixaEstoque[] {
                    new ProdutoBaixaEstoque{
                        IdProd = (int)idProd,
                        IdProdBaixa = (int)idProd,
                        Qtde = 1
                    }
                };

                foreach (ProdutoBaixaEstoque p in pbe)
                {
                    var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(sessao, (int)idProd);
                    
                    //Se não for lançamento manual, não for mov. de produção e o produto for chapa de vidro mov. a matéria-prima
                    if (!lancManual && (tipoSubgrupo == TipoSubgrupoProd.ChapasVidro || tipoSubgrupo == TipoSubgrupoProd.ChapasVidroLaminado) && alterarProdBase)
                    {
                        var m2Chapa = ProdutoDAO.Instance.ObtemM2Chapa(sessao, p.IdProdBaixa);
                        var idProdBase = ProdutoDAO.Instance.ObtemValorCampo<uint?>(sessao, "idProdBase", "idProd=" + p.IdProdBaixa);

                        if (idProdBase == p.IdProdBaixa)
                            throw new Exception("O produto base não pode ser o próprio produto.");

                        if (idProdBase.HasValue)
                        {
                            MovimentaEstoque(sessao, idProdBase.Value, idLoja, tipoMov, idPedido, idCompra, idLiberarPedido, idProdPedProducao,
                                idTrocaDevolucao, idNf, idPedidoInterno, idProdPed, idProdCompra, idProdLiberarPedido, idProdTrocaDev,
                                idProdTrocado, idProdNf, idProdPedInterno, idRetalhoProducao, idPerdaChapaVidro, idCarregamento, idVolume,
                                idInventarioEstoque, idProdImpressaoChapa, lancManual, qtdeMov * m2Chapa, total, alterarMateriaPrima, alterarMateriaPrimaProdutoProducao,
                                true, dataMov, alterarProdBase, observacao);
                        }
                    }
                    
                    var qtde = qtdeMov * (decimal)p.Qtde;
                    decimal saldoQtdeAnterior = 0, saldoValorAnterior = 0, saldoQtdeValidar = 0;

                    //Verifica se idloja = 0, caso for tenta recuperar a loja do Funcionario que está realizando a operação.
                    if (idLoja == 0 && UserInfo.GetUserInfo != null && UserInfo.GetUserInfo.IdLoja > 0)
                        idLoja = UserInfo.GetUserInfo.IdLoja;

                    // Registra a alteração do estoque
                    MovEstoque movEstoque = new MovEstoque();
                    movEstoque.IdProd = (uint)p.IdProdBaixa;
                    movEstoque.IdLoja = idLoja;

                    /* Chamado 44947. */
                    if (Configuracoes.Geral.ConsiderarLojaClientePedidoFluxoSistema && (idPedido > 0 || idTrocaDevolucao > 0))
                    {
                        var idPedidoMov = idPedido > 0 ? (int)idPedido : TrocaDevolucaoDAO.Instance.ObterIdPedido(sessao, (int)idTrocaDevolucao);
                        var apenasTransferencia = PedidoDAO.Instance.ObtemValorCampo<bool?>(sessao, "ApenasTransferencia", "IdPedido=" + idPedidoMov).GetValueOrDefault();

                        if (!apenasTransferencia && idPedidoMov > 0)
                            movEstoque.IdLoja = PedidoDAO.Instance.ObtemIdLoja(sessao, (uint)idPedidoMov);
                    }

                    ValidarMovimentarEstoque(sessao, p.IdProdBaixa, (int)movEstoque.IdLoja, dataMov, tipoMov, qtde, ref saldoQtdeAnterior,
                        ref saldoValorAnterior, ref saldoQtdeValidar);

                    movEstoque.IdFunc = usuario != null ? usuario.CodUser : UserInfo.GetUserInfo.CodUser;
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
                    if (dataMov.Date != DateTime.Now.Date) movEstoque.DataCad = DateTime.Now;
                    movEstoque.QtdeMov = qtde;
                    movEstoque.Obs = observacao;

                    movEstoque.SaldoQtdeMov = Math.Round(saldoQtdeAnterior + (tipoMov == MovEstoque.TipoMovEnum.Entrada ? qtde : -qtde), Glass.Configuracoes.Geral.NumeroCasasDecimaisTotM);

                    if (movEstoque.SaldoQtdeMov < 0)
                    {
                        movEstoque.ValorMov = 0;
                        movEstoque.SaldoValorMov = 0;
                    }
                    else if (tipoMov == MovEstoque.TipoMovEnum.Entrada)
                    {
                        decimal perc = tipoMov == MovEstoque.TipoMovEnum.Entrada && qtdeMov > movEstoque.SaldoQtdeMov ?
                            qtdeMov / (movEstoque.SaldoQtdeMov > 0 ? movEstoque.SaldoQtdeMov : 1) : 1;

                        movEstoque.ValorMov = Math.Abs(total);
                        movEstoque.SaldoValorMov = saldoValorAnterior + (movEstoque.ValorMov * perc);
                    }
                    else
                    {
                        decimal valorUnit = saldoValorAnterior / (saldoQtdeAnterior > 0 ? saldoQtdeAnterior : 1);
                        movEstoque.ValorMov = Math.Abs(valorUnit * qtde);
                        movEstoque.SaldoValorMov = saldoValorAnterior - (valorUnit * qtde);
                    }

                    movEstoque.IdMovEstoque = Insert(sessao, movEstoque);

                    // Chamado 15184: Sempre atualiza o saldo, para resolver o erro de não recalcular o saldo
                    /*var idMovAnterior = ObtemIdMovAnterior(sessao, movEstoque.IdMovEstoque, movEstoque.IdProd, movEstoque.IdLoja, movEstoque.DataMov);
                    if (idMovAnterior != null)
                        AtualizaSaldo(sessao, idMovAnterior.Value);
                    else
                        AtualizaSaldo(sessao, movEstoque.IdMovEstoque);*/
                    /* Chamado 28255.
                     * O saldo deve ser atualizado sempre com base na movimentação inserida. */
                    AtualizaSaldo(sessao, movEstoque.IdMovEstoque);

                    AtualizaProdutoLoja(sessao, movEstoque.IdProd, movEstoque.IdLoja);

                    // Atualiza o total de m2 dos boxes
                    decimal m2 = ProdutoDAO.Instance.IsProdutoProducao(sessao, (int)p.IdProdBaixa) ? ProdutoDAO.Instance.ObtemM2BoxPadrao(sessao, (int)p.IdProdBaixa) : 0;
                    if (m2 > 0)
                        objPersistence.ExecuteCommand(sessao, "Update produto_loja Set m2=QtdEstoque*" + m2.ToString().Replace(',', '.') +
                            " Where idProd=" + p.IdProdBaixa + " And idLoja=" + idLoja);
                }
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("MovEstoque", ex);
                throw ex;
            }
        }
 
        public void ValidarMovimentarEstoque(GDASession session, int idProd, int idLoja, DateTime dataMovimentacao,
            MovEstoque.TipoMovEnum tipoMovimentacao, decimal quantidade, bool recuperarProdutoBaixaEstoque)
        {
            decimal saldoQtdeAnterior = 0, saldoValorAnterior = 0, saldoQtdeValidar = 0;

            ValidarMovimentarEstoque(session, idProd, idLoja, dataMovimentacao, tipoMovimentacao, quantidade,
                ref saldoQtdeAnterior, ref saldoValorAnterior, ref saldoQtdeValidar, recuperarProdutoBaixaEstoque);
        }

        public void ValidarMovimentarEstoque(GDASession session, int idProd, int idLoja, DateTime dataMovimentacao,
            MovEstoque.TipoMovEnum tipoMovimentacao, decimal quantidade, ref decimal saldoQtdeAnterior,
            ref decimal saldoValorAnterior, ref decimal saldoQtdeValidar)
        {
            ValidarMovimentarEstoque(session, idProd, idLoja, dataMovimentacao, tipoMovimentacao, quantidade,
                ref saldoQtdeAnterior, ref saldoValorAnterior, ref saldoQtdeValidar, false);
        }

        public void ValidarMovimentarEstoque(GDASession session, int idProd, int idLoja, DateTime dataMovimentacao,
            MovEstoque.TipoMovEnum tipoMovimentacao, decimal quantidade, ref decimal saldoQtdeAnterior,
            ref decimal saldoValorAnterior, ref decimal saldoQtdeValidar, bool recuperarProdutoBaixaEstoque)
        {
            ProdutoBaixaEstoque[] produtosBaixaEstoque = null;

            //Verifica se o idloja = 0 se for recupera a loja do usuario.
            if (idLoja == 0 && UserInfo.GetUserInfo != null && UserInfo.GetUserInfo.IdLoja > 0)
                idLoja = (int)UserInfo.GetUserInfo.IdLoja;

            // A baixa do produto será recuperada na validação feita ao emitir a nota fiscal.
            if (recuperarProdutoBaixaEstoque)
                produtosBaixaEstoque = ProdutoBaixaEstoqueDAO.Instance.GetByProd(session, (uint)idProd, true);
            // No procedimento de movimentação de estoque este método será chamado com os dados da baixa.
            else
                produtosBaixaEstoque =
                    new ProdutoBaixaEstoque[]
                    {
                        new ProdutoBaixaEstoque
                        {
                            IdProd = idProd, IdProdBaixa = idProd, Qtde = (float)quantidade
                        }
                    };

            foreach (var produtoBaixaEstoque in produtosBaixaEstoque)
            {
                // No procedimento de movimentação de estoque este método será chamado com os dados da baixa.
                // Já na tela de nota fiscal a baixa do produto será recuperada e a quantidade da movimentação deve ser calculada.
                if (recuperarProdutoBaixaEstoque)
                    quantidade *= (decimal)produtoBaixaEstoque.Qtde;

                // Recupera os dados da movimentação anterior.
                saldoQtdeAnterior = ObtemSaldoQtdeMov(session, null, (uint)produtoBaixaEstoque.IdProdBaixa, (uint)idLoja, dataMovimentacao, true);
                saldoValorAnterior = ObtemSaldoValorMov(session, null, (uint)produtoBaixaEstoque.IdProdBaixa, (uint)idLoja, dataMovimentacao, true);
                /* Chamado 24084. */
                saldoQtdeValidar = ObtemSaldoQtdeMov(session, null, (uint)produtoBaixaEstoque.IdProdBaixa, (uint)idLoja, dataMovimentacao, false);
                
                /* Chamado 14680.
                 * O cliente informou que ao efetuar o lançamento manual de estoque em um produto o saldo em quantidade do mesmo
                 * havia ficado negativo, sendo que o subgrupo do mesmo estava marcado para bloquear estoque.
                 * Esta verificação irá impedir que isto ocorra novamente. */
                // Verifica se, ao registrar a movimentação, o saldo em estoque do produto ficará negativo.
                if (tipoMovimentacao == MovEstoque.TipoMovEnum.Saida)
                {
                    /* Chamado 36690. */
                    /* Chamado 36804.
                    var produtoLoja = ProdutoLojaDAO.Instance.GetElement(session, (uint)idLoja, (uint)produtoBaixaEstoque.IdProdBaixa);
                    produtoLoja.QtdEstoque -= (double)quantidade;*/

                    if ((saldoQtdeAnterior - quantidade) < 0 || (saldoQtdeValidar - quantidade) < 0)
                        /* Chamado 36690. */
                        /* Chamado 36804.
                        (produtoLoja.Disponivel < 0)*/
                    {
                        var idGrupoProdBaixa = ProdutoDAO.Instance.ObtemIdGrupoProd(session, produtoBaixaEstoque.IdProdBaixa);
                        var idSubgrupoProdBaixa = ProdutoDAO.Instance.ObtemValorCampo<int?>(session, "IdSubGrupoProd", string.Format("IdProd={0}", produtoBaixaEstoque.IdProdBaixa));

                        // Verifica se o subgrupo ou o grupo do produto estão marcados para bloquear estoque.
                        if (GrupoProdDAO.Instance.BloquearEstoque(session, idGrupoProdBaixa, idSubgrupoProdBaixa))
                            throw new Exception(MensagemAlerta.FormatErrorMsg(
                                string.Format("O Grupo: {0} ou o Subgrupo: {1} do produto {2} - {3} está marcado para bloquear estoque, portanto, o estoque (Disponível) não pode ser negativo (verificar o extrato de estoque deste produto).",
                                GrupoProdDAO.Instance.GetDescricao(session, idGrupoProdBaixa), SubgrupoProdDAO.Instance.GetDescricao(session, idSubgrupoProdBaixa > 0 ? idSubgrupoProdBaixa.Value : 0),
                                ProdutoDAO.Instance.GetCodInterno(session, produtoBaixaEstoque.IdProdBaixa),
                                    ProdutoDAO.Instance.GetDescrProduto(session, produtoBaixaEstoque.IdProdBaixa)), null));
                    }
                }
            }
        }

        #endregion

        #region Atualiza o saldo de estoque

        private void AtualizaSaldoQtd(uint idMovEstoque)
        {
            AtualizaSaldoQtd(null, idMovEstoque);
        }

        private void AtualizaSaldoQtd(GDASession sessao, uint idMovEstoque)
        {
            uint idProd = ObtemValorCampo<uint>(sessao, "idProd", "idMovEstoque=" + idMovEstoque);
            uint idLoja = ObtemValorCampo<uint>(sessao, "idLoja", "idMovEstoque=" + idMovEstoque);
            DateTime dataMov = ObtemValorCampo<DateTime>(sessao, "dataMov", "idMovEstoque=" + idMovEstoque);

            string sql = @"
                set @saldo := coalesce((select saldoQtdeMov from mov_estoque
                    where (dataMov<?data or (dataMov=?data and idMovEstoque<?id))
                    and idProd=?idProd and idLoja=?idLoja
                    order by dataMov desc, idMovEstoque desc limit 1), 0);
                
                update mov_estoque set saldoQtdeMov=(@saldo := @saldo + if(tipoMov=1, qtdeMov, -qtdeMov))
                where (dataMov>?data or (dataMov=?data and idMovEstoque>=?id)) and idProd=?idProd and idLoja=?idLoja
                order by dataMov asc, idMovEstoque asc";

            objPersistence.ExecuteCommand(sessao, sql, 
                new GDAParameter("?data", dataMov), new GDAParameter("?id", idMovEstoque),
                new GDAParameter("?idProd", idProd), new GDAParameter("?idLoja", idLoja));
        }

        private void AtualizaSaldoTotal(uint idMovEstoque)
        {
            AtualizaSaldoTotal(null, idMovEstoque);
        }

        private void AtualizaSaldoTotal(GDASession sessao, uint idMovEstoque)
        {
            uint idProd = ObtemValorCampo<uint>(sessao, "idProd", "idMovEstoque=" + idMovEstoque);
            uint idLoja = ObtemValorCampo<uint>(sessao, "idLoja", "idMovEstoque=" + idMovEstoque);
            DateTime dataMov = ObtemValorCampo<DateTime>(sessao, "dataMov", "idMovEstoque=" + idMovEstoque);

            uint idMovEstoqueAnt = ObtemIdMovAnterior(sessao, idMovEstoque, idProd, idLoja, dataMov).GetValueOrDefault();

            string sql = @"
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
                idMovEstoque = ExecuteScalar<uint>(sessao, @"select idMovEstoque from mov_estoque where idProd=?idProd
                    and idLoja=?idLoja and idMovEstoque<>?id order by dataMov asc, idMovEstoque asc limit 1",
                    new GDAParameter("?id", idMovEstoque), new GDAParameter("?idProd", idProd), 
                    new GDAParameter("?idLoja", idLoja));

                if (idMovEstoque > 0)
                    AtualizaSaldoTotal(sessao, idMovEstoque);
            }
        }

        /// <summary>
        /// Atualiza a tabela produto_loja com o saldo do estoque
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        private void AtualizaProdutoLoja(uint idProd, uint idLoja)
        {
            AtualizaProdutoLoja(null, idProd, idLoja);
        }

        /// <summary>
        /// Atualiza a tabela produto_loja com o saldo do estoque
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        private void AtualizaProdutoLoja(GDASession sessao, uint idProd, uint idLoja)
        {
            ProdutoLojaDAO.Instance.NewProd(sessao, (int)idProd, (int)idLoja);

            decimal saldoQtde = ObtemSaldoQtdeMov(sessao, null, idProd, idLoja, false);

            objPersistence.ExecuteCommand(sessao, "Update produto_loja Set qtdEstoque=?qtd Where idProd=" +
                idProd + " And idLoja=" + idLoja, new GDAParameter("?qtd", saldoQtde));
        }

        /// <summary>
        /// Atualiza o saldo a partir de uma movimentação.
        /// </summary>
        /// <param name="idMovEstoque"></param>
        public void AtualizaSaldo(uint idMovEstoque)
        {
            AtualizaSaldo(null, idMovEstoque);
        }

        /// <summary>
        /// Atualiza o saldo a partir de uma movimentação.
        /// </summary>
        /// <param name="idMovEstoque"></param>
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

            foreach (var produtoNotaFiscal in ProdutosNfDAO.Instance.GetByNf(idNf))
                MovEstoqueDAO.Instance.ValidarMovimentarEstoque(null, (int)produtoNotaFiscal.IdProd, (int)idLoja, DateTime.Now,
                    MovEstoque.TipoMovEnum.Saida,
                    (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(produtoNotaFiscal.IdProd, produtoNotaFiscal.TotM,
                        produtoNotaFiscal.Qtde, produtoNotaFiscal.Altura, produtoNotaFiscal.Largura, false, false), true);

            foreach (var movEstoque in ExecuteMultipleScalar<uint>(sessao, "select idMovEstoque from mov_estoque where idNf=" + idNf))
                DeleteByPrimaryKey(sessao, movEstoque);
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
                throw new Exception("Esta movimentação não foi gerada a partir de um lançamento manual, portanto, não é possível excluí-la.");

            LogCancelamentoDAO.Instance.LogMovEstoque(session, mov, null, true);

            // Zera a movimentação para recalcular o saldo
            objPersistence.ExecuteCommand(session, "update mov_estoque set qtdeMov=0, valorMov=0 where idMovEstoque=" + idMovEstoque);
            AtualizaSaldo(session, idMovEstoque);

            // Atualiza o saldo na tabela produto_loja
            AtualizaProdutoLoja(session, mov.IdProd, mov.IdLoja);

            return base.DeleteByPrimaryKey(session, idMovEstoque);
        }

        #endregion
    }
}
