using Colosoft;
using System.Collections.Generic;

namespace Glass.Fiscal.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de nota fiscal.
    /// </summary>
    public class NotaFiscalFluxo : INotaFiscalFluxo
    {
        /// <summary>
        /// Recupera os códigos da situação da operação do Simples Nacional.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Colosoft.IEntityDescriptor> ObtemCSOSNs()
        {
            yield return new EntityDescriptor(101, "101");
            yield return new EntityDescriptor(102, "102");
            yield return new EntityDescriptor(103, "103");
            yield return new EntityDescriptor(300, "300");
            yield return new EntityDescriptor(400, "400");
            yield return new EntityDescriptor(201, "201");
            yield return new EntityDescriptor(202, "202");
            yield return new EntityDescriptor(203, "203");
            yield return new EntityDescriptor(500, "500");
            yield return new EntityDescriptor(900, "900");
        }

        /// <summary>
        /// Recupera as possíveis origens de CST de um produto.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Colosoft.IEntityDescriptor> ObtemOrigemCST()
        {
            yield return new EntityDescriptor(0, "0");
            yield return new EntityDescriptor(1, "1");
            yield return new EntityDescriptor(2, "2");
            yield return new EntityDescriptor(3, "3");
            yield return new EntityDescriptor(4, "4");
            yield return new EntityDescriptor(5, "5");
            yield return new EntityDescriptor(6, "6");
            yield return new EntityDescriptor(7, "7");
            yield return new EntityDescriptor(8, "8");
        }
    }
}
