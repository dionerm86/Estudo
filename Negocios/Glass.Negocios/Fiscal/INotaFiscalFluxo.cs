using System.Collections.Generic;

namespace Glass.Fiscal.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de nota fiscal.
    /// </summary>
    public interface INotaFiscalFluxo
    {
        /// <summary>
        /// Recupera os códigos da situação da operação do Simples Nacional.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Colosoft.IEntityDescriptor> ObtemCSOSNs();

        /// <summary>
        /// Recupera as possíveis origens de CST de um produto.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Colosoft.IEntityDescriptor> ObtemOrigemCST();
    }
}
