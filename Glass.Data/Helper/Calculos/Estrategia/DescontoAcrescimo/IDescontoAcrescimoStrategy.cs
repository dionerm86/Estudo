using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo
{
    public interface IDescontoAcrescimoStrategy
    {
        bool Aplicar(TipoValor tipo, decimal valorAplicar, IEnumerable<IProdutoCalculo> produtos,
            IContainerCalculo container);

        bool Remover(IEnumerable<IProdutoCalculo> produtos, IContainerCalculo container);
    }
}
