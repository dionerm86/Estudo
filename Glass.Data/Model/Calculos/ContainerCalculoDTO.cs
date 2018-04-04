using static Glass.Data.Model.Pedido;

namespace Glass.Data.Model.Calculos
{
    class ContainerCalculoDTO : IContainerCalculo
    {
        public enum TipoContainer
        {
            Orcamento,
            Pedido,
            Projeto
        }

        public TipoContainer Tipo { get; private set; }
        public uint Id { get; private set; }
        public ICliente Cliente { get; private set; }
        public uint? IdObra { get; private set; }
        public uint? IdParcela { get; private set; }
        public bool IsPedidoProducaoCorte { get; private set; }
        public bool Reposicao { get; private set; }
        public bool MaoDeObra { get; private set; }
        public int? TipoEntrega { get; private set; }
        public int? TipoVenda { get; private set; }

        internal ContainerCalculoDTO(Orcamento orcamento)
        {
            this.Tipo = TipoContainer.Orcamento;
            this.Id = orcamento.IdOrcamento;
            this.IdObra = null;
            this.TipoEntrega = orcamento.TipoEntrega;
            this.TipoVenda = orcamento.TipoVenda;
            this.Reposicao = orcamento.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição;
            this.MaoDeObra = false;
            this.IsPedidoProducaoCorte = false;
            this.IdParcela = orcamento.IdParcela;
            this.Cliente = orcamento.IdCliente.HasValue
                ? new ClienteDTO(orcamento.IdCliente.Value)
                : null;
        }

        internal ContainerCalculoDTO(Pedido pedido)
        {
            this.Tipo = TipoContainer.Pedido;
            this.Id = pedido.IdPedido;
            this.Cliente = new ClienteDTO(pedido.IdCli);
            this.IdObra = pedido.IdObra;
            this.TipoEntrega = pedido.TipoEntrega;
            this.Reposicao = pedido.TipoVenda == (int)TipoVendaPedido.Reposição;
            this.IdParcela = pedido.IdParcela;
            this.MaoDeObra = pedido.MaoDeObra;
            this.TipoVenda = pedido.TipoVenda;
            this.IsPedidoProducaoCorte = pedido.IdPedidoRevenda.HasValue
                && pedido.TipoPedido == (int)TipoPedidoEnum.Producao;
        }

        internal ContainerCalculoDTO(Projeto projeto)
        {
            this.Tipo = TipoContainer.Projeto;
            this.Id = projeto.IdProjeto;
            this.IdObra = null;
            this.TipoEntrega = projeto.TipoEntrega;
            this.TipoVenda = projeto.TipoVenda;
            this.Reposicao = projeto.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição;
            this.MaoDeObra = false;
            this.IsPedidoProducaoCorte = false;
            this.IdParcela = null;
            this.Cliente = projeto.IdCliente.HasValue
                ? new ClienteDTO(projeto.IdCliente.Value)
                : null;
        }
    }
}
