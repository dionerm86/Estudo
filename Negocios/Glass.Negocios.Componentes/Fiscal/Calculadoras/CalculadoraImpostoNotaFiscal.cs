using Colosoft.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft;

namespace Glass.Fiscal.Negocios.Componentes.Calculadoras
{
    /// <summary>
    /// Implementação da calculadora de imposto da nota fiscal.
    /// </summary>
    public class CalculadoraImpostoNotaFiscal :
        Data.ICalculadoraImposto<Data.Model.NotaFiscal, Data.Model.ProdutosNf>
    {
        #region Propriedades

        /// <summary>
        /// Calculadora base dos impostos.
        /// </summary>
        private ICalculadoraImposto Calculadora { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="calculadora"></param>
        /// <param name="localizadorNaturezaOperacao"></param>
        public CalculadoraImpostoNotaFiscal(ICalculadoraImposto calculadora)
        {
            Calculadora = calculadora;
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Recupera os itens de impostos com base nos produtos da nota fiscal.
        /// </summary>
        /// <param name="notaFiscal"></param>
        /// <param name="produtosNf"></param>
        /// <param name="produtos"></param>
        /// <returns></returns>
        private IEnumerable<IItemImposto> ObterItensImposto(
            Data.Model.NotaFiscal notaFiscal,
            IEnumerable<Data.Model.ProdutosNf> produtosNf,
            IEnumerable<Global.Negocios.Entidades.Produto> produtos)
        {

            // Carrega as naturezas de operações que serão usadas no cálculo
            var idsNaturezaOperacao = produtosNf
                .Where(f => f.IdNaturezaOperacao.HasValue)
                .Select(f => (int)f.IdNaturezaOperacao.Value);

            if (notaFiscal.IdNaturezaOperacao.HasValue)
                idsNaturezaOperacao = idsNaturezaOperacao.Concat(new[] { (int)notaFiscal.IdNaturezaOperacao.Value });

            var ids = string.Join(",", idsNaturezaOperacao.Distinct());

            IEnumerable<Entidades.NaturezaOperacao> naturezasOperacao = null;

            if (!string.IsNullOrEmpty(ids))
                naturezasOperacao = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.NaturezaOperacao>()
                    .Where($"IdNaturezaOperacao IN ({ids})")
                    .ProcessLazyResult<Entidades.NaturezaOperacao>()
                    .ToList();
            else
                naturezasOperacao = new Entidades.NaturezaOperacao[0];

            foreach(var produtoNf in produtosNf)
            {
                var idNaturezaOperacao = produtoNf.IdNaturezaOperacao ?? notaFiscal.IdNaturezaOperacao;

                var naturezaOperacao = idNaturezaOperacao.HasValue ?
                    naturezasOperacao.FirstOrDefault(f => f.IdNaturezaOperacao == idNaturezaOperacao.Value) : null;

                yield return new ProdutoNfItemImposto(produtoNf, naturezaOperacao,
                    produtos.FirstOrDefault(f => f.IdProd == (int)produtoNf.IdProd));
            }
        }

        /// <summary>
        /// Recupera o container dos itens de impostos da nota fiscal.
        /// </summary>
        /// <param name="notaFiscal"></param>
        /// <param name="itens"></param>
        /// <returns></returns>
        private IItemImpostoContainer ObterContainer(Data.Model.NotaFiscal notaFiscal, IEnumerable<Data.Model.ProdutosNf> produtosNf)
        {
            Global.Negocios.Entidades.Cliente cliente = null;
            Global.Negocios.Entidades.Loja loja = null;
            Global.Negocios.Entidades.Fornecedor fornecedor = null;
            IEnumerable<Global.Negocios.Entidades.Produto> produtos = null;
            IEnumerable<IItemImposto> itens = null;

            var consultas = SourceContext.Instance.CreateMultiQuery()
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cliente>()
                    .Where("IdCli=?id")
                    .Add("?id", notaFiscal.IdCliente),
                    (sender, query, result) =>
                        cliente = EntityManager.Instance
                            .ProcessLazyResult<Global.Negocios.Entidades.Cliente>(result, SourceContext.Instance)
                            .FirstOrDefault())

                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Loja>()
                    .Where("IdLoja=?id")
                    .Add("?id", notaFiscal.IdLoja),
                    (sender, query, result) =>
                        loja = EntityManager.Instance
                            .ProcessLazyResult<Global.Negocios.Entidades.Loja>(result, SourceContext.Instance)
                            .FirstOrDefault())

                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Fornecedor>()
                    .Where("IdFornec=?id")
                    .Add("?id", notaFiscal.IdFornec),
                    (sender, query, result) =>
                        fornecedor = EntityManager.Instance
                            .ProcessLazyResult<Global.Negocios.Entidades.Fornecedor>(result, SourceContext.Instance)
                            .FirstOrDefault());

            if (produtosNf == null)
            {
                // Consulta os produtos da nota
                consultas
                    // Consulta os produtos dos produtos da nota
                    .Add(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.Produto>()
                        .Where("IdProd IN ?subProdutos")
                        .Add("?subProdutos", SourceContext.Instance.CreateQuery()
                            .From<Data.Model.ProdutosNf>()
                            .Where("IdNf=?idNf")
                            .Add("?idNf", notaFiscal.IdNf)
                            .SelectDistinct("IdProd")),
                        (sender, query, result) =>
                            produtos = EntityManager.Instance
                                .ProcessLazyResult<Global.Negocios.Entidades.Produto>(result, SourceContext.Instance)
                                .ToList())

                    .Add<Data.Model.ProdutosNf>(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.ProdutosNf>()
                        .Where("IdNf=?id")
                        .Add("?id", notaFiscal.IdNf),
                        (sender, query, result) =>
                            itens = ObterItensImposto(notaFiscal, result.ToList(), produtos).ToList())

                    .Execute();
            }
            else
            {
                var idsProd = string.Join(",", produtosNf.Select(f => f.IdProd).Distinct());

                if (!string.IsNullOrEmpty(idsProd))
                    consultas
                        .Add(SourceContext.Instance.CreateQuery()
                            .From<Data.Model.Produto>()
                            .Where($"IdProd IN  ({idsProd})"),
                            (sender, query, result) =>
                                produtos = EntityManager.Instance
                                .ProcessLazyResult<Global.Negocios.Entidades.Produto>(result, SourceContext.Instance)
                                .ToList());

                consultas.Execute();
                itens = ObterItensImposto(notaFiscal, produtosNf, produtos).ToList();
            }

            var pedidoImposto = new NotaFiscalImpostoContainer(notaFiscal, cliente, loja, fornecedor, itens);

            return pedidoImposto;
        }

