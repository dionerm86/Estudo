// <copyright file="MetroQuadradoStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Beneficiamentos.Total.Dto;
using Glass.Pool;

namespace Glass.Data.Beneficiamentos.Total.Estrategia
{
    /// <summary>
    /// Classe com a estratégia de cálculo por m².
    /// </summary>
    internal class MetroQuadradoStrategy : Singleton<MetroQuadradoStrategy>, ICalculoValorTotalStrategy
    {
        private MetroQuadradoStrategy() { }

        /// <inheritdoc/>
        public decimal CalcularValor(DadosCalculoDto dadosCalculo, BeneficiamentoDto beneficiamento, ItemBeneficiamentoDto itemSelecionado, decimal valorUnitario)
        {
            var baseCalculo = this.ObterBaseCalculo(dadosCalculo);
            return valorUnitario * baseCalculo;
        }

        /// <inheritdoc/>
        public decimal CalcularCusto(DadosCalculoDto dadosCalculo, BeneficiamentoDto beneficiamento, ItemBeneficiamentoDto itemSelecionado, decimal custoUnitario)
        {
            return this.CalcularValor(dadosCalculo, beneficiamento, itemSelecionado, custoUnitario);
        }

        private decimal ObterBaseCalculo(DadosCalculoDto dadosCalculo)
        {
            return Configuracoes.Beneficiamentos.UsarM2CalcBeneficiamentos
                ? (decimal)dadosCalculo.Produto.AreaCalculadaM2
                : (decimal)dadosCalculo.Produto.AreaM2;
        }
    }
}
