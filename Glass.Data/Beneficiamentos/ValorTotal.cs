// <copyright file="ValorTotal.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Beneficiamentos.Total;
using Glass.Data.Beneficiamentos.Total.Dto;
using Glass.Data.Beneficiamentos.Total.Estrategia;
using Glass.Pool;
using System.Linq;

namespace Glass.Data.Beneficiamentos
{
    /// <summary>
    /// Classe que contém os métodos de fachada para o cálculo do valor total dos beneficiamentos.
    /// </summary>
    public class ValorTotal : Singleton<ValorTotal>
    {
        private ValorTotal() { }

        /// <summary>
        /// Calcula o valor total de um beneficiamento.
        /// </summary>
        /// <param name="dadosCalculo">Dados adicionais para o cálculo associado ao beneficiamento.</param>
        /// <param name="beneficiamento">Os dados do beneficiamento aplicado.</param>
        /// <param name="itemSelecionado">Os dados do item selecionado (pode ser o pai ou um dos filhos).</param>
        /// <returns>Um valor que indica o total daquele beneficiamento aplicado para o produto.</returns>
        public TotalDto Calcular(
            DadosCalculoDto dadosCalculo,
            BeneficiamentoDto beneficiamento,
            ItemBeneficiamentoDto itemSelecionado)
        {
            var selecionado = this.ObterBeneficiamentoSelecionado(beneficiamento, itemSelecionado);
            var preco = new SelecionaPrecoBeneficiamento().ObterPrecoBeneficiamento(dadosCalculo, selecionado);

            var valorUnitario = this.ObterValorUnitario(dadosCalculo, preco, beneficiamento.TipoControle);
            valorUnitario = this.ObterValorComPercentualComissao(valorUnitario, dadosCalculo.PercentualComissao);

            var estrategia = ValorTotalStrategyFactory.Instance.ObterEstrategia(selecionado);

            return new TotalDto
            {
                ValorUnitario = valorUnitario,
                ValorTotal = estrategia.CalcularValor(dadosCalculo, selecionado, itemSelecionado, valorUnitario),
                CustoTotal = estrategia.CalcularCusto(dadosCalculo, selecionado, itemSelecionado, preco.CustoUnitario),
            };
        }

        private BeneficiamentoDto ObterBeneficiamentoSelecionado(
            BeneficiamentoDto beneficiamento,
            ItemBeneficiamentoDto itemSelecionado)
        {
            if (beneficiamento.Id == itemSelecionado.Id)
            {
                return beneficiamento;
            }

            foreach (var filho in beneficiamento.Filhos)
            {
                var item = this.ObterBeneficiamentoSelecionado(filho, itemSelecionado);

                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }

        private decimal ObterValorUnitario(
            DadosCalculoDto dadosCalculo,
            PrecoBeneficiamentoDto precoBeneficiamento,
            Model.TipoControleBenef tipoControle)
        {
            var percentualDescontoAcrescimo = (decimal)dadosCalculo.PercentualDescontoAcrescimoCliente;
            if (!dadosCalculo.UsarDescontoAcrescimoClienteNosBeneficiamentos
                || (tipoControle != Model.TipoControleBenef.Lapidacao
                    && tipoControle != Model.TipoControleBenef.Bisote))
            {
                percentualDescontoAcrescimo = 1;
            }

            if (dadosCalculo.ValorBeneficiamentoEstaEditavelNoControle)
            {
                return precoBeneficiamento.CustoUnitario * percentualDescontoAcrescimo;
            }

            if (dadosCalculo.ClienteRevenda)
            {
                return precoBeneficiamento.ValorAtacadoUnitario * percentualDescontoAcrescimo;
            }

            var tiposEntregaBalcao = new[]
            {
                Model.Pedido.TipoEntregaPedido.Balcao,
                Model.Pedido.TipoEntregaPedido.Entrega,
            };

            if (tiposEntregaBalcao.Contains(dadosCalculo.TipoEntrega))
            {
                return precoBeneficiamento.ValorBalcaoUnitario * percentualDescontoAcrescimo;
            }

            return precoBeneficiamento.ValorObraUnitario * percentualDescontoAcrescimo;
        }

        private decimal ObterValorComPercentualComissao(decimal valor, double percentualComissao)
        {
            return valor / ((100 - (decimal)percentualComissao) / 100);
        }
    }
}
