using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Pedido
{
    /// <summary>
    /// Assinatura de uma foto de pedido.
    /// </summary>
    public interface IFotoPedido
    {
        /// <summary>
        /// Identificador da foto.
        /// </summary>
        int IdFoto { get; }

        /// <summary>
        /// Identificador do pedido.
        /// </summary>
        int IdPedido { get; }

        /// <summary>
        /// Descrição da foto.
        /// </summary>
        string Descricao { get; }

        /// <summary>
        /// Imagem.
        /// </summary>
        byte[] Imagem
        {
            get;
        }

        /// <summary>
        /// Extensao da imagem.
        /// </summary>
        string Extensao { get; }
    }
}
