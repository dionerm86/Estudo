using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Data.Domain
{
    /// <summary>
    /// Representa o evento acionado quando a nota fiscal for gerada.
    /// </summary>
    public sealed class NotaFiscalGerada : Colosoft.Domain.CompositeDomainEvent<NotaFiscalEventoArgs>
    {
    }
}
