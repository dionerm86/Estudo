using System.Collections.Generic;

namespace Glass.Fiscal.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio do centro de custo.
    /// </summary>
    public interface ICentroCustoFluxo
    {
        #region Centro de custo

        /// <summary>
        /// Recupera os descritores dos centros de custo.
        /// </summary>
        IList<Colosoft.IEntityDescriptor> ObtemDescritoresCentroCusto();

        #endregion
    }
}
