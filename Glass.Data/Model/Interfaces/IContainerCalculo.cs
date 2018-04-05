namespace Glass.Data.Model
{
    public interface IContainerCalculo
    {
        uint Id { get; }
        ICliente Cliente { get; }
        uint? IdObra { get; }
        int? TipoEntrega { get; }
        int? TipoVenda { get; }
        bool Reposicao { get; }
        bool MaoDeObra { get; }
        bool IsPedidoProducaoCorte { get; }
        uint? IdParcela { get; }
        IDadosProduto DadosProduto { get; }
        IDadosChapaVidro DadosChapaVidro { get; }
    }
}
