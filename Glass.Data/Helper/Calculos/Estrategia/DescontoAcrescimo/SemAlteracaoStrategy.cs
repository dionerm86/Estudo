using System;
using System.Collections.Generic;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Data.Model;
using Glass.Pool;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo
{
    class SemAlteracaoStrategy : PoolableObject<SemAlteracaoStrategy>, IDescontoAcrescimoStrategy
    {
        private SemAlteracaoStrategy() { }

        public bool Aplicar(TipoValor tipo, decimal valorAplicar, IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container)
        {
            return false;
        }

        public bool Remover(IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container)
        {
            return false;
        }
    }
}
