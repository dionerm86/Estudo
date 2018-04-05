using GDA;

namespace Glass.Data.Model
{
    public interface IDadosProduto
    {
        bool CalcularAreaMinima(GDASession sessao, IProdutoCalculo produto, int numeroBeneficiamentos);
        float AreaMinima(GDASession sessao, IProdutoCalculo produto);
        int IdGrupoProd(GDASession sessao, IProdutoCalculo produto);
        bool ProdutoEAluminio(GDASession sessao, IProdutoCalculo produto);
        bool ProdutoEVidro(GDASession sessao, IProdutoCalculo produto);
        string Descricao(GDASession sessao, IProdutoCalculo produto);
        bool ProdutoDeProducao(GDASession sessao, IProdutoCalculo produto);
        string DescricaoSubgrupo(GDASession sessao, IProdutoCalculo produto);
        decimal ValorTabela(GDASession sessao, IProdutoCalculo produto, bool usarCliente = true);
        TipoCalculoGrupoProd TipoCalculo(GDASession sessao, IProdutoCalculo produto, bool fiscal = false);
    }
}
