namespace Glass.Data.Model
{
    interface IContainerDescontoAcrescimo
    {
        uint Id { get; }
        uint? IdCliente { get; }
        uint? IdObra { get; }
        int? TipoEntrega { get; }
        int? TipoVenda { get; }
        bool Reposicao { get; }
        bool IsPedidoProducaoCorte { get; }
        uint? IdParcela { get; }
    }
}
