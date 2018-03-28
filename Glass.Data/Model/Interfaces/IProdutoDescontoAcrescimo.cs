using Glass.Data.Helper;

namespace Glass.Data.Model
{
    interface IProdutoDescontoAcrescimo
    {
        uint Id { get; }
        uint IdParent { get; }
        uint? IdObra { get; }

        decimal ValorTabelaPedido { get; }
        decimal ValorUnit { get; set; }
        decimal Total { get; set; }
        decimal ValorBenef { get; }
        decimal ValorUnitarioBruto { get; set; }
        decimal TotalBruto { get; set; }

        decimal ValorAcrescimo { get; set; }
        decimal ValorAcrescimoProd { get; set; }
        decimal ValorDesconto { get; set; }
        decimal ValorDescontoProd { get; set; }
        decimal ValorDescontoQtde { get; set; }
        decimal ValorDescontoCliente { get; set; }
        decimal ValorAcrescimoCliente { get; set; }
        decimal ValorComissao { get; set; }
        
        uint IdProduto { get; }
        int Largura { get; }
        float Qtde { get; }
        int QtdeAmbiente { get; }
        float Espessura { get; }
        bool Redondo { get; }
        float Altura { get; }
        float AlturaCalc { get; }
        float TotM { get; }
        float TotM2Calc { get; }
        float PercDescontoQtde { get; }
        bool RemoverDescontoQtde { get; set; }
        GenericBenefCollection Beneficiamentos { get; set; }
        int? AlturaBenef { get; }
        int? LarguraBenef { get; }
    }
}