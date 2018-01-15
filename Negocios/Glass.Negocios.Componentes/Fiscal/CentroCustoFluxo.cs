using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Fiscal.Negocios.Componentes
{
    public class CentroCustoFluxo : Negocios.ICentroCustoFluxo
    {
        /// <summary>
        /// Recupera os descritores dos centros de custo.
        /// </summary>
        public IList<Colosoft.IEntityDescriptor> ObtemDescritoresCentroCusto()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.CentroCusto>()
                .ProcessResultDescriptor<Entidades.CentroCusto>()
                .ToList();
        }
    }
}
