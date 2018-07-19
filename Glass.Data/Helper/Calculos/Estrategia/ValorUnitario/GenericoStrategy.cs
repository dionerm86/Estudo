using GDA;
using Glass.Data.Helper.Calculos.Estrategia.ValorTotal.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.ValorUnitario
{
    class GenericoStrategy : QtdBaseStrategy<GenericoStrategy>
    {
        private GenericoStrategy() { }

        protected override decimal Calcular(GDASession sessao, IProdutoCalculo produto, int qtdeAmbiente, decimal total,
            ArredondarAluminio arredondarAluminio, bool calcularMultiploDe5, bool nf, int numeroBeneficiamentos,
            int alturaBeneficiamento, int larguraBeneficiamento)
        {
            CalculaTotalM2(sessao, produto);

            return base.Calcular(
                sessao,
                produto,
                qtdeAmbiente,
                total,
                arredondarAluminio,
                calcularMultiploDe5,
                nf,
                numeroBeneficiamentos,
                alturaBeneficiamento,
                larguraBeneficiamento
            );
        }

        private void CalculaTotalM2(GDASession sessao, IProdutoCalculo produto)
        {
            if (produto.Altura > 0 && produto.Largura > 0)
            {
                var calcularMultiploDe5 = produto.DadosProduto.DadosGrupoSubgrupo.ProdutoEVidro() && produto.DadosProduto.DadosGrupoSubgrupo.TipoCalculo() != TipoCalculoGrupoProd.Qtd;

                produto.TotM = CalculoM2.Instance.Calcular(sessao, produto.Container, produto, calcularMultiploDe5);
            }
        }
    }
}
