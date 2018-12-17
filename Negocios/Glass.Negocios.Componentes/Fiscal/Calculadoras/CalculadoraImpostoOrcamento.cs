using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colosoft.Business;

namespace Glass.Fiscal.Negocios.Componentes.Calculadoras
{
    /// <summary>
    /// Implementação da calculadora de imposto do orçamento.
    /// </summary>
    public class CalculadoraImpostoOrcamento :
        Data.ICalculadoraImposto<Data.Model.Orcamento>
    {
        #region Propriedades

        /// <summary>
        /// Calculadora base dos impostos.
        /// </summary>
        private ICalculadoraImposto Calculadora { get; }

        /// <summary>
        /// Localizador das naturezas de operação.
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

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="calculadora"></param>
        /// <param name="localizadorNaturezaOperacao"></param>
        /// <param name="provedorMvaProdutoUf"></param>
        public CalculadoraImpostoOrcamento(
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
        /// Recupera os itens de impostos com base nos produtos do orçamento.
        /// </summary>
        /// <param name="orcamento">Orçamento pai.</param>
        /// <param name="produtosOrcamento"></param>
        /// <param name="cliente"></param>
        /// <param name="loja"></param>
        /// <param name="produtos"></param>
        /// <param name="ambientes">Ambientes.</param>
        /// <returns></returns>
        private IEnumerable<IItemImposto> ObterItensImposto(
            Data.Model.Orcamento orcamento,
            IEnumerable<Data.Model.ProdutosOrcamento> produtosOrcamento,
            Global.Negocios.Entidades.Cliente cliente, Global.Negocios.Entidades.Loja loja,
            IEnumerable<Global.Negocios.Entidades.Produto> produtos,
            IEnumerable<Data.Model.ProdutosOrcamento> produtosAmbienteOrcamento)
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

            var totalOrcamentoSemDesconto = new Lazy<decimal>(() =>
            {
                return Data.DAL.OrcamentoDAO.Instance.ObterTotalSemDesconto(null, (int)orcamento.IdOrcamento, orcamento.Total);
            });

            foreach (var produtoOrcamento in produtosOrcamento)
            {
                var descontoRateadoImpostos = 0m;
                var descontoAmbienteRateadoImpostos = 0m;

                if (!Configuracoes.PedidoConfig.RatearDescontoProdutos)
                {
                    if (orcamento.Desconto != 0m)
                    {
                        if (orcamento.TipoDesconto == 1)
                        {
                            descontoRateadoImpostos += (produtoOrcamento.Total.GetValueOrDefault() + produtoOrcamento.ValorBenef) *
                                orcamento.Desconto / 100m;
                        }
                        else
                        {
                            descontoRateadoImpostos += (produtoOrcamento.Total.GetValueOrDefault() + produtoOrcamento.ValorBenef) *
                                (orcamento.Desconto / Math.Max(totalOrcamentoSemDesconto.Value, 1));
                        }
                    }

                    var produtoAmbienteOrcamento = produtoOrcamento.IdProdParent > 0 ?
                        produtosAmbienteOrcamento.FirstOrDefault(f => f.IdProd == produtoOrcamento.IdProdParent.Value) : null;

                    if (produtoAmbienteOrcamento?.IdProd > 0)
                    {
                        produtoAmbienteOrcamento.TotalProdutos = (produtosOrcamento?.Where(f => f.IdProdParent == produtoAmbienteOrcamento.IdProd)?.Sum(f => f.Total + f.ValorBenef)).GetValueOrDefault();
                    }

                    if (produtoAmbienteOrcamento?.Desconto > 0)
                    {
                        if (produtoAmbienteOrcamento.TipoDesconto == 1)
                        {
                            descontoAmbienteRateadoImpostos += (produtoOrcamento.Total.GetValueOrDefault() + produtoOrcamento.ValorBenef) *
                                (produtoAmbienteOrcamento.Desconto / 100m);
                        }
                        else
                        {
                            descontoAmbienteRateadoImpostos += (produtoOrcamento.Total.GetValueOrDefault() + produtoOrcamento.ValorBenef) *
                                (produtoAmbienteOrcamento.Desconto / (produtoAmbienteOrcamento.TotalProdutos + produtoAmbienteOrcamento.ValorDescontoAtualAmbiente));
                        }
                    }
                }

                var produtoNateruzaOperacao = produtosNaturezaOperacao[(int)produtoOrcamento.IdProduto];

                yield return new ProdutoOrcamentoItemImposto(
                    produtoOrcamento,
                    loja, cliente,
                    produtoNateruzaOperacao.Item2,
                    produtoNateruzaOperacao.Item1,
                    produtosMva[(int)produtoOrcamento.IdProduto],
                    descontoRateadoImpostos,
                    ProvedorCodValorFiscal);
            }
        }

        /// <summary>
        /// Recupera o container dos itens de impostos do orçamento.
        /// </summary>
        /// <param name="orçamento"></param>
        /// <param name="loja">Instancia da loja associada com o orçamento.</param>
        /// <param name="cliente">Instancia do cliente associado com o orçamento.</param>
        /// <returns></returns>
        private IItemImpostoContainer ObterContainer(Data.Model.Orcamento orcamento,
            out Global.Negocios.Entidades.Loja loja, out Global.Negocios.Entidades.Cliente cliente)
        {
            Global.Negocios.Entidades.Cliente cliente1 = null;
            Global.Negocios.Entidades.Loja loja1 = null;
            IEnumerable<Global.Negocios.Entidades.Produto> produtos = null;
            IEnumerable<Data.Model.ProdutosOrcamento> produtosAmbienteOrcamento = null;
            IEnumerable<Data.Model.ProdutosOrcamento> produtosOrcamento = null;
            IEnumerable<IItemImposto> itens = null;

            produtosAmbienteOrcamento = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ProdutosOrcamento>()
                .Where("IdOrcamento=?id AND (IdProdParent IS NULL OR IdProdParent = 0)")
                .Add("?id", orcamento.IdOrcamento)
                .Execute<Data.Model.ProdutosOrcamento>().ToList();

            produtosOrcamento = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ProdutosOrcamento>()
                .Where("IdOrcamento=?id AND IdProdParent IS NOT NULL AND IdProdParent > 0 AND (IdProdOrcamentoParent IS NULL OR IdProdOrcamentoParent = 0)")
                .Add("?id", orcamento.IdOrcamento)
                .Execute<Data.Model.ProdutosOrcamento>().ToList();

            SourceContext.Instance.CreateMultiQuery()
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cliente>()
                    .Where("IdCli=?id")
                    .Add("?id", orcamento.IdCliente),
                    (sender, query, result) =>
                        cliente1 = EntityManager.Instance
                            .ProcessLazyResult<Global.Negocios.Entidades.Cliente>(result, SourceContext.Instance)
                            .FirstOrDefault())

                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Loja>()
                    .Where("IdLoja=?id")
                    .Add("?id", orcamento.IdLoja),
                    (sender, query, result) =>
                        loja1 = EntityManager.Instance
                            .ProcessLazyResult<Global.Negocios.Entidades.Loja>(result, SourceContext.Instance)
                            .FirstOrDefault())

