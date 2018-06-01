namespace Glass.Data.Model
{
    public interface IProdutoIcmsSt
    {
        int IdProd { get; }

        decimal Total { get; }

        float MvaProdutoNf { get; }

        float AliquotaIcms { get; }
        decimal ValorIcms { get; }

        float AliquotaIpi { get; }
        decimal ValorIpi { get; }

        float AliquotaIcmsSt { get; }

        decimal ValorDesconto { get; }
        decimal ValorFrete { get; }
        decimal ValorSeguro { get; }
        decimal ValorOutrasDespesas { get; }

        float PercentualReducaoBaseCalculo { get; }
    }
}