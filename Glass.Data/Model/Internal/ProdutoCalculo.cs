using Glass.Data.Helper;

namespace Glass.Data.Model.Internal
{
    public class ProdutoCalculo : IProdutoCalculo
    {
        public float Altura { get; set; }

        public int? AlturaBenef { get; set; }

        public float AlturaCalc { get; set; }

        public GenericBenefCollection Beneficiamentos { get; set; }

        public float Espessura { get; set; }

        public uint Id { get; set; }

        public uint? IdObra { get; set; }

        public uint IdParent { get; set; }

        public uint IdProduto { get; set; }

        public int Largura { get; set; }

        public int? LarguraBenef { get; set; }

        public float PercDescontoQtde { get; set; }

        public float Qtde { get; set; }

        public int QtdeAmbiente { get; set; }

        public bool Redondo { get; set; }

        public decimal Total { get; set; }

        public decimal TotalBruto { get; set; }

        public float TotM { get; set; }

        public float TotM2Calc { get; set; }

        public decimal ValorAcrescimo { get; set; }

        public decimal ValorAcrescimoCliente { get; set; }

        public decimal ValorAcrescimoProd { get; set; }

        public decimal ValorBenef { get; set; }

        public decimal ValorComissao { get; set; }

        public decimal ValorDesconto { get; set; }

        public decimal ValorDescontoCliente { get; set; }

        public decimal ValorDescontoProd { get; set; }

        public decimal ValorDescontoQtde { get; set; }

        public decimal ValorTabelaPedido { get; set; }

        public decimal ValorUnit { get; set; }

        public decimal ValorUnitarioBruto { get; set; }
    }
}
