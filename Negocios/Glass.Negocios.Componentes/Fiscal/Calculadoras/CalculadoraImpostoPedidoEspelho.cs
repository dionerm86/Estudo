using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft.Business;

namespace Glass.Fiscal.Negocios.Componentes.Calculadoras
{
    /// <summary>
    /// Implementação da calculadora de imposto do pedido espelho.
    /// </summary>
    public class CalculadoraImpostoPedidoEspelho :
        Data.ICalculadoraImposto<Data.Model.PedidoEspelho>
    {
        #region Propriedades

        /// <summary>
        /// Calculadora base dos impostos.
        /// </summary>
        private ICalculadoraImposto Calculadora { get; }

        /// <summary>
        /// Localiza das naturezas de operação.
        /// </summary>
        private ILocalizadorNaturezaOperacao LocalizadorNaturezaOperacao { get; }

        /// <summary>
        /// Provedor dos MVA do produto por UF.
        /// </summary>
        private Entidades.IProvedorMvaProdutoUf ProvedorMvaProdutoUf { get; }

        /// <summary>
        /// Provedor do código do valor fiscal.
        /// </summary>
        private IProvedorCodValorFiscal ProvedorCodValorFiscal { get; }

        #endregion

        #region Constructores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="calculadora"></param>
        /// <param name="localizadorNaturezaOperacao"></param>
        public CalculadoraImpostoPedidoEspelho(
            ICalculadoraImposto calculadora,
            ILocalizadorNaturezaOperacao localizadorNaturezaOperacao,
            Entidades.IProvedorMvaProdutoUf provedorMvaProdutoUf,
            IProvedorCodValorFiscal provedorCodValorFiscal)
        {
            Calculadora = calculadora;
            LocalizadorNaturezaOperacao = localizadorNaturezaOperacao;
            ProvedorMvaProdutoUf = provedorMvaProdutoUf;
            ProvedorCodValorFiscal = provedorCodValorFiscal;
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Recupera os itens de impostos com base nos produtos do pedido.
        /// </summary>
        /// <param name="produtosPedido"></param>
        /// <param name="cliente"></param>
        /// <param name="loja"></param>
        /// <param name="produtos"></param>
        /// <returns></returns>
        private IEnumerable<IItemImposto> ObterItensImposto(
            IEnumerable<Data.Model.ProdutosPedidoEspelho> produtosPedido,
            Global.Negocios.Entidades.Cliente cliente, Global.Negocios.Entidades.Loja loja,
            IEnumerable<Global.Negocios.Entidades.Produto> produtos)
        {
            var naturezasOperacao = LocalizadorNaturezaOperacao.Buscar(cliente, loja, produtos);
            var mvas = ProvedorMvaProdutoUf.ObterMvaPorProdutos(produtos, loja, null, cliente, true);

            var produtosNaturezaOperacao = new Dictionary<int, Tuple<Global.Negocios.Entidades.Produto, Entidades.NaturezaOperacao>>();
            var produtosMva = new Dictionary<int, float>();

            // Carrega a natureza de operação e o mva dos produtos
            using (var produtoEnumerator = produtos.GetEnumerator())
            using (var naturezasOperacaoEnumerator = naturezasOperacao.GetEnumerator())
            using (var mvaEnumerator = mvas.GetEnumerator())
                while (produtoEnumerator.MoveNext())
                {
                    if (naturezasOperacaoEnumerator.MoveNext())
                        produtosNaturezaOperacao.Add(produtoEnumerator.Current.IdProd,
                            new Tuple<Global.Negocios.Entidades.Produto, Entidades.NaturezaOperacao>(
                                produtoEnumerator.Current, naturezasOperacaoEnumerator.Current));

                    if (mvaEnumerator.MoveNext())
                        produtosMva.Add(produtoEnumerator.Current.IdProd, mvaEnumerator.Current);
                }

            foreach (var produtoPedido in produtosPedido)
            {
                var produtoNateruzaOperacao = produtosNaturezaOperacao[(int)produtoPedido.IdProd];

                yield return new ProdutoPedidoEspelhoItemImposto(
                    produtoPedido,
                    loja,
                    produtoNateruzaOperacao.Item2,
                    produtoNateruzaOperacao.Item1,
                    produtosMva[(int)produtoPedido.IdProd],
                    ProvedorCodValorFiscal);
            }
        }

        /// <summary>
        /// Recupera o container dos itens de impostos do pedido.
        /// </summary>
        /// <param name="pedidoEspelho"></param>
        /// <returns></returns>
        private IItemImpostoContainer ObterContainer(Data.Model.PedidoEspelho pedidoEspelho)
        {
            Global.Negocios.Entidades.Cliente cliente = null;
            Global.Negocios.Entidades.Loja loja = null;
            IEnumerable<Global.Negocios.Entidades.Produto> produtos = null;
            IEnumerable<IItemImposto> itens = null;

            SourceContext.Instance.CreateMultiQuery()
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cliente>("c")
                    .InnerJoin<Data.Model.Pedido>("p.IdCli=c.IdCli", "p")
                    .Where("p.IdPedido=?id")
                    .Add("?id", pedidoEspelho.IdPedido),
                    (sender, query, result) =>
                        cliente = EntityManager.Instance
                            .ProcessLazyResult<Global.Negocios.Entidades.Cliente>(result, SourceContext.Instance)
                            .FirstOrDefault())

                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Loja>("l")
                    .InnerJoin<Data.Model.Pedido>("p.IdLoja=l.IdLoja", "p")
                    .Where("p.IdPedido=?id")
                    .Add("?id", pedidoEspelho.IdPedido),
                    (sender, query, result) =>
                        loja = EntityManager.Instance
                            .ProcessLazyResult<Global.Negocios.Entidades.Loja>(result, SourceContext.Instance)
                            .FirstOrDefault())

                // Consulta os produtos dos produtos do pedido
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Produto>()
                    .Where("IdProd IN ?subProdutos")
                    .Add("?subProdutos", SourceContext.Instance.CreateQuery()
                        .From<Data.Model.ProdutosPedidoEspelho>()
                        .Where("IdPedido=?idPedido")
                        .Add("?idPedido", pedidoEspelho.IdPedido)
                        .SelectDistinct("IdProd")),
                    (sender, query, result) =>
                        produtos = EntityManager.Instance
                            .ProcessLazyResult<Global.Negocios.Entidades.Produto>(result, SourceContext.Instance)
                            .ToList())

