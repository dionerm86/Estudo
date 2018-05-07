namespace Glass.Data.Model
{
    public interface IContainerCalculo
    {
        uint Id { get; }
        IDadosCliente Cliente { get; }
        IDadosAmbiente Ambientes { get; }

        uint? IdObra { get; }
        int? TipoEntrega { get; }
        int? TipoVenda { get; }
        bool Reposicao { get; }
        bool MaoDeObra { get; }
        bool IsPedidoProducaoCorte { get; }
        uint? IdParcela { get; }

        int TipoAcrescimo { get; }
        decimal Acrescimo { get; }
        int TipoDesconto { get; }
        decimal Desconto { get; }
        float PercComissao { get; }
    }
}
