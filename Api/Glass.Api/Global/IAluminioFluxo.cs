using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Global
{
    /// <summary>
    /// Assinatura da entidade cor de alumínio.
    /// </summary>
    public interface ICorAluminio
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
    /// Assinatura do fluxo de cor de alumínio.
    /// </summary>
    public interface IAluminioFluxo
    {

        /// <summary>
        /// Recupera as cores dos aluminios.
        /// </summary>
        /// <returns></returns>
        IList<ICorAluminio> ObterCores();
    }
}
