using System.Collections.Generic;

namespace Glass.PCP.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio das perdas do sistema.
    /// </summary>
    public interface IPerdaFluxo
    {
        #region Tipo Perda

        /// <summary>
        /// Pesquisa os tipos de perda.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.TipoPerdaPesquisa> PesquisarTiposPerda();

        /// <summary>
        /// Recupera os dados do tipo de perda.
        /// </summary>
        /// <param name="idTipoPerda"></param>
        /// <returns></returns>
        Entidades.TipoPerda ObtemTipoPerda(int idTipoPerda);

        /// <summary>
        /// Salva o tipo de perda.
        /// </summary>
        /// <param name="tipoPerda">Instancia com os dado que serão salvos.</param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarTipoPerda(Entidades.TipoPerda tipoPerda);

        /// <summary>
        /// Apaga os dados do tipo de perda.
        /// </summary>
        /// <param name="tipoPerda"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarTipoPerda(Entidades.TipoPerda tipoPerda);

        #endregion

        #region Subtipo Perda

        /// <summary>
        /// Pesquisa os subtipos de perda associados com a perda informada.
        /// </summary>
        /// <param name="idTipoPerda">Identificador da perda pai.</param>
        /// <returns></returns>
        IList<Entidades.SubtipoPerda> PesquisarSubtiposPerda(int idTipoPerda);

        /// <summary>
        /// Recupera os descritores dos subtipos de perda associados com o tipo da perda.
        /// </summary>
        /// <param name="idTipoPerda">Identificador do tipo da perda.</param>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemSubtiposPerda(int idTipoPerda);

        /// <summary>
        /// Recupera os dados do subtipo de perda.
        /// </summary>
        /// <param name="idSubtipoPerda">Identificador do subtipo.</param>
        /// <returns></returns>
        Entidades.SubtipoPerda ObtemSubtipoPerda(int idSubtipoPerda);

        /// <summary>
        /// Salva o subtipo de perda.
        /// </summary>
        /// <param name="subtipoPerda"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarSubtipoPerda(Entidades.SubtipoPerda subtipoPerda);

        /// <summary>
        /// Apaga o subtipo de perda.
        /// </summary>
        /// <param name="subtipoPerda"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarSubtipoPerda(Entidades.SubtipoPerda subtipoPerda);

        #endregion
    }
}
