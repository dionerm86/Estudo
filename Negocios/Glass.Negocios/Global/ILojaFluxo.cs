using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio de lojas.
    /// </summary>
    public interface ILojaFluxo
    {
        #region Loja

        /// <summary>
        /// Pesquisa as lojas do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.LojaPesquisa> PesquisarLojas();

        /// <summary>
        /// Recupera as lojas cadastradas no sistema.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemLojas();

        /// <summary>
        /// Recupera os descritores das lojas ativas.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemLojasAtivas();

        /// <summary>
        /// Recupera a loja pelo código informado.
        /// </summary>
        /// <param name="IdLoja"></param>
        /// <returns></returns>
        Entidades.Loja ObtemLoja(int IdLoja);

        /// <summary>
        /// Recupera o descritor de uma loja.
        /// </summary>
        /// <returns></returns>
        Colosoft.IEntityDescriptor ObtemDescritorLoja(int idLoja);

        /// <summary>
        /// Salva os dados da loja.
        /// </summary>
        /// <param name="loja"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarLoja(Entidades.Loja loja);

        /// <summary>
        /// Altera a situação da loja (ativa se estiver inativa e vice-versa).
        /// </summary>
        /// <returns></returns>
        Colosoft.Business.SaveResult AlterarSituacaoLoja(int idLoja);

        /// <summary>
        /// Apaga os dados da loja.
        /// </summary>
        /// <param name="loja"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarLoja(Entidades.Loja loja);

        #endregion
    }
}
