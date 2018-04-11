using Glass.Configuracoes;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Desconto
{
    abstract class BaseDescontoStrategy<T> : BaseStrategy<T>
        where T : BaseDescontoStrategy<T>
    {
        protected override bool PermiteAplicarOuRemover(IEnumerable<IProdutoCalculo> produtos)
        {
            return PedidoConfig.RatearDescontoProdutos
                && produtos.First().Container.Desconto > 0;
        }

        protected override decimal CalcularTotalDesejado(TipoValor tipo, decimal valorAplicar, decimal totalAtual)
        {
            decimal totalAplicar = valorAplicar;

            if (tipo == TipoValor.Percentual)
            {
                totalAplicar = totalAtual * valorAplicar / 100;
            }

            return totalAtual - Math.Round(totalAplicar, 2);
        }
    }
}
