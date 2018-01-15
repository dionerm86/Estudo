using System.Collections.Generic;

namespace Glass.PCP.Negocios
{
    /// <summary>
    /// Assinatura do fluxo da mesa de corte.
    /// </summary>
    public interface IMesaCorteFluxo
    {
        #region ArquivoMesaCorte

        /// <summary>
        /// Recupera os descritores dos arquivos de mesa de corte.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemArquivosMesaCorte();

        #endregion
    }
}
