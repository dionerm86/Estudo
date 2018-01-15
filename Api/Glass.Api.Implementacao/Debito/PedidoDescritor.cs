using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Implementacao.Debito
{
    public class DebitoDescritor : Glass.Api.Debito.IDebitoDescritor
    {
        public string Referencia { get; set; }

        public decimal Valor { get; set; }

        public string Vencimento { get; set; }

        public DebitoDescritor(Glass.Data.Model.ContasReceber contasReceber)
        {
            Referencia = "Ped.: " + (int)contasReceber.IdPedido.Value;
            Valor = contasReceber.ValorVec;
            Vencimento = contasReceber.DataVec == DateTime.MinValue ? null : contasReceber.DataVec.ToString("dd/MM/yyyy");
        }
    }
}
