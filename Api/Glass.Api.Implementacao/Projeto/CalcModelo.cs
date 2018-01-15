using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Projeto
{
    /// <summary>
    /// Representa um modelo para cálculo
    /// </summary>
    public class CalcModelo : Glass.Api.Projeto.ICalcModelo
    {
        public int IdProjeto { get; set; }

        public int IdItemProjeto { get; set; }

        public int IdProjetoModelo { get; set; }

        public bool MedidaExata { get; set; }

        public string Ambiente { get; set; }

        public int EspessuraVidro { get; set; }

        public int IdCorVidro { get; set; }

        public List<EditableItemValued<Glass.Data.Model.MedidaProjetoModelo>> Medidas { get; set; }

        public List<EditableItemValued<Glass.Data.Model.PecaItemProjeto>> Pecas { get; set; }
    }
}
