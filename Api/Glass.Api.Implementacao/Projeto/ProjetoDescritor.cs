using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Projeto
{

    /// <summary>
    /// Implementação do descritor da entidade projeto.
    /// </summary>
    public class ProjetoDescritor : Glass.Api.Projeto.IProjetoDescritor
    {
        public int Id { get; }

        public string CodPed { get; }

        public string Data { get;  }

        public string Valor { get; }

        public int Situacao { get; }

        public ProjetoDescritor(Glass.Data.Model.Projeto projeto)
        {
            Id = (int)projeto.IdProjeto;
            Valor = projeto.Total.ToString("c", new CultureInfo("pt-BR"));
            CodPed = projeto.PedCli == null ? string.Empty : projeto.PedCli;
            Data = projeto.DataCad.ToString("dd/MM/yyyy");
            Situacao = projeto.Situacao;
        }
    }
}
