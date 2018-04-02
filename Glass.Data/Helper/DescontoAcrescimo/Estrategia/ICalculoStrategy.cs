using Glass.Data.Helper.DescontoAcrescimo.Estrategia.Enum;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.Helper.DescontoAcrescimo.Estrategia
{
    public interface ICalculoStrategy
    {
        bool Aplicar(TipoValor tipo, decimal valorAplicar, IEnumerable<IProdutoDescontoAcrescimo> produtos,
            IContainerDescontoAcrescimo container);

        bool Remover(IEnumerable<IProdutoDescontoAcrescimo> produtos, IContainerDescontoAcrescimo container);
    }
}
