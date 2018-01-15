using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Pedido
{
    /// <summary>
    /// Assinatura da regra de negocio do pedido.
    /// </summary>
    public interface IPedidoFluxo
    {
        /// <summary>
        /// Gera o pedido baseado no identificador do projeto.
        /// </summary>
        /// <param name="idProjeto"></param>
        /// <returns></returns>
        int GerarPedido(int idProjeto);

        /// <summary>
        /// Recuperar os pedidos do cliente.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="codCliente"></param>
        /// <param name="dtIni"></param>
        /// <param name="dtFim"></param>
        /// <param name="apenasAbertos"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IList<IPedidoDescritor> ObterPedidos(int idPedido, string codCliente, DateTime? dtIni, DateTime? dtFim, bool apenasAbertos,
            string sortExpression, int startRow, int pageSize);

        /// <summary>
        /// Salva a foto do pedido.
        /// </summary>
        /// <param name="fotoPedido"></param>
        Glass.Api.Pedido.IFotoPedidoDescritor SalvarFotoPedido(Glass.Api.Pedido.IFotoPedido fotoPedido);

        /// <summary>
        /// Recupera as fotos do pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        IList<Glass.Api.Pedido.IFotoPedidoDescritor> ObterFotosPedido(int idPedido);

        /// <summary>
        /// Apaga a foto do pedido.
        /// </summary>
        /// <param name="idFoto"></param>
        void ApagarFotoPedido(int idFoto);

        /// <summary>
        /// Atualiza a descricao da foto pedido.
        /// </summary>
        /// <param name="idFoto"></param>
        /// <param name="descricao"></param>
        void AtualizarFotoPedidoDescricao(int idFoto, string descricao);
    }
}
