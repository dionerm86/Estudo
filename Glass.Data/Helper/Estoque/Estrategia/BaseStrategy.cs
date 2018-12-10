using GDA;
using Glass.Data.Helper.Estoque.Estrategia.Models;
using System;

namespace Glass.Data.Helper.Estoque.Estrategia
{
    /// <summary>
    /// Classe base da estratégia de controle de estoque.
    /// </summary>
    internal abstract class BaseStrategy : IEstoqueStrategy
    {
        /// <inheritdoc/>
        public void Baixar(GDASession sessao, MovimentacaoDto movimentacao)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Creditar(GDASession sessao, MovimentacaoDto movimentacao)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void ValidarMovimentacao(GDASession sessao, MovimentacaoDto movimentacao)
        {
            throw new NotImplementedException();
        }
    }
}