// <copyright file="ICalculoValorTotalStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Beneficiamentos.Total.Dto;

namespace Glass.Data.Beneficiamentos.Total.Estrategia
{
    /// <summary>
    /// Interface para o cálculo do valor total de um beneficiamento.
    /// </summary>
    internal interface ICalculoValorTotalStrategy
    {
        /// <summary>
        /// Calcula o valor total de um beneficiamento.
        /// </summary>
        /// <param name="dadosCalculo">Dados adicionais para o cálculo associado ao beneficiamento.</param>
        /// <param name="beneficiamento">Os dados do beneficiamento aplicado.</param>
        /// <param name="itemSelecionado">Os dados do item selecionado (pode ser o pai ou um dos filhos).</param>
        /// <param name="valorUnitario">O valor unitário usado para o cálculo do total.</param>
        /// <returns>Um valor que indica o total daquele beneficiamento aplicado para o produto.</returns>
        decimal CalcularValor(DadosCalculoDto dadosCalculo, BeneficiamentoDto beneficiamento, ItemBeneficiamentoDto itemSelecionado, decimal valorUnitario);

        /// <summary>
        /// Calcula o custo total de um beneficiamento.
        /// </summary>
        /// <param name="dadosCalculo">Dados adicionais para o cálculo associado ao beneficiamento.</param>
        /// <param name="beneficiamento">Os dados do beneficiamento aplicado.</param>
        /// <param name="itemSelecionado">Os dados do item selecionado (pode ser o pai ou um dos filhos).</param>
        /// <returns>Um valor que indica o total daquele beneficiamento aplicado para o produto.</returns>
        decimal CalcularCusto(DadosCalculoDto dadosCalculo, BeneficiamentoDto beneficiamento, ItemBeneficiamentoDto itemSelecionado);
    }
}
