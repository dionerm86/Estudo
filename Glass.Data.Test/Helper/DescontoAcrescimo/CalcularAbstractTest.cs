using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.Model.Calculos;
using System.Linq;

namespace Glass.Data.Test.Helper.DescontoAcrescimo
{
    class CalcularHelperClass
    {
        public IContainerCalculo GerarContainer()
        {
            return new ContainerCalculoDTO()
            {
                Id = 1,
                Tipo = ContainerCalculoDTO.TipoContainer.Pedido,
                Cliente = new ClienteDTO(),
                TipoEntrega = (int)Pedido.TipoEntregaPedido.Balcao,
                TipoVenda = (int)Pedido.TipoVendaPedido.AVista
            };
        }

        public IProdutoCalculo[] GerarProdutos(IContainerCalculo container)
        {
            return new[] {
                GerarProduto(container, 1, true),
                GerarProduto(container, 2, false),
                GerarProduto(container, 3, false)
            };
        }

        private IProdutoCalculo GerarProduto(IContainerCalculo container, uint id, bool possuiBeneficiamentos)
        {
            var produto = new ProdutoCalculoDTO()
            {
                Id = id,
                Altura = 1000,
                Largura = 1000,
                Espessura = 8,
                AlturaCalc = 1000,
                IdProduto = 1,
                PercDescontoQtde = 10,
                Qtde = 1,
                QtdeAmbiente = 1,
                Total = 100,
                TotM = 1,
                TotM2Calc = 1,
                TotalBruto = 100,
                ValorUnit = 100,
                ValorUnitarioBruto = 100,
                Container = container,
                DadosProduto = new DadosProdutoDTO()
            };

            if (possuiBeneficiamentos)
            {
                produto.Beneficiamentos = GerarBeneficiamentos();
                produto.ValorBenef = produto.Beneficiamentos.Sum(p => p.Valor);
            }

            return produto;
        }

        private GenericBenefCollection GerarBeneficiamentos()
        {
            var beneficiamentos = new GenericBenefCollection();

            beneficiamentos.Add(new GenericBenef()
            {
                IdBenefConfig = 1,
                Valor = 10,
                Qtd = 1,
                ValorUnit = 10
            });

            return beneficiamentos;
        }
    }
}
