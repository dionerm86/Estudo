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
