using Glass.Data.Helper.DescontoAcrescimo.Estrategia.Enum;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia
{
    interface ICalculoStrategy
    {
        bool Calcular(TipoValor tipo, decimal valorAplicar, decimal totalAtual, decimal totalDesejado,
            IEnumerable<IProdutoDescontoAcrescimo> produtos, IContainerDescontoAcrescimo container);
    }
}
