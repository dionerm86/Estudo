// <copyright file="IEstoqueStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.Helper.Estoque.Estrategia.Models;

namespace Glass.Data.Helper.Estoque.Estrategia
{
    /// <summary>
    /// Classe responsável por controlar o fluxo de estoque do sistema.
    /// </summary>
    public interface IEstoqueStrategy
    {
        /// <summary>
        /// Realiza a baixa do produto no estoque.
        /// </summary>
        /// <param name="sessao">Sessão do banco de dados.</param>
        /// <param name="movimentacao">Dados da movimentação.</param>
        void Baixar(GDASession sessao, MovimentacaoDto movimentacao);

        /// <summary>
        /// Realiza crédito do produto no estoque.
        /// </summary>
        /// <param name="sessao">Sessão do banco de dados.</param>
        /// <param name="movimentacao">Dados da movimentação.</param>
        void Creditar(GDASession sessao, MovimentacaoDto movimentacao);

        /// <summary>
        /// Valida a movimentação de estoque.
        /// </summary>
        /// <param name="sessao">Sessão do banco de dados.</param>
        /// <param name="movimentacao">Dados da movimentação.</param>
        void ValidarMovimentacao(GDASession sessao, MovimentacaoDto movimentacao);
    }
}