using System.Collections.Generic;

namespace Glass.Fiscal.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio do CEST.
    /// </summary>
    public interface ICestFluxo
    {
        IEnumerable<Fiscal.Negocios.Entidades.Cest> ObtemCESTs(string Codigo);
    }
}
