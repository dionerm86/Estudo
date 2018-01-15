using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Pedido
{
    /// <summary>
    /// Assinatura do descritor da foto do pedido.
    /// </summary>
    public interface IFotoPedidoDescritor
    {
        /// <summary>
        /// Identificador do pedido.
        /// </summary>
        int IdFoto { get; }

        /// <summary>
        /// Descricão da imagem.
        /// </summary>
        string Descricao { get; }

        /// <summary>
        /// Endereço da imagem.
        /// </summary>
        string ImageUrl { get; }
    }
}
