using GDA;
using Glass.Data.Helper.Calculos.Estrategia;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.Helper.Calculos
{
    sealed class DescontoAcrescimo : BaseCalculo<DescontoAcrescimo>
    {
        private DescontoAcrescimo() { }

        #region Aplica acréscimo no valor dos produtos

        /// <summary>
        /// Aplica acréscimo no valor dos produtos.
        /// </summary>
        public bool AplicaAcrescimo(GDASession sessao, IContainerCalculo container, int tipoAcrescimo,
            decimal acrescimo, IEnumerable<IProdutoCalculo> produtos)
        {
            AtualizaDadosProdutosCalculo(produtos, sessao, container);

            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Geral
            );

            return Aplicar(sessao, estrategia, (TipoValor)tipoAcrescimo, acrescimo, produtos);
        }

        /// <summary>
        /// Aplica acréscimo do ambiente no valor dos produtos.
        /// </summary>
        public bool AplicaAcrescimoAmbiente(GDASession sessao, IContainerCalculo container, int tipoAcrescimo,
            decimal acrescimo, IEnumerable<IProdutoCalculo> produtos)
        {
            AtualizaDadosProdutosCalculo(produtos, sessao, container);

            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Ambiente
            );

            return Aplicar(sessao, estrategia, (TipoValor)tipoAcrescimo, acrescimo, produtos);
        }

        #endregion

        #region Remove acréscimo no valor dos produtos

        /// <summary>
        /// Remove acréscimo no valor dos produtos.
        /// </summary>
        public bool RemoveAcrescimo(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos)
        {
            AtualizaDadosProdutosCalculo(produtos, sessao, container);

            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Geral
            );

            return Remover(sessao, estrategia, produtos);
        }

        /// <summary>
        /// Remove acréscimo do ambiente no valor dos produtos.
        /// </summary>
        public bool RemoveAcrescimoAmbiente(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos)
        {
            AtualizaDadosProdutosCalculo(produtos, sessao, container);

            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Ambiente
            );

            return Remover(sessao, estrategia, produtos);
        }

        #endregion

        #region Aplica desconto no valor dos produtos

        /// <summary>
        /// Aplica desconto no valor dos produtos.
        /// </summary>
        public bool AplicaDesconto(GDASession sessao, IContainerCalculo container, int tipoDesconto,
            decimal desconto, IEnumerable<IProdutoCalculo> produtos)
        {
            AtualizaDadosProdutosCalculo(produtos, sessao, container);

            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Geral
            );

            return Aplicar(sessao, estrategia, (TipoValor)tipoDesconto, desconto, produtos);
        }

        /// <summary>
        /// Aplica desconto do ambiente no valor dos produtos.
        /// </summary>
        public bool AplicaDescontoAmbiente(GDASession sessao, IContainerCalculo container, int tipoDesconto,
            decimal desconto, IEnumerable<IProdutoCalculo> produtos)
        {
            AtualizaDadosProdutosCalculo(produtos, sessao, container);

            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Ambiente
            );

            return Aplicar(sessao, estrategia, (TipoValor)tipoDesconto, desconto, produtos);
        }

        /// <summary>
        /// Aplica desconto por quantidade no valor dos produtos.
        /// </summary>
        public bool AplicaDescontoQtde(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto)
        {
            AtualizaDadosProdutosCalculo(produto, sessao, container);

            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Quantidade
            );

            return Aplicar(sessao, estrategia, TipoValor.Percentual, (decimal)produto.PercDescontoQtde, new[] { produto });
        }

        #endregion

        #region Remove desconto no valor dos produtos

        /// <summary>
        /// Remove desconto no valor dos produtos.
        /// </summary>
        public bool RemoveDesconto(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos)
        {
            AtualizaDadosProdutosCalculo(produtos, sessao, container);

            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Geral
            );

            return Remover(sessao, estrategia, produtos);
        }

        /// <summary>
        /// Remove desconto do ambiente no valor dos produtos.
        /// </summary>
        public bool RemoveDescontoAmbiente(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos)
        {
            AtualizaDadosProdutosCalculo(produtos, sessao, container);

            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Ambiente
            );

            return Remover(sessao, estrategia, produtos);
        }

        /// <summary>
        /// Remove desconto por quantidade no valor dos produtos.
        /// </summary>
        public bool RemoveDescontoQtde(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto)
        {
            AtualizaDadosProdutosCalculo(produto, sessao, container);

            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Quantidade
            );

            return Remover(sessao, estrategia, new[] { produto });
        }

        #endregion

        #region Aplica comissão no valor dos produtos

        /// <summary>
        /// Aplica comissão no valor dos produtos.
        /// </summary>
        public bool AplicaComissao(GDASession sessao, IContainerCalculo container, float percentualComissao,
            IEnumerable<IProdutoCalculo> produtos)
        {
            AtualizaDadosProdutosCalculo(produtos, sessao, container);

            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Comissao,
                TipoAplicacao.Geral
            );

            return Aplicar(sessao, estrategia, TipoValor.Percentual, (decimal)percentualComissao, produtos);
        }

        #endregion

        #region Remove comissão no valor dos produtos

        /// <summary>
        /// Remove comissão no valor dos produtos.
        /// </summary>
        public bool RemoveComissao(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos)
        {
            AtualizaDadosProdutosCalculo(produtos, sessao, container);

            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Comissao,
                TipoAplicacao.Geral
            );

            return Remover(sessao, estrategia, produtos);
        }

        #endregion

        #region Métodos privados

        private bool Aplicar(GDASession sessao, IDescontoAcrescimoStrategy estrategia, TipoValor tipoValor, decimal valor,
            IEnumerable<IProdutoCalculo> produtos)
        {
            return estrategia.Aplicar(sessao, tipoValor, valor, produtos);
        }

        private bool Remover(GDASession sessao, IDescontoAcrescimoStrategy estrategia, IEnumerable<IProdutoCalculo> produtos)
        {
            return estrategia.Remover(sessao, produtos);
        }

        #endregion
    }
}