using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api.Global
{
    /// <summary>
    /// Assunatura da entidade de negocio da cor do vidro.
    /// </summary>
    public interface ICorVidro
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
    /// Assinatura da entidade de negocio da espessura do vidro.
    /// </summary>
    public interface IEspessuraVidro
    {
        /// <summary>
        /// Descrição.
        /// </summary>
        string Descricao { get; }

        /// <summary>
        /// Valor.
        /// </summary>
        int Valor { get; }
    }

    /// <summary>
    /// Assinatura do fluxo de negocio do vidro.
    /// </summary>
    public interface IVidroFluxo
    {
        /// <summary>
        /// Recupera as epessuras do vidro.
        /// </summary>
        /// <returns></returns>
        IList<IEspessuraVidro> ObterEspessuras();

        /// <summary>
        /// Recupera as cores do vidro.
        /// </summary>
        /// <returns></returns>
        IList<ICorVidro> ObterCores(); 
    }
}
