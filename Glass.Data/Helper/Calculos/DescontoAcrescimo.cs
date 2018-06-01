using GDA;
using Glass.Configuracoes;
using Glass.Data.Helper.Calculos.Estrategia;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Data.Model;
using Glass.Pool;
using System.Collections.Generic;
using System.Linq;

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
                TipoAplicacao.Geral
            );

            bool aplicado = Aplicar(sessao, estrategia, container, produtos, (TipoValor)tipoAcrescimo, acrescimo);

            aplicado = ReaplicarDesconto(sessao, container, produtos, aplicado)
                || aplicado;

            return aplicado;
        }

        /// <summary>
        /// Aplica acréscimo do ambiente no valor dos produtos.
        /// </summary>
        public bool AplicarAcrescimoAmbiente(GDASession sessao, IContainerCalculo container, int tipoAcrescimo,
            decimal acrescimo, IEnumerable<IProdutoCalculo> produtos)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Ambiente
            );

            bool aplicado = Aplicar(sessao, estrategia, container, produtos, (TipoValor)tipoAcrescimo, acrescimo);

            aplicado = ReaplicarComissao(sessao, container, produtos, aplicado)
                || aplicado;

            return aplicado;
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
                TipoAplicacao.Geral
            );

            bool removido = Remover(sessao, estrategia, container, produtos);

            removido = ReaplicarDesconto(sessao, container, produtos, removido)
                || removido;

            return removido;
        }

        /// <summary>
        /// Remove acréscimo do ambiente no valor dos produtos.
        /// </summary>
        public bool RemoverAcrescimoAmbiente(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Ambiente
            );

            bool removido = Remover(sessao, estrategia, container, produtos);

            removido = ReaplicarDesconto(sessao, container, produtos, removido)
                || removido;

            return removido;
        }

        #endregion

        #region Aplica desconto no valor dos produtos

        /// <summary>
        /// Aplica desconto no valor dos produtos.
        /// </summary>
        public bool AplicarDesconto(GDASession sessao, IContainerCalculo container, int tipoDesconto,
            decimal desconto, IEnumerable<IProdutoCalculo> produtos)
        {
            var aplicado = AplicarDesconto(sessao, container, tipoDesconto, desconto, produtos, true);
            RemoverDescontosConfiguracao(sessao, container, produtos);

            return aplicado;
        }

        private bool AplicarDesconto(GDASession sessao, IContainerCalculo container, int tipoDesconto,
            decimal desconto, IEnumerable<IProdutoCalculo> produtos, bool reaplicarComissao)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Geral
            );

            bool aplicado = Aplicar(sessao, estrategia, container, produtos, (TipoValor)tipoDesconto, desconto);

            aplicado = ReaplicarComissao(sessao, container, produtos, reaplicarComissao && aplicado)
                || aplicado;

            return aplicado;
        }

        /// <summary>
        /// Aplica desconto do ambiente no valor dos produtos.
        /// </summary>
        public bool AplicarDescontoAmbiente(GDASession sessao, IContainerCalculo container, int tipoDesconto,
            decimal desconto, IEnumerable<IProdutoCalculo> produtos)
        {
            var aplicado = AplicarDescontoAmbiente(sessao, container, tipoDesconto, desconto, produtos, true);
            RemoverDescontosConfiguracao(sessao, container, produtos);

            return aplicado;
        }

        private bool AplicarDescontoAmbiente(GDASession sessao, IContainerCalculo container, int tipoDesconto,
            decimal desconto, IEnumerable<IProdutoCalculo> produtos, bool reaplicarComissao)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Ambiente
            );

            bool aplicado = Aplicar(sessao, estrategia, container, produtos, (TipoValor)tipoDesconto, desconto);

            aplicado = ReaplicarComissao(sessao, container, produtos, reaplicarComissao && aplicado)
                || aplicado;

            return aplicado;
        }

        /// <summary>
        /// Aplica desconto por quantidade no valor dos produtos.
        /// </summary>
        public bool AplicarDescontoQtde(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto)
        {
            return AplicarDescontoQtde(sessao, container, new[] { produto }, true);
        }

        private bool AplicarDescontoQtde(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos,
            bool reaplicarComissao)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Quantidade
            );

            var produtosAplicar = produtos
                .Where(p => p.PercDescontoQtde > 0)
                .ToList();

            bool aplicado = Aplicar(sessao, estrategia, container, produtosAplicar, TipoValor.Percentual, 1);

            aplicado = ReaplicarComissao(sessao, container, produtos, reaplicarComissao && aplicado)
                || aplicado;

            return aplicado;
        }

        #endregion

        #region Remove desconto no valor dos produtos

        /// <summary>
        /// Remove desconto no valor dos produtos.
        /// </summary>
        public bool RemoverDesconto(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos)
        {
            return RemoverDesconto(sessao, container, produtos, true);
        }

        private bool RemoverDesconto(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos,
            bool reaplicarComissao)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Geral
            );

            bool removido = Remover(sessao, estrategia, container, produtos);

            removido = ReaplicarComissao(sessao, container, produtos,
                reaplicarComissao && (removido || !PedidoConfig.RatearDescontoProdutos))
                || removido;

            return removido;
        }

        /// <summary>
        /// Remove desconto do ambiente no valor dos produtos.
        /// </summary>
        public bool RemoverDescontoAmbiente(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos)
        {
            return RemoverDescontoAmbiente(sessao, container, produtos, true);
        }

        private bool RemoverDescontoAmbiente(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos,
            bool reaplicarComissao)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Ambiente
            );

            bool removido = Remover(sessao, estrategia, container, produtos);

            removido = ReaplicarComissao(sessao, container, produtos,
                reaplicarComissao && (removido || !PedidoConfig.RatearDescontoProdutos))
                || removido;

            return removido;
        }

        /// <summary>
        /// Remove desconto por quantidade no valor dos produtos.
        /// </summary>
        public bool RemoverDescontoQtde(GDASession sessao, IContainerCalculo container, IProdutoCalculo produto)
        {
            return RemoverDescontoQtde(sessao, container, new[] { produto }, true);
        }

        private bool RemoverDescontoQtde(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos,
            bool reaplicarComissao)
        {
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Desconto,
                TipoAplicacao.Quantidade
            );
            
            bool removido = Remover(sessao, estrategia, container, produtos);

            removido = ReaplicarComissao(sessao, container, produtos, reaplicarComissao && removido)
                || removido;

            return removido;
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

            AplicarDescontosConfiguracao(sessao, container, produtos);

            bool aplicado = Aplicar(sessao, estrategia, container, produtos, TipoValor.Percentual, (decimal)percentualComissao);

            RemoverDescontosConfiguracao(sessao, container, produtos);

            return aplicado;
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
                TipoAplicacao.Geral
            );

            return Remover(sessao, estrategia, container, produtos);
        }

        #endregion

        #region Métodos privados

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

        private bool ReaplicarDesconto(GDASession sessao, IContainerCalculo container,
            IEnumerable<IProdutoCalculo> produtos, bool reaplicar)
        {
            if (reaplicar)
            {
                bool reaplicado = ReaplicarDescontosProdutos(sessao, container, produtos);
                reaplicado = AplicarDescontoQtde(sessao, container, produtos, false)
                    || reaplicado;

                reaplicado = ReaplicarComissao(sessao, container, produtos, true)
                    || reaplicado;

                RemoverDescontosConfiguracao(sessao, container, produtos);

                return reaplicado;
            }

            return false;
        }

        private bool ReaplicarDescontosProdutos(GDASession sessao, IContainerCalculo container,
            IEnumerable<IProdutoCalculo> produtos)
        {
            bool reaplicar = false;

            if (container.Desconto > 0)
            {
                reaplicar = AplicarDesconto(sessao, container, container.TipoDesconto, container.Desconto, produtos, false);
            }

            if (container.Ambientes != null)
            {
                foreach (var ambiente in container.Ambientes.Obter())
                {
                    if (ambiente.Desconto == 0)
                        continue;

                    var produtosAmbiente = produtos
                        .Where(produto => produto.IdAmbiente == ambiente.Id)
                        .ToList();

                    reaplicar = AplicarDescontoAmbiente(sessao, container, ambiente.TipoDesconto, ambiente.Desconto, produtosAmbiente, false)
                        || reaplicar;
                }
            }

            return reaplicar;
        }

        private bool ReaplicarComissao(GDASession sessao, IContainerCalculo container,
            IEnumerable<IProdutoCalculo> produtos, bool reaplicar)
        {
            if (reaplicar && container.PercComissao > 0)
            {
                bool aplicado = AplicarComissao(sessao, container, container.PercComissao, produtos);
                RemoverDescontosConfiguracao(sessao, container, produtos);

                return aplicado;
            }

            return false;
        }

        private void AplicarDescontosConfiguracao(GDASession sessao, IContainerCalculo container,
            IEnumerable<IProdutoCalculo> produtos)
        {
            if (!PedidoConfig.RatearDescontoProdutos)
            {
                ReaplicarDescontosProdutos(sessao, container, produtos);
            }
        }

        private void RemoverDescontosConfiguracao(GDASession sessao, IContainerCalculo container,
            IEnumerable<IProdutoCalculo> produtos)
        {
            if (!PedidoConfig.RatearDescontoProdutos)
            {
                RemoverDesconto(sessao, container, produtos, false);
                RemoverDescontoAmbiente(sessao, container, produtos, false);
            }
        }

        #endregion
    }
}