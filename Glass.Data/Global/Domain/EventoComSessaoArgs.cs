using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Data.Domain
{
    /// <summary>
    /// Representa o evento de domínio com a sessão do GDA.
    /// </summary>
    public abstract class EventoComSessaoArgs
    {
        /// <summary>
        /// Obtém a sessão associada com o evento.
        /// </summary>
        public GDA.GDASession Sessao { get; }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="EventoComSessaoArgs"/>.
        /// </summary>
        /// <param name="sessao">Sessão associada com o evento.</param>
        public EventoComSessaoArgs(GDA.GDASession sessao)
        {
            this.Sessao = sessao;
        }
    }
}
