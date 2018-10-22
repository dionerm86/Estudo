using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Data.Domain
{
    /// <summary>
    /// Argumentos dos eventos associados com nota fiscal.
    /// </summary>
    public class NotaFiscalEventoArgs : EventoComSessaoArgs
    {
        /// <summary>
        /// Obtém a nota fiscal associada.
        /// </summary>
        public Model.NotaFiscal NotaFiscal { get; }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="NotaFiscalEventoArgs"/>.
        /// </summary>
        /// <param name="sessao">Sessão com o banco de dados.</param>
        /// <param name="notaFiscal">Nota fiscal associada.</param>
        public NotaFiscalEventoArgs(GDA.GDASession sessao, Model.NotaFiscal notaFiscal)
            : base(sessao)
        {
            this.NotaFiscal = notaFiscal;
        }
    }
}
