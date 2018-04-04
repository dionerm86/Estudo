using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.Model.Calculos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Data.Test.Helper.DescontoAcrescimo
{
    class CalcularHelperClass
    {
        public IProdutoCalculo[] GerarProdutos()
        {
            return new[] {
                GerarProduto(1, true),
                GerarProduto(2, false),
                GerarProduto(3, false)
            };
        }

        public IContainerCalculo GerarContainer()
        {
            return new ContainerCalculoDTO()
            {
                Id = 1,
                Tipo = ContainerCalculoDTO.TipoContainer.Pedido,
                IdCliente = 1,
                TipoEntrega = (int)Pedido.TipoEntregaPedido.Balcao,
                TipoVenda = (int)Pedido.TipoVendaPedido.AVista
            };
        }

        private IProdutoCalculo GerarProduto(uint id, bool possuiBeneficiamentos)
        {
            var produto = new ProdutoCalculoDTO()
            {
                Id = id,
                Altura = 1000,
                Largura = 1000,
                Espessura = 8,
                AlturaCalculo = 1000,
                IdProduto = 1,
                PercDescontoQtde = 10,
                Qtde = 1,
                QtdeAmbiente = 1,
                Total = 100,
                TotM = 1,
                TotM2Calc = 1,
                TotalBruto = 100,
                ValorUnit = 100,
                ValorUnitarioBruto = 100
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
