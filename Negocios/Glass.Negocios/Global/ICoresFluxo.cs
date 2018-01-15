using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio das cores do sistema.
    /// </summary>
    public interface ICoresFluxo
    {
        #region Cor Vidro

        /// <summary>
        /// Pesquisa as cores dos vidros do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.CorVidro> PesquisarCoresVidro();

        /// <summary>
        /// Recupera as cores de vidro.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemCoresVidro();

        /// <summary>
        /// Recupera a cor do vidro.
        /// </summary>
        /// <param name="idCorVidro"></param>
        /// <returns></returns>
        Entidades.CorVidro ObtemCorVidro(int idCorVidro);

        /// <summary>
        /// Salva os dados do cor do vidro.
        /// </summary>
        /// <param name="corVidro"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarCorVidro(Entidades.CorVidro corVidro);

        /// <summary>
        /// Apaga a cor do vidro.
        /// </summary>
        /// <param name="corVidro">Cor do vidro.</param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarCorVidro(Entidades.CorVidro corVidro);

        #endregion

        #region Cor Ferragem

        /// <summary>
        /// Pesquisa as cores de ferragem.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.CorFerragem> PesquisarCoresFerragem();

        /// <summary>
        /// Recupera as cores de ferragem.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemCoresFerragem();

        /// <summary>
        /// Recupera a cor da ferragem.
        /// </summary>
        /// <param name="idCorFerragem">Identificador da cor.</param>
        /// <returns></returns>
        Entidades.CorFerragem ObtemCorFerragem(int idCorFerragem);

        /// <summary>
        /// Salva a cor da ferragem.
        /// </summary>
        /// <param name="corFerragem"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarCorFerragem(Entidades.CorFerragem corFerragem);

        /// <summary>
        /// Apaga a cor da ferragem.
        /// </summary>
        /// <param name="corFerragem"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarCorFerragem(Entidades.CorFerragem corFerragem);

        #endregion

        #region Cor Aluminio

        /// <summary>
        /// Pesquisa as cores de aluminio.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.CorAluminio> PesquisarCoresAluminio();

        /// <summary>
        /// Recupera as cores de alumínio.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemCoresAluminio();

        /// <summary>
        /// Recupera as cores de alumínio.
        /// </summary>
        /// <param name="idCorAluminio">Identificador da cor.</param>
        /// <returns></returns>
        Entidades.CorAluminio ObtemCorAluminio(int idCorAluminio);

        /// <summary>
        /// Salva a cor da alumínio.
        /// </summary>
        /// <param name="corAluminio"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarCorAluminio(Entidades.CorAluminio corAluminio);

        /// <summary>
        /// Apaga a cor da aluminio.
        /// </summary>
        /// <param name="corAluminio"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarCorAluminio(Entidades.CorAluminio corAluminio);

        #endregion
    }
}
