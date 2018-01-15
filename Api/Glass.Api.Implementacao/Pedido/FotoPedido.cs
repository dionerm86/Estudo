using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Pedido
{
    /// <summary>
    /// Representa uma foto de pedido.
    /// </summary>
    public class FotoPedido : Glass.Api.Pedido.IFotoPedido
    {
        /// <summary>
        /// Identificador da foto.
        /// </summary>
        public int IdFoto { get; set; }

        /// <summary>
        /// Identificador do pedido.
        /// </summary>
        public int IdPedido { get; set; }

        /// <summary>
        /// Imagem.
        /// </summary>
        public byte[] Imagem { get; set; }

        /// <summary>
        ///  Descricao.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Dados da imagem.
        /// </summary>
        public string ImgData { get; set; }

        /// <summary>
        /// Extensao da imagem.
        /// </summary>
        public string Extensao { get; set; }
    }
}