                // Consulta os produtos dos produtos do orçamento
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Produto>()
                    .Where("IdProd IN ?subProdutos")
                    .Add("?subProdutos", SourceContext.Instance.CreateQuery()
                        .From<Data.Model.ProdutosOrcamento>()
                        .Where("IdOrcamento=?idOrcamento")
                        .Add("?idOrcamento", orcamento.IdOrcamento)
                        .SelectDistinct("IdProduto")),
                    (sender, query, result) =>
                        produtos = EntityManager.Instance
                            .ProcessLazyResult<Global.Negocios.Entidades.Produto>(result, SourceContext.Instance)
                            .ToList())
                .Execute();

            itens = ObterItensImposto(orcamento, produtosOrcamento, cliente1, loja1, produtos, produtosAmbienteOrcamento).ToList();

            var orcamentoImposto = new OrcamentoImpostoContainer(orcamento, cliente1, loja1, itens);

            loja = loja1;
            cliente = cliente1;
            return orcamentoImposto;
        }

        #endregion

        #region Membros de Data.ICalculadoraImposto<Data.Model.Orcamento>

        /// <summary>
        /// Realiza o calculo do imposto para a instancia informada.
        /// </summary>
        /// <param name="sessao">Sessão com o banco de dados que será usada para realizar os calculos.</param>
        /// <param name="instancia">Instancia para qual serão calculado os valores.</param>
        /// <returns></returns>
        Data.ICalculoImpostoResultado Data.ICalculadoraImposto<Data.Model.Orcamento>.Calcular(GDA.GDASession sessao, Data.Model.Orcamento instancia)
        {
            Global.Negocios.Entidades.Loja loja;
            Global.Negocios.Entidades.Cliente cliente;
            var orcamentoContainer = ObterContainer(instancia, out loja, out cliente);

            var resultado = Calculadora.Calcular(orcamentoContainer);
            return new Resultado(orcamentoContainer, resultado, loja, cliente);
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
            /// Loja associada com o orçamento.
            /// </summary>
            private Global.Negocios.Entidades.Loja Loja { get; }

            /// <summary>
            /// Cliente associado com o orçamento.
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
                var produtoOrcamentoImposto = item.Referencia as ProdutoOrcamentoItemImposto;
                if (produtoOrcamentoImposto == null) return;

                var produtoOrcamento = produtoOrcamentoImposto.ProdutoOrcamento;

                produtoOrcamento.IdNaturezaOperacao = (uint?)item.NaturezaOperacao?.IdNaturezaOperacao;
                produtoOrcamento.Mva = produtoOrcamentoImposto.Mva;
                produtoOrcamento.CodValorFiscal = produtoOrcamentoImposto.CodValorFiscal;
                produtoOrcamento.Csosn = ((int?)produtoOrcamentoImposto.Csosn) ?? 0;
                produtoOrcamento.Cst = ((int?)produtoOrcamentoImposto.Cst) ?? 0;

                produtoOrcamento.AliquotaIpi = item.AliqIpi;
                produtoOrcamento.ValorIpi = item.ValorIpi;
                produtoOrcamento.CstIpi = produtoOrcamentoImposto.CstIpi;

                produtoOrcamento.AliquotaIcms = item.AliqIcms;
                produtoOrcamento.BcIcms = item.BcIcms;
                produtoOrcamento.BcIcmsSemReducao = item.BcIcmsSemReducao;
                produtoOrcamento.ValorIcms = item.ValorIcms;
                produtoOrcamento.PercRedBcIcms = produtoOrcamentoImposto.PercRedBcIcms;

                produtoOrcamento.AliqFcp = item.AliqFcp;
                produtoOrcamento.BcFcp = item.BcFcp;
                produtoOrcamento.ValorFcp = item.ValorFcp;

                produtoOrcamento.AliqIcmsSt = item.AliqIcmsSt;
                produtoOrcamento.BcIcmsSt = item.BcIcmsSt;
                produtoOrcamento.ValorIcmsSt = item.ValorIcmsSt;

                produtoOrcamento.AliqFcpSt = item.AliqFcpSt;
                produtoOrcamento.BcFcpSt = item.BcFcpSt;
                produtoOrcamento.ValorFcpSt = item.ValorFcpSt;

                produtoOrcamento.AliqPis = item.AliqPis;
                produtoOrcamento.BcPis = item.BcPis;
                produtoOrcamento.ValorPis = item.ValorPis;
                produtoOrcamento.CstPis = produtoOrcamentoImposto.CstPis;

                produtoOrcamento.AliqCofins = item.AliqCofins;
                produtoOrcamento.BcCofins = item.BcCofins;
                produtoOrcamento.ValorCofins = item.ValorCofins;
                produtoOrcamento.CstCofins = produtoOrcamentoImposto.CstCofins;

                Data.DAL.ProdutosOrcamentoDAO.Instance.AtualizarImpostos(sessao, produtoOrcamento);

            }

            /// <summary>
            /// Aplica os impostos do resultado do calculo.
            /// </summary>
            /// <param name="sessao"></param>
            /// <param name="resultado"></param>
            private void AplicarImpostos(GDA.GDASession sessao, ICalculoImpostoResultado resultado)
            {
                var orcamentoImpostoContainer = resultado.Container as OrcamentoImpostoContainer;
                if (orcamentoImpostoContainer == null) return;

                var orcamento = orcamentoImpostoContainer.Orcamento;

                foreach (var item in resultado.Itens)
                    AplicarImpostos(sessao, item);

                var atualizarTotalOrcamento = false;
                if (Loja.CalcularIpiPedido && Cliente.CobrarIpi)
                {
                    orcamento.ValorIpi = resultado.Itens.Sum(f => f.ValorIpi);
                    orcamento.AliquotaIpi = resultado.Itens.Any(f => f.AliqIpi > 0) ?
                        resultado.Itens.Sum(f => f.AliqIpi) / resultado.Itens.Count(f => f.AliqIpi > 0f) : 0f;

                    orcamento.Total += orcamento.ValorIpi;
                    atualizarTotalOrcamento = true;
                }
                else
                {
                    orcamento.ValorIpi = 0;
                    orcamento.AliquotaIpi = 0;
                }

                if (Loja.CalcularIcmsPedido && Cliente.CobrarIcmsSt)
                {
                    orcamento.ValorIcms = resultado.Itens.Sum(f => f.ValorIcmsSt);
                    orcamento.AliquotaIcms = resultado.Itens.Any(f => f.AliqIcmsSt > 0) ?
                        resultado.Itens.Sum(f => f.AliqIcmsSt) / resultado.Itens.Count(f => f.AliqIcmsSt > 0f) : 0f;

                    orcamento.Total += orcamento.ValorIcms;
                    atualizarTotalOrcamento = true;
                }
                else
                {
                    orcamento.ValorIcms = 0;
                    orcamento.AliquotaIcms = 0;
                }

                Data.DAL.OrcamentoDAO.Instance.AtualizarImpostos(sessao, orcamento, atualizarTotalOrcamento);
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
