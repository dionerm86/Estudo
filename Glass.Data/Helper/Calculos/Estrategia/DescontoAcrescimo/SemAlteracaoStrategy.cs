using System;
using System.Collections.Generic;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Data.Model;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo
{
    class DescontoAcrescimoSemAlteracaoStrategy : IDescontoAcrescimoStrategy
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
