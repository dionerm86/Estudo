// <copyright file="PorcentagemStrategy.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Beneficiamentos.Total.Dto;
using Glass.Pool;

namespace Glass.Data.Beneficiamentos.Total.Estrategia
{
    /// <summary>
    /// Classe com a estratégia de cálculo por percentual.
    /// </summary>
    internal class PorcentagemStrategy : Singleton<PorcentagemStrategy>, ICalculoValorTotalStrategy
    {
        private PorcentagemStrategy() { }

        /// <inheritdoc/>
        public decimal CalcularValor(DadosCalculoDto dadosCalculo, BeneficiamentoDto beneficiamento, ItemBeneficiamentoDto itemSelecionado, decimal valorUnitario)
        {
            var quantidade = itemSelecionado.Quantidade ?? 1;
            if (quantidade == 0)
            {
                quantidade = 1;
            }

            var baseCalculo = this.ObterBaseCalculo(dadosCalculo, dadosCalculo.Produto.ValorUnitario);
            return valorUnitario * baseCalculo * quantidade;
        }

        /// <inheritdoc/>
        public decimal CalcularCusto(DadosCalculoDto dadosCalculo, BeneficiamentoDto beneficiamento, ItemBeneficiamentoDto itemSelecionado)
        {
            var valor = dadosCalculo.Produto.CustoUnitario > 0
                ? dadosCalculo.Produto.CustoUnitario
                : dadosCalculo.Produto.ValorUnitario;

            var baseCalculo = this.ObterBaseCalculo(dadosCalculo, valor);
            return beneficiamento.CustoUnitario * baseCalculo;
        }

        private decimal ObterBaseCalculo(DadosCalculoDto dadosCalculo, decimal valorProduto)
        {
            var areaM2 = Configuracoes.Beneficiamentos.UsarM2CalcBeneficiamentos
                ? dadosCalculo.Produto.AreaCalculadaM2
                : dadosCalculo.Produto.AreaM2;

            return valorProduto / 100 * (decimal)areaM2;
        }
    }
}
