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

        public TipoContainer Tipo { get; set; }
        public uint Id { get; set; }
        public ICliente Cliente { get; set; }
        public uint? IdObra { get; set; }
        public uint? IdParcela { get; set; }
        public bool IsPedidoProducaoCorte { get; set; }
        public bool Reposicao { get; set; }
        public bool MaoDeObra { get; set; }
        public int? TipoEntrega { get; set; }
        public int? TipoVenda { get; set; }
        public IDadosProduto DadosProduto { get; private set; }
        public IDadosChapaVidro DadosChapaVidro { get; private set; }

        internal ContainerCalculoDTO()
        {
            DadosProduto = new DadosProduto();
            DadosChapaVidro = new DadosChapaVidro();
        }

        internal ContainerCalculoDTO(Orcamento orcamento)
            : base()
        {
            Tipo = TipoContainer.Orcamento;
            Id = orcamento.IdOrcamento;
            IdObra = null;
            TipoEntrega = orcamento.TipoEntrega;
            TipoVenda = orcamento.TipoVenda;
            Reposicao = orcamento.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição;
            MaoDeObra = false;
            IsPedidoProducaoCorte = false;
            IdParcela = orcamento.IdParcela;
            Cliente = orcamento.IdCliente.HasValue
                ? new ClienteDTO(orcamento.IdCliente.Value)
                : null;
        }

        internal ContainerCalculoDTO(Pedido pedido)
            : base()
        {
            Tipo = TipoContainer.Pedido;
            Id = pedido.IdPedido;
            Cliente = new ClienteDTO(pedido.IdCli);
            IdObra = pedido.IdObra;
            TipoEntrega = pedido.TipoEntrega;
            Reposicao = pedido.TipoVenda == (int)TipoVendaPedido.Reposição;
            IdParcela = pedido.IdParcela;
            MaoDeObra = pedido.MaoDeObra;
            TipoVenda = pedido.TipoVenda;
            IsPedidoProducaoCorte = pedido.IdPedidoRevenda.HasValue
                && pedido.TipoPedido == (int)TipoPedidoEnum.Producao;
        }

        internal ContainerCalculoDTO(Projeto projeto)
            : base()
        {
            Tipo = TipoContainer.Projeto;
            Id = projeto.IdProjeto;
            IdObra = null;
            TipoEntrega = projeto.TipoEntrega;
            TipoVenda = projeto.TipoVenda;
            Reposicao = projeto.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição;
            MaoDeObra = false;
            IsPedidoProducaoCorte = false;
            IdParcela = null;
            Cliente = projeto.IdCliente.HasValue
                ? new ClienteDTO(projeto.IdCliente.Value)
                : null;
        }
    }
}
