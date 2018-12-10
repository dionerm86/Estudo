// <copyright file="SemAlteracaoStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.Helper.Estoque.Estrategia.Models;

namespace Glass.Data.Helper.Estoque.Estrategia
{
    /// <summary>
    /// Implementação vazia da estratégia.
    /// </summary>
    internal class SemAlteracaoStrategy : IEstoqueStrategy
    {
        /// <inheritdoc/>
        public void Baixar(GDASession sessao, MovimentacaoDto movimentacao)
        {
            // Method intentionally left empty.
        }

        /// <inheritdoc/>
        public void Creditar(GDASession sessao, MovimentacaoDto movimentacao)
        {
            // Method intentionally left empty.
        }

        /// <inheritdoc/>
        public void ValidarMovimentacao(GDASession sessao, MovimentacaoDto movimentacao)
        {
            // Method intentionally left empty.
        }
    }
}