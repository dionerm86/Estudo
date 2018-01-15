using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio de localização.
    /// </summary>
    public interface ILocalizacaoFluxo
    {
        #region Cidade

        /// <summary>
        /// Recupera os descritores da cidades do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemCidades();

        /// <summary>
        /// Recupera os dados da cidade.
        /// </summary>
        /// <param name="idCidade"></param>
        /// <returns></returns>
        Entidades.Cidade ObtemCidade(int idCidade);

        #endregion

        #region Uf

        /// <summary>
        /// Recupera as unidades federativas.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Colosoft.IEntityDescriptor> ObtemUfs();

        #endregion

        #region Pais

        /// <summary>
        /// Recupera os descritores dos paises.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemPaises();

        /// <summary>
        /// Recupera os dados do país.
        /// </summary>
        /// <param name="idPais"></param>
        /// <returns></returns>
        Entidades.Pais ObtemPais(int idPais);

        #endregion
    }
}
