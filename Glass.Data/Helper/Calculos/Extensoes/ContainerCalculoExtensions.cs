using Glass.Data.Model;
using Glass.Data.Model.Calculos;

namespace Glass.Data.Helper.Calculos
{
    public static class ContainerCalculoExtensions
    {
        public static int? IdPedido(this IContainerCalculo container)
        {
            return container is Pedido
                || container is PedidoEspelho 
                || ContainerInternoEDoTipo(container, ContainerCalculoDTO.TipoContainer.Pedido)
                ? (int?)container.Id
                : null;
        }

        public static int? IdOrcamento(this IContainerCalculo container)
        {
            return container is Orcamento
                || ContainerInternoEDoTipo(container, ContainerCalculoDTO.TipoContainer.Orcamento)
                ? (int?)container.Id
                : null;
        }

        public static int? IdProjeto(this IContainerCalculo container)
        {
            return container is Projeto
                || ContainerInternoEDoTipo(container, ContainerCalculoDTO.TipoContainer.Projeto)
                ? (int?)container.Id
                : null;
        }

        private static bool ContainerInternoEDoTipo(IContainerCalculo container, ContainerCalculoDTO.TipoContainer tipo)
        {
            var containerInterno = container as ContainerCalculoDTO;
            if (containerInterno == null)
                return false;

            return containerInterno.Tipo == tipo;
        }
    }
}
