using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Financeiro.Negocios
{
    public interface IChequesFluxo
    {
        /// <summary>
        /// Método de geração de arquivo de exportação do cheque.
        /// </summary>
        /// <param name="idDeposito">id do depósito.</param>
        /// <returns>arquivo de exportação.</returns>
        Entidades.Cheques.Arquivo GerarArquivoCheques(uint idDeposito);
    }
}
