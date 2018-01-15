using System.Collections.Generic;

namespace Glass.Financeiro.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de contas bancárias.
    /// </summary>
    public interface IContaBancariaFluxo
    {
        #region Banco

        /// <summary>
        /// Recupera os bancos.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Colosoft.IEntityDescriptor> ObtemBancos();

        #endregion

        #region ContaBanco

        /// <summary>
        /// Pesquisa as contas de bancos
        /// </summary>
        /// <returns></returns>
        IList<Entidades.ContaBancoPesquisa> PesquisarContasBanco();

        /// <summary>
        /// Recupera os descritores das contas bancárias.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemContasBanco();

        /// <summary>
        /// Recupera os descritores das contas bancárias.
        /// </summary>
        /// <param name="idContaR"></param>
        /// <param name="idNf"></param>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemContasBanco(int idContaR, int idNf);

        /// <summary>
        /// Recupera os dados da conta do banco.
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <returns></returns>
        Entidades.ContaBanco ObtemContaBanco(int idContaBanco);

        /// <summary>
        /// Salva os dados da conta bancária.
        /// </summary>
        /// <param name="contaBanco"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarContaBanco(Entidades.ContaBanco contaBanco);

        /// <summary>
        /// Apaga os dados da conta do banco.
        /// </summary>
        /// <param name="contaBanco"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarContaBanco(Entidades.ContaBanco contaBanco);

        #endregion
    }
}
