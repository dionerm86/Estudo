using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Pedido
{
    /// <summary>
    /// Representa os filtro da pesquisa de pedido.
    /// </summary>
    public class PesquisaPedido
    {
        public int IdPedido { get; set; }

        public string CodCliente { get; set; }

        public DateTime DtIni { get; set; }

        public DateTime DtFim { get; set; }

        public bool ApenasAbertos { get; set; }

        public string SortExpression { get; set; }

        public int StartRow { get; set; }

        public int PageSize { get; set; }
    }
}
