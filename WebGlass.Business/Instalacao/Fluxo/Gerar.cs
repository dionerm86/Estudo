using System;
using Glass.Data.DAL;

namespace WebGlass.Business.Instalacao.Fluxo
{
    public sealed class Gerar : BaseFluxo<Gerar>
    {
        private Gerar() { }

        public uint GerarInstalacao(uint idPedido, int tipoInstalacao)
        {
            DateTime dataEntrega = PedidoDAO.Instance.ObtemDataEntrega(null, idPedido).Value;
            return InstalacaoDAO.Instance.NovaInstalacao(idPedido, dataEntrega, tipoInstalacao, true);
        }
    }
}
