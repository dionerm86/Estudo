using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using System;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Acrescimo
{
    abstract class BaseAcrescimoStrategy<T> : BaseStrategy<T>
        where T : BaseAcrescimoStrategy<T>
    { 
        protected override decimal CalcularTotalDesejado(TipoValor tipo, decimal valorAplicar, decimal totalAtual)
        {
            decimal totalAplicar = valorAplicar;

            if (tipo == TipoValor.Percentual)
            {
                totalAplicar = totalAtual * valorAplicar / 100;
            }

            return totalAtual + Math.Round(totalAplicar, 2);
        }
    }
}
