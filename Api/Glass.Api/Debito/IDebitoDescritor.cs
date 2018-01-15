using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Debito
{
    /// <summary>
    /// Representa o descritor do debito.
    /// </summary>
    public interface IDebitoDescritor
    {
        string Referencia { get; }

        decimal Valor { get; }

        string Vencimento { get; }
    }
}
