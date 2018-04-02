using System;
using System.Collections.Generic;
using Glass.Data.Helper.DescontoAcrescimo.Estrategia.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia
{
    class SemAlteracaoStrategy : ICalculoStrategy
    {
        public bool Aplicar(TipoValor tipo, decimal valorAplicar, IEnumerable<IProdutoDescontoAcrescimo> produtos,
            IContainerDescontoAcrescimo container)
        {
            return false;
        }

        public bool Remover(IEnumerable<IProdutoDescontoAcrescimo> produtos, IContainerDescontoAcrescimo container)
        {
            return false;
        }
    }
}
