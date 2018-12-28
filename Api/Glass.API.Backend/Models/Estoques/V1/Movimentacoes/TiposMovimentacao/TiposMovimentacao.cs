// <copyright file="TiposMovimentacao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.Estoques.V1.Movimentacoes.TiposMovimentacao
{
    /// <summary>
    /// Enumerador com o tipo de movimentação.
    /// </summary>
    public enum TipoMovimentacao
    {
        /// <summary>
        /// Entrada.
        /// </summary>
        [Description("Entrada")]
        TipoMovimentacaoEntrada = 1,

        /// <summary>
        /// Saída.
        /// </summary>
        [Description("Saida")]
        TipoMovimentacaoSaida = 2,
    }
}