        #endregion

        #region Membros de Data.ICalculadoraImposto<Data.Model.NotaFiscal>

        /// <summary>
        /// Realiza o calculo do imposto para a instancia informada.
        /// </summary>
        /// <param name="sessao">Sessão com o banco de dados que será usada para realizar os calculos.</param>
        /// <param name="instancia">Instancia para qual serão calculado os valores.</param>
        /// <returns></returns>
        Data.ICalculoImpostoResultado Data.ICalculadoraImposto<Data.Model.NotaFiscal>
            .Calcular(GDA.GDASession sessao, Data.Model.NotaFiscal instancia)
        {
            var pedidoContainer = ObterContainer(instancia, null);
            var resultado = Calculadora.Calcular(pedidoContainer);
            return new Resultado(pedidoContainer, resultado);
        }

        /// <summary>
        /// Realiza o calculo do imposto para a instancia informada.
        /// </summary>
        /// <param name="sessao">Sessão com o banco de dados que será usada para realizar os calculos.</param>
        /// <param name="instancia">Instancia para qual serão calculado os valores.</param>
        /// <param name="itens">Itens que serão usados no calculo.</param>
        /// <returns></returns>
        Data.ICalculoImpostoResultado Data.ICalculadoraImposto<Data.Model.NotaFiscal, Data.Model.ProdutosNf>
            .Calcular(GDA.GDASession sessao, Data.Model.NotaFiscal instancia, IEnumerable<Data.Model.ProdutosNf> itens)
        {
            var pedidoContainer = ObterContainer(instancia, itens);
            var resultado = Calculadora.Calcular(pedidoContainer);
            var resultado2 = new Resultado(pedidoContainer, resultado);

            resultado2.AplicarImpostos(itens);

            return resultado2;
        }

        #endregion

        #region Tipos Aninhados

