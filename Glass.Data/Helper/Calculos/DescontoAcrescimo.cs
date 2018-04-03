using Glass.Data.Helper.Calculos.Estrategia;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Data.Model;
using Glass.Pool;
using System.Collections.Generic;

namespace Glass.Data.Helper.Calculos
{
    sealed class DescontoAcrescimo : PoolableObject<DescontoAcrescimo>
    {
        private DescontoAcrescimo() { }

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

            return estrategia.Aplicar(
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

            return estrategia.Aplicar(
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

            return estrategia.Remover(
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

            return estrategia.Remover(
                produtos,
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

            return estrategia.Aplicar(
                (TipoValor)tipoDesconto,
                desconto,
                produtos,
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

            return estrategia.Aplicar(
                (TipoValor)tipoDesconto,
                desconto,
                produtos,
                container
            );
        }

        /// <summary>
        /// Aplica desconto por quantidade no valor dos produtos.
        /// </summary>
        public bool AplicaDescontoQtde(IProdutoCalculo produto, IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Quantidade
            );

            return estrategia.Aplicar(
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

            return estrategia.Remover(
                produtos,
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

            return estrategia.Remover(
                produtos,
                container
            );
        }

        /// <summary>
        /// Remove desconto por quantidade no valor dos produtos.
        /// </summary>
        public bool RemoveDescontoQtde(IProdutoCalculo produto, IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Quantidade
            );

            return estrategia.Remover(
                new[] { produto },
                container
            );
        }

        #endregion

        #region Aplica comissão no valor dos produtos

        /// <summary>
        /// Aplica desconto por quantidade no valor dos produtos.
        /// </summary>
        public bool AplicaComissao(float percentualComissao, IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Comissao,
                TipoAplicacao.Geral
            );

            return estrategia.Aplicar(
                TipoValor.Percentual,
                (decimal)percentualComissao,
                produtos,
                container
            );
        }

        #endregion

        #region Remove comissão no valor dos produtos

        /// <summary>
        /// Remove desconto por quantidade no valor dos produtos.
        /// </summary>
        public bool RemoveComissao(IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Comissao,
                TipoAplicacao.Geral
            );

            return estrategia.Remover(
                produtos,
                container
            );
        }

        #endregion
    }
}