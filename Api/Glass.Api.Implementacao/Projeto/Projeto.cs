using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Projeto
{
    /// <summary>
    /// Representa um projeto.
    /// </summary>
    public class Projeto
    {
        public int IdProjeto { get; set; }

        public int IdCliente { get; set; }

        public int IdTipoEntrega { get; set; }

        public string CodPedido { get; set; }
    }
}
