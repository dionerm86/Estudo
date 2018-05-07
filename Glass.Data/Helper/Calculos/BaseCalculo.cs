using GDA;
using Glass.Data.Helper.Calculos.Cache;
using Glass.Data.Model;
using Glass.Data.Model.Calculos;
using Glass.Pool;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.Helper.Calculos
{
    abstract class BaseCalculo<T> : Singleton<T>
        where T : BaseCalculo<T>
    {
        private readonly CacheCalculo<IProdutoCalculo, uint> cacheProdutos;

        protected BaseCalculo()
        {
            cacheProdutos = new CacheCalculo<IProdutoCalculo, uint>(
                "produtos",
                produto => produto.Id
            );
        }

        protected void AtualizaDadosProdutosCalculo(IEnumerable<IProdutoCalculo> produtosCalculo,
            GDASession sessao, IContainerCalculo container)
        {
            foreach (var produtoCalculo in produtosCalculo)
            {
                AtualizaDadosProdutosCalculo(produtoCalculo, sessao, container);
            }
        }

        protected void AtualizaDadosProdutosCalculo(IProdutoCalculo produtoCalculo, GDASession sessao,
            IContainerCalculo container)
        {
            produtoCalculo.InicializarParaCalculo(sessao, container);
        }

        protected IEnumerable<IProdutoCalculo> FiltrarProdutosParaExecucao(IEnumerable<IProdutoCalculo> produtos)
        {
            if (produtos == null)
                return new IProdutoCalculo[] { };

            return produtos
                .Where(produto => DeveExecutar(produto))
                .ToList();
        }

        protected bool DeveExecutar(IProdutoCalculo produto)
        {
            if (produto == null)
                return false;

            return !cacheProdutos.ItemEstaNoCache(produto);
        }

        protected void AtualizarDadosCache(IEnumerable<IProdutoCalculo> produtos)
        {
            foreach (var produto in produtos)
            {
                AtualizarDadosCache(produto);
            }
        }

        protected void AtualizarDadosCache(IProdutoCalculo produto)
        {
            cacheProdutos.AtualizarItemNoCache(produto);
        }
    }
}
