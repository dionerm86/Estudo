namespace Glass.Data.Model
{
    public interface IDadosProduto
    {
        IDadosGrupoSubgrupo DadosGrupoSubgrupo { get; }
        IDadosChapaVidro DadosChapaVidro { get; }
        IDadosBaixaEstoque DadosBaixaEstoque { get; }

        bool CalcularAreaMinima(int numeroBeneficiamentos);
        float AreaMinima();
        int? AlturaProduto();
        int? LarguraProduto();
        string Descricao();        
        decimal ValorTabela(bool usarCliente = true);
        decimal CustoCompra();      
    }
}
