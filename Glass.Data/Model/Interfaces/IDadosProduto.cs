namespace Glass.Data.Model
{
    public interface IDadosProduto
    {
        bool CalcularAreaMinima(IProdutoCalculo produto, int numeroBeneficiamentos);
        float AreaMinima(IProdutoCalculo produto);
        int IdGrupoProd(IProdutoCalculo produto);
        bool ProdutoEAluminio(IProdutoCalculo produto);
        string Descricao(IProdutoCalculo produto);
        bool ProdutoDeProducao(IProdutoCalculo produto);
        string DescricaoSubgrupo(IProdutoCalculo produto);
    }
}
