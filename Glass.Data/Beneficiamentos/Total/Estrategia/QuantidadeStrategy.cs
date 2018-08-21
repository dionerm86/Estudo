// <copyright file="QuantidadeStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Beneficiamentos.Total.Dto;
using Glass.Pool;

namespace Glass.Data.Beneficiamentos.Total.Estrategia
{
    /// <summary>
    /// Classe com a estratégia de cálculo por quantidade.
    /// </summary>
    internal class QuantidadeStrategy : Singleton<QuantidadeStrategy>, ICalculoValorTotalStrategy
    {
        private QuantidadeStrategy() { }

        /// <inheritdoc/>
        public decimal CalcularValor(DadosCalculoDto dadosCalculo, BeneficiamentoDto beneficiamento, ItemBeneficiamentoDto itemSelecionado, decimal valorUnitario)
        {
            var quantidades = itemSelecionado.Quantidade ?? 1;

            var baseCalculo = this.ObterBaseCalculo(dadosCalculo, quantidades);
            return valorUnitario * baseCalculo;
        }

        /// <inheritdoc/>
        public decimal CalcularCusto(DadosCalculoDto dadosCalculo, BeneficiamentoDto beneficiamento, ItemBeneficiamentoDto itemSelecionado, decimal custoUnitario)
        {
            return this.CalcularValor(dadosCalculo, beneficiamento, itemSelecionado, custoUnitario);
        }

        private decimal ObterBaseCalculo(DadosCalculoDto dadosCalculo, int quantidades)
        {
            var quantidade = dadosCalculo.Produto.Quantidade
                * (dadosCalculo.Produto.QuantidadeAmbiente ?? 1);

            return quantidade * quantidades;
        }
    }
}
