using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Rentabilidade
{
    /// <summary>
    /// Assinatura do provedor do descritor de registro de rentabilidade.
    /// </summary>
    public interface IProvedorDescritorRegistroRentabilidade
    {
        #region Métodos

        /// <summary>
        /// Recupera os descritores com base no tipo de registro informado.
        /// </summary>
        /// <param name="tipo">Tipo de registro que será usado para filtrar os descritores.</param>
        /// <returns></returns>
        IEnumerable<DescritorRegistroRentabilidade> ObterDescritores(TipoRegistroRentabilidade tipo);

        /// <summary>
        /// Recupera o descritor.
        /// </summary>
        /// <param name="tipo">Tipo do registro.</param>
        /// <param name="id">Identificador do registor.</param>
        /// <returns></returns>
        DescritorRegistroRentabilidade ObterDescritor(TipoRegistroRentabilidade tipo, int id);

        /// <summary>
        /// Recupera o identificador do registro pelo tipo e pelo nome informado.
        /// </summary>
        /// <param name="tipo">Tipo de registro.</param>
        /// <param name="nome">Nome do registro.</param>
        /// <returns>Identificador do registro.</returns>
        int ObterRegistro(TipoRegistroRentabilidade tipo, string nome);

        #endregion
    }
}