                // Consulta os produtos do pedido que serão usados para o cálculo
                .Add<Data.Model.ProdutosPedidoEspelho>(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ProdutosPedidoEspelho>()
                    .Where("IdPedido=?id AND InvisivelFluxo=0 AND IdProdPedParent IS NULL")
                    .Add("?id", pedidoEspelho.IdPedido),
                    (sender, query, result) =>
                        itens = ObterItensImposto(result, cliente, loja, produtos).ToList())

                .Execute();

            var pedidoImposto = new PedidoEspelhoImpostoContainer(pedidoEspelho, cliente, loja, itens);

            return pedidoImposto;
        }

        #endregion

        #region Membros de Data.ICalculadoraImposto<Data.Model.PedidoEspelho>

        /// <summary>
        /// Realiza o calculo do imposto para a instancia informada.
        /// </summary>
        /// <param name="sessao">Sessão com o banco de dados que será usada para realizar os calculos.</param>
        /// <param name="instancia">Instancia para qual serão calculado os valores.</param>
        /// <returns></returns>
        Data.ICalculoImpostoResultado Data.ICalculadoraImposto<Data.Model.PedidoEspelho>
            .Calcular(GDA.GDASession sessao, Data.Model.PedidoEspelho instancia)
        {
            var pedidoContainer = ObterContainer(instancia);

            var resultado = Calculadora.Calcular(pedidoContainer);
            return new Resultado(pedidoContainer, resultado);
        }

        #endregion

        #region Tipos Aninhados

        /// <summary>
        /// Implementação do resultado do calculo.
        /// </summary>
        class Resultado : Data.ICalculoImpostoResultado
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
                var produtoPedidoImposto = item.Referencia as ProdutoPedidoEspelhoItemImposto;
                if (produtoPedidoImposto == null) return;

                var produtoPedido = produtoPedidoImposto.ProdutoPedido;

                produtoPedido.IdNaturezaOperacao = (uint?)item.NaturezaOperacao?.IdNaturezaOperacao;
                produtoPedido.Mva = produtoPedidoImposto.Mva;
                produtoPedido.CodValorFiscal = produtoPedidoImposto.CodValorFiscal;
                produtoPedido.Csosn = ((int?)produtoPedidoImposto.Csosn) ?? 0;
                produtoPedido.Cst = ((int?)produtoPedidoImposto.Cst) ?? 0;

                produtoPedido.AliqIpi = item.AliqIpi;
                produtoPedido.ValorIpi = item.ValorIpi;
                produtoPedido.CstIpi = produtoPedidoImposto.CstIpi;

                produtoPedido.AliqIcms = item.AliqIcms;
                produtoPedido.BcIcms = item.BcIcms;
                produtoPedido.ValorIcms = item.ValorIcms;
                produtoPedido.PercRedBcIcms = produtoPedidoImposto.PercRedBcIcms;

                produtoPedido.AliqFcp = item.AliqFcp;
                produtoPedido.BcFcp = item.BcFcp;
                produtoPedido.ValorFcp = item.ValorFcp;

                produtoPedido.AliqIcmsSt = item.AliqIcmsSt;
                produtoPedido.BcIcmsSt = item.BcIcmsSt;
                produtoPedido.ValorIcmsSt = item.ValorIcmsSt;

                produtoPedido.AliqFcpSt = item.AliqFcpSt;
                produtoPedido.BcFcpSt = item.BcFcpSt;
                produtoPedido.ValorFcpSt = item.ValorFcpSt;

                produtoPedido.AliqPis = item.AliqPis;
                produtoPedido.BcPis = item.BcPis;
                produtoPedido.ValorPis = item.ValorPis;
                produtoPedido.CstPis = produtoPedidoImposto.CstPis;

                produtoPedido.AliqCofins = item.AliqCofins;
                produtoPedido.BcCofins = item.BcCofins;
                produtoPedido.ValorCofins = item.ValorCofins;
                produtoPedido.CstCofins = produtoPedidoImposto.CstCofins;

                Data.DAL.ProdutosPedidoEspelhoDAO.Instance.AtualizarImpostos(sessao, produtoPedido);

            }

            /// <summary>
            /// Aplica os impostos do resultado do calculo.
            /// </summary>
            /// <param name="sessao"></param>
            /// <param name="resultado"></param>
            private void AplicarImpostos(GDA.GDASession sessao, ICalculoImpostoResultado resultado)
            {
                var pedidoImpostoContainer = resultado.Container as PedidoEspelhoImpostoContainer;
                if (pedidoImpostoContainer == null) return;

                var pedido = pedidoImpostoContainer.PedidoEspelho;

                foreach (var item in resultado.Itens)
                    AplicarImpostos(sessao, item);

                pedido.ValorIpi = resultado.Itens.Sum(f => f.ValorIpi);
                pedido.ValorIcms = resultado.Itens.Sum(f => f.ValorIcms);

                Data.DAL.PedidoEspelhoDAO.Instance.AtualizarImpostos(sessao, pedido);
            }

            #endregion

            #region Métodos Públicos

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
