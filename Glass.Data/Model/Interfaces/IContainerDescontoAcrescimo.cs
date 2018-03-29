namespace Glass.Data.Model
{
    interface IContainerDescontoAcrescimo
    {
        uint Id { get; }
        uint? IdCliente { get; }
        uint? IdObra { get; }
        int? TipoEntrega { get; }
        bool Reposicao { get; }
        decimal TotalAtual { get; }
        bool IsPedidoProducaoCorte { get; }
    }
}
