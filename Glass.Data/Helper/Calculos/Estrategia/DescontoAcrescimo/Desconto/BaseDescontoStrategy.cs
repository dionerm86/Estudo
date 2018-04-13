using Glass.Configuracoes;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using System;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Desconto
{
    abstract class BaseDescontoStrategy<T> : BaseStrategy<T>
        where T : BaseDescontoStrategy<T>
    {
        protected override decimal BaseCalculoTotalProduto(IProdutoCalculo produto)
        {
            return base.BaseCalculoTotalProduto(produto)
                + produto.ValorAcrescimo
                + produto.ValorAcrescimoProd;
        }
    }
}
