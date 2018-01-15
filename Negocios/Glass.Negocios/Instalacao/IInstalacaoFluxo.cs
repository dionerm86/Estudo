using System.Collections.Generic;

namespace Glass.Instalacao.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de instalação do sistema.
    /// </summary>
    public interface IInstalacaoFluxo
    {
        #region Fixacao Vidro

        /// <summary>
        /// Pesquisa as fixações do vidro.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.FixacaoVidro> PesquisarFixacoesVidro();

        /// <summary>
        /// Recupera as fixações dos vidros.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemFixacoesVidro();

        /// <summary>
        /// Recupera a fixacao do vidro.
        /// </summary>
        /// <param name="idFixacaoVidro"></param>
        /// <returns></returns>
        Entidades.FixacaoVidro ObtemFixacaoVidro(int idFixacaoVidro);

        /// <summary>
        /// Salva a fixação do vidro.
        /// </summary>
        /// <param name="fixacaoVidro"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarFixacaoVidro(Entidades.FixacaoVidro fixacaoVidro);

        /// <summary>
        /// Apaga a fixação do vidro.
        /// </summary>
        /// <param name="idFixacaoVidro"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarFixacaoVidro(int idFixacaoVidro);

        /// <summary>
        /// Apaga a fixação do vidro.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarFixacaoVidro(Entidades.FixacaoVidro fixacaoVidro);

        #endregion
    }
}
