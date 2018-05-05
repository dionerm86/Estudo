using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.RelModel
{
    /// <summary>
    /// Classe de retorno para exibição na verificação de importação
    /// </summary>
    public class LinhaRemessaRetorno
    {
        public int NumeroLinha { get; set; }

        public int Agencia { get; set; }

        public long Conta { get; set; }

        public string NossoNumero { get; set; }

        public string NumeroDocumento { get; set; }

        public decimal ValorTitulo { get; set; } 

        public decimal ValorPagoPeloSacado { get; set; }

        public decimal JurosMultaEncargos { get; set; }

        public DateTime DataVencimento { get; set; }

        public bool Quitada { get; set; }
    }
}
