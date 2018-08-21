// <copyright file="ValorTotalStrategyFactory.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Beneficiamentos.Total.Dto;
using Glass.Pool;

namespace Glass.Data.Beneficiamentos.Total.Estrategia
{
    /// <summary>
    /// Classe que recupera a estratégia de cálculo do valor total de um beneficiamento.
    /// </summary>
    internal sealed class ValorTotalStrategyFactory : Singleton<ValorTotalStrategyFactory>
    {
        private ValorTotalStrategyFactory() { }

        /// <summary>
        /// Recupera a estratégia para o cálculo do valor total do beneficiamento.
        /// </summary>
        /// <param name="beneficiamento">O beneficiamento que está sendo calculado.</param>
        /// <returns>O objeto com a estratégia de cálculo.</returns>
        public ICalculoValorTotalStrategy ObterEstrategia(BeneficiamentoDto beneficiamento)
        {
            switch (beneficiamento.TipoCalculo)
            {
                case Model.TipoCalculoBenef.MetroQuadrado:
                    return MetroQuadradoStrategy.Instance;

                case Model.TipoCalculoBenef.MetroLinear:
                    return MetroLinearStrategy.Instance;

                case Model.TipoCalculoBenef.Quantidade:
                    return QuantidadeStrategy.Instance;

                case Model.TipoCalculoBenef.Porcentagem:
                    return PorcentagemStrategy.Instance;
            }

            return new SemAlteracaoStrategy();
        }
    }
}
