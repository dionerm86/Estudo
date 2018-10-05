using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Data.Domain
{
    /// <summary>
    /// Representa evento associados com produtos.
    /// </summary>
    public class ProdutoEventoArgs : EventoComSessaoArgs
    {
        /// <summary>
        /// Obtém o produto associado.
        /// </summary>
        public Model.Produto Produto { get; }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ProdutoEventoArgs"/>.
        /// </summary>
        /// <param name="sessao">Sessão para ser usada no evento;</param>
        /// <param name="produto">Produto associado.</param>
        public ProdutoEventoArgs(GDA.GDASession sessao, Model.Produto produto)
            : base(sessao)
        {
            this.Produto = produto;
        }
    }
}
