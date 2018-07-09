// <copyright file="SemAlteracaoStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Beneficiamentos.Total.Dto;

namespace Glass.Data.Beneficiamentos.Total.Estrategia
{
    /// <summary>
    /// Classe que implementa uma estratégia de cálculo de valor total de beneficiamento.
    /// Não realiza operação nenhuma.
    /// </summary>
    internal class SemAlteracaoStrategy : ICalculoValorTotalStrategy
    {
        /// <inheritdoc/>
        public decimal CalcularCusto(DadosCalculoDto dadosCalculo, BeneficiamentoDto beneficiamento, ItemBeneficiamentoDto itemSelecionado)
        {
            return 0;
        }

        /// <inheritdoc/>
        public decimal CalcularValor(DadosCalculoDto dadosCalculo, BeneficiamentoDto beneficiamento, ItemBeneficiamentoDto itemSelecionado, decimal valorUnitario)
        {
            return 0;
        }
    }
}
