using GDA;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo
{
    public interface IDescontoAcrescimoStrategy
    {
        bool Aplicar(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos,
            TipoValor tipo, decimal valorAplicar);

        bool Remover(GDASession sessao, IContainerCalculo container, IEnumerable<IProdutoCalculo> produtos);
    }
}
