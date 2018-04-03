using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo
{
    public interface IDescontoAcrescimoStrategy
    {
        bool Aplicar(TipoValor tipo, decimal valorAplicar, IEnumerable<IProdutoDescontoAcrescimo> produtos,
            IContainerDescontoAcrescimo container);

        bool Remover(IEnumerable<IProdutoDescontoAcrescimo> produtos, IContainerDescontoAcrescimo container);
    }
}
