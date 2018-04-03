using Glass.Data.Model;
using Glass.Data.Model.Internal;

namespace Glass.Data.Helper.Calculos
{
    public static class ContainerDescontoAcrescimoExtensions
    {
        public static int? IdPedido(this IContainerCalculo container)
        {
            return container is Pedido
                || container is PedidoEspelho 
                || ContainerInternoEDoTipo(container, ContainerCalculo.TipoContainer.Pedido)
                ? (int?)container.Id
                : null;
        }

        public static int? IdOrcamento(this IContainerCalculo container)
        {
            return container is Orcamento
                || ContainerInternoEDoTipo(container, ContainerCalculo.TipoContainer.Orcamento)
                ? (int?)container.Id
                : null;
        }

        public static int? IdProjeto(this IContainerCalculo container)
        {
            return container is Projeto
                || ContainerInternoEDoTipo(container, ContainerCalculo.TipoContainer.Projeto)
                ? (int?)container.Id
                : null;
        }

        private static bool ContainerInternoEDoTipo(IContainerCalculo container, ContainerCalculo.TipoContainer tipo)
        {
            var containerInterno = container as ContainerCalculo;
            if (containerInterno == null)
                return false;

            return containerInterno.Tipo == tipo;
        }
    }
}
