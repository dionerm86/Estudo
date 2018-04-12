using Glass.Configuracoes;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using System;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Desconto
{
    abstract class BaseDescontoStrategy<T> : BaseStrategy<T>
        where T : BaseDescontoStrategy<T>
    {
        protected override bool PermiteAplicar()
        {
            return PedidoConfig.RatearDescontoProdutos;
        }
    }
}
