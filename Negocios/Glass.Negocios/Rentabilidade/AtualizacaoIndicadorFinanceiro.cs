using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Rentabilidade.Negocios
{
    /// <summary>
    /// Representa os dados de atualização do indicador financeiro.
    /// </summary>
    public class AtualizacaoIndicadorFinanceiro
    {
        /// <summary>
        /// Obtém ou define o nome do indicador.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Obtém ou define o valor do indicador.
        /// </summary>
        public decimal Valor { get; set; }

        /// <summary>
        /// Obtém ou define a data do valor do indicador.
        /// </summary>
        public DateTime Data { get; set; }
    }
}
