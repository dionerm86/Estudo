using Glass.Data.Helper;
using Glass.Data.Helper.Calculos.Estrategia;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Xunit;

namespace Glass.Data.Test.Helper.DescontoAcrescimo
{
    public class CalcularTest
    {
        private readonly CalcularHelperClass helper = new CalcularHelperClass();

        [Fact]
        public void TesteAplicarAcrescimoAmbientePercentual()
        {
            // Given
            var produtos = helper.GerarProdutos();
            var container = helper.GerarContainer();
            var estrategia = DescontoAcrescimoStrategyFactory.Instance.RecuperaEstrategia(
                TipoCalculo.Acrescimo,
                TipoAplicacao.Ambiente
            );

            // When
            bool aplicado = estrategia.Aplicar(TipoValor.Percentual, 50, produtos, container);

            // Then
            Assert.True(aplicado);
            
            foreach (var produto in produtos)
            {
                Assert.Equal(produto.TotalBruto / 2, produto.ValorAcrescimoProd);
                Assert.Equal(produto.TotalBruto, produto.Total - produto.ValorAcrescimoProd);

                foreach (var beneficiamento in (produto.Beneficiamentos ?? GenericBenefCollection.EMPTY))
                {
                    Assert.Equal(beneficiamento.TotalBruto / 2, beneficiamento.ValorAcrescimoProd);
                    Assert.Equal(beneficiamento.TotalBruto, beneficiamento.Valor - produto.ValorAcrescimoProd);
                }
            }
        }
    }
}
