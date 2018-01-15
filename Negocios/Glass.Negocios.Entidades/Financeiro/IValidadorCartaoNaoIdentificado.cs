using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Validador da Entidade de Cartão não identificado
    /// </summary>
    public interface IValidadorCartaoNaoIdentificado
    {
        /// <summary>
        /// Verifica se o CNI passado pode ser inserido
        /// </summary>
        bool VerificarPodeInserir(string numAutCartao, int tipoCartao, out string msgErro);
    }
}
