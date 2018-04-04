using Glass.Data.Helper.Calculos.Cache;
using Glass.Data.Model;
using Glass.Pool;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.Helper.Calculos
{
    abstract class BaseCalculo<T> : Singleton<T>
        where T : BaseCalculo<T>
    {
        private readonly CacheCalculo<IProdutoCalculo> cacheProdutos;
        private readonly CacheCalculo<IContainerCalculo> cacheContainer;

        protected BaseCalculo(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome não pode ser nulo ou vazio.", "nome");

            cacheProdutos = new CacheCalculo<IProdutoCalculo>(
                string.Format("{0}-produtos", nome), 
                produto => string.Format("{0}:{1}", produto.ToString(), produto.Id.ToString())
            );

            cacheContainer = new CacheCalculo<IContainerCalculo>(
                string.Format("{0}-container", nome),
                container => string.Format("{0}:{1}", container.ToString(), container.Id.ToString())
            );
        }

        protected IEnumerable<IProdutoCalculo> FiltrarProdutosParaExecucao(IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
        {
            if (produtos == null || !DeveExecutarParaOContainer(container))
                return new IProdutoCalculo[] { };

            return produtos
                .Where(produto => DeveExecutarParaOProduto(produto))
                .ToList();
        }

        protected bool DeveExecutarParaOsItens(IProdutoCalculo produto, IContainerCalculo container)
        {
            return DeveExecutarParaOProduto(produto)
                || DeveExecutarParaOContainer(container);
        }

        protected void AtualizarDadosCache(IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
        {
            AtualizarDadosContainerCache(container);

            foreach (var produto in produtos)
            {
                AtualizarDadosProdutoCache(produto);
            }
        }

        protected void AtualizarDadosCache(IProdutoCalculo produto, IContainerCalculo container)
        {
            AtualizarDadosProdutoCache(produto);
            AtualizarDadosContainerCache(container);
        }

        private bool DeveExecutarParaOProduto(IProdutoCalculo produto)
        {
            if (produto == null)
                return false;

            return !cacheProdutos.ItemEstaNoCache(produto);
        }

        private bool DeveExecutarParaOContainer(IContainerCalculo container)
        {
            if (container == null)
                return false;

            return !cacheContainer.ItemEstaNoCache(container);
        }

        private void AtualizarDadosProdutoCache(IProdutoCalculo produto)
        {
            cacheProdutos.AtualizarItemNoCache(produto);
        }

        private void AtualizarDadosContainerCache(IContainerCalculo container)
        {
            cacheContainer.AtualizarItemNoCache(container);
        }
    }
}
