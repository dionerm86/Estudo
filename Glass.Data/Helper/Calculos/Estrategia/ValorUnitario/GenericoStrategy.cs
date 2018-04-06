using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario
{
    class GenericoStrategy : QtdBaseStrategy<GenericoStrategy>
    {
        private GenericoStrategy() { }

        protected override decimal Calcular(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container,
            int qtdeAmbiente, decimal total, ArredondarAluminio arredondarAluminio, bool calcMult5, bool nf, int numeroBenef,
            int alturaBenef, int larguraBenef)
        {
            CalculaTotalM2(sessao, produto, container);

            return base.Calcular(
                sessao,
                produto,
                container,
                qtdeAmbiente,
                total,
                arredondarAluminio,
                calcMult5,
                nf,
                numeroBenef,
                alturaBenef,
                larguraBenef
            );
        }

        private void CalculaTotalM2(GDASession sessao, IProdutoCalculo produto, IContainerCalculo container)
        {
            if (produto.Altura > 0 && produto.Largura > 0)
            {
                produto.TotM = CalculoM2.Instance.Calcular(sessao, produto, container, true);
            }
        }
    }
}
