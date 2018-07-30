using GDA;
using Glass.Data.Helper;

namespace Glass.Data.Model
{
    public interface IProdutoCalculo
    {
        IContainerCalculo Container { get; set; }
        IAmbienteCalculo Ambiente { get; set; }
        IDadosProduto DadosProduto { get; set; }

        uint Id { get; }
        uint? IdAmbiente { get; }

        decimal CustoProd { get; set; }
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
        float PercDescontoQtde { get; }

        uint IdProduto { get; }
        int Largura { get; }
        float Qtde { get; set; }
        int QtdeAmbiente { get; }
        float Espessura { get; }
        bool Redondo { get; }
        float Altura { get; set; }
        float AlturaCalc { get; }
        float TotM { get; set; }
        float TotM2Calc { get; set; }
        int TipoCalc { get; }

        GenericBenefCollection Beneficiamentos { get; set; }
        int? AlturaBenef { get; }
        int? LarguraBenef { get; }
    }
}