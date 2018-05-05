using System.Collections.Generic;

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
        public IDadosCliente Cliente { get; set; }
        public IDadosAmbiente Ambientes { get; set; }
        public uint? IdObra { get; set; }
        public uint? IdParcela { get; set; }
        public bool IsPedidoProducaoCorte { get; set; }
        public bool Reposicao { get; set; }
        public bool MaoDeObra { get; set; }
        public int? TipoEntrega { get; set; }
        public int? TipoVenda { get; set; }

        public int TipoAcrescimo { get; set; }
        public decimal Acrescimo { get; set; }
        public int TipoDesconto { get; set; }
        public decimal Desconto { get; set; }
        public float PercComissao { get; set; }

        public ContainerCalculoDTO()
        {
            Cliente = new ClienteDTO(() => 0);
            Ambientes = new DadosAmbienteDTO(this, () => new IAmbienteCalculo[0]);
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
            Ambientes = container.Ambientes;
            IdObra = container.IdObra;
            IdParcela = container.IdParcela;
            IsPedidoProducaoCorte = container.IsPedidoProducaoCorte;
            Reposicao = container.Reposicao;
            MaoDeObra = container.MaoDeObra;
            TipoEntrega = container.TipoEntrega;
            TipoVenda = container.TipoVenda;
            TipoAcrescimo = container.TipoAcrescimo;
            Acrescimo = container.Acrescimo;
            TipoDesconto = container.TipoDesconto;
            Desconto = container.Desconto;
            PercComissao = container.PercComissao;
        }
    }
}
