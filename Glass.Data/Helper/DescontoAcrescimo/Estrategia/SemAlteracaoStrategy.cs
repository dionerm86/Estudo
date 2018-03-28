using System.Collections.Generic;
using Glass.Data.Helper.DescontoAcrescimo.Estrategia.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia
{
    class SemAlteracaoStrategy : ICalculoStrategy
    {
        public bool Calcular(TipoValor tipo, decimal valorAplicar, decimal totalAtual, decimal totalDesejado,
            IEnumerable<IProdutoDescontoAcrescimo> produtos, IContainerDescontoAcrescimo container)
        {
            return false;
        }
    }
}
