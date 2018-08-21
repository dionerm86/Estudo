// <copyright file="SelecionaPrecoBeneficiamento.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Beneficiamentos.Total.Dto;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.Beneficiamentos.Total
{
    /// <summary>
    /// Classe que contém a lógica para seleção do preço do beneficiamento que será utilizado para um produto.
    /// </summary>
    internal class SelecionaPrecoBeneficiamento
    {
        /// <summary>
        /// Recupera o preço do beneficiamento para ser usado no cálculo de valores.
        /// </summary>
        /// <param name="dadosCalculo">Dados adicionais para o cálculo associado ao beneficiamento.</param>
        /// <param name="beneficiamento">Os dados do beneficiamento aplicado.</param>
        /// <returns>Um objeto com o preço do beneficiamento, se encontrado.</returns>
        public PrecoBeneficiamentoDto ObterPrecoBeneficiamento(
            DadosCalculoDto dadosCalculo,
            BeneficiamentoDto beneficiamento)
        {
            var precos = beneficiamento.Precos ?? new PrecoBeneficiamentoDto[0];
            precos = this.FiltrarPrecosPorSubgrupo(precos, dadosCalculo.Produto.IdSubgrupo);
            precos = this.FiltrarPrecosPorCor(precos, dadosCalculo.Produto.IdCor);
            precos = this.OrdenarPrecos(precos);

            if (beneficiamento.CalculoPorEspessura)
            {
                precos = this.FiltrarPrecosPorEspessura(precos, dadosCalculo.Produto.Espessura);
            }

            return precos.FirstOrDefault() ?? new PrecoBeneficiamentoDto();
        }

        private IEnumerable<PrecoBeneficiamentoDto> FiltrarPrecosPorSubgrupo(
            IEnumerable<PrecoBeneficiamentoDto> precos,
            int? idSubgrupo)
        {
            return precos.Where(preco => !preco.IdSubgrupo.HasValue || preco.IdSubgrupo == idSubgrupo);
        }

        private IEnumerable<PrecoBeneficiamentoDto> FiltrarPrecosPorCor(
            IEnumerable<PrecoBeneficiamentoDto> precos,
            int? idCor)
        {
            return precos.Where(preco => !preco.IdCor.HasValue || preco.IdCor == idCor);
        }

        private IEnumerable<PrecoBeneficiamentoDto> OrdenarPrecos(
            IEnumerable<PrecoBeneficiamentoDto> precos)
        {
            return precos.OrderByDescending(preco =>
            {
                var temSubgrupo = preco.IdSubgrupo.HasValue ? 1 : 0;
                var temCor = preco.IdCor.HasValue ? 1 : 0;

                return temSubgrupo + temCor;
            });
        }

        private IEnumerable<PrecoBeneficiamentoDto> FiltrarPrecosPorEspessura(
            IEnumerable<PrecoBeneficiamentoDto> precos,
            double? espessura)
        {
            var ordenados = precos.OrderByDescending(preco => preco.Espessura);
            var filtrados = ordenados.Where(preco => preco.Espessura <= espessura);

            if (!filtrados.Any())
            {
                filtrados = new[] { ordenados.LastOrDefault() };
            }

            return filtrados;
        }
    }
}
