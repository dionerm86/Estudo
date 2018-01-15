using System;
using Glass.Data.DAL;

namespace WebGlass.Business.PedidoEspelho.Fluxo
{
    public sealed class BuscarEValidar : BaseFluxo<BuscarEValidar>
    {
        private BuscarEValidar() { }

        public void BuscarCompraPcp(uint idPedido)
        {
            if (!PedidoEspelhoDAO.Instance.ExisteEspelho(idPedido))
                throw new Exception("Pedido não encontrado");

            Glass.Data.Model.PedidoEspelho.SituacaoPedido situacao = PedidoEspelhoDAO.Instance.ObtemSituacao(idPedido);
            if (situacao != Glass.Data.Model.PedidoEspelho.SituacaoPedido.Finalizado && 
                situacao != Glass.Data.Model.PedidoEspelho.SituacaoPedido.Impresso &&
                situacao != Glass.Data.Model.PedidoEspelho.SituacaoPedido.ImpressoComum)
                throw new Exception("Pedido deve estar finalizado para ser utilizado para compra");
        }
    }
}
