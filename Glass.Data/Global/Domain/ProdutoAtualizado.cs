using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Data.Domain
{
    /// <summary>
    /// Representa o evento de atualização do produto.
    /// </summary>
    public class ProdutoAtualizado : Colosoft.Domain.CompositeDomainEvent<ProdutoEventoArgs>
    {
    }
}
