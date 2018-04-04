using Glass.Data.Helper.Calculos.Estrategia;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.Helper.Calculos
{
    sealed class DescontoAcrescimo : BaseCalculo<DescontoAcrescimo>
    {
        private DescontoAcrescimo()
            : base("descontoAcrescimo")
        {
        }

        #region Aplica acréscimo no valor dos produtos

        /// <summary>
        /// Aplica acréscimo no valor dos produtos.
        /// </summary>
        public bool AplicaAcrescimo(int tipoAcrescimo, decimal acrescimo, IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Geral
            );

            return Aplicar(
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
        public bool AplicaAcrescimoAmbiente(int tipoAcrescimo, decimal acrescimo, IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Ambiente
            );

            return Aplicar(
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
        public bool RemoveAcrescimo(IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Geral
            );

            return Remover(
                estrategia,
                produtos,
                container
            );
        }

        /// <summary>
        /// Remove acréscimo do ambiente no valor dos produtos.
        /// </summary>
        public bool RemoveAcrescimoAmbiente(IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Ambiente
            );

            return Remover(
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
        public bool AplicaDesconto(int tipoDesconto, decimal desconto, IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Geral
            );

            return Aplicar(
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
        public bool AplicaDescontoAmbiente(int tipoDesconto, decimal desconto, IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Ambiente
            );

            return Aplicar(
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
        public bool AplicaDescontoQtde(IProdutoCalculo produto, IContainerCalculo container)
        {
            if (!DeveExecutarParaOsItens(produto, container))
                return false;

            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Quantidade
            );

            return Aplicar(
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
        public bool RemoveDesconto(IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Geral
            );

            return Remover(
                estrategia,
                FiltrarProdutosParaExecucao(produtos, container),
                container
            );
        }

        /// <summary>
        /// Remove desconto do ambiente no valor dos produtos.
        /// </summary>
        public bool RemoveDescontoAmbiente(IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Ambiente
            );

            return Remover(
                estrategia,
                FiltrarProdutosParaExecucao(produtos, container),
                container
            );
        }

        /// <summary>
        /// Remove desconto por quantidade no valor dos produtos.
        /// </summary>
        public bool RemoveDescontoQtde(IProdutoCalculo produto, IContainerCalculo container)
        {
            if (!DeveExecutarParaOsItens(produto, container))
                return false;

            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Quantidade
            );

            return Remover(
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
        public bool AplicaComissao(float percentualComissao, IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Comissao,
                TipoAplicacao.Geral
            );

            return Aplicar(
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
        public bool RemoveComissao(IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Comissao,
                TipoAplicacao.Geral
            );

            return Remover(
                estrategia,
                FiltrarProdutosParaExecucao(produtos, container),
                container
            );
        }

        #endregion

        #region Métodos privados

        private bool Aplicar(IDescontoAcrescimoStrategy estrategia, TipoValor tipoValor, decimal valor,
            IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container)
        {
            bool retorno = estrategia.Aplicar(
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

        private bool Remover(IDescontoAcrescimoStrategy estrategia, IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
        {
            bool retorno = estrategia.Remover(
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