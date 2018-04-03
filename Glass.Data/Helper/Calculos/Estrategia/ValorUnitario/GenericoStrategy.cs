using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.Model;
using Glass.Pool;
using Glass.Global;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario
{
    class GenericoStrategy : BaseStrategy<GenericoStrategy>
    {
        private GenericoStrategy() { }

        protected override decimal Calcular(IProdutoCalculo produto, IContainerCalculo container, int qtdeAmbiente,
            decimal total, bool arredondarAluminio, bool calcMult5, bool nf, int numeroBenef,
            bool calcularAreaMinima, int alturaBenef, int larguraBenef)
        {
            CalculaTotalM2(produto);

            decimal divisor = Divisor(produto, qtdeAmbiente);
            return total / divisor;
        }

        private decimal Divisor(IProdutoCalculo produto, int qtdeAmbiente)
        {
            decimal qtde = produto.Qtde > 0 ? (decimal)produto.Qtde : 1;
            return qtde * qtdeAmbiente;
        }

        private void CalculaTotalM2(IProdutoCalculo produto)
        {
            if (produto.Altura > 0 && produto.Largura > 0)
            {
                produto.TotM = CalculosFluxo.ArredondaM2(
                    null,
                    produto.Largura,
                    (int)produto.Altura,
                    produto.Qtde,
                    (int)produto.IdProduto,
                    produto.Redondo
                );
            }
        }
    }
}
