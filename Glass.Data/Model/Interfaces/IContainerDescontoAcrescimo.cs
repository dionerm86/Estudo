namespace Glass.Data.Model
{
    interface IContainerDescontoAcrescimo
    {
        uint Id { get; }
        uint? IdCliente { get; }
        int? TipoEntrega { get; }
        bool Reposicao { get; }
    }
}
