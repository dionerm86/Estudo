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

            ReaplicarDesconto(sessao, container, produtos, aplicado);

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

            ReaplicarComissao(sessao, container, produtos, aplicado);

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

            ReaplicarDesconto(sessao, container, produtos, removido);

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

            ReaplicarDesconto(sessao, container, produtos, removido);

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
            bool aplicado = AplicarDesconto(sessao, container, tipoDesconto, desconto, produtos, true);
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

            ReaplicarComissao(sessao, container, produtos, reaplicarComissao && aplicado);

            return aplicado;
        }

        /// <summary>
        /// Aplica desconto do ambiente no valor dos produtos.
        /// </summary>
        public bool AplicarDescontoAmbiente(GDASession sessao, IContainerCalculo container, int tipoDesconto,
            decimal desconto, IEnumerable<IProdutoCalculo> produtos)
        {
            bool aplicado = AplicarDescontoAmbiente(sessao, container, tipoDesconto, desconto, produtos, true);
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

            ReaplicarComissao(sessao, container, produtos, reaplicarComissao && aplicado);

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

            ReaplicarComissao(sessao, container, produtos, reaplicarComissao && aplicado);

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
            ReaplicarComissao(sessao, container, produtos, reaplicarComissao && removido);

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
            ReaplicarComissao(sessao, container, produtos, reaplicarComissao && removido);

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
            ReaplicarComissao(sessao, container, produtos, reaplicarComissao && removido);

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

        private void ReaplicarDesconto(GDASession sessao, IContainerCalculo container,
            IEnumerable<IProdutoCalculo> produtos, bool reaplicar)
        {
            if (reaplicar)
            {
                ReaplicarDescontosProdutos(sessao, container, produtos);
                AplicarDescontoQtde(sessao, container, produtos, false);

                ReaplicarComissao(sessao, container, produtos, true);
            }
        }

        private void ReaplicarDescontosProdutos(GDASession sessao, IContainerCalculo container,
            IEnumerable<IProdutoCalculo> produtos)
        {
            if (container.Desconto > 0)
            {
                AplicarDesconto(sessao, container, container.TipoDesconto, container.Desconto, produtos, false);
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

                    AplicarDescontoAmbiente(sessao, container, ambiente.TipoDesconto, ambiente.Desconto, produtosAmbiente, false);
                }
            }
        }

        private void ReaplicarComissao(GDASession sessao, IContainerCalculo container,
            IEnumerable<IProdutoCalculo> produtos, bool reaplicar)
        {
            if (reaplicar && container.PercComissao > 0)
            {
                AplicarComissao(sessao, container, container.PercComissao, produtos);
            }
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