using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Global
{
    /// <summary>
    /// Assinatura da entidade cor de ferragem.
    /// </summary>
    public interface ICorFerragem
    {
        /// <summary>
        /// Identificador.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Descrição.
        /// </summary>
        string Descricao { get; }
    }

    /// <summary>
    /// Assinatura do fluxo de cor de ferragem.
    /// </summary>
    public interface IFerragemFluxo
    {

        /// <summary>
        /// Recupera as cores dos ferragens.
        /// </summary>
        /// <returns></returns>
        IList<ICorFerragem> ObterCores();
    }
}
