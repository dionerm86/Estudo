using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de fornecedores.
    /// </summary>
    public class FornecedorFluxo : IFornecedorFluxo, Entidades.IValidadorFornecedor
    {
        #region Fornecedor

        /// <summary>
        /// Pesquisa os fornecedores.
        /// </summary>
        public IList<Entidades.FornecedorPesquisa> PesquisarFornecedores
            (int? idFornec, string nomeFornec, Data.Model.SituacaoFornecedor? situacao,
             string cnpj, bool comCredito, Data.Model.TipoPessoa? tipoPessoa, int idPlanoConta, int idTipoPagto,
             string endereco, string vendedor)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Fornecedor>("f")
                .LeftJoin<Data.Model.Cidade>("f.IdCidade = c.IdCidade", "c")
                .InnerJoin<Data.Model.Pais>("f.IdPais = p.IdPais", "p")
                .LeftJoin<Data.Model.PlanoContas>("f.IdConta = pc.IdConta", "pc")
                .LeftJoin<Data.Model.Parcelas>("par.IdParcela = f.TipoPagto", "par")
                .OrderBy("IdFornec")
                .Select(
                    @"f.IdFornec, f.TipoPessoa, f.Nomefantasia, f.Razaosocial,
                      f.CpfCnpj, f.RgInscEst, f.Suframa, f.Crt, f.Endereco,
                      f.Numero, f.Compl, f.Bairro, f.Cep, f.Email, f.Telcont,
                      f.Fax, f.Dtultcompra, f.Situacao, f.Vendedor, f.Telcelvend, 
                      f.Credito, f.Obs, c.NomeCidade AS Cidade, c.NomeUf AS Uf,
                      p.NomePais AS Pais, pc.Descricao AS PlanoContas, par.Descricao AS Parcela");

            if (idFornec.HasValue && idFornec.Value > 0)
                consulta.WhereClause
                    .And("f.IdFornec=?id")
                    .Add("?id", idFornec.Value);

            if (!string.IsNullOrEmpty(nomeFornec))
                consulta.WhereClause
                    .And("(f.Nomefantasia LIKE ?nomeFornec OR f.Razaosocial LIKE ?nomeFornec)")
                    .Add("?nomeFornec", string.Format("%{0}%", nomeFornec))
                    .AddDescription(string.Format("Fornecedor: {0}", nomeFornec));

            if (!string.IsNullOrEmpty(cnpj))
                consulta.WhereClause
                    .And("REPLACE(REPLACE(REPLACE(f.CpfCnpj, '.', ''), '-', ''), '/', '') LIKE ?cnpj")
                    .Add("?cnpj", Glass.Formatacoes.LimpaCpfCnpj(cnpj))
                    .AddDescription("CNPJ: " + cnpj);

            if (situacao.HasValue && (int)situacao.Value > 0)
                consulta.WhereClause
                    .And("f.Situacao=?situacao")
                    .Add("?situacao", situacao.Value)
                    .AddDescription(string.Format("Situacao: {0}", situacao.Translate().Format()));

            if (comCredito)
                consulta.WhereClause
                    .And("f.Credito > 0")
                    .AddDescription("Fornecedores com crédito");

            if (tipoPessoa.HasValue && (int)tipoPessoa.Value > 0)
                consulta.WhereClause
                    .And("f.TipoPessoa=?tipoPessoa")
                    .Add("?tipoPessoa", (char)tipoPessoa.Value)
                    .AddDescription(String.Format("Tipo de Pessoa: {0}", tipoPessoa.Translate().Format()));

            if (idPlanoConta > 0)
                consulta.WhereClause
                    .And("f.IdConta=?id")
                    .Add("?id", idPlanoConta);

            if (idTipoPagto > 0)
                consulta.WhereClause
                    .And("f.TipoPagto=?id")
                    .Add("?id", idTipoPagto);

            if (!endereco.IsNullOrEmpty())
                consulta.WhereClause
                    .And("f.Endereco LIKE ?endereco")
                    .Add("?endereco", string.Format("%{0}%", endereco))
                    .AddDescription(string.Format("Endereço: {0}    ", endereco));

            if (!vendedor.IsNullOrEmpty())
                consulta.WhereClause
                    .And("f.Vendedor LIKE ?vendedor")
                    .Add("?vendedor", string.Format("%{0}%", vendedor))
                    .AddDescription(string.Format("Vendedor: {0}    ", vendedor));

            return consulta.ToVirtualResult<Entidades.FornecedorPesquisa>();
        }

        /// <summary>
        /// Recupera os descritores dos fornecedores do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemFornecedores()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Fornecedor>()
                .OrderBy("Nomefantasia, Razaosocial")
                .ProcessResultDescriptor<Entidades.Fornecedor>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados do fornecedor.
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        public Entidades.Fornecedor ObtemFornecedor(int idFornec)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Fornecedor>()
                .Where("IdFornec=?id")
                .Add("?id", idFornec)
                .ProcessLazyResult<Entidades.Fornecedor>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Recupera os detalhes do fornecedor.
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        public Entidades.FornecedorPesquisa ObtemDetalhesFornecedor(int idFornec)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Fornecedor>("f")
                .LeftJoin<Data.Model.Cidade>("f.IdCidade = c.IdCidade", "c")
                .InnerJoin<Data.Model.Pais>("f.IdPais = p.IdPais", "p")
                .LeftJoin<Data.Model.PlanoContas>("f.IdConta = pc.IdConta", "pc")
                .LeftJoin<Data.Model.Parcelas>("par.IdParcela = f.TipoPagto", "par")
                .Where("IdFornec=?id").Add("?id", idFornec)
                .Select(
                    @"f.IdFornec, f.TipoPessoa, f.Nomefantasia, f.Razaosocial,
                      f.CpfCnpj, f.RgInscEst, f.Suframa, f.Crt, f.Endereco,
                      f.Numero, f.Compl, f.Bairro, f.Cep, f.Email, f.Telcont,
                      f.Fax, f.Dtultcompra, f.Situacao, f.Vendedor, f.Telcelvend, 
                      f.Credito, f.Obs, c.NomeCidade AS Cidade, c.NomeUf AS Uf,
                      p.NomePais AS Pais, pc.Descricao AS PlanoContas, par.Descricao AS Parcela")                                        
                .Execute<Entidades.FornecedorPesquisa>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados do fornecedor.
        /// </summary>
        /// <param name="fornecedor"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarFornecedor(Entidades.Fornecedor fornecedor)
        {
            fornecedor.Require("fornecedor").NotNull();

            // Remover posteriormente, ao converter os logs para o banco de dados
            var salvarLogs = fornecedor.IdFornec > 0;

            var descricaoParcelas = salvarLogs ?
                Data.DAL.ParcelasNaoUsarDAO.Instance.ObtemDescricao(null, fornecedor.IdFornec) :
                null;

            #region Limita o tamanho do campo endereço em 250

            /* Chamado 35083. */
            fornecedor.Endereco =
                !string.IsNullOrEmpty(fornecedor.Endereco) ?
                    (fornecedor.Endereco.Length > 250 ?
                        fornecedor.Endereco.Substring(0, 250) :
                        fornecedor.Endereco) :
                    null;

            #endregion

            Colosoft.Business.SaveResult retorno;

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = fornecedor.Save(session);

                if (!resultado)
                    return resultado;

                retorno = session.Execute(false).ToSaveResult();
            }

            // Remover posteriormente, ao converter os logs para o banco de dados
            if (retorno && salvarLogs)
                Data.DAL.ParcelasNaoUsarDAO.Instance.AtualizaLog(null, fornecedor.IdFornec, descricaoParcelas);

            return retorno;
        }

        /// <summary>
        /// Apaga os dados do fornecedor.
        /// </summary>
        /// <param name="fornecedor"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarFornecedor(Entidades.Fornecedor fornecedor)
        {
            fornecedor.Require("fornecedor").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = fornecedor.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #region Membros de IValidadorFornecedor

        /// <summary>
        /// Valida a existencia do fornecedor.
        /// </summary>
        /// <param name="fornecedor"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorFornecedor.ValidaExistencia(Entidades.Fornecedor fornecedor)
        {
            var mensagens = new List<IMessageFormattable>();

            SourceContext.Instance.CreateMultiQuery()
                // Verifica se existem compras para este fornecedor
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Compra>()
                    .Where("IdFornec=?id").Add("?id", fornecedor.IdFornec)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            mensagens.Add(
                                "Este fornecedor não pode ser excluído pois existem compras relacionadas à ele.".GetFormatter());
                    })
                // Verifica se existem contas a pagar/pagas para este fornecedor
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ContasPagar>()
                    .Where("IdFornec=?id").Add("?id", fornecedor.IdFornec)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            mensagens.Add(
                                "Este fornecedor não pode ser excluído pois existem contas a pagar/pagas relacionadas à ele.".GetFormatter());
                    })
                // Verifica se existem produtos associados à este fornecedor
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Produto>()
                    .Where("IdFornec=?id").Add("?id", fornecedor.IdFornec)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            mensagens.Add(
                                "Este fornecedor não pode ser excluído pois existem produtos relacionados à ele.".GetFormatter());
                    })
                // Verifica se existem notas fiscais associadas à este fornecedor
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.NotaFiscal>()
                    .Where("IdFornec=?id").Add("?id", fornecedor.IdFornec)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            mensagens.Add(
                                "Este fornecedor não pode ser excluído pois existem notas fiscais relacionadas à ele.".GetFormatter());
                    })
                .Execute();

            return mensagens.ToArray();
        }

        #endregion

        #endregion

        #region Produtos Fornecedor

        /// <summary>
        /// Pesquisa os produtos do fornecedores.
        /// </summary>
        /// <param name="idFornec"></param>
        /// <param name="idProdFornec">Identificador do ProdutoFornecedor.</param>
        /// <param name="idProd">Identificador do produto.</param>
        /// <param name="codigoProduto">Código do produto.</param>
        /// <param name="exibirSemDataVigencia">Identifica se é para exibir sem a data de vigência.</param>
        /// <param name="descricaoProduto">Código do produto que será pesquisado.</param>
        /// <returns></returns>
        public IList<Entidades.ProdutoFornecedorPesquisa> PesquisarProdutosFornecedor
            (int? idFornec, int? idProd, bool? exibirSemDataVigencia,
             string codigoProduto, string descricaoProduto)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ProdutoFornecedor>("pf")
                .InnerJoin<Data.Model.Fornecedor>("pf.IdFornec = f.IdFornec", "f")
                .InnerJoin<Data.Model.Produto>("pf.IdProd = p.IdProd", "p")
                .Select(
                    @"pf.IdProdFornec, pf.IdProd, pf.IdFornec, pf.CodFornec, pf.PrazoEntregaDias, pf.CustoCompra,
                      pf.DataVigencia, p.CodInterno as CodInternoProd, p.Descricao AS DescricaoProduto, f.Nomefantasia AS NomeFantasiaFornecedor, 
                      f.Razaosocial AS RazaoSocialFornecedor");

            if (idFornec.HasValue && idFornec.Value > 0)
                consulta.WhereClause
                    .And("pf.IdFornec=?idFornec")
                    .Add("?idFornec", idFornec.Value)
                    // Adiciona o nome do fornecedor como critério
                    .AddDescription(() => string.Format("Fornecedor: {0}",
                        SourceContext.Instance.CreateQuery()
                            .From<Data.Model.Fornecedor>()
                            .Where("IdFornec=?id")
                            .Add("?id", idFornec)
                            .Select("ISNULL(Nomefantasia, Razaosocial)")
                            .Execute()
                            .Select(f => f.GetString(0))
                            .FirstOrDefault()));

            if (idProd.HasValue && idProd.Value > 0)
                consulta.WhereClause
                    .And("pf.IdProd=?idProd")
                    .Add("?idProd", idProd.Value)
                    // Adiciona a descrição do produto como critério
                    .AddDescription(() => string.Format("Produto: {0}",
                        SourceContext.Instance.CreateQuery()
                            .From<Data.Model.Produto>()
                            .Where("IdProd=?id")
                            .Add("?id", idProd)
                            .Select("Descricao")
                            .Execute().Select(f => f.GetString(0))
                            .FirstOrDefault()));

            if (exibirSemDataVigencia.HasValue)
            {
                if (!exibirSemDataVigencia.Value)
                    consulta.WhereClause
                        .And("pf.DataVigencia IS NOT NULL")
                        .AddDescription("Com data de vigência    ");
                else
                    consulta.WhereClause
                        .And("pf.DataVigencia IS NULL")
                        .AddDescription("Sem data de vigência    ");
            }

            if (!string.IsNullOrEmpty(codigoProduto))
                consulta.WhereClause
                    .And("p.CodInterno LIKE ?codProduto")
                    .Add("?codProduto", string.Format("%{0}%", codigoProduto))
                    .AddDescription("Produto: " + codigoProduto + (!string.IsNullOrEmpty(descricaoProduto) ? " - " + descricaoProduto : "") + "   ");

            if (!string.IsNullOrEmpty(descricaoProduto))
                consulta.WhereClause
                    .And("p.Descricao LIKE ?descricaoProduto")
                    .Add("?descricaoProduto", string.Format("%{0}%", descricaoProduto))
                    .AddDescription("Produto: " + descricaoProduto + "   ");

            return consulta.ToVirtualResultLazy<Entidades.ProdutoFornecedorPesquisa>();

        }

        /// <summary>
        /// Recupera a relação dos descritores os produtod dos fornecedores.
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemProdutosFornecedor(int idFornec)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.ProdutoFornecedor>()
                .Where("IdFornec=?id").Add("?id", idFornec)
                .OrderBy("CodFornec")
                .ProcessResultDescriptor<Entidades.ProdutoFornecedor>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados do produto do fornecedor.
        /// </summary>
        /// <param name="idProdFornec"></param>
        /// <returns></returns>
        public Entidades.ProdutoFornecedor ObtemProdutoFornecedor(int idProdFornec)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.ProdutoFornecedor>()
                .Where("IdProdFornec=?id")
                .Add("?id", idProdFornec)
                .ProcessLazyResult<Entidades.ProdutoFornecedor>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados do produto do fornecedor.
        /// </summary>
        /// <param name="produtoFornecedor"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarProdutoFornecedor(Entidades.ProdutoFornecedor produtoFornecedor)
        {
            produtoFornecedor.Require("produtoFornecedor").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = produtoFornecedor.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados do produto do fornecedor.
        /// </summary>
        /// <param name="produtoFornecedor"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarProdutoFornecedor(Entidades.ProdutoFornecedor produtoFornecedor)
        {
            produtoFornecedor.Require("produtoFornecedor").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = produtoFornecedor.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion
    }
}
