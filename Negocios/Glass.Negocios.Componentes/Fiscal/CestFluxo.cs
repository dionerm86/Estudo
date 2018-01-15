using System.Collections.Generic;
using Colosoft;
using Glass.Fiscal.Negocios.Entidades;

namespace Glass.Fiscal.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio do CFOP.
    /// </summary>
    public class CestFluxo : ICestFluxo
    {
        public IEnumerable<Cest> ObtemCESTs(string codigo)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cest>();

            if (!string.IsNullOrEmpty(codigo))
                consulta.WhereClause
                    .And("Codigo=?codigo")
                    .Add("?codigo", codigo);

            return consulta.ToVirtualResultLazy<Cest>();
        }
    }
}
