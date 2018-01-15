using System.Collections.Generic;

namespace Glass.Financeiro.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócios de parcelas.
    /// </summary>
    public interface IParcelasFluxo
    {
        #region Parcelas

        /// <summary>
        /// Pesquisa as parcelas.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.Parcelas> PesquisarParcelas();

        /// <summary>
        /// Recupera os descritores das parcelas.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemParcelas();

        /// <summary>
        /// Recupera os dados da parcela.
        /// </summary>
        /// <param name="idParcela"></param>
        /// <returns></returns>
        Entidades.Parcelas ObtemParcela(int idParcela);

        /// <summary>
        /// Salva os dados da parcela.
        /// </summary>
        /// <param name="parcela"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarParcela(Entidades.Parcelas parcela);

        /// <summary>
        /// Apaga os dados da parcela.
        /// </summary>
        /// <param name="parcela"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarParcela(Entidades.Parcelas parcela);

        #endregion
    }
}
