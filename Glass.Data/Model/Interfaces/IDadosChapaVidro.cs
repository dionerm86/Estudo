namespace Glass.Data.Model
{
    public interface IDadosChapaVidro
    {
        bool ProdutoPossuiChapaVidro(IProdutoCalculo produto);
        int AlturaMinimaChapaVidro(IProdutoCalculo produto);
        int AlturaChapaVidro(IProdutoCalculo produto);
        int LarguraMinimaChapaVidro(IProdutoCalculo produto);
        int LarguraChapaVidro(IProdutoCalculo produto);
        float PercentualAcrescimoM2ChapaVidro(IProdutoCalculo produto, float m2);
    }
}