        /// <summary>
        /// Implementação do resultado do calculo.
        /// </summary>
        class Resultado : Data.ICalculoImpostoResultado<Data.Model.ProdutosNf>
        {
            #region Propriedades

            private IItemImpostoContainer Container { get; }

            /// <summary>
            /// Representa o resultado interno.
            /// </summary>
            private ICalculoImpostoResultado ResultadoInterno { get; }

            #endregion

            #region Construtores

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="container"></param>
            /// <param name="resultadoInterno"></param>
            public Resultado(IItemImpostoContainer container, ICalculoImpostoResultado resultadoInterno)
            {
                Container = container;
                ResultadoInterno = resultadoInterno;
            }

            #endregion

            #region Métodos Privados

            /// <summary>
            /// Aplica os impostos calculados.
            /// </summary>
            /// <param name="sessao"></param>
            /// <param name="item"></param>
            private void AplicarImpostos(GDA.GDASession sessao, IItemCalculoImpostoResultado item)
            {
                var produtoNfImposto = item.Referencia as ProdutoNfItemImposto;
                if (produtoNfImposto == null) return;

                var produtoNf = produtoNfImposto.ProdutoNf;
                AplicarImpostos(item, produtoNf);

                Data.DAL.ProdutosNfDAO.Instance.AtualizarImpostos(sessao, produtoNf);
            }

            /// <summary>
            /// Aplica os impostos calculados sobre o produto da nota fiscal.
            /// </summary>
            /// <param name="item"></param>
            /// <param name="produtoNf"></param>
            private static void AplicarImpostos(IItemCalculoImpostoResultado item, Data.Model.ProdutosNf produtoNf)
            {
                produtoNf.AliqIpi = item.AliqIpi;
                produtoNf.ValorIpi = item.ValorIpi;

                produtoNf.AliqIcms = item.AliqIcms;
                produtoNf.BcIcms = item.BcIcms;
                produtoNf.ValorIcms = item.ValorIcms;

                produtoNf.AliqFcp = item.AliqFcp;
                produtoNf.BcFcp = item.BcFcp;
                produtoNf.ValorFcp = item.ValorFcp;

                produtoNf.AliqIcmsSt = item.AliqIcmsSt;
                produtoNf.BcIcmsSt = item.BcIcmsSt;
                produtoNf.ValorIcmsSt = item.ValorIcmsSt;

                produtoNf.AliqFcpSt = item.AliqFcpSt;
                produtoNf.BcFcpSt = item.BcFcpSt;
                produtoNf.ValorFcpSt = item.ValorFcpSt;

                produtoNf.AliqPis = item.AliqPis;
                produtoNf.BcPis = item.BcPis;
                produtoNf.ValorPis = item.ValorPis;

                produtoNf.AliqCofins = item.AliqCofins;
                produtoNf.BcCofins = item.BcCofins;
                produtoNf.ValorCofins = item.ValorCofins;
            }

            /// <summary>
            /// Aplica os impostos do resultado do calculo.
            /// </summary>
            /// <param name="sessao"></param>
            /// <param name="resultado"></param>
            private void AplicarImpostos(GDA.GDASession sessao, ICalculoImpostoResultado resultado)
            {
                foreach (var item in resultado.Itens)
                    AplicarImpostos(sessao, item);
            }

            #endregion

            #region Métodos Públicos

            /// <summary>
            /// Aplica os impostos calculados para os itens informados.
            /// </summary>
            /// <param name="itens"></param>
            public void AplicarImpostos(IEnumerable<Data.Model.ProdutosNf> itens)
            {
                foreach(var itemResultado in ResultadoInterno.Itens)
                {
                    var item = (ProdutoNfItemImposto)itemResultado.Referencia;
                    var produtoNf = itens.FirstOrDefault(f => f.IdProdNf == item.ProdutoNf.IdProdNf);

                    if (produtoNf != null)
                        AplicarImpostos(itemResultado, produtoNf);
                }
            }

            /// <summary>
            /// Salva os dados usando a sessão informada.
            /// </summary>
            /// <param name="sessao"></param>
            public void Salvar(GDA.GDASession sessao)
            {
                AplicarImpostos(sessao, ResultadoInterno);
            }

            #endregion
        }

        #endregion
    }
}
