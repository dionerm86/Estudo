using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Pedido
{
    public interface IPedidoDescritor
    {
        /// <summary>
        /// Identificador do pedido.
        /// </summary>
        int Pedido { get; }

        /// <summary>
        /// Código do pedido do cliente.
        /// </summary>
        string PedidoCli { get; }

        /// <summary>
        /// Situação produção.
        /// </summary>
        string SituacaoProd { get; }

        /// <summary>
        /// Valor total do pedido.
        /// </summary>
        string Total { get; }
    }
}
