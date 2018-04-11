using System;
using static Glass.Data.Model.Pedido;

namespace Glass.Data.Model.Calculos
{
    public class ContainerCalculoDTO : IContainerCalculo
    {
        public enum TipoContainer
        {
            Orcamento,
            Pedido,
            Projeto
        }

        public TipoContainer? Tipo { get; set; }
        public uint Id { get; set; }
        public ICliente Cliente { get; set; }
        public uint? IdObra { get; set; }
        public uint? IdParcela { get; set; }
        public bool IsPedidoProducaoCorte { get; set; }
        public bool Reposicao { get; set; }
        public bool MaoDeObra { get; set; }
        public int? TipoEntrega { get; set; }
        public int? TipoVenda { get; set; }

        public ContainerCalculoDTO()
        {
            Cliente = new ClienteDTO(() => 0);
        }

        internal ContainerCalculoDTO(Pedido pedido)
            : this(pedido as IContainerCalculo)
        {
            Tipo = TipoContainer.Pedido;
        }

        internal ContainerCalculoDTO(PedidoEspelho pedidoEspelho)
            : this(pedidoEspelho as IContainerCalculo)
        {
            Tipo = TipoContainer.Pedido;
        }

        private ContainerCalculoDTO(IContainerCalculo container)
        {
            Id = container.Id;
            Cliente = container.Cliente;
            IdObra = container.IdObra;
            IdParcela = container.IdParcela;
            IsPedidoProducaoCorte = container.IsPedidoProducaoCorte;
            Reposicao = container.Reposicao;
            MaoDeObra = container.MaoDeObra;
            TipoEntrega = container.TipoEntrega;
            TipoVenda = container.TipoVenda;
        }
    }
}
