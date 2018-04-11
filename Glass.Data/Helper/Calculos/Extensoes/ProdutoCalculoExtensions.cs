using GDA;
using Glass.Data.Model;
using Glass.Data.Model.Calculos;

namespace Glass.Data.Helper.Calculos
{
    public static class ProdutoCalculoExtensions
    {
        /// <summary>
        /// Cria no produto cálculo as variáveis necessárias para permitir seu uso nos métodos de cálculo.
        /// </summary>
        public static void InicializarParaCalculo(this IProdutoCalculo produtoCalculo, GDASession sessao,
            IContainerCalculo container)
        {
            produtoCalculo.Container = container;
            produtoCalculo.DadosProduto = new DadosProdutoDTO(sessao, produtoCalculo);
        }
    }
}
