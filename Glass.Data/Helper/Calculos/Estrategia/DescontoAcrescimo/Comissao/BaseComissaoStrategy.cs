using Glass.Configuracoes;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using System;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Comissao
{
    abstract class BaseComissaoStrategy<T> : BaseStrategy<T>
        where T : BaseComissaoStrategy<T>
    {
        protected override bool PermiteAplicar()
        {
            return PedidoConfig.Comissao.ComissaoPedido;
        }

        protected override decimal CalcularTotalDesejado(TipoValor tipo, decimal valorAplicar, decimal totalAtual)
        {
            var valorTotalAplicar = CalculaValorComissao(totalAtual, valorAplicar);
            return totalAtual + Math.Round(valorTotalAplicar, 2);
        }

        private decimal CalculaValorComissao(decimal baseCalculo, decimal percentual)
        {
            var percentualCalculo = (100 - percentual) / 100;
            return Math.Round(baseCalculo / percentualCalculo - baseCalculo, 2);
        }
    }
}
