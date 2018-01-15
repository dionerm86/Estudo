using System.Collections.Generic;

namespace Glass.Fiscal.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio do CTe.
    /// </summary>
    public interface ICTeFluxo
    {
        #region Seguradora

        /// <summary>
        /// Pesquisa as seguradoras.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.Seguradora> PesquisarSeguradoras();

        /// <summary>
        /// Obtem os descritores das seguradoras.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemSeguradoras();

        /// <summary>
        /// Recupera os dados da seguradora.
        /// </summary>
        /// <param name="idSeguradora"></param>
        /// <returns></returns>
        Entidades.Seguradora ObtemSeguradora(int idSeguradora);

        /// <summary>
        /// Salva os dados da seguradora.
        /// </summary>
        /// <param name="seguradora"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarSeguradora(Entidades.Seguradora seguradora);

        /// <summary>
        /// Apaga os dados da seguradora.
        /// </summary>
        /// <param name="seguradora"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarSeguradora(Entidades.Seguradora seguradora);

        #endregion

        #region Chave de Acesso

        /// <summary>
        /// Pesquisa as chaves de acesso do CT-e do sistema
        /// </summary>
        /// <returns></returns>
        IList<Entidades.Cte.ChaveAcessoCte> PesquisarChavesAcesso(int idCte);

        /// <summary>
        /// Recupera os dados da chave de acesso
        /// </summary>
        /// <param name="IdChaveAcessoCte"></param>
        /// <returns></returns>
        Entidades.Cte.ChaveAcessoCte ObtemChaveAcesso(int IdChaveAcessoCte);

        /// <summary>
        /// Salva os dados da chave de acesso
        /// </summary>
        /// <param name="chaveAcessoCte"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarChaveAcesso(Entidades.Cte.ChaveAcessoCte chaveAcessoCte);

        /// <summary>
        /// Apaga os dados da chave de acesso
        /// </summary>
        /// <param name="idChaveAcessoCte"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarChaveAcesso(int idChaveAcessoCte);

        /// <summary>
        /// Apaga os dados da chave de acesso
        /// </summary>
        /// <param name="chaveAcessoCte"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarChaveAcesso(Entidades.Cte.ChaveAcessoCte chaveAcessoCte);

        #endregion
    }
}
