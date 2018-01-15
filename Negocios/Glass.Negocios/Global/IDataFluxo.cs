using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de dados do sistema.
    /// </summary>
    public interface IDataFluxo : IProvedorFeriados
    {
        #region Feriado

        /// <summary>
        /// Pesquisa os feriados do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.Feriado> PesquisarFeriados();

        /// <summary>
        /// Recupera as relação dos feriados do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemFeriados();

        /// <summary>
        /// Recupera os dados feriado.
        /// </summary>
        /// <param name="idFeriado"></param>
        /// <returns></returns>
        Entidades.Feriado ObtemFeriado(int idFeriado);

        /// <summary>
        /// Salva os dados do feriado.
        /// </summary>
        /// <param name="feriado"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarFeriado(Entidades.Feriado feriado);

        /// <summary>
        /// Apaga os dados do feriado.
        /// </summary>
        /// <param name="idFeriado"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarFeriado(int idFeriado);

        /// <summary>
        /// Apaga os dados do feriado.
        /// </summary>
        /// <param name="feriado"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarFeriado(Entidades.Feriado feriado);

        #endregion
    }
}
