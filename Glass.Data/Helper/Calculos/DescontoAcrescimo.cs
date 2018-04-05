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
        public bool AplicaAcrescimo(GDASession sessao, int tipoAcrescimo, decimal acrescimo, IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Geral
            );

            return Aplicar(
                sessao,
                estrategia,
                (TipoValor)tipoAcrescimo,
                acrescimo,
                produtos,
                container
            );
        }

        /// <summary>
        /// Aplica acréscimo do ambiente no valor dos produtos.
        /// </summary>
        public bool AplicaAcrescimoAmbiente(GDASession sessao, int tipoAcrescimo, decimal acrescimo, IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Ambiente
            );

            return Aplicar(
                sessao,
                estrategia,
                (TipoValor)tipoAcrescimo,
                acrescimo,
                produtos,
                container
            );
        }

        #endregion

        #region Remove acréscimo no valor dos produtos

        /// <summary>
        /// Remove acréscimo no valor dos produtos.
        /// </summary>
        public bool RemoveAcrescimo(GDASession sessao, IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Geral
            );

            return Remover(
                sessao,
                estrategia,
                produtos,
                container
            );
        }

        /// <summary>
        /// Remove acréscimo do ambiente no valor dos produtos.
        /// </summary>
        public bool RemoveAcrescimoAmbiente(GDASession sessao, IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Ambiente
            );

            return Remover(
                sessao,
                estrategia,
                FiltrarProdutosParaExecucao(produtos, container),
                container
            );
        }

        #endregion

        #region Aplica desconto no valor dos produtos

        /// <summary>
        /// Aplica desconto no valor dos produtos.
        /// </summary>
        public bool AplicaDesconto(GDASession sessao, int tipoDesconto, decimal desconto, IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Geral
            );

            return Aplicar(
                sessao,
                estrategia,
                (TipoValor)tipoDesconto,
                desconto,
                FiltrarProdutosParaExecucao(produtos, container),
                container
            );
        }

        /// <summary>
        /// Aplica desconto do ambiente no valor dos produtos.
        /// </summary>
        public bool AplicaDescontoAmbiente(GDASession sessao, int tipoDesconto, decimal desconto, IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Ambiente
            );

            return Aplicar(
                sessao,
                estrategia,
                (TipoValor)tipoDesconto,
                desconto,
                FiltrarProdutosParaExecucao(produtos, container),
                container
            );
        }

        /// <summary>
        /// Aplica desconto por quantidade no valor dos produtos.
        /// </summary>
        public bool AplicaDescontoQtde(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container)
        {
            if (!DeveExecutarParaOsItens(produto, container))
                return false;

            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Quantidade
            );

            return Aplicar(
                sessao,
                estrategia,
                TipoValor.Percentual,
                (decimal)produto.PercDescontoQtde,
                new[] { produto },
                container
            );
        }

        #endregion

        #region Remove desconto no valor dos produtos

        /// <summary>
        /// Remove desconto no valor dos produtos.
        /// </summary>
        public bool RemoveDesconto(GDASession sessao, IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Geral
            );

            return Remover(
                sessao,
                estrategia,
                FiltrarProdutosParaExecucao(produtos, container),
                container
            );
        }

        /// <summary>
        /// Remove desconto do ambiente no valor dos produtos.
        /// </summary>
        public bool RemoveDescontoAmbiente(GDASession sessao, IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Ambiente
            );

            return Remover(
                sessao,
                estrategia,
                FiltrarProdutosParaExecucao(produtos, container),
                container
            );
        }

        /// <summary>
        /// Remove desconto por quantidade no valor dos produtos.
        /// </summary>
        public bool RemoveDescontoQtde(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container)
        {
            if (!DeveExecutarParaOsItens(produto, container))
                return false;

            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Quantidade
            );

            return Remover(
                sessao,
                estrategia,
                new[] { produto },
                container
            );
        }

        #endregion

        #region Aplica comissão no valor dos produtos

        /// <summary>
        /// Aplica comissão no valor dos produtos.
        /// </summary>
        public bool AplicaComissao(GDASession sessao, float percentualComissao, IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Comissao,
                TipoAplicacao.Geral
            );

            return Aplicar(
                sessao,
                estrategia,
                TipoValor.Percentual,
                (decimal)percentualComissao,
                FiltrarProdutosParaExecucao(produtos, container),
                container
            );
        }

        #endregion

        #region Remove comissão no valor dos produtos

        /// <summary>
        /// Remove comissão no valor dos produtos.
        /// </summary>
        public bool RemoveComissao(GDASession sessao, IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Comissao,
                TipoAplicacao.Geral
            );

            return Remover(
                sessao,
                estrategia,
                FiltrarProdutosParaExecucao(produtos, container),
                container
            );
        }

        #endregion

        #region Métodos privados

        private bool Aplicar(GDASession sessao, IDescontoAcrescimoStrategy estrategia, TipoValor tipoValor, decimal valor,
            IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container)
        {
            bool retorno = estrategia.Aplicar(
                sessao,
                tipoValor,
                valor,
                FiltrarProdutosParaExecucao(produtos, container),
                container
            );

            if (retorno)
            {
                AtualizarDadosCache(produtos, container);
            }

            return retorno;
        }

        private bool Remover(GDASession sessao, IDescontoAcrescimoStrategy estrategia, IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
        {
            bool retorno = estrategia.Remover(
                sessao,
                FiltrarProdutosParaExecucao(produtos, container),
                container
            );

            if (retorno)
            {
                AtualizarDadosCache(produtos, container);
            }

            return retorno;
        }

        #endregion
    }
}