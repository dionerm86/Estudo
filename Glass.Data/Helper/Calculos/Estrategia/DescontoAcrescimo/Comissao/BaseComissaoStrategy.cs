using Glass.Configuracoes;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Comissao
{
    abstract class BaseComissaoStrategy<T> : BaseStrategy<T>
        where T : BaseComissaoStrategy<T>
    {
        protected override bool PermiteAplicarOuRemover(IEnumerable<IProdutoCalculo> produtos)
        {
            return PedidoConfig.Comissao.ComissaoPedido
                && produtos.First().Container.PercComissao > 0;
        }

        protected override decimal CalcularTotalDesejado(TipoValor tipo, decimal valorAplicar, decimal totalAtual)
        {
            var percentual = CalcularPercentualTotalAplicar(0, valorAplicar);
            var valorTotalAplicar = CalculaValorComissao(totalAtual, percentual);
            return totalAtual + Math.Round(valorTotalAplicar, 2);
        }

        protected override decimal CalcularPercentualTotalAplicar(decimal totalAtual, decimal valorAplicar)
        {
            return (100 - valorAplicar) / 100;
        }

        protected decimal CalculaValorComissao(decimal baseCalculo, decimal percentual)
        {
            return Math.Round(baseCalculo / percentual - baseCalculo, 2);
        }
    }
}
