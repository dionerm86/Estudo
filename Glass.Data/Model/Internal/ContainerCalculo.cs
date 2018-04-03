namespace Glass.Data.Model.Internal
{
    public class ContainerCalculo : IContainerCalculo
    {
        public enum TipoContainer
        {
            Pedido,
            Orcamento,
            Projeto
        };

        public TipoContainer? Tipo { get; set; }

        public uint Id { get; set; }

        public uint? IdCliente { get; set; }

        public uint? IdObra { get; set; }

        public uint? IdParcela { get; set; }

        public bool IsPedidoProducaoCorte { get; set; }

        public bool Reposicao { get; set; }

        public bool MaoDeObra { get; set; }

        public int? TipoEntrega { get; set; }

        public int? TipoVenda { get; set; }
    }
}
