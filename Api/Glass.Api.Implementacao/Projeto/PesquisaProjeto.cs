using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Projeto
{
    public class PesquisaProjeto
    {
        public int IdProjeto { get; set; }

        public DateTime DtIni { get; set; }

        public DateTime DtFim { get; set; }

        public int StartRow { get; set; }

        public int PageSize { get; set; }
    }
}
