using GDA;
using Glass.Data.Helper.Calculos.Estrategia;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Data.Model;
using Glass.Pool;
using System.Collections.Generic;

namespace Glass.Data.Helper.Calculos
{
    sealed class DescontoAcrescimo : Singleton<DescontoAcrescimo>
    {
        private DescontoAcrescimo() { }

        #region Aplica acréscimo no valor dos produtos

        /// <summary>
        /// Aplica acréscimo no valor dos produtos.
        /// </summary>
        public bool AplicarAcrescimo(GDASession sessao, IContainerCalculo container, int tipoAcrescimo,
            decimal acrescimo, IEnumerable<IProdutoCalculo> produtos)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Geral);

            return Aplicar(sessao, estrategia, container, produtos, (TipoValor)tipoAcrescimo, acrescimo);
        }

        /// <summary>
        /// Aplica acréscimo do ambiente no valor dos produtos.
        /// </summary>
        public bool AplicarAcrescimoAmbiente(GDASession sessao, IContainerCalculo container, int tipoAcrescimo,
            decimal acrescimo, IEnumerable<IProdutoCalculo> produtos)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Ambiente);

            return Aplicar(sessao, estrategia, container, produtos, (TipoValor)tipoAcrescimo, acrescimo);
        }

        #endregion

        #region Remove acréscimo no valor dos produtos

        /// <summary>
        /// Remove acréscimo no valor dos produtos.
        /// </summary>
        public bool RemoverAcrescimo(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                 TipoCalculo.Acrescimo,
                 TipoAplicacao.Geral);

            return Remover(sessao, estrategia, container, produtos);
        }

        /// <summary>
        /// Remove acréscimo do ambiente no valor dos produtos.
        /// </summary>
        public bool RemoverAcrescimoAmbiente(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Ambiente);

            return Remover(sessao, estrategia, container, produtos);
        }

        #endregion

        #region Aplica desconto no valor dos produtos

        /// <summary>
        /// Aplica desconto no valor dos produtos.
        /// </summary>
        public bool AplicarDesconto(GDASession sessao, IContainerCalculo container, int tipoDesconto,
            decimal desconto, IEnumerable<IProdutoCalculo> produtos)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Geral);

            return Aplicar(sessao, estrategia, container, produtos, (TipoValor)tipoDesconto, desconto);
        }

        /// <summary>
        /// Aplica desconto do ambiente no valor dos produtos.
        /// </summary>
        public bool AplicarDescontoAmbiente(GDASession sessao, IContainerCalculo container, int tipoDesconto,
            decimal desconto, IEnumerable<IProdutoCalculo> produtos)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Ambiente);

            return Aplicar(sessao, estrategia, container, produtos, (TipoValor)tipoDesconto, desconto);
        }

        /// <summary>
        /// Aplica desconto por quantidade no valor dos produtos.
        /// </summary>
        public bool AplicarDescontoQtde(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Quantidade);

            return Aplicar(sessao, estrategia, container, new[] { produto }, TipoValor.Percentual, (decimal)produto.PercDescontoQtde);
        }

        #endregion

        #region Remove desconto no valor dos produtos

        /// <summary>
        /// Remove desconto no valor dos produtos.
        /// </summary>
        public bool RemoverDesconto(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Geral);

            return Remover(sessao, estrategia, container, produtos);
        }

        /// <summary>
        /// Remove desconto do ambiente no valor dos produtos.
        /// </summary>
        public bool RemoverDescontoAmbiente(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Ambiente);

            return Remover(sessao, estrategia, container, produtos);
        }

        /// <summary>
        /// Remove desconto por quantidade no valor dos produtos.
        /// </summary>
        public bool RemoverDescontoQtde(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Quantidade);

            return Remover(sessao, estrategia, container, new[] { produto });
        }

        #endregion

        #region Aplica comissão no valor dos produtos

        /// <summary>
        /// Aplica comissão no valor dos produtos.
        /// </summary>
        public bool AplicarComissao(GDASession sessao, IContainerCalculo container, float percentualComissao,
            IEnumerable<IProdutoCalculo> produtos)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Comissao,
                TipoAplicacao.Geral
            );

            return Aplicar(sessao, estrategia, container, produtos, TipoValor.Percentual, (decimal)percentualComissao);
        }

        #endregion

        #region Remove comissão no valor dos produtos

        /// <summary>
        /// Remove comissão no valor dos produtos.
        /// </summary>
        public bool RemoverComissao(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Comissao,
                TipoAplicacao.Geral);

            return Remover(sessao, estrategia, container, produtos);
        }

        #endregion

        #region Métodos de suporte

        private bool Aplicar(GDASession sessao, IDescontoAcrescimoStrategy estrategia, IContainerCalculo container,
            IEnumerable<IProdutoCalculo> produtos, TipoValor tipoValor, decimal valor)
        {
            return estrategia.Aplicar(sessao, container, produtos, tipoValor, valor);
        }

        private bool Remover(GDASession sessao, IDescontoAcrescimoStrategy estrategia, IContainerCalculo container,
            IEnumerable<IProdutoCalculo> produtos)
        {
            return estrategia.Remover(sessao, container, produtos);
        }

        #endregion
    }
}