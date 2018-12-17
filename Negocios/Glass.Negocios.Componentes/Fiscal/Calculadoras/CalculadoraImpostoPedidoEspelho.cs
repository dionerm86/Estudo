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
        /// <param name="pedido">Pedido pai.</param>
        /// <param name="produtosPedido"></param>
        /// <param name="cliente"></param>
        /// <param name="loja"></param>
        /// <param name="produtos"></param>
        /// <param name="ambientes">Ambientes do pedido.</param>
        /// <returns></returns>
        private IEnumerable<IItemImposto> ObterItensImposto(
            Data.Model.PedidoEspelho pedido,
            IEnumerable<Data.Model.ProdutosPedidoEspelho> produtosPedido,
            Global.Negocios.Entidades.Cliente cliente, Global.Negocios.Entidades.Loja loja,
            IEnumerable<Global.Negocios.Entidades.Produto> produtos,
            IEnumerable<Data.Model.AmbientePedidoEspelho> ambientes)
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

            var totalPedidoSemDesconto = new Lazy<decimal>(() =>
            {
                var percFastDelivery = 1f;

                if (Configuracoes.PedidoConfig.Pedido_FastDelivery.FastDelivery && pedido.FastDelivery)
                    percFastDelivery = 1 + (Configuracoes.PedidoConfig.Pedido_FastDelivery.TaxaFastDelivery / 100f);

                return Data.DAL.PedidoEspelhoDAO.Instance.GetTotalSemDesconto(pedido.IdPedido, (pedido.Total / (decimal)percFastDelivery));
            });

            foreach (var produtoPedido in produtosPedido)
            {
                var descontoRateadoImpostos = 0m;

                if (!Configuracoes.PedidoConfig.RatearDescontoProdutos)
                {
                    if (pedido.Desconto != 0m)
                    {
                        descontoRateadoImpostos =
                            (pedido.TipoDesconto == 1 ?
                                pedido.Desconto / 100m :
                                pedido.Desconto / Math.Max(totalPedidoSemDesconto.Value, 1)) *
                            (produtoPedido.Total + produtoPedido.ValorBenef);
                    }

                    var ambiente = produtoPedido.IdAmbientePedido.HasValue ?
                        ambientes.FirstOrDefault(f => f.IdAmbientePedido == produtoPedido.IdAmbientePedido.Value) : null;

                    if (ambiente != null && ambiente.Desconto > 0)
                        descontoRateadoImpostos +=
                            (ambiente.TipoDesconto == 1 ?
                                ambiente.Desconto / 100m :
                                ambiente.Desconto / (ambiente.TotalProdutos + ambiente.ValorDescontoAtual)) *
                            (produtoPedido.Total + produtoPedido.ValorBenef);
                }

                var produtoNateruzaOperacao = produtosNaturezaOperacao[(int)produtoPedido.IdProd];

                yield return new ProdutoPedidoEspelhoItemImposto(
                    produtoPedido,
                    loja,
                    cliente,
                    produtoNateruzaOperacao.Item2,
                    produtoNateruzaOperacao.Item1,
                    produtosMva[(int)produtoPedido.IdProd],
                    descontoRateadoImpostos,
                    ProvedorCodValorFiscal);
            }
        }

        /// <summary>
        /// Recupera o container dos itens de impostos do pedido.
        /// </summary>
        /// <param name="pedidoEspelho"></param>
        /// <param name="loja">Instancia da loja associada com o pedido.</param>
        /// <param name="cliente">Instancia do cliente associado com o pedido.</param>
        /// <returns></returns>
        private IItemImpostoContainer ObterContainer(Data.Model.PedidoEspelho pedidoEspelho,
            out Global.Negocios.Entidades.Loja loja, out Global.Negocios.Entidades.Cliente cliente)
        {
            Global.Negocios.Entidades.Cliente cliente1 = null;
            Global.Negocios.Entidades.Loja loja1 = null;
            IEnumerable<Global.Negocios.Entidades.Produto> produtos = null;
            IEnumerable<Data.Model.AmbientePedidoEspelho> ambientes = null;
            IEnumerable<IItemImposto> itens = null;

            SourceContext.Instance.CreateMultiQuery()
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cliente>("c")
                    .InnerJoin<Data.Model.Pedido>("p.IdCli=c.IdCli", "p")
                    .Where("p.IdPedido=?id")
                    .Add("?id", pedidoEspelho.IdPedido),
                    (sender, query, result) =>
                        cliente1 = EntityManager.Instance
                            .ProcessLazyResult<Global.Negocios.Entidades.Cliente>(result, SourceContext.Instance)
                            .FirstOrDefault())

                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Loja>("l")
                    .InnerJoin<Data.Model.Pedido>("p.IdLoja=l.IdLoja", "p")
                    .Where("p.IdPedido=?id")
                    .Add("?id", pedidoEspelho.IdPedido),
                    (sender, query, result) =>
                        loja1 = EntityManager.Instance
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

                .Add<Data.Model.AmbientePedidoEspelho>(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.AmbientePedido>()
                    .Where("IdPedido=?id")
                    .Add("?id", pedidoEspelho.IdPedido),
                    (sender, query, result) =>
                        ambientes = result.ToList())

                // Consulta os produtos do pedido que serão usados para o cálculo
                .Add<Data.Model.ProdutosPedidoEspelho>(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ProdutosPedidoEspelho>()
                    .Where("IdPedido=?id AND InvisivelFluxo=0 AND IdProdPedParent IS NULL")
                    .Add("?id", pedidoEspelho.IdPedido),
                    (sender, query, result) =>
                        itens = ObterItensImposto(pedidoEspelho, result, cliente1, loja1, produtos, ambientes).ToList())

                .Execute();

            var pedidoImposto = new PedidoEspelhoImpostoContainer(pedidoEspelho, cliente1, loja1, itens);

            loja = loja1;
            cliente = cliente1;
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
            Global.Negocios.Entidades.Loja loja;
            Global.Negocios.Entidades.Cliente cliente;
            var pedidoContainer = ObterContainer(instancia, out loja, out cliente);

            var resultado = Calculadora.Calcular(pedidoContainer);
            return new Resultado(pedidoContainer, resultado, loja, cliente);
        }

        #endregion

        #region Tipos Aninhados

        /// <summary>
        /// Implementação do resultado do calculo.
        /// </summary>
        class Resultado : Data.ICalculoImpostoResultado
        {
            #region Propriedades

            /// <summary>
            /// Loja associada com o pedido.
            /// </summary>
            private Global.Negocios.Entidades.Loja Loja { get; }

            /// <summary>
            /// Cliente associado com o pedido.
            /// </summary>
            private Global.Negocios.Entidades.Cliente Cliente { get; }

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
            /// <param name="loja"></param>
            /// <param name="cliente"></param>
            public Resultado(
                IItemImpostoContainer container, ICalculoImpostoResultado resultadoInterno,
                Global.Negocios.Entidades.Loja loja, Global.Negocios.Entidades.Cliente cliente)
            {
                Container = container;
                ResultadoInterno = resultadoInterno;
                Loja = loja;
                Cliente = cliente;
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
                produtoPedido.BcIcmsSemReducao = item.BcIcmsSemReducao;
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

                var atualizarTotalPedido = false;

                if (Loja.CalcularIpiPedido && Cliente.CobrarIpi)
                {
                    pedido.ValorIpi = resultado.Itens.Sum(f => f.ValorIpi);
                    pedido.AliquotaIpi = resultado.Itens.Any(f => f.AliqIpi > 0) ?
                        resultado.Itens.Sum(f => f.AliqIpi) / resultado.Itens.Count(f => f.AliqIpi > 0f) : 0f;

                    pedido.Total += pedido.ValorIpi;
                    atualizarTotalPedido = true;
                }
                else
                {
                    pedido.ValorIpi = 0;
                    pedido.AliquotaIpi = 0;
                }

                if (Loja.CalcularIcmsPedido && Cliente.CobrarIcmsSt)
                {
                    pedido.ValorIcms = resultado.Itens.Sum(f => f.ValorIcmsSt);
                    pedido.AliquotaIcms = resultado.Itens.Any(f => f.AliqIcmsSt > 0) ?
                        resultado.Itens.Sum(f => f.AliqIcmsSt) / resultado.Itens.Count(f => f.AliqIcmsSt > 0f) : 0f;

                    pedido.Total += pedido.ValorIcms;
                    atualizarTotalPedido = true;
                }
                else
                {
                    pedido.ValorIcms = 0;
                    pedido.AliquotaIcms = 0;
                }

                Data.DAL.PedidoEspelhoDAO.Instance.AtualizarImpostos(sessao, pedido, atualizarTotalPedido);
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
