using Glass.Data.Model;
using Glass.Data.Model.Internal;

namespace Glass.Data.Helper.Calculos
{
    public static class ContainerDescontoAcrescimoExtensions
    {
        public static int? IdPedido(this IContainerDescontoAcrescimo container)
        {
            return container is Pedido
                || container is PedidoEspelho 
                || ContainerInternoEDoTipo(container, ContainerDescontoAcrescimo.TipoContainer.Pedido)
                ? (int?)container.Id
                : null;
        }

        public static int? IdOrcamento(this IContainerDescontoAcrescimo container)
        {
            return container is Orcamento
                || ContainerInternoEDoTipo(container, ContainerDescontoAcrescimo.TipoContainer.Orcamento)
                ? (int?)container.Id
                : null;
        }

        public static int? IdProjeto(this IContainerDescontoAcrescimo container)
        {
            return container is Projeto
                || ContainerInternoEDoTipo(container, ContainerDescontoAcrescimo.TipoContainer.Projeto)
                ? (int?)container.Id
                : null;
        }

        private static bool ContainerInternoEDoTipo(IContainerDescontoAcrescimo container, ContainerDescontoAcrescimo.TipoContainer tipo)
        {
            var containerInterno = container as ContainerDescontoAcrescimo;
            if (containerInterno == null)
                return false;

            return containerInterno.Tipo == tipo;
        }
    }
}
