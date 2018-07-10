using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Data.NFeUtils;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class MovEstoqueFiscalDAO : BaseDAO<MovEstoqueFiscal, MovEstoqueFiscalDAO>
    {
        //private MovEstoqueFiscalDAO() { }

        #region Recupera listagem

        private string Sql(uint idLoja, string codInterno, string descricao, string ncm, int? numeroNfe, string dataIni, string dataFim, int tipoMov, 
            int situacaoProd, uint idCfop, uint idGrupoProd, uint idSubgrupoProd, uint idCorVidro, uint idCorFerragem,
            uint idCorAluminio, bool apenasLancManual, bool selecionar)
        {
            var exibirPedido = Configuracoes.EstoqueConfig.ExibirPedidosEstoqueFiscal;

            var criterio = String.Empty;
            var campos = selecionar ? @"me.*, p.Descricao as DescrProduto, p.Ncm, g.Descricao as DescrGrupo, sg.Descricao as DescrSubgrupo, 
                u.codigo as codUnidade, f.nome as nomeFunc, Coalesce(fnf.nomeFantasia, fnf.razaoSocial, '') As nomeFornec" + 
                (exibirPedido ? ", (Select group_concat(idpedido) From pedidos_nota_fiscal Where idNf=me.idNf) as IdsPedido" : String.Empty) +
                ", '$$$' as criterio" : "Count(*)";

            var sql = @"
                Select distinct " + campos + @" 
                From mov_estoque_fiscal me
                    Left Join produto p On (me.idProd=p.idProd) 
                    Left join grupo_prod g on (p.idGrupoProd=g.idGrupoProd) 
                    Left join subgrupo_prod sg on (p.idSubgrupoProd=sg.idSubgrupoProd) 
                    Left join unidade_medida u On (p.idUnidadeMedida=u.idUnidadeMedida)
                    Left join loja l on (me.idLoja=l.idLoja)
                    Left Join funcionario f On (me.idFunc=f.idFunc)
                    Left Join produtos_nf pnf On (me.idProdNf=pnf.idProdNf)
                    Left Join nota_fiscal nf On (pnf.idNf=nf.idNf)
                    Left Join fornecedor fnf On (nf.idFornec=fnf.idFornec)
                    LEFT JOIN
                    (
                        SELECT * FROM produto_ncm
                    ) AS ncm ON (me.IdLoja = ncm.IdLoja AND p.IdProd = ncm.IdProd)
                Where 1";

            // Retorna movimentação apenas se a loja e o produto tiverem sido informados
            if (idLoja == 0 || (string.IsNullOrEmpty(codInterno) && numeroNfe.GetValueOrDefault() == 0))
                return sql + " And false";

            if (idLoja > 0)
            {
                sql += " And me.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (!String.IsNullOrEmpty(codInterno) || !String.IsNullOrEmpty(descricao))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(codInterno, descricao);
                sql += " And me.idProd In (" + ids + ")";

                if (!String.IsNullOrEmpty(descricao))
                    criterio += "Produto: " + descricao + "    ";
                else
                    criterio += "Produto: " + ProdutoDAO.Instance.GetDescrProduto(codInterno) + "    ";
            }

            if (!string.IsNullOrWhiteSpace(ncm))
            {
                sql += string.Format(" AND (p.Ncm=?ncm OR ncm.Ncm=?ncm)");
                criterio += string.Format("NCM: {0}    ", ncm);
            }

            if (numeroNfe > 0)
            {
                sql += string.Format(" AND nf.NumeroNfe={0}", numeroNfe.Value);
                criterio += string.Format("Nota Fiscal: {0}    ", numeroNfe.Value);
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
                sql += " And tipoMov=" + tipoMov;
                criterio += "Apenas movimentações de " + (tipoMov == (int)MovEstoque.TipoMovEnum.Entrada ? "entrada" : "saída");
            }

            if (situacaoProd > 0)
            {
                sql += " and p.situacao=" + situacaoProd;
                criterio += "Situação: " + (situacaoProd == 1 ? "Ativos" : "Inativos") + "    ";
            }

            if (idCfop > 0)
            {
                sql += " and pnf.idNaturezaOperacao in (select idNaturezaOperacao from natureza_operacao where idCfop=" + idCfop + ")";
                criterio += "CFOP: " + CfopDAO.Instance.ObtemCodInterno(idCfop) + "    ";
            }

            if (idGrupoProd > 0)
            {
                sql += " and p.idGrupoProd=" + idGrupoProd;
                criterio += "Grupo: " + GrupoProdDAO.Instance.GetDescricao((int)idGrupoProd) + "    ";
            }

            if (idSubgrupoProd > 0)
            {
                sql += " and p.idSubgrupoProd=" + idSubgrupoProd;
                criterio += "Subgrupo: " + SubgrupoProdDAO.Instance.GetDescricao((int)idSubgrupoProd) + "    ";
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
                sql += " Order By me.dataMov asc, me.idMovEstoqueFiscal Asc";

            return sql.Replace("$$$", criterio);
        }

        public IList<MovEstoqueFiscal> GetForRpt(uint idLoja, string codInterno, string descricao, string ncm, int? numeroNfe, string dataIni, string dataFim, int tipoMov,
            int situacaoProd, uint idCfop, uint idGrupoProd, uint idSubgrupoProd, uint idCorVidro, uint idCorFerragem, uint idCorAluminio, bool naoBuscarEstoqueZero, bool apenasLancManual)
        {
            return objPersistence.LoadData(Sql(idLoja, codInterno, descricao, ncm, numeroNfe, dataIni, dataFim, tipoMov, situacaoProd,
                idCfop, idGrupoProd, idSubgrupoProd, idCorVidro, idCorFerragem, idCorAluminio, apenasLancManual, true), GetParam(dataIni, dataFim, ncm)).ToList();
        }

        public IList<MovEstoqueFiscal> GetList(uint idLoja, string codInterno, string descricao, string ncm, int? numeroNfe, string dataIni, string dataFim, int tipoMov,
            int situacaoProd, uint idCfop, uint idGrupoProd, uint idSubgrupoProd, uint idCorVidro, uint idCorFerragem, uint idCorAluminio, bool apenasLancManual)
        {
            return objPersistence.LoadData(Sql(idLoja, codInterno, descricao, ncm, numeroNfe, dataIni, dataFim, tipoMov, situacaoProd,
                idCfop, idGrupoProd, idSubgrupoProd, idCorVidro, idCorFerragem, idCorAluminio, apenasLancManual, true), GetParam(dataIni, dataFim, ncm)).ToList();
        }

        private GDAParameter[] GetParam(string dataIni, string dataFim, string ncm)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!string.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            if (!string.IsNullOrEmpty(ncm))
                lstParam.Add(new GDAParameter("?ncm", ncm));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Obtém o saldo do produto

        private uint? ObtemPrimeiroIdMovEstoqueFiscal(uint idProd, uint idLoja)
        {
            return ExecuteScalar<uint?>(@"select idMovEstoqueFiscal from mov_estoque_fiscal where idProd=?idProd
                and idLoja=?idLoja order by dataMov desc, idMovEstoqueFiscal asc limit 1",
                new GDAParameter("?idProd", idProd), new GDAParameter("?idLoja", idLoja));
        }

        private uint? ObtemUltimoIdMovEstoqueFiscal(GDASession sessao, uint idProd, uint idLoja)
        {
            return ExecuteScalar<uint?>(sessao, @"select idMovEstoqueFiscal from mov_estoque_fiscal where idProd=?idProd
                and idLoja=?idLoja order by dataMov desc, idMovEstoqueFiscal desc limit 1",
                new GDAParameter("?idProd", idProd), new GDAParameter("?idLoja", idLoja));
        }

        /// <summary>
        /// Obtem o id da movimentação anterior.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <param name="dataMov"></param>
        /// <returns></returns>
        private uint? ObtemIdMovAnterior(GDASession sessao, uint? idMovEstoqueFiscal, uint idProd, uint idLoja, DateTime dataMov)
        {
            // Remove os milisegundos da hora da movimentação
            dataMov = dataMov.AddMilliseconds(-dataMov.Millisecond);
            
            if (idMovEstoqueFiscal.GetValueOrDefault() == 0)
            {
                // Adiciona 1 segundo na datamov, para pegar a movimentação correta (Chamado 12177)
                dataMov = dataMov.AddSeconds(1);

                idMovEstoqueFiscal = ExecuteScalar<uint>(sessao, @"select idMovEstoqueFiscal from mov_estoque_fiscal
                    where idProd=?idProd and idLoja=?idLoja and dataMov<=?data order by dataMov desc, 
                    idMovEstoqueFiscal desc limit 1", new GDAParameter("?idProd", idProd), 
                    new GDAParameter("?idLoja", idLoja), new GDAParameter("?data", dataMov));
            }

            return ExecuteScalar<uint?>(sessao, "select idMovEstoqueFiscal from mov_estoque_fiscal me where idProd=" + idProd + 
                " and idLoja=" + idLoja + @" and (dataMov<?data or (dataMov=?data and idMovEstoqueFiscal<" + idMovEstoqueFiscal + @"))
                order by dataMov desc, idMovEstoqueFiscal desc limit 1", new GDAParameter("?data", dataMov));
        }

        /// <summary>
        /// Obtém o saldo em estoque de determinado produto em determinada loja
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal ObtemSaldoQtdeMov(uint? idMovEstoqueFiscal, uint idProd, uint idLoja, bool anterior)
        {
            return ObtemSaldoQtdeMov(null, idMovEstoqueFiscal, idProd, idLoja, DateTime.Now, anterior);
        }

        /// <summary>
        /// Obtém o saldo em estoque de determinado produto em determinada loja em determinado dia
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <param name="dataMov"></param>
        /// <returns></returns>
        public decimal ObtemSaldoQtdeMov(GDASession sessao, uint? idMovEstoqueFiscal, uint idProd, uint idLoja, DateTime dataMov, bool anterior)
        {
            if (anterior)
                idMovEstoqueFiscal = ObtemIdMovAnterior(sessao, idMovEstoqueFiscal, idProd, idLoja, dataMov);
            else if (idMovEstoqueFiscal.GetValueOrDefault() == 0)
                idMovEstoqueFiscal = ObtemUltimoIdMovEstoqueFiscal(sessao, idProd, idLoja);

            return ObtemValorCampo<decimal>(sessao, "saldoQtdeMov", "idMovEstoqueFiscal=" + idMovEstoqueFiscal.GetValueOrDefault());
        }

        /// <summary>
        /// Obtém o valor total em estoque de determinado produto em determinada loja
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal ObtemSaldoValorMov(GDASession sessao, uint? idMovEstoqueFiscal, uint idProd, uint idLoja, bool anterior)
        {
            return ObtemSaldoValorMov(sessao, idMovEstoqueFiscal, idProd, idLoja, DateTime.Now, anterior);
        }

        /// <summary>
        /// Obtém o valor total em estoque de determinado produto em determinada loja em determinado dia
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal ObtemSaldoValorMov(GDASession sessao, uint? idMovEstoqueFiscal, uint idProd, uint idLoja, DateTime dataMov, bool anterior)
        {
            if (anterior)
                idMovEstoqueFiscal = ObtemIdMovAnterior(sessao, idMovEstoqueFiscal, idProd, idLoja, dataMov);
            else if (idMovEstoqueFiscal.GetValueOrDefault() == 0)
                idMovEstoqueFiscal = ObtemUltimoIdMovEstoqueFiscal(sessao, idProd, idLoja);

            return ObtemValorCampo<decimal>(sessao, "saldoValorMov", "idMovEstoqueFiscal=" + idMovEstoqueFiscal.GetValueOrDefault());
        }

        #endregion

        #region Baixa Estoque

        public void BaixaEstoqueNotaFiscal(uint idProd, uint idLoja, uint idNaturezaOperacao, uint idNf, uint idProdNf, decimal qtdeBaixa, bool produtoSobra)
        {
            BaixaEstoqueNotaFiscal(null, idProd, idLoja, idNaturezaOperacao, idNf, idProdNf, qtdeBaixa, produtoSobra);
        }

        public void BaixaEstoqueNotaFiscal(GDASession sessao, uint idProd, uint idLoja, uint idNaturezaOperacao, uint idNf, uint idProdNf, decimal qtdeBaixa, bool produtoSobra)
        {
            var itens = ProdutoNfItemProjetoDAO.Instance.GetByIdProdNf(idProdNf);

            if (produtoSobra)
                itens = null;

            bool isNotaDeSaida = NotaFiscalDAO.Instance.GetTipoDocumento(sessao, idNf) == (int)NotaFiscal.TipoDoc.Saída;

            if (itens == null || itens.Count == 0)
            {
                // Gera sobra na produção
                if (isNotaDeSaida)
                    NotaFiscalDAO.Instance.BaixaCreditaSobraProducao(sessao, false, idNf, idLoja, idProdNf, idProd, idNaturezaOperacao, ref qtdeBaixa);

                MovimentaEstoqueFiscal(sessao, idProd, idLoja, idNaturezaOperacao, MovEstoque.TipoMovEnum.Saida, idNf, idProdNf, false, qtdeBaixa,
                    !produtoSobra ? Instance.GetTotalProdNF(sessao, idProdNf) : 0, DateTime.Now, null, false);
            }
            else
            {
                foreach (ProdutoNfItemProjeto item in itens)
                    foreach (MaterialItemProjeto m in MaterialItemProjetoDAO.Instance.GetByItemProjeto(sessao, item.IdItemProjeto))
                    {
                        decimal qtdMaterial = (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(m.IdProd, m.TotM, m.Qtde, m.Altura, m.Largura, true, true);

                        // Gera sobra na produção
                        if (isNotaDeSaida)
                            NotaFiscalDAO.Instance.BaixaCreditaSobraProducao(sessao, false, idNf, idLoja, idProdNf, m.IdProd, idNaturezaOperacao, ref qtdMaterial);

                        MovimentaEstoqueFiscal(sessao, m.IdProd, idLoja, idNaturezaOperacao, MovEstoque.TipoMovEnum.Saida, idNf, idProdNf, false, qtdMaterial,
                            m.Total + m.ValorBenef, DateTime.Now, null, false);
                    }
            }
        }

        public void BaixaEstoqueManual(uint idProd, uint idLoja, decimal qtdeBaixa, decimal? valor, DateTime dataMov, string obs)
        {
            MovimentaEstoqueFiscal(null, idProd, idLoja, null, MovEstoque.TipoMovEnum.Saida, null, null, true, qtdeBaixa,
                valor.GetValueOrDefault(MovEstoqueDAO.Instance.GetTotalEstoqueManual(idProd, qtdeBaixa)), dataMov, obs, false);
        }

        public void BaixaEstoqueManual(GDASession sessao, uint idProd, uint idLoja, decimal qtdeBaixa, decimal? valor, DateTime dataMov, string obs)
        {
            MovimentaEstoqueFiscal(sessao, idProd, idLoja, null, MovEstoque.TipoMovEnum.Saida, null, null, true, qtdeBaixa,
                valor.GetValueOrDefault(MovEstoqueDAO.Instance.GetTotalEstoqueManual(idProd, qtdeBaixa)), dataMov, obs, false);
        }

        #endregion

        #region Credita Estoque

        public void CreditaEstoqueNotaFiscal(uint idProd, uint idLoja, uint idNaturezaOperacao, uint idNf, uint idProdNf, decimal qtdeEntrada,
            bool produtoSobra, bool estorno)
        {
            CreditaEstoqueNotaFiscal(null, idProd, idLoja, idNaturezaOperacao, idNf, idProdNf, qtdeEntrada,
                produtoSobra, estorno);
        }

        public void CreditaEstoqueNotaFiscal(GDASession sessao, uint idProd, uint idLoja, uint idNaturezaOperacao, uint idNf, uint idProdNf, decimal qtdeEntrada,
            bool produtoSobra, bool estorno)
        {
            DateTime dataMov = DateTime.Now;
            var tipoDoc = (NotaFiscal.TipoDoc)NotaFiscalDAO.Instance.GetTipoDocumento(sessao, idNf);

            if (tipoDoc == NotaFiscal.TipoDoc.NotaCliente || tipoDoc == NotaFiscal.TipoDoc.EntradaTerceiros)
                dataMov = NotaFiscalDAO.Instance.ObtemValorCampo<DateTime?>(sessao, "dataSaidaEnt", "idNf=" + idNf).GetValueOrDefault(dataMov);
 
            var itens = ProdutoNfItemProjetoDAO.Instance.GetByIdProdNf(sessao, idProdNf);

            if (produtoSobra)
                itens = null;

            bool isNotaDeSaida = tipoDoc == NotaFiscal.TipoDoc.Saída;

            ProdutoLojaDAO.Instance.NewProd(sessao, (int)idProd, (int)idLoja);
 
            if (itens == null || itens.Count == 0)
            {
                // Gera sobra na produção
                if (isNotaDeSaida)
                    NotaFiscalDAO.Instance.BaixaCreditaSobraProducao(sessao, false, idNf, idLoja, idProdNf, idProd, idNaturezaOperacao, ref qtdeEntrada);

                MovimentaEstoqueFiscal(sessao, idProd, idLoja, idNaturezaOperacao, MovEstoque.TipoMovEnum.Entrada, idNf, idProdNf, false, qtdeEntrada,
                    !produtoSobra ? MovEstoqueFiscalDAO.Instance.GetTotalProdNF(sessao, idProdNf) : 0, dataMov, null, estorno);
            }
            else
            {
                foreach (ProdutoNfItemProjeto item in itens)
                    foreach (MaterialItemProjeto m in MaterialItemProjetoDAO.Instance.GetByItemProjeto(sessao, item.IdItemProjeto))
                    {
                        decimal qtdMaterial = (decimal)ProdutosNfDAO.Instance.ObtemQtdDanfe(sessao, m.IdProd, m.TotM, m.Qtde, m.Altura, m.Largura, true, true);

                        // Gera sobra na produção
                        if (isNotaDeSaida)
                            NotaFiscalDAO.Instance.BaixaCreditaSobraProducao(sessao, false, idNf, idLoja, idProdNf, m.IdProd, idNaturezaOperacao, ref qtdMaterial);

                        MovimentaEstoqueFiscal(sessao, m.IdProd, idLoja, idNaturezaOperacao, MovEstoque.TipoMovEnum.Entrada, idNf, idProdNf, false, qtdMaterial,
                            m.Total + m.ValorBenef, dataMov, null, false);
                    }
            }
        }

        public void CreditaEstoqueManual(uint idProd, uint idLoja, decimal qtdeEntrada, decimal? valor, DateTime dataMov, string obs)
        {
            MovimentaEstoqueFiscal(null, idProd, idLoja, null, MovEstoque.TipoMovEnum.Entrada, null, null, true, qtdeEntrada,
                valor.GetValueOrDefault(MovEstoqueDAO.Instance.GetTotalEstoqueManual(idProd, qtdeEntrada)), dataMov, obs, false);
        }

        public void CreditaEstoqueManual(GDASession sessao, uint idProd, uint idLoja, decimal qtdeEntrada, decimal? valor, DateTime dataMov, string obs)
        {
            MovimentaEstoqueFiscal(sessao, idProd, idLoja, null, MovEstoque.TipoMovEnum.Entrada, null, null, true, qtdeEntrada,
                valor.GetValueOrDefault(MovEstoqueDAO.Instance.GetTotalEstoqueManual(idProd, qtdeEntrada)), dataMov, obs, false);
        }

        #endregion

        #region Verifica se há uma movimentação posterior

        /// <summary>
        /// Verifica se há uma movimentação posterior.
        /// </summary>
        /// <param name="idMovEstoqueFiscal"></param>
        /// <returns></returns>
        public bool TemMovimentacaoPosterior(uint idMovEstoqueFiscal)
        {
            DateTime dataMov = ObtemValorCampo<DateTime>("dataMov", "idMovEstoqueFiscal=" + idMovEstoqueFiscal);
            string sql = @"select count(*) from mov_estoque_fiscal where dataMov>?data
                or (dataMov=?data and idMovEstoqueFiscal>" + idMovEstoqueFiscal + ")";

            return objPersistence.ExecuteSqlQueryCount(sql, new GDAParameter("?data", dataMov)) > 0;
        }

        #endregion

        #region Verifica se o estoque deve ser alterado

        /// <summary>
        /// Verifica se o estoque deve ser alterado
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idCfop"></param>
        /// <returns></returns>
        public bool AlteraEstoqueFiscal(uint idProd, uint? idNaturezaOperacao)
        {
            var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd((int)idProd);
            var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd((int)idProd);

            // Altera o estoque somente se estiver marcado para alterar no cadastro de subgrupo, no cadastro de CFOP e 
            // se o tipo de ambiente da NFe estiver em produção
            if (Glass.Data.DAL.GrupoProdDAO.Instance.NaoAlterarEstoqueFiscal(idGrupoProd, idSubgrupoProd) || ConfigNFe.TipoAmbiente == ConfigNFe.TipoAmbienteNfe.Homologacao ||
                (idNaturezaOperacao != null && !NaturezaOperacaoDAO.Instance.AlterarEstoqueFiscal(idNaturezaOperacao.Value)))
                return false;

            return true;
        }

        #endregion

        #region Movimenta Estoque

        /// <summary>
        /// Dá baixa no estoque no produto da loja passados
        /// </summary>
        /// <param name="idProd">Código do Produto.</param>
        /// <param name="idLoja">Código da Loja.</param>
        /// <param name="idCfop">Código do CFOP.</param>
        /// <param name="tipoMov">Tipo da movimentação.
        /// 1 - Entrada
        /// 2 - Saída</param>
        /// <param name="idNf">Código da nota fiscal.</param>
        /// <param name="idProdNf">Código do produto da nota fiscal.</param>
        /// <param name="lancManual">Informa se a movimentação é manual ou não.</param>
        /// <param name="qtdeMov">Quantidade do produto a ser baixada/creditada no estoque.</param>
        /// <param name="total">Total, em reias, da movimentação.</param>
        /// <param name="dataMov">Data da movimentação do estoque.</param>
        /// <param name="estorno">Informa se é ou não estorno de alguma movimentação lançada anteriormente.</param>
        /// <returns>Número de registros afetados</returns>
        private void MovimentaEstoqueFiscal(uint idProd, uint idLoja, uint? idNaturezaOperacao, MovEstoque.TipoMovEnum tipoMov, uint? idNf,
            uint? idProdNf, bool lancManual, decimal qtdeMov, decimal total, DateTime dataMov, bool estorno)
        {
            MovimentaEstoqueFiscal(null, idProd, idLoja, idNaturezaOperacao, tipoMov, idNf, idProdNf, lancManual, qtdeMov, total, dataMov, null, estorno);
        }

        /// <summary>
        /// Dá baixa no estoque no produto da loja passados
        /// </summary>
        private void MovimentaEstoqueFiscal(GDASession sessao, uint idProd, uint idLoja, uint? idNaturezaOperacao, MovEstoque.TipoMovEnum tipoMov, uint? idNf,
            uint? idProdNf, bool lancManual, decimal qtdeMov, decimal total, DateTime dataMov, string obs, bool estorno)
        {
            var usarValorProdNf = Configuracoes.EstoqueConfig.ConsiderarTotalProdNfMovEstoqueFiscal;
            
            try
            {
                ProdutoBaixaEstoqueFiscal[] prodBaixEstFisc;

                if (!lancManual)
                    prodBaixEstFisc = ProdutoBaixaEstoqueFiscalDAO.Instance.GetByProd(idProd);
                else
                    prodBaixEstFisc = new ProdutoBaixaEstoqueFiscal[]
                    {
                        new ProdutoBaixaEstoqueFiscal()
                        {
                            IdProd = (int)idProd,
                            IdProdBaixa = (int)idProd,
                            Qtde = 1
                        }
                    };

                if (AlteraEstoqueFiscal(idProd, idNaturezaOperacao) || lancManual)
                {
                    foreach (var pbef in prodBaixEstFisc)
                    {
                        var qtde = qtdeMov * (decimal)pbef.Qtde;
                        var totalMov = total * (decimal)pbef.Qtde;

                        if (pbef.IdProdBaixa != idProd)
                        {
                            if (estorno || usarValorProdNf)
                            {
                                /* Chamado 38441. */
                                if (LojaDAO.Instance.ObtemCalculaIcmsStPedido(sessao, idLoja) && idProdNf > 0)
                                {
                                    var valorIcms = ProdutosNfDAO.Instance.ObterValorIcms(sessao, (int)idProdNf.GetValueOrDefault());
                                    totalMov -= valorIcms;
                                }
                            }
                            else
                                totalMov = MovEstoqueDAO.Instance.GetTotalEstoqueManual(sessao, (uint)pbef.IdProdBaixa, qtde);
                        }
                        
                        // Recupera os dados da movimentação anterior
                        decimal saldoQtdeAnterior = ObtemSaldoQtdeMov(sessao, null, (uint)pbef.IdProdBaixa, idLoja, dataMov, true);
                        decimal saldoValorAnterior = ObtemSaldoValorMov(sessao, null, (uint)pbef.IdProdBaixa, idLoja, dataMov, true);

                        /* Chamado 16616.
                         * O saldo do ficou negativo após a emissão de uma nota fiscal.
                         * Esta verificação irá impedir que isto ocorra novamente. */
                        // Verifica se, ao registrar a movimentação, o saldo em estoque do produto ficará negativo.
                        if ((saldoQtdeAnterior - qtde) < 0 && tipoMov == MovEstoque.TipoMovEnum.Saida)
                        {
                            var validarEstoqueDisponivel = true;

                            /* Chamado 53204. */
                            if (idNf > 0)
                                validarEstoqueDisponivel = NotaFiscalDAO.Instance.GetTipoDocumento(sessao, idNf.Value) == (int)NotaFiscal.TipoDoc.EntradaTerceiros;

                            if (validarEstoqueDisponivel)
                            {
                                var idGrupoProdBaixa = ProdutoDAO.Instance.ObtemIdGrupoProd(pbef.IdProdBaixa);
                                var idSubgrupoProdBaixa = ProdutoDAO.Instance.ObtemValorCampo<int?>("IdSubGrupoProd", "IdProd=" + pbef.IdProdBaixa);

                                // Verifica se o subgrupo ou o grupo do produto estão marcados para bloquear estoque.
                                if (GrupoProdDAO.Instance.BloquearEstoque((int)idGrupoProdBaixa, idSubgrupoProdBaixa))
                                    throw new Exception
                                    (
                                        "O grupo/subgrupo do produto está marcado para bloquear estoque, portanto, o estoque não pode ser negativo."
                                    );
                            }
                        }

                        // Registra a alteração do estoque
                        MovEstoqueFiscal movEstoque = new MovEstoqueFiscal();
                        movEstoque.IdProd = (uint)pbef.IdProdBaixa;
                        movEstoque.IdLoja = idLoja;
                        movEstoque.IdFunc = UserInfo.GetUserInfo.CodUser;
                        movEstoque.IdNf = idNf;
                        movEstoque.IdProdNf = idProdNf;
                        movEstoque.LancManual = lancManual;
                        movEstoque.TipoMov = (int)tipoMov;
                        movEstoque.DataMov = dataMov;
                        movEstoque.Obs = obs;
                        if (dataMov.Date != DateTime.Now.Date) movEstoque.DataCad = DateTime.Now;
                        movEstoque.QtdeMov = qtde;

                        movEstoque.SaldoQtdeMov = Math.Round(saldoQtdeAnterior + (tipoMov == MovEstoque.TipoMovEnum.Entrada ? qtde : -qtde), 2);

                        if (movEstoque.SaldoQtdeMov < 0)
                        {
                            movEstoque.ValorMov = 0;
                            movEstoque.SaldoValorMov = 0;
                        }
                        // Caso o tipo da nota fiscal seja "Saída" o tipo da movimentação será entrada somente no caso de um estorno,
                        // e o estorno da nota fiscal de saída deve calcular o valor e a quantidade da movimentação da mesma forma que é
                        // calculado na movimentação de saída da nota. Por isso, neste momento, deve ser verificado se a nota é de saída.
                        else if (tipoMov == MovEstoque.TipoMovEnum.Entrada &&
                            (idNf.GetValueOrDefault() > 0 ? NotaFiscalDAO.Instance.ObtemValorCampo<int>(sessao, "tipoDocumento", "IdNf=" + idNf.Value) !=
                            (int)NotaFiscal.TipoDoc.Saída : true))
                        {
                            decimal perc = qtde > movEstoque.SaldoQtdeMov ?
                                qtde / (movEstoque.SaldoQtdeMov > 0 ? movEstoque.SaldoQtdeMov : 1) : 1;

                            movEstoque.ValorMov = Math.Abs(totalMov);
                            movEstoque.SaldoValorMov = saldoValorAnterior + (movEstoque.ValorMov * perc);
                        }
                        else
                        {
                            decimal valorUnit = saldoValorAnterior / (saldoQtdeAnterior > 0 ? saldoQtdeAnterior : 1);
                            movEstoque.ValorMov = Math.Abs(valorUnit * qtde);
                            movEstoque.SaldoValorMov = saldoValorAnterior - (valorUnit * qtde);
                        }

                        movEstoque.IdMovEstoqueFiscal = Insert(sessao, movEstoque);

                        // Chamado 15184: Sempre atualiza o saldo, para resolver o erro de não recalcular o saldo
                        var idMovAnterior = ObtemIdMovAnterior(sessao, movEstoque.IdMovEstoqueFiscal, movEstoque.IdProd, movEstoque.IdLoja, movEstoque.DataMov);
                        if (idMovAnterior != null)
                            AtualizaSaldo(sessao, idMovAnterior.Value);
                        else
                            AtualizaSaldo(sessao, movEstoque.IdMovEstoqueFiscal);

                        // Atualiza a tabela produto_loja
                        AtualizaProdutoLoja(sessao, movEstoque.IdProd, movEstoque.IdLoja);
                    }
                }

                if (!lancManual &&
                    idNaturezaOperacao > 0 &&
                    CfopDAO.Instance.AlterarEstoqueTerceiros(sessao, NaturezaOperacaoDAO.Instance.ObtemIdCfop(sessao, idNaturezaOperacao.Value)))
                {
                    if (prodBaixEstFisc != null && prodBaixEstFisc.Length > 0 && prodBaixEstFisc[0].IdProdBaixa > 0)
                    {
                        foreach (var pbef in prodBaixEstFisc)
                            AtualizaProdutoLojaQtdeTerceiros(sessao, tipoMov, idLoja, (uint)pbef.IdProdBaixa, qtdeMov);
                    }
                    else
                        AtualizaProdutoLojaQtdeTerceiros(sessao, tipoMov, idLoja, idProd, qtdeMov);
                }
            }
            catch (Exception ex)
            {
                if (idNf > 0)
                { 
                    LogNfDAO.Instance.NewLog(idNf.Value, "Estoque", 2, MensagemAlerta.FormatErrorMsg("Falha ao movimentar estoque.", ex));
                    ErroDAO.Instance.InserirFromException("MovEstoqueFiscal", ex);
                }

                // Chamado 26887, não estava lançando exception
                if (lancManual)
                    throw ex;
            }
        }

        #endregion

        #region Atualiza o saldo de estoque

        private void AtualizaSaldoQtd(GDASession sessao, uint idMovEstoqueFiscal)
        {
            uint idProd = ObtemValorCampo<uint>(sessao, "idProd", "idMovEstoqueFiscal=" + idMovEstoqueFiscal);
            uint idLoja = ObtemValorCampo<uint>(sessao, "idLoja", "idMovEstoqueFiscal=" + idMovEstoqueFiscal);
            DateTime dataMov = ObtemValorCampo<DateTime>(sessao, "dataMov", "idMovEstoqueFiscal=" + idMovEstoqueFiscal);

            string sql = @"
                set @saldo := coalesce((select saldoQtdeMov from mov_estoque_fiscal
                    where (dataMov<?data or (dataMov=?data and idMovEstoqueFiscal<?id))
                    and idProd=?idProd and idLoja=?idLoja
                    order by dataMov desc, idMovEstoqueFiscal desc limit 1), 0);
                
                update mov_estoque_fiscal set saldoQtdeMov=(@saldo := @saldo + if(tipoMov=1, qtdeMov, -qtdeMov))
                where (dataMov>?data or (dataMov=?data and idMovEstoqueFiscal>=?id)) and idProd=?idProd and idLoja=?idLoja
                order by dataMov asc, idMovEstoqueFiscal asc";

            objPersistence.ExecuteCommand(sessao, sql, 
                new GDAParameter("?data", dataMov), new GDAParameter("?id", idMovEstoqueFiscal),
                new GDAParameter("?idProd", idProd), new GDAParameter("?idLoja", idLoja));
        }

        private void AtualizaSaldoTotal(GDASession sessao, uint idMovEstoqueFiscal)
        {
            uint idProd = ObtemValorCampo<uint>(sessao, "idProd", "idMovEstoqueFiscal=" + idMovEstoqueFiscal);
            uint idLoja = ObtemValorCampo<uint>(sessao, "idLoja", "idMovEstoqueFiscal=" + idMovEstoqueFiscal);
            DateTime dataMov = ObtemValorCampo<DateTime>(sessao, "dataMov", "idMovEstoqueFiscal=" + idMovEstoqueFiscal);

            uint idMovEstoqueFiscalAnt = ObtemIdMovAnterior(sessao, idMovEstoqueFiscal, idProd, idLoja, dataMov).GetValueOrDefault();

            string sql = @"
                /**
                 * Recupera algumas variáveis para uso durante o cálculo dos valores das movimentações:
                 * @saldo - o saldo de valor da movimentação anterior à movimentação que está sendo alterada
                 * @valorUnit - o valor unitário da movimentação anterior à movimentação que está sendo alterada
                 * @valorProd - o maior valor salvo para o produto (para normalização de valores, ver abaixo)
                 */
                set @saldo := coalesce((
                    select if(saldoQtdeMov<0, 0, Coalesce(saldoValorMov, 0))
                    from mov_estoque_fiscal where idMovEstoqueFiscal=?idAnt
                ), 0), 

                @valorUnit := coalesce((
                    select if(saldoQtdeMov<0, 0, abs(coalesce(saldoValorMov/if(saldoQtdeMov<>0, saldoQtdeMov, 1), 0)))
                    from mov_estoque_fiscal where idMovEstoqueFiscal=?idAnt
                ), 0),

                @valorProd := 0 /* Removido - erro no cálculo de produtos com valor muito baixo - coalesce((
                    select greatest(valorAtacado, valorBalcao, valorObra, custoCompra, custoFabBase)
                    from produto where idProd=?idProd
                ), 0) */;

                update mov_estoque_fiscal
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
                                
                                    ), 0)))>
                                    
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

                where (dataMov>?data or (dataMov=?data and idMovEstoqueFiscal>=?id)) and idProd=?idProd and idLoja=?idLoja
                order by dataMov asc, idMovEstoqueFiscal asc";

            objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?idAnt", idMovEstoqueFiscalAnt),
                new GDAParameter("?data", dataMov), new GDAParameter("?id", idMovEstoqueFiscal),
                new GDAParameter("?idProd", idProd), new GDAParameter("?idLoja", idLoja));

            if (idMovEstoqueFiscalAnt == 0)
            {
                idMovEstoqueFiscal = ExecuteScalar<uint>(sessao, @"select idMovEstoqueFiscal from mov_estoque_fiscal where idProd=?idProd
                    and idLoja=?idLoja and idMovEstoqueFiscal<>?id order by dataMov asc, idMovEstoqueFiscal asc limit 1",
                    new GDAParameter("?id", idMovEstoqueFiscal), new GDAParameter("?idProd", idProd), 
                    new GDAParameter("?idLoja", idLoja));

                if (idMovEstoqueFiscal > 0)
                    AtualizaSaldoTotal(sessao, idMovEstoqueFiscal);
            }
        }

        /// <summary>
        /// Atualiza a tabela produto_loja com o saldo do estoque fiscal
        /// </summary>
        /// <param name="idMovEstoqueFiscal"></param>
        private void AtualizaProdutoLoja(GDASession sessao, uint idProd, uint idLoja)
        {
            decimal saldoQtde = ObtemSaldoQtdeMov(sessao, null, idProd, idLoja, DateTime.Now, false);

            objPersistence.ExecuteCommand(sessao, "Update produto_loja Set estoqueFiscal=?qtd Where idProd=" +
                idProd + " And idLoja=" + idLoja, new GDAParameter("?qtd", saldoQtde));
        }

        /// <summary>
        /// Atualiza o campo Qntde Terceiro da tabela produto_loja
        /// </summary>
        /// <param name="tipoMov"></param>
        /// <param name="idLoja"></param>
        /// <param name="idProd"></param>
        /// <param name="qtdeMov"></param>
        private void AtualizaProdutoLojaQtdeTerceiros(GDASession sessao, MovEstoque.TipoMovEnum tipoMov, uint idLoja, uint idProd, decimal qtdeMov)
        {
            ProdutoLoja plAtual = ProdutoLojaDAO.Instance.GetElement(idLoja, idProd);

            if (plAtual == null)
                return;

            ProdutoLoja plNovo = ProdutoLojaDAO.Instance.GetElement(idLoja, idProd);

            if (tipoMov == MovEstoque.TipoMovEnum.Entrada)
                plNovo.QtdePosseTerceiros += Convert.ToDouble(qtdeMov);
            else
                plNovo.QtdePosseTerceiros -= Convert.ToDouble(qtdeMov);

            ProdutoLojaDAO.Instance.Update(sessao, plNovo);

            // Salva log da alteração no estoque de terceiros
            LogAlteracaoDAO.Instance.LogProdutoLoja(sessao, plAtual);
        }

        /// <summary>
        /// Atualiza o saldo a partir de uma movimentação.
        /// </summary>
        /// <param name="idMovEstoqueFiscal"></param>
        public void AtualizaSaldo(GDASession sessao, uint idMovEstoqueFiscal)
        {
            AtualizaSaldoQtd(sessao, idMovEstoqueFiscal);
            AtualizaSaldoTotal(sessao, idMovEstoqueFiscal);
        }

        /// <summary>
        /// Atualiza o saldo de um produto e loja.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idLoja"></param>
        public void AtualizaSaldo(uint idProd, uint idLoja)
        {
            var idMovEstoqueFiscal = ObtemPrimeiroIdMovEstoqueFiscal(idProd, idLoja);
            if (idMovEstoqueFiscal > 0)
                AtualizaSaldo(null, idMovEstoqueFiscal.Value);
        }

        #endregion

        #region Apaga as movimentações de NF-e

        /// <summary>
        /// Apaga as movimentações de NF-e.
        /// </summary>
        /// <param name="idNf"></param>
        internal void DeleteByNf(GDASession sessao, uint idNf)
        {
            foreach (var movEstoque in ExecuteMultipleScalar<uint>(sessao, "select idMovEstoqueFiscal from mov_estoque_fiscal where idNf=" + idNf))
                DeleteByPrimaryKey(sessao, movEstoque);
        }

        #endregion

        #region Métodos sobrescritos

        public override int Update(MovEstoqueFiscal objUpdate)
        {
            LogAlteracaoDAO.Instance.LogMovEstoqueFiscal(objUpdate);
            return base.Update(objUpdate);
        }

        public override int Delete(MovEstoqueFiscal objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdMovEstoqueFiscal);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            return DeleteByPrimaryKey(null, Key);
        }

        public override int DeleteByPrimaryKey(GDASession sessao, uint Key)
        {
            MovEstoqueFiscal mov = GetElementByPrimaryKey(sessao, Key);
            LogCancelamentoDAO.Instance.LogMovEstoqueFiscal(sessao, mov, null, true);

            // Zera a movimentação para recalcular o saldo
            objPersistence.ExecuteCommand(sessao, "update mov_estoque_fiscal set qtdeMov=0, valorMov=0 where idMovEstoqueFiscal=" + Key);
            AtualizaSaldo(sessao, Key);

            // Atualiza o saldo na tabela produto_loja
            AtualizaProdutoLoja(sessao, mov.IdProd, mov.IdLoja);

            return base.DeleteByPrimaryKey(sessao, Key);
        }

        #endregion

        #region Recupera o valor total das movimentações

        internal decimal GetTotalProdNF(GDASession sessao, uint idProdNf)
        {
            var total = ProdutosNfDAO.Instance.ObtemValorCampo<decimal>(sessao, "total", "idProdNf=" + idProdNf);

            if (FiscalConfig.SomarImpostosValorUnMovFiscal || FiscalConfig.SubtrairImpostosValorUnMovFiscal)
            {
                var prodNf = ProdutosNfDAO.Instance.GetElement(sessao, idProdNf);

                // Soma o valor do IPI, ICMSST, Despesas e subtraí o desconto.
                if (FiscalConfig.SomarImpostosValorUnMovFiscal)
                {
                    ProdutosNfDAO.Instance.CalcDespesas(sessao, ref prodNf);
                    total += prodNf.ValorIpi + prodNf.ValorIcmsSt + prodNf.ValorFcpSt + prodNf.ValorFrete + prodNf.ValorSeguro + prodNf.ValorOutrasDespesas - prodNf.ValorDesconto;
                }

                // Subtraí o PIS e COFINS.
                if (FiscalConfig.SubtrairImpostosValorUnMovFiscal)
                    total -= prodNf.ValorPis + prodNf.ValorCofins;
            }

            return total;
        }

        #endregion
    }
}
