using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Debito
{
    public class PesquisaDebito
    {
        public int IdPedido { get; set; }

        public int StartRow { get; set; }

        public int PageSize { get; set; }
    }
}
