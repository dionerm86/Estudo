using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de produtos.
    /// </summary>
    public class ProdutoFluxo : IProdutoFluxo, 
        Entidades.IProdutoBeneficiamentosRepositorio,
        Entidades.IValidadorProduto,
        Entidades.IValidadorCorAluminio,
        Entidades.IValidadorCorFerragem,
        Entidades.IValidadorCorVidro
    {
        #region Genero Produto

        /// <summary>
        /// Recuper aos generos de produto.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.GeneroProduto> ObtemGenerosProduto()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.GeneroProduto>("gp")
                .OrderBy("gp.Codigo")
                .ProcessResult<Entidades.GeneroProduto>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados do genero do produto.
        /// </summary>
        /// <param name="idGeneroProduto"></param>
        /// <returns></returns>
        public Entidades.GeneroProduto ObtemGeneroProduto(int idGeneroProduto)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.GeneroProduto>("gp")
                .Where("gp.IdGeneroProduto=?id")
                .Add("?id", idGeneroProduto)
                .ProcessResult<Entidades.GeneroProduto>()
                .FirstOrDefault();
        }

        #endregion

        #region Produto

        /// <summary>
        /// Cria a instancia de um novo produto.
        /// </summary>
        /// <returns></returns>
        public Entidades.Produto CriarProduto()
        {
            return SourceContext.Instance.Create<Entidades.Produto>();
        }

        /// <summary>
        /// Verifica se o grupo de produtos calcula beneficiamento.
        /// </summary>
        /// <param name="idGrupoProd"></param>
        /// <returns></returns>
        public bool CalculaBeneficiamento(int idGrupoProd)
        {
            if (!Glass.Configuracoes.Geral.UsarBeneficiamentosTodosOsGrupos)
                return (int)Glass.Data.Model.NomeGrupoProd.Vidro == idGrupoProd;
            
            else
                return true;
        }

        /// <summary>
        /// Pesquisa os produtos.
        /// </summary>
        public IList<Entidades.ProdutoPesquisa> PesquisarProdutos(
            string codInterno, string descricao, Glass.Situacao? situacao, int? idLoja, int? idFornec, string nomeFornecedor,
            string idGrupo, string idSubgrupo, TipoNegociacaoProduto? tipoNegociacao,
            bool apenasProdutosEstoqueBaixa, bool agruparEstoqueLoja, 
            decimal? alturaInicio, decimal? alturaFim, decimal? larguraInicio, decimal? larguraFim, string ordenacao)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Produto>("p")
                .InnerJoin<Data.Model.GrupoProd>("p.IdGrupoProd=gp.IdGrupoProd", "gp")
                .LeftJoin<Data.Model.SubgrupoProd>("p.IdSubgrupoProd=sg.IdSubgrupoProd", "sg")
                .LeftJoin<Data.Model.Funcionario>("p.UsuAlt=falt.IdFunc", "falt")
                .LeftJoin<Data.Model.Funcionario>("p.Usucad=fcad.IdFunc", "fcad")
                .Select(
                    @"p.IdProd, ISNULL(sg.TipoCalculo, gp.TipoCalculo) AS TipoCalculo, 
                      p.IdGrupoProd, p.IdSubgrupoProd, p.CodInterno, p.Descricao, 
                      CONCAT(gp.Descricao, ' ', ISNULL(sg.Descricao, '')) AS TipoProduto,
                      gp.Descricao AS Grupo, sg.Descricao AS Subgrupo,
                      p.Altura, p.Largura, p.Custofabbase, p.CustoCompra, p.ValorAtacado,
                      p.ValorBalcao, p.ValorObra, p.ValorReposicao, p.ValorMinimo,
                      pl.QtdeEstoque, pl.Reserva, pl.Liberacao,
                      fcad.Nome AS NomeUsuarioCad, p.DataCad, falt.Nome AS NomeUsuarioAlt, p.DataAlt,
                      ?qtdBenef AS QtdeBeneficiamentos")
                .Add("?qtdBenef", SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ProdutoBenef>("pb")
                    .Where("pb.IdProd = p.IdProd")
                    .Select("COUNT(*) AS Qtde"));

            // Cria uma subconsulta para recuperar os totais o ProdutoLoja
            var consultaTotais = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ProdutoLoja>("pl")
                .InnerJoin<Data.Model.Loja>("pl.IdLoja=l.IdLoja", "l")
                .Select(
                    @"IdProd, SUM(QtdEstoque) AS QtdeEstoque, SUM(Reserva) AS Reserva,
                      SUM(Liberacao) AS Liberacao, SUM(M2) AS M2,
                      SUM(EstMinimo) AS EstoqueMinimo")
                .Where("l.Situacao=" + (int)Glass.Situacao.Ativo);

            if (idLoja.HasValue && idLoja.Value > 0)
                consultaTotais.WhereClause
                    .And("IdLoja=?idLoja")
                    .Add("?idLoja", idLoja);

            if (agruparEstoqueLoja)
                consultaTotais.GroupBy("IdProd, IdLoja");
            else
                consultaTotais.GroupBy("IdProd");

            consulta.LeftJoin(consultaTotais, "pl.IdProd = p.IdProd", "pl");

            if (!string.IsNullOrEmpty(codInterno))
                consulta.WhereClause
                    .And("p.CodInterno LIKE ?codInterno")
                    .Add("?codInterno", string.Format("%{0}%", codInterno));

            if (!string.IsNullOrEmpty(descricao))
                consulta.WhereClause
                    .And("p.Descricao LIKE ?descricao")
                    .Add("?descricao", string.Format("%{0}%", descricao));

            if (situacao != null)
                consulta.WhereClause
                    .And("p.Situacao=?situacao")
                    .Add("?situacao", situacao)
                    .AddDescription(string.Format("Situação: {0}", situacao.Translate().Format()));

            if (idFornec.HasValue && idFornec.Value > 0)
                consulta.WhereClause
                    .And("p.IdFornec=?idFornec")
                    .Add("?idFornec", idFornec)
                    .AddDescription(string.Format("Fornecedor: {0} - {1}", idFornec, nomeFornecedor));

            else if (!string.IsNullOrEmpty(nomeFornecedor))
                consulta
                    .LeftJoin<Data.Model.Fornecedor>("p.IdFornec=f.IdFornec", "f")
                    .WhereClause
                    .And("(f.RazaoSocial LIKE ?nomeFornec or f.NomeFantasia LIKE ?nomeFornec)")
                    .Add("?nomeFornec", string.Format("%{0}%", nomeFornecedor))
                    .AddDescription(string.Format("Fornecedor: {0}", nomeFornecedor));


            if (!string.IsNullOrEmpty(idGrupo))
                consulta.WhereClause
                    .And(string.Format("p.IdGrupoProd IN ({0})", idGrupo))
                    .AddDescription(() =>
                        string.Format("Grupo: {0}",
                        string.Join(", ",
                            SourceContext.Instance.CreateQuery()
                                .From<Data.Model.GrupoProd>()
                                .Select<Data.Model.GrupoProd>(f => f.Descricao)
                                .Where(string.Format("IdGrupoProd IN ({0})", idGrupo))
                                .Execute()
                                .Select(f => f.GetString(0)).ToArray())));

            if (!string.IsNullOrEmpty(idSubgrupo))
                consulta.WhereClause
                    .And(string.Format("p.IdSubgrupoProd IN ({0})", idSubgrupo))
                    .AddDescription(() =>
                        string.Format("Subgrupo: {0}",
                        string.Join(", ",
                            SourceContext.Instance.CreateQuery()
                                .From<Data.Model.SubgrupoProd>()
                                .Select<Data.Model.SubgrupoProd>(f => f.Descricao)
                                .Where(string.Format("IdSubgrupoProd IN ({0})", idSubgrupo))
                                .Execute()
                                .Select(f => f.GetString(0)).ToArray())));

            if (apenasProdutosEstoqueBaixa)
            {
                consulta.WhereClause
                    .And("p.IdProd IN  (?baixaEstoque)")
                    .Add("?baixaEstoque",
                        SourceContext.Instance.CreateQuery()
                            .From<Data.Model.ProdutoBaixaEstoque>()
                            .Where("IdProdBaixa > 0")
                            .Select("IdProd")
                            .GroupBy("IdProd"))
                    .AddDescription("Apenas produtos que indicam produto para baixa");
            }

            if (alturaInicio > 0 || alturaFim > 0)
            {
                consulta.WhereClause
                    .And("p.Altura >= ?alturaInicio")
                    .Add("?alturaInicio", alturaInicio);

                if (alturaFim > 0)
                    consulta.WhereClause
                        .And("p.Altura <= ?alturaFim")
                        .Add("?alturaFim", alturaFim);

                consulta.WhereClause.AddDescription(string.Format("Altura: {0}  Até {1}", alturaInicio, alturaFim));
            }

            if (larguraInicio > 0 || larguraFim > 0)
            {
                consulta.WhereClause
                    .And("p.Largura >= ?larguraInicio")
                    .Add("?larguraInicio", larguraInicio);

                if (alturaFim > 0)
                    consulta.WhereClause
                        .And("p.Largura <= ?larguraFim")
                        .Add("?larguraFim", larguraFim);

                consulta.WhereClause.AddDescription(string.Format("Largura: {0}  Até {1}", alturaInicio, alturaFim));
            }

            if (tipoNegociacao.HasValue)
            {
                switch (tipoNegociacao.Value)
                {
                    case TipoNegociacaoProduto.Venda:
                        consulta.WhereClause
                            .And("p.Compra=?compra OR p.Compra IS NULL")
                            .Add("?compra", false)
                            .AddDescription("Produtos de venda");
                        break;
            
                    case TipoNegociacaoProduto.Compra:
            
                        consulta.WhereClause
                            .And("p.Compra=?compra")
                            .Add("?compra", true)
                            .AddDescription("Produtos de compra");
                        break;
                    default:
                        consulta.WhereClause.AddDescription("Produtos de compra e venda");
                        break;
                }
            }

            #region Ordenação

            switch (ordenacao)
            {
                case "CodInterno":
                    consulta.OrderBy("p.CodInterno ASC");
                    break;
                case "Descricao":
                    consulta.OrderBy("p.Descricao ASC");
                    break;
                default:
                    consulta.OrderBy("p.CodInterno ASC");
                    break;
            }

            #endregion


            return consulta.ToVirtualResultLazy<Entidades.ProdutoPesquisa>();
        }

        /// <summary>
        /// Pesquisa os produtos.
        /// </summary>
        /// <param name="codInterno">Código interno do produto.</param>
        /// <param name="descricao">Descrição.</param>
        /// <param name="situacao">Situação.</param>
        /// <param name="idFornec"></param>
        /// <param name="nomeFornecedor">Nome do fornecedor.</param>
        /// <param name="idGrupo">Identificador do grupo</param>
        /// <param name="idSubgrupo">Identificador do subgrupo.</param>
        /// <param name="tipoNegociacao">Tipo de negociação.</param>
        /// <param name="apenasProdutosEstoqueBaixa">Identifica se é para recuperar apenas produtos com baixa no estoque.</param>
        /// <param name="ordenacao">Ordenação inicial da lista.</param>
        /// <returns></returns>
        public IList<Entidades.ProdutoListagemItem> PesquisarListagemProdutos(
            string codInterno, string descricao, Glass.Situacao? situacao, int? idFornec,
            string nomeFornecedor, string idsGrupos, string idsSubgrupos, TipoNegociacaoProduto? tipoNegociacao,
            bool apenasProdutosEstoqueBaixa, string ordenacao)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Produto>("p")
                .InnerJoin<Data.Model.GrupoProd>("p.IdGrupoProd=gp.IdGrupoProd", "gp")
                .LeftJoin<Data.Model.SubgrupoProd>("p.IdSubgrupoProd=sg.IdSubgrupoProd", "sg")
                .Select(
                    @"p.IdProd, ISNULL(sg.TipoCalculo, gp.TipoCalculo) AS TipoCalculo, 
                      p.IdGrupoProd, p.IdSubgrupoProd, p.CodInterno, p.Descricao, 
                      p.Altura, p.Largura, p.Custofabbase, p.CustoCompra, p.ValorAtacado,
                      p.ValorBalcao, p.ValorObra, p.ValorReposicao, p.ValorMinimo")
                .OrderBy(string.IsNullOrEmpty(ordenacao) ? "CodInterno" : ordenacao);

            if (!string.IsNullOrEmpty(codInterno))
                consulta.WhereClause
                    .And("p.CodInterno LIKE ?codInterno")
                    .Add("?codInterno", string.Format("%{0}%", codInterno));

            if (!string.IsNullOrEmpty(descricao))
                consulta.WhereClause
                    .And("p.Descricao LIKE ?descricao")
                    .Add("?descricao", string.Format("%{0}%", descricao));

            if (situacao != null)
                consulta.WhereClause
                    .And("p.Situacao=?situacao")
                    .Add("?situacao", situacao)
                    .AddDescription(string.Format("Situação: {0}", situacao.Translate().Format()));

            if (idFornec.HasValue && idFornec.Value > 0)
                consulta.WhereClause
                    .And("p.IdFornec=?idFornec")
                    .Add("?idFornec", idFornec)
                    .AddDescription(string.Format("Fornecedor: {0} - {1}", idFornec, nomeFornecedor));

            else if (!string.IsNullOrEmpty(nomeFornecedor))
                consulta
                    .LeftJoin<Data.Model.Fornecedor>("p.IdFornec=f.IdFornec", "f")
                    .WhereClause
                    .And("(f.RazaoSocial LIKE ?nomeFornec or f.NomeFantasia LIKE ?nomeFornec)")
                    .Add("?nomeFornec", string.Format("%{0}%", nomeFornecedor))
                    .AddDescription(string.Format("Fornecedor: {0}", nomeFornecedor));


            if (!string.IsNullOrEmpty(idsGrupos))
                consulta.WhereClause
                    .And(string.Format("p.IdGrupoProd IN ({0})", idsGrupos))
                    .AddDescription(() =>
                        string.Format("Grupo: {0}",
                        string.Join(", ",
                            SourceContext.Instance.CreateQuery()
                                .From<Data.Model.GrupoProd>()
                                .Select<Data.Model.GrupoProd>(f => f.Descricao)
                                .Where(string.Format("IdGrupoProd IN ({0})", idsGrupos))
                                .Execute()
                                .Select(f => f.GetString(0)).ToArray())));

            if (!string.IsNullOrEmpty(idsSubgrupos))
                consulta.WhereClause
                    .And(string.Format("p.IdSubgrupoProd IN ({0})", idsSubgrupos))
                    .AddDescription(() =>
                        string.Format("Subgrupo: {0}",
                        string.Join(", ",
                            SourceContext.Instance.CreateQuery()
                                .From<Data.Model.SubgrupoProd>()
                                .Select<Data.Model.SubgrupoProd>(f => f.Descricao)
                                .Where(string.Format("IdSubgrupoProd IN ({0})", idsSubgrupos))
                                .Execute()
                                .Select(f => f.GetString(0)).ToArray())));

            if (apenasProdutosEstoqueBaixa)
            {
                consulta.WhereClause
                    .And("p.IdProd IN  (?baixaEstoque)")
                    .Add("?baixaEstoque",
                        SourceContext.Instance.CreateQuery()
                            .From<Data.Model.ProdutoBaixaEstoque>()
                            .Where("IdProdBaixa > 0")
                            .Select("IdProd")
                            .GroupBy("IdProd"))
                    .AddDescription("Apenas produtos que indicam produto para baixa");
            }

            if (tipoNegociacao.HasValue)
            {
                switch (tipoNegociacao.Value)
                {
                    case TipoNegociacaoProduto.Venda:
                        consulta.WhereClause
                            .And("p.Compra=?compra OR p.Compra IS NULL")
                            .Add("?compra", false)
                            .AddDescription("Produtos de venda");
                        break;

                    case TipoNegociacaoProduto.Compra:
                        consulta.WhereClause
                            .And("p.Compra=?compra")
                            .Add("?compra", true)
                            .AddDescription("Produtos de compra");
                        break;

                    default:
                        consulta.WhereClause.AddDescription("Produtos de compra e venda");
                        break;
                }
            }

            return consulta.ToVirtualResultLazy<Entidades.ProdutoListagemItem>();
        }

        /// <summary>
        /// Completa os dados das fichas dos produtos.
        /// </summary>
        /// <param name="produtos"></param>
        private void CompletaDados(IEnumerable<Entidades.FichaProduto> produtos)
        {
            var count = produtos.Count();
            var index = 0;
            var mvas = new List<Data.Model.MvaProdutoUf>();
            var icmss = new List<Data.Model.IcmsProdutoUf>();

            var consultas = SourceContext.Instance.CreateMultiQuery();

            while (count > 0)
            {
                var take = count < 100 ? count : 100;
                
                // Recupera o filtro dos identificadores do produto
                var identificadores = string.Format("IdProd IN ({0})",
                            string.Join(",",
                                produtos
                                    .Skip(index).Take(take)
                                    .Select(f => f.IdProd.ToString())
                                    .ToArray()));

                // Adiciona a consulta dos Mvas do produto
                consultas.Add<Data.Model.MvaProdutoUf>(
                    SourceContext.Instance.CreateQuery()
                        .From<Data.Model.MvaProdutoUf>()
                        .Where(identificadores), 
                          (sender, q, result) =>
                          {
                              mvas.AddRange(result);
                          });

                consultas.Add<Data.Model.IcmsProdutoUf>(
                    SourceContext.Instance.CreateQuery()
                        .From<Data.Model.IcmsProdutoUf>()
                        .Where(identificadores),
                        (sender, q, result) =>
                        {
                            icmss.AddRange(result);
                        });

                index += take;
                count -= take;
            }

            consultas.Execute();

            // Processa os produtos
            foreach (var prod in produtos)
            {
                // Recupera descrição do Mva do produto
                prod.Mva = string.Join(", ", mvas
                    .Where(f => f.IdProd == prod.IdProd)
                    .GroupBy(f => string.Format("{0}:{1}", f.MvaOriginal, f.MvaSimples))
                    .Select(f =>
                        {
                            var text = new StringBuilder();
                            var mva = f.First();

                            if (f.Count() == 1)
                            {
                                if (!string.IsNullOrEmpty(mva.UfOrigem))
                                    text.AppendFormat("UF Origem: {0} ", mva.UfOrigem);

                                if (!string.IsNullOrEmpty(mva.UfDestino))
                                    text.AppendFormat("UF Destino: {0} ", mva.UfDestino);
                            }

                            return text.AppendFormat("MVA Original: {0} MVA Simples: {1}", mva.MvaOriginal, mva.MvaSimples).ToString();
                            
                        })
                        .ToArray());

                // Recupera descrição da Aliquota ICMS
                prod.AliqICMS = string.Join(", ", icmss
                    .Where(f => f.IdProd == prod.IdProd)
                    .GroupBy(f => string.Format("{0}:{1}:{2}",
                                 f.AliquotaIntraestadual,
                                 f.AliquotaInterestadual,
                                 f.IdTipoCliente))
                    .Select(f =>
                        {
                            var text = new StringBuilder();
                            var icms = f.First();

                            if (f.Count() == 1)
                            {
                                if (!string.IsNullOrEmpty(icms.UfOrigem))
                                    text.AppendFormat("UF Origem: {0} ", icms.UfOrigem);

                                if (!string.IsNullOrEmpty(icms.UfDestino))
                                    text.AppendFormat("UF Destino: {0} ", icms.UfDestino);
                            }

                            return text.AppendFormat("Alíq. ICMS Intraestadual: {0} Alíq. ICMS Interestadual: {1}",
                                icms.AliquotaIntraestadual, icms.AliquotaInterestadual).ToString();

                        }).ToArray());
            }
        }

        /// <summary>
        /// Recupera os dados da ficha do produto.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <returns></returns>
        public Entidades.FichaProduto ObtemFichaProduto(int idProd)
        {
            var resultado = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Produto>("p")
                .LeftJoin<Data.Model.Fornecedor>("p.IdFornec = f.IdFornec", "f")
                .LeftJoin<Data.Model.GrupoProd>("p.IdGrupoProd = g.IdGrupoProd", "g")
                .LeftJoin<Data.Model.SubgrupoProd>("p.IdSubgrupoProd = sg.IdSubgrupoProd", "sg")
                .LeftJoin<Data.Model.GeneroProduto>("p.IdGeneroProduto = gp.IdGeneroProduto", "gp")
                .LeftJoin<Data.Model.PlanoContaContabil>("p.IdContaContabil = pcc.IdContaContabil", "pcc")
                .LeftJoin<Data.Model.CorVidro>("p.IdCorVidro = cv.IdCorVidro", "cv")
                .LeftJoin<Data.Model.UnidadeMedida>("p.IdUnidadeMedida = um.IdUnidadeMedida", "um")
                .LeftJoin<Data.Model.UnidadeMedida>("p.IdUnidadeMedidaTrib = umt.IdUnidadeMedida", "umt")
                .LeftJoin(
                    SourceContext.Instance.CreateQuery()
                        .From<Data.Model.ProdutoBaixaEstoqueFiscal>("pbef")
                        .InnerJoin<Data.Model.Produto>("pbef.IdProdBaixa = p2.IdProd", "p2")
                        .Select("pbef.IdProd, p2.CodInterno, p2.Descricao")
                        .GroupBy("pbef.IdProd"), "p.IdProd = pbef.IdProd", "pbef")
                .Select(
                    @"p.IdProd, p.CodInterno, p.Descricao, ISNULL(f.Nomefantasia, f.Razaosocial) AS NomeFornecedor,
                      p.Altura, p.Largura, pcc.Descricao AS PlanoContaContabil, g.Descricao AS Grupo, sg.Descricao AS Subgrupo,
                      p.Situacao, p.TipoMercadoria, p.AreaMinima, cv.Descricao AS Cor, p.Espessura,
                      p.Peso, p.Forma, p.ValorFiscal, p.ValorObra, p.ValorAtacado, p.ValorBalcao, 
                      p.ValorReposicao, p.ValorTransferencia,
                      p.Custofabbase, p.CustoCompra, p.AliqICMSST, p.AliqIPI, p.Cst, p.Csosn, p.CstIpi,
                      p.Ncm, p.GTINProduto, p.GTINUnidTrib, gp.IdGeneroProduto, gp.Descricao AS GeneroProduto,
                      um.Codigo AS Unidade, umt.Codigo AS UnidadeTrib, p.AtivarAreaMinima,
                      pbef.IdProd AS IdProdBaixaEstoqueFiscal,
                      pbef.CodInterno AS CodInternoBaixaEstoqueFiscal, 
                      pbef.Descricao AS DescricaoBaixaEstoqueFiscal,
                      ?pbeQtde AS QtdeBaixaEstoque")
                .Where("p.IdProd=?id")
                .Add("?pbeQtde", 
                    SourceContext.Instance.CreateQuery()
                        .From<Data.Model.ProdutoBaixaEstoque>()
                        .Count()
                        .Where("IdProd = p.IdProd"))
                .Add("?id", idProd)
                .Execute<Entidades.FichaProduto>()
                .ToList();

            CompletaDados(resultado);
            return resultado.FirstOrDefault();
        }

        /// <summary>
        /// Recupera as fichas dos produtos que se enquadram no filtro aplicado.
        /// </summary>
        /// <param name="idFornec">Identificador do fornecedor.</param>
        /// <param name="nomeFornec">Nome do fornecedor.</param>
        /// <param name="idGrupoProd">Identificador do grupo de produtos.</param>
        /// <param name="idSubgrupoProd">Identificador do subgrupo de produtos.</param>
        /// <param name="codInterno">Código interno do produto.</param>
        /// <param name="descricao">Descrção do produto</param>
        /// <param name="tipoNegociacao">Tipo de negociacao do produto.</param>
        /// <param name="situacao">Situação.</param>
        /// <param name="apenasProdutosEstoqueBaixa"></param>
        /// <param name="alturaInicio"></param>
        /// <param name="alturaFim"></param>
        /// <param name="larguraInicio"></param>
        /// <param name="larguraFim"></param>
        /// <param name="ordenacao"></param>
        /// <returns></returns>
        public IList<Entidades.FichaProduto> ObtemFichasProdutos(
            int? idFornec, string nomeFornec, int? idGrupoProd, int? idSubgrupoProd, string codInterno, string descricao,
            TipoNegociacaoProduto tipoNegociacao, Glass.Situacao? situacao, bool apenasProdutosEstoqueBaixa, 
            decimal alturaInicio, decimal alturaFim, decimal larguraInicio, decimal larguraFim, string ordenacao)
        {
            var consulta = SourceContext.Instance.CreateQuery()
               .From<Data.Model.Produto>("p")
               .LeftJoin<Data.Model.Fornecedor>("p.IdFornec = f.IdFornec", "f")
               .LeftJoin<Data.Model.GrupoProd>("p.IdGrupoProd = g.IdGrupoProd", "g")
               .LeftJoin<Data.Model.SubgrupoProd>("p.IdSubgrupoProd = sg.IdSubgrupoProd", "sg")
               .LeftJoin<Data.Model.GeneroProduto>("p.IdGeneroProduto = gp.IdGeneroProduto", "gp")
               .LeftJoin<Data.Model.PlanoContaContabil>("p.IdContaContabil = pcc.IdContaContabil", "pcc")
               .LeftJoin<Data.Model.CorVidro>("p.IdCorVidro = cv.IdCorVidro", "cv")
               .LeftJoin<Data.Model.UnidadeMedida>("p.IdUnidadeMedida = um.IdUnidadeMedida", "um")
               .LeftJoin<Data.Model.UnidadeMedida>("p.IdUnidadeMedidaTrib = umt.IdUnidadeMedida", "umt")
               .LeftJoin(
                   SourceContext.Instance.CreateQuery()
                       .From<Data.Model.ProdutoBaixaEstoque>("pbe")
                       .InnerJoin<Data.Model.Produto>("pbe.IdProdBaixa = p2.IdProd", "p2")
                       .Select("pbe.IdProd, p2.CodInterno, p2.Descricao")
                       .GroupBy("pbe.IdProd"), "p.IdProd = pbe.IdProd", "pbe")
               .Select(
                   @" p.IdProd, p.CodInterno, p.Descricao, ISNULL(f.Nomefantasia, f.Razaosocial) AS NomeFornecedor,
                      p.Altura, p.Largura, pcc.Descricao AS PlanoContaContabil, g.Descricao AS Grupo, sg.Descricao AS Subgrupo,
                      p.Situacao, p.TipoMercadoria, p.AreaMinima, cv.Descricao AS Cor, p.Espessura,
                      p.Peso, p.Forma, p.ValorFiscal, p.ValorObra, p.ValorAtacado, p.ValorBalcao, 
                      p.ValorReposicao, p.ValorTransferencia,
                      p.Custofabbase, p.CustoCompra, p.AliqICMSST, p.AliqIPI, p.Cst, p.Csosn, p.CstIpi,
                      p.Ncm, p.GTINProduto, p.GTINUnidTrib, gp.IdGeneroProduto, gp.Descricao AS GeneroProduto,
                      um.Codigo AS Unidade, umt.Codigo AS UnidadeTrib, p.AtivarAreaMinima,
                      pbe.IdProd AS IdProdBaixaEstoqueFiscal,
                      pbe.CodInterno AS CodInternoBaixaEstoqueFiscal, 
                      pbe.Descricao AS DescricaoBaixaEstoqueFiscal,
                      ?pbeQtde AS QtdeBaixaEstoque")
                .Add("?pbeQtde",
                    SourceContext.Instance.CreateQuery()
                        .From<Data.Model.ProdutoBaixaEstoque>()
                        .Count()
                        .Where("IdProd = p.IdProd"));

            var whereClause = consulta.WhereClause;
            if (idFornec.HasValue && idFornec.Value > 0)
            {
                whereClause.And("p.IdFornec=?idFornec").Add("?idFornec", idFornec)
                    .AddDescription(string.Format("Fornecedor: {0} - {1}", idFornec, nomeFornec));
            }
            else if (!string.IsNullOrEmpty(nomeFornec))
            {
                whereClause
                    .And("(f.RazaoSocial LIKE ?nomeFornec or f.NomeFantasia LIKE ?nomeFornec)")
                    .Add("?nomeFornec", string.Format("%{0}%", nomeFornec))
                    .AddDescription(string.Format("Fornecedor: {0}", nomeFornec));
            }

            if (idGrupoProd.HasValue && idGrupoProd.Value > 0)
                whereClause
                    .And("p.IdGrupoProd=?idGrupoProd")
                    .Add("?idGrupoProd", idGrupoProd)
                    .AddDescription(() =>
                        string.Format("Grupo: {0}", 
                            SourceContext.Instance.CreateQuery()
                                .From<Data.Model.GrupoProd>()
                                .Select<Data.Model.GrupoProd>(f => f.Descricao)
                                .Where("IdGrupoProd=?id")
                                .Add("?id", idGrupoProd)
                                .Execute()
                                .Select(f => f.GetString(0))
                                .FirstOrDefault()));

            if (idSubgrupoProd.HasValue && idSubgrupoProd.Value > 0)
                whereClause
                    .And("p.IdSubgrupoProd=?idSubgrupoProd")
                    .Add("?idSubgrupoProd", idGrupoProd)
                    .AddDescription(() =>
                        string.Format("Subgrupo: {0}", 
                            SourceContext.Instance.CreateQuery()
                                .From<Data.Model.SubgrupoProd>()
                                .Select<Data.Model.SubgrupoProd>(f => f.Descricao)
                                .Where("IdSubgrupoProd=?id")
                                .Add("?id", idSubgrupoProd)
                                .Execute()
                                .Select(f => f.GetString(0))
                                .FirstOrDefault()));

            if (!string.IsNullOrEmpty(codInterno))
                whereClause
                    .And("p.CodInterno LIKE ?codInterno")
                    .Add("?codInterno", string.Format("%{0}%", codInterno));

            if (!string.IsNullOrEmpty(descricao))
                whereClause
                    .And("p.Descricao LIKE ?descricao")
                    .Add("?descricao", string.Format("%{0}%", descricao))
                    .AddDescription(string.Format("Descrição: {0}", descricao));


            if (tipoNegociacao == TipoNegociacaoProduto.Venda)
                whereClause
                    .And("(p.Compra=0 OR p.Compra is null)")
                    .AddDescription("Produtos de venda   ");
            
            else if (tipoNegociacao == TipoNegociacaoProduto.Compra)
                whereClause
                    .And("p.Compra=1")
                    .AddDescription("Produtos de compra   ");
            
            else
                whereClause.AddDescription("Produtos de compra e venda    ");

            if (situacao.HasValue)
                whereClause
                    .And("p.Situacao=?situacao")
                    .Add("?situacao", situacao)
                    .AddDescription(string.Format("Situação: {0}", situacao.Translate().Format()));

            if (apenasProdutosEstoqueBaixa)
            {
                whereClause.And("p.IdProd IN  (?baixaEstoque)")
                    .Add("?baixaEstoque", 
                        SourceContext.Instance.CreateQuery()
                            .From<Data.Model.ProdutoBaixaEstoque>()
                            .Where("IdProdBaixa > 0")
                            .Select("IdProd")
                            .GroupBy("IdProd"))
                    .AddDescription("Apenas produtos que indicam produto para baixa    ");
            }

            if (alturaInicio > 0 || alturaFim > 0)
            {
                whereClause
                    .And("p.Altura >= ?alturaInicio")
                    .Add("?alturaInicio", alturaInicio);
                
                if (alturaFim > 0)
                    whereClause
                        .And("p.Altura <= ?alturaFim")
                        .Add("?alturaFim", alturaFim);

                whereClause.AddDescription(string.Format("Altura: {0}  Até {1}", alturaInicio, alturaFim));
            }

            if (larguraInicio > 0 || larguraFim > 0)
            {
                whereClause
                    .And("p.Largura >= ?larguraInicio")
                    .Add("?larguraInicio", larguraInicio);
                
                if (alturaFim > 0)
                    whereClause
                        .And("p.Largura <= ?larguraFim")
                        .Add("?larguraFim", larguraFim);

                whereClause.AddDescription(string.Format("Largura: {0}  Até {1}", alturaInicio, alturaFim));
            }

            var resultado = consulta.Execute<Entidades.FichaProduto>().ToList();
            CompletaDados(resultado);

            return resultado;               
        }

        /// <summary>
        /// Recupera descritor de todos os produtos.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemProdutos()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Recupera os produtos pelos identificadores informados.
        /// </summary>
        /// <param name="idProds">Identificadores dos produtos que devem ser recuperados.</param>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemProdutos(IEnumerable<int> idProds)
        {
            var ids = string.Join(",", idProds.Select(f => f.ToString()).ToArray());

            if (string.IsNullOrEmpty(ids)) return new List<Colosoft.IEntityDescriptor>();

            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Produto>()
                .Where(string.Format("IdProd IN ({0})", ids))
                .ProcessResultDescriptor<Entidades.Produto>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados do produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        public Entidades.Produto ObtemProduto(int idProd)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Produto>()
                .Where("IdProd=?id")
                .Add("?id", idProd)
                .ProcessLazyResult<Entidades.Produto>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Recupera as descriçõa dos beneficiamentos associados com o produto.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <returns></returns>
        IEnumerable<string> Entidades.IProdutoBeneficiamentosRepositorio.ObtemDescricoes(int idProd)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.ProdutoBenef>("pb")
                .LeftJoin<Data.Model.BenefConfig>("pb.IdBenefConfig = b.IdBenefConfig", "b")
                .LeftJoin<Data.Model.BenefConfig>("b.IdParent = p.IdBenefConfig", "p")
                .Where("pb.IdProd=?idProd")
                .Add("?idProd", idProd)
                .Select("b.Descricao, p.Descricao AS DescricaoPai")
                .Execute()
                .Select(f =>
                    new
                    {
                        Descricao = f.GetString(0),
                        DescricaoPai = f.GetString(1)
                    })
                .Select(f =>
                    f.DescricaoPai == null ||
                    f.Descricao.IndexOf(f.DescricaoPai, StringComparison.InvariantCultureIgnoreCase) > -1 ?
                    f.Descricao : f.DescricaoPai + " " + f.Descricao);
        }

        /// <summary>
        /// Salva os dados do produto.
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarProduto(Entidades.Produto produto)
        {
            produto.Require("produto").NotNull();

            IEnumerable<Estoque.Negocios.Entidades.ProdutoLoja> produtosLoja = null;

            // Verifica se é um produto novo
            if (!produto.ExistsInStorage)
            {
                // Cria os estoques que precisam ser inseridos
                produtosLoja = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Loja>()
                    .Select("IdLoja")
                    .Execute()
                    .Select(f => new Estoque.Negocios.Entidades.ProdutoLoja
                    {
                        IdProd = produto.IdProd,
                        IdLoja = f.GetInt32(0)
                    })
                    .ToList();

                if(produtosLoja == null || produtosLoja.Count() <= 0)
                    return new Colosoft.Business.SaveResult(false, "Falha ao incluir produto loja.".GetFormatter());
            }
            else
                produtosLoja = new Estoque.Negocios.Entidades.ProdutoLoja[0];

            /* Chamado 52522. */
            if (produto.Subgrupo == null || produto.IdSubgrupoProd.GetValueOrDefault() == 0)
                return new Colosoft.Business.SaveResult(false, "Informe o subgrupo do produto.".GetFormatter());

            // Se produto for do tipo mercadoria produto para acabado, obriga a informar os campos matéria prima e produto para baixa
            if (produto.IdGrupoProd == (int)Data.Model.NomeGrupoProd.Vidro && produto.TipoMercadoria == Data.Model.TipoMercadoria.ProdutoAcabado)
            {
                if (!produto.BaixasEstoque.Any())
                    return new Colosoft.Business.SaveResult(false,
                        "Produtos do tipo de mercadoria Produto Acabado devem informar a matéria prima.".GetFormatter());

                if (!produto.BaixasEstoqueFiscal.Any())
                    return new Colosoft.Business.SaveResult(false,
                        "Produtos do tipo de mercadoria Produto Acabado devem informar os produtos para baixa fiscal.".GetFormatter());
            }

            // Se produto for do tipo mercadoria produto para acabado, obriga a informar os campos matéria prima e produto para baixa
            if (produto.Subgrupo.TipoSubgrupo == Data.Model.TipoSubgrupoProd.ChapasVidro)
                if (produto.IdProdBase.GetValueOrDefault() == 0)
                    return new Colosoft.Business.SaveResult(false,
                        "Produtos do subgrupo chapa de vidro deve informar o produto base.".GetFormatter());

            /* Chamado 50689. */
            if (produto.IdGrupoProd != produto.Subgrupo.IdGrupoProd)
                return new Colosoft.Business.SaveResult(false, "O subgrupo informado não pertence ao grupo informado. Informe um subgrupo que esteja associado ao grupo selecionado.".GetFormatter());

            /* Chamado 37411. */
            if (produto.Subgrupo.TipoSubgrupo == Data.Model.TipoSubgrupoProd.Modulado)
                if (produto.BaixasEstoque == null || produto.BaixasEstoque.Count != 1 || produto.BaixasEstoque.FirstOrDefault().IdProdBaixa <= 0)
                    return new Colosoft.Business.SaveResult(false, ("Informe a matéria prima do produto, somente uma matéria prima pode ser informada. " +
                        "A matéria prima é obrigatória para produtos do subgrupo Modulado.").GetFormatter());

            /* Chamado 52702. */
            // Verifica se o subgrupo do produto que está sendo salvo é do tipo Vidro Laminado ou Vidro Duplo e se possui baixa de estoque real ou fiscal.
            if (produto.Subgrupo.TipoSubgrupo == Data.Model.TipoSubgrupoProd.VidroLaminado || produto.Subgrupo.TipoSubgrupo == Data.Model.TipoSubgrupoProd.VidroDuplo)
            {
                #region Baixa estoque
                
                if (produto.BaixasEstoque != null && produto.BaixasEstoque.Count() > 0)
                {
                    // Não permite que o próprio produto duplo ou laminado seja referenciado como baixa de estoque.
                    if (produto.BaixasEstoque.Count(f => f.IdProdBaixa == produto.IdProd) > 0)
                        return new Colosoft.Business.SaveResult(false, "O produto duplo/laminado não pode ser configurado com ele próprio na baixa de etoque.".GetFormatter());

                    // Percorre as baixas de estoque do produto que está sendo salvo.
                    foreach (var baixaEstoque in produto.BaixasEstoque)
                    {
                        // Recupera a baixa de estoque do produto que está sendo salvo.
                        var produtoBaixaEstoque = ObtemProduto(baixaEstoque.IdProdBaixa);

                        // Verifica se o subgrupo do produto configurado como baixa, do produto que está sendo salvo, é do tipo Vidro Laminado ou Vidro Duplo e se possui baixa de estoque.
                        if ((produtoBaixaEstoque.Subgrupo.TipoSubgrupo == Data.Model.TipoSubgrupoProd.VidroLaminado || produtoBaixaEstoque.Subgrupo.TipoSubgrupo == Data.Model.TipoSubgrupoProd.VidroDuplo) &&
                            produtoBaixaEstoque.BaixasEstoque != null && produtoBaixaEstoque.BaixasEstoque.Count() > 0)
                        {
                            // Percorre as baixas de estoque da baixa de estoque do produto que está sendo salvo.
                            foreach (var baixaEstoqueBaixa in produtoBaixaEstoque.BaixasEstoque)
                            {
                                // Recupera o produto da baixa de estoque da baixa de estoque do produto que está sendo salvo.
                                var produtoBaixaEstoqueBaixa = ObtemProduto(baixaEstoqueBaixa.IdProdBaixa);

                                // Verifica se o subgrupo do produto configurado como baixa, da baixa de estoque do produto que está sendo salvo, é do tipo Vidro Laminado ou Vidro Duplo e se possui baixa de estoque.
                                // Caso esta baixa esteja configurada com um produto laminado ou duplo, o bloqueio deve ocorrer, pois, será criada uma ligação de bisavô entre os produtos e a ligação máxima é de avô.
                                if ((produtoBaixaEstoqueBaixa.Subgrupo.TipoSubgrupo == Data.Model.TipoSubgrupoProd.VidroLaminado || produtoBaixaEstoqueBaixa.Subgrupo.TipoSubgrupo == Data.Model.TipoSubgrupoProd.VidroDuplo) &&
                                    produtoBaixaEstoqueBaixa.BaixasEstoque != null && produtoBaixaEstoqueBaixa.BaixasEstoque.Count() > 0)
                                    return new Colosoft.Business.SaveResult(false, "O produto duplo/laminado pode possuir no máximo 2 produtos em sua hierarquia de composição de baixa de estoque.".GetFormatter());
                            }
                        }
                    }
                }

                #endregion
            }

            /* Chamado 22919. */
            // Trata caracteres que não podem ser incluídos no código do produto.
            produto.CodInterno = produto.CodInterno.Replace("'", "").Replace("\"", "").Replace("\t", "").Replace("\n", "");

            // Trata caracteres que não podem ser incluídos na descrição do produto.
            produto.Descricao = produto.Descricao.Replace("'", "").Replace("\"", "").Replace("\t", "").Replace("\n", "");

            Colosoft.Business.SaveResult retorno;

            using (var session = SourceContext.Instance.CreateSession())
            {
                if (produto.ExistsInStorage)
                {
                    /* Chamado 15737.
                     * Verifica se o produto está sendo inativado e se o mesmo está associado a algum produto de projeto. */
                    if (produto.ChangedProperties.Contains("Situacao") && produto.Situacao == Situacao.Inativo &&
                        !String.IsNullOrEmpty(SourceContext.Instance.CreateQuery()
                            .From<Data.Model.ProdutoProjetoConfig>()
                            .Where("IdProd=?id")
                            .Add("?id", produto.IdProd)
                            .Select("IdProd")
                            .Execute()
                            .Select(f => f.GetString(0))
                            .FirstOrDefault()))
                        return new Colosoft.Business.SaveResult(false,
                            "Este produto possui produto(s) de projeto associados a ele, portanto, o mesmo não pode ser inativado.".GetFormatter());

                    // Chamado 43961
                    var produtoAntigo = ObtemProduto(produto.IdProd);
                    MensagemDAO.Instance.EnviarMsgPrecoProdutoAlterado(produtoAntigo.DataModel, produto.DataModel);
                    Email.EnviaEmailAdministradorPrecoProdutoAlterado(null, produtoAntigo.DataModel, produto.DataModel);
                    SMS.EnviaSmsAdministradorPrecoProdutoAlterado(produtoAntigo.DataModel, produto.DataModel);
                }

                var resultado = produto.Save(session);

                if (!resultado)
                    return resultado;

                foreach (var i in produtosLoja)
                {
                    // Salva o estoque
                    resultado = i.Save(session);
                    if (!resultado)
                        return resultado;
                }

                // Verifica se está atualizando o produto
                if (produto.ExistsInStorage)
                {
                    // Caso o grupo/subgrupo tenha sido alterado, atualiza os valores na tabela de desconto/acréscimo
                    if (produto.ChangedProperties.Contains("IdGrupoProd"))
                        session.Update(new Data.Model.DescontoAcrescimoCliente
                            {
                                IdGrupoProd = produto.IdGrupoProd
                            },
                            Colosoft.Query.ConditionalContainer
                                .Parse("IdProduto=?idProd")
                                .Add("?idProd", produto.IdProd), "IdGrupoProd");

                    if (produto.ChangedProperties.Contains("IdSubgrupoProd"))
                        session.Update(new Data.Model.DescontoAcrescimoCliente
                        {
                            IdSubgrupoProd = produto.IdSubgrupoProd
                        },
                            Colosoft.Query.ConditionalContainer
                                .Parse("IdProduto=?idProd")
                                .Add("?idProd", produto.IdProd), "IdSubgrupoProd");
                }

                retorno = session.Execute(false).ToSaveResult();
            }

            if (retorno)
                LogAlteracaoDAO.Instance.LogProduto(produto.DataModel, LogAlteracaoDAO.SequenciaObjeto.Novo);

            return retorno;
        }

        /// <summary>
        /// Apaga os dados do produto.
        /// </summary>
        /// <param name="produto">Instancia do produto que será apagada.</param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarProduto(Entidades.Produto produto)
        {
            produto.Require("produto").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = produto.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion

        #region Membros de IValidadorProduto

        /// <summary>
        /// Valida a existencia do produto.
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorProduto.ValidaAtualizacao(Entidades.Produto produto)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Produto>()
                    .Where("CodInterno=?codInterno")
                    .Add("?codInterno", produto.CodInterno);

            if (produto.ExistsInStorage)
                consulta.WhereClause
                    .And("IdProd <> ?idProd")
                    .Add("?idProd", produto.IdProd);
            
            if (consulta.ExistsResult())
                return new IMessageFormattable[]
                {
                    "Já existe um produto cadastrado com o código informado.".GetFormatter()
                };

            if (produto.IdArquivoMesaCorte.HasValue && !produto.TipoArquivo.HasValue ||
                !produto.IdArquivoMesaCorte.HasValue && produto.TipoArquivo.HasValue)
                return new IMessageFormattable[]
                {
                    "O Arquivo de mesa de corte deve ser informado junto com o tipo do arquivo.".GetFormatter()
                };

            return new IMessageFormattable[0];
        }

        /// <summary>
        /// Valida a existencia do produto.
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorProduto.ValidaExistencia(Entidades.Produto produto)
        {
            var mensagens = new List<string>();

            // Handler para criar a consulta padrão da existencia do registro
            var criarConsulta = new Func<Type, Colosoft.Query.Queryable>(type =>
                SourceContext.Instance.CreateQuery()
                    .From(new Colosoft.Query.EntityInfo(type.FullName))
                    .Count()
                    .Where("IdProd=?idProd")
                    .Add("?idProd", produto.IdProd));

            // Handler para tratar o resultado da consulta de validação
            var tratarResultado = new Func<string, Colosoft.Query.QueryCallBack>(mensagem =>
               (sender, query, result) =>
                   {
                       if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0 && 
                           !mensagens.Contains(mensagem))
                           mensagens.Add(mensagem);
                   });

            var consultas = SourceContext.Instance.CreateMultiQuery()
                // Verifica se o produto está sendo usado em algum pedido
                .Add(criarConsulta(typeof(Data.Model.ProdutosPedido)),
                    tratarResultado("Este produto não pode ser excluído pois existem pedidos utilizando-o."))
                // Verifica se o produto está sendo usado em algum orçamento
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ProdutosOrcamento>()
                    .Count()
                    .Where("IdProduto=?idProd")
                    .Add("?idProd", produto.IdProd),
                    tratarResultado("Este produto não pode ser excluído pois existem orçamentos utilizando-o."))
                // Verifica se o produto está sendo usado em alguma compra
                .Add(criarConsulta(typeof(Data.Model.ProdutosCompra)),
                    tratarResultado("Este produto não pode ser excluído pois existem compras utilizando-o."))
                // Verifica se o produto está sendo usado em alguma nota fiscal
                .Add(criarConsulta(typeof(Data.Model.ProdutosNf)),
                    tratarResultado("Este produto não pode ser excluído pois existem notas fiscais utilizando-o."))
                // Verifica se o produto está sendo usado em algum material de projeto
                .Add(criarConsulta(typeof(Data.Model.MaterialItemProjeto)),
                    tratarResultado("Este produto não pode ser excluído pois existem cálculos de projeto utilizando-o."))
                // Verifica se o produto está sendo na configuração do projeto
                .Add(criarConsulta(typeof(Data.Model.ProdutoProjeto)),
                    tratarResultado("Este produto não pode ser excluído pois existem produtos no projeto utilizando-o."))
                // Verifica se o produto está sendo na configuração do projeto
                .Add(criarConsulta(typeof(Data.Model.ProdutoProjetoConfig)),
                    tratarResultado("Este produto não pode ser excluído pois existem produtos no projeto utilizando-o."))

                // Verifica se o produto está sendo usado para baixa de estoque fiscal de outro
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ProdutoBaixaEstoqueFiscal>()
                    .Count()
                    .Where("IdProd=?idProd OR IdProdBaixa=?idProd")
                    .Add("?idProd", produto.IdProd),
                    tratarResultado("Este produto não pode ser excluído pois existem outros produtos associados ao mesmo como baixa de estoque."))

                // Verifica se o produto está sendo usado para baixa de estoque de outro
               .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ProdutoBaixaEstoque>()
                    .Count()
                    .Where("IdProd=?idProd OR IdProdBaixa=?idProd")
                    .Add("?idProd", produto.IdProd),
                    tratarResultado("Este produto não pode ser excluído pois existem outros produtos associados ao mesmo como baixa de estoque."))

                // Verifica se o produto está sendo usado como produto base de chapa de vidro
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Produto>()
                    .Where("IdProdBase=?idProd")
                    .Add("?idProd", produto.IdProd)
                    .Count(),
                    tratarResultado("Este produto não pode ser excluído pois existem chapas de vidro relacionadas ao mesmo."))

                // Verifica se o produto está sendo usado como produto original de retlaho de produção
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Produto>()
                    .Where("IdProdOrig=?idProd")
                    .Add("?idProd", produto.IdProd)
                    .Count(),
                    tratarResultado("Este produto não pode ser excluído pois existem retalhos de produção relacionados ao mesmo."))

                // Verifica se o produto possui fornecedores associados.
                .Add(criarConsulta(typeof(Data.Model.ProdutoFornecedor)),
                    tratarResultado("Este produto não pode ser excluído pois existem fornecedores relacionados ao mesmo."));
                 
            consultas.Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        #endregion

        #region IValidadorCorAluminio Members

        /// <summary>
        /// Valida a existencia do dados da cor do alumínio.
        /// </summary>
        /// <returns></returns>
        public IMessageFormattable[] ValidaExistencia(Entidades.CorAluminio corAluminio)
        {
            var mensagens = new List<string>();

            // Handler para criar a consulta padrão da existencia do registro
            var criarConsulta = new Func<Type, Colosoft.Query.Queryable>(type =>
                SourceContext.Instance.CreateQuery()
                .From(new Colosoft.Query.EntityInfo(type.FullName))
                .Count()
                .Where("IdCorAluminio=?id")
                .Add("?id", corAluminio.IdCorAluminio));

            // Handler para tratar o resultado da consulta de validação
            var tratarResultado = new Func<string, Colosoft.Query.QueryCallBack>(mensagem =>
               (sender, query, result) =>
               {
                   if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0 &&
                       !mensagens.Contains(mensagem))
                       mensagens.Add(mensagem);
               });

            var consultas = SourceContext.Instance.CreateMultiQuery()

                // Verifica se o produto está sendo usado em algum item de projeto
                .Add(criarConsulta(typeof(Data.Model.ItemProjeto)),
                    tratarResultado("Esta cor de alumínio não pode ser excluída pois existem cálculos de projeto utilizando-a."))

                // Verifica se o produto está sendo usado em algum produto de projeto
                .Add(criarConsulta(typeof(Data.Model.ProdutoProjetoConfig)),
                    tratarResultado("Esta cor de alumínio não pode ser excluída pois existem produtos de projeto utilizando-a."))

                // Verifica se o produto está sendo usado em alguma regra de natureza de operação
                .Add(criarConsulta(typeof(Data.Model.RegraNaturezaOperacao)),
                    tratarResultado("Esta cor de alumínio não pode ser excluída pois existem regras de natureza de operação utilizando-a."))

                // Verifica se o produto está sendo usado em algum produto
                .Add(criarConsulta(typeof(Data.Model.Produto)),
                    tratarResultado("Esta cor de alumínio não pode ser excluída pois existem produtos utilizando-a."));

            consultas.Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        #endregion

        #region IValidadorCorFerragem Members

        /// <summary>
        /// Valida a existencia do dados da cor da ferragem.
        /// </summary>
        /// <returns></returns>
        public IMessageFormattable[] ValidaExistencia(Entidades.CorFerragem corFerragem)
        {
            var mensagens = new List<string>();

            // Handler para criar a consulta padrão da existencia do registro
            var criarConsulta = new Func<Type, Colosoft.Query.Queryable>(type =>
                SourceContext.Instance.CreateQuery()
                .From(new Colosoft.Query.EntityInfo(type.FullName))
                .Count()
                .Where("IdCorFerragem=?id")
                .Add("?id", corFerragem.IdCorFerragem));

            // Handler para tratar o resultado da consulta de validação
            var tratarResultado = new Func<string, Colosoft.Query.QueryCallBack>(mensagem =>
               (sender, query, result) =>
               {
                   if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0 &&
                       !mensagens.Contains(mensagem))
                       mensagens.Add(mensagem);
               });

            var consultas = SourceContext.Instance.CreateMultiQuery()

                // Verifica se o produto está sendo usado em algum item de projeto
                .Add(criarConsulta(typeof(Data.Model.ItemProjeto)),
                    tratarResultado("Esta cor de ferragem não pode ser excluída pois existem cálculos de projeto utilizando-a."))

                // Verifica se o produto está sendo usado em algum produto de projeto
                .Add(criarConsulta(typeof(Data.Model.ProdutoProjetoConfig)),
                    tratarResultado("Esta cor de ferragem não pode ser excluída pois existem produtos de projeto utilizando-a."))

                // Verifica se o produto está sendo usado em alguma regra de natureza de operação
                .Add(criarConsulta(typeof(Data.Model.RegraNaturezaOperacao)),
                    tratarResultado("Esta cor de ferragem não pode ser excluída pois existem regras de natureza de operação utilizando-a."))

                // Verifica se o produto está sendo usado em algum produto
                .Add(criarConsulta(typeof(Data.Model.Produto)),
                    tratarResultado("Esta cor de ferragem não pode ser excluída pois existem produtos utilizando-a."));

            consultas.Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        #endregion

        #region IValidadorCorVidro Members

        /// <summary>
        /// Valida a existencia do dados da cor do vidro.
        /// </summary>
        /// <returns></returns>
        public IMessageFormattable[] ValidaExistencia(Entidades.CorVidro corVidro)
        {
            var mensagens = new List<string>();

            // Handler para criar a consulta padrão da existencia do registro
            var criarConsulta = new Func<Type, Colosoft.Query.Queryable>(type =>
                SourceContext.Instance.CreateQuery()
                .From(new Colosoft.Query.EntityInfo(type.FullName))
                .Count()
                .Where("IdCorVidro=?id")
                .Add("?id", corVidro.IdCorVidro));

            // Handler para tratar o resultado da consulta de validação
            var tratarResultado = new Func<string, Colosoft.Query.QueryCallBack>(mensagem =>
               (sender, query, result) =>
               {
                   if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0 &&
                       !mensagens.Contains(mensagem))
                       mensagens.Add(mensagem);
               });

            var consultas = SourceContext.Instance.CreateMultiQuery()

                // Verifica se o produto está sendo usado em algum beneficiamento
                .Add(criarConsulta(typeof(Data.Model.BenefConfigPreco)),
                    tratarResultado("Esta cor de vidro não pode ser excluída pois existem beneficiamentos utilizando-a."))

                // Verifica se o produto está sendo usado em algum item de projeto
                .Add(criarConsulta(typeof(Data.Model.ItemProjeto)),
                    tratarResultado("Esta cor de vidro não pode ser excluída pois existem cálculos de projeto utilizando-a."))

                // Verifica se o produto está sendo usado em algum produto de projeto
                .Add(criarConsulta(typeof(Data.Model.ProdutoProjetoConfig)),
                    tratarResultado("Esta cor de vidro não pode ser excluída pois existem produtos de projeto utilizando-a."))

                // Verifica se o produto está sendo usado em alguma regra de natureza de operação
                .Add(criarConsulta(typeof(Data.Model.RegraNaturezaOperacao)),
                    tratarResultado("Esta cor de vidro não pode ser excluída pois existem regras de natureza de operação utilizando-a."))

                // Verifica se o produto está sendo usado em algum produto
                .Add(criarConsulta(typeof(Data.Model.Produto)),
                    tratarResultado("Esta cor de vidro não pode ser excluída pois existem produtos utilizando-a."));

            consultas.Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        #endregion
    }
}
