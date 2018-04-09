using GDA;
using Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo.Enum;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.Helper.Calculos.Estrategia.DescontoAcrescimo
{
    public interface IDescontoAcrescimoStrategy
    {
        bool Aplicar(GDASession sessao, TipoValor tipo, decimal valorAplicar, IEnumerable<IProdutoCalculo> produtos);

        bool Remover(GDASession sessao, IEnumerable<IProdutoCalculo> produtos);
    }
}
