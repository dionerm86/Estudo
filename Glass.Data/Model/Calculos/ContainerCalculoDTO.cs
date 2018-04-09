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
    }
}
