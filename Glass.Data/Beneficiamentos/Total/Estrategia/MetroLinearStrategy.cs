// <copyright file="MetroLinearStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Beneficiamentos.Total.Dto;
using Glass.Pool;

namespace Glass.Data.Beneficiamentos.Total.Estrategia
{
    /// <summary>
    /// Classe com a estratégia de cálculo por metro linear.
    /// </summary>
    internal class MetroLinearStrategy : Singleton<MetroLinearStrategy>, ICalculoValorTotalStrategy
    {
        private MetroLinearStrategy() { }

        /// <inheritdoc/>
        public decimal CalcularValor(DadosCalculoDto dadosCalculo, BeneficiamentoDto beneficiamento, ItemBeneficiamentoDto itemSelecionado, decimal valorUnitario)
        {
            var alturas = itemSelecionado.Altura ?? 2;
            var larguras = itemSelecionado.Largura ?? 2;

            var baseCalculo = this.ObterBaseCalculo(dadosCalculo, alturas, larguras);
            return valorUnitario * baseCalculo;
        }

        /// <inheritdoc/>
        public decimal CalcularCusto(DadosCalculoDto dadosCalculo, BeneficiamentoDto beneficiamento, ItemBeneficiamentoDto itemSelecionado)
        {
            return this.CalcularValor(dadosCalculo, beneficiamento, itemSelecionado, beneficiamento.CustoUnitario);
        }

        private decimal ObterBaseCalculo(DadosCalculoDto dadosCalculo, int alturas, int larguras)
        {
            var quantidade = dadosCalculo.Produto.Quantidade
                * (dadosCalculo.Produto.QuantidadeAmbiente ?? 1);

            var altura = (decimal)dadosCalculo.Produto.Altura * alturas;
            var largura = dadosCalculo.Produto.Largura * larguras;

            return quantidade * (altura + largura) / 1000;
        }
    }
}
