using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Fiscal.Negocios
{
    /// <summary>
    /// Assinatura da calculadora de imposto.
    /// </summary>
    public interface ICalculadoraImposto
    {
        /// <summary>
        /// Realiza o calculo do impostao para o container de itens informado.
        /// </summary>
        /// <param name="container"></param>
        ICalculoImpostoResultado Calcular(IItemImpostoContainer container);
    }
}
