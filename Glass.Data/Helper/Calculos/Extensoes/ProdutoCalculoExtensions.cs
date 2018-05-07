using GDA;
using Glass.Data.Model;
using Glass.Data.Model.Calculos;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.Helper.Calculos
{
    public static class ProdutoCalculoExtensions
    {
        /// <summary>
        /// Cria em cada produto cálculo da lista as variáveis necessárias para permitir seu uso nos métodos de cálculo.
        /// </summary>
        public static void InicializarParaCalculo(this IEnumerable<IProdutoCalculo> produtosCalculo, GDASession sessao,
            IContainerCalculo container)
        {
            foreach (var produto in (produtosCalculo ?? new IProdutoCalculo[0]))
                InicializarParaCalculo(produto, sessao, container);
        }

        /// <summary>
        /// Cria no produto cálculo as variáveis necessárias para permitir seu uso nos métodos de cálculo.
        /// </summary>
        public static void InicializarParaCalculo(this IProdutoCalculo produtoCalculo, GDASession sessao,
            IContainerCalculo container)
        {
            produtoCalculo.Container = container;
            produtoCalculo.DadosProduto = new DadosProdutoDTO(sessao, produtoCalculo);

            ObterAmbientes(container, produtoCalculo);
        }

        private static void ObterAmbientes(IContainerCalculo container, IProdutoCalculo produtoCalculo)
        {
            if (!produtoCalculo.IdAmbiente.HasValue || produtoCalculo.IdAmbiente == 0)
                return;

            IEnumerable<IAmbienteCalculo> ambientes = container.Ambientes.Obter();
            if (ambientes == null)
                return;

            produtoCalculo.Ambiente = ambientes.FirstOrDefault(ambiente => ambiente.Id == produtoCalculo.IdAmbiente);
        }
    }
}
