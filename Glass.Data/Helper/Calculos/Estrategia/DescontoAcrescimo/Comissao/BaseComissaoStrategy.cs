using Glass.Configuracoes;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using System;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Comissao
{
    abstract class BaseComissaoStrategy<T> : BaseStrategy<T>
        where T : BaseComissaoStrategy<T>
    {
        protected override bool PermiteAplicar()
        {
            return PedidoConfig.Comissao.ComissaoPedido;
        }

        protected override decimal CalcularValorAplicar(TipoValor tipo, decimal valorAplicar, decimal totalAtual)
        {
            var percentualCalculo = (100 - valorAplicar) / 100;
            return Math.Round(totalAtual / percentualCalculo - totalAtual, 2);
        }

        protected override decimal BaseCalculoTotalProduto(IProdutoCalculo produto)
        {
            return base.BaseCalculoTotalProduto(produto) + produto.ValorAcrescimo;
        }
    }
}
