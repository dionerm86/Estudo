using System.Collections.Generic;

namespace Glass.PCP.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negocios do cavelete
    /// </summary>
    public interface ICavaleteFluxo
    {
        /// <summary>
        /// Pequisa os cavaletes do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.Cavalete> PesquisarCavaletes();

        /// <summary>
        /// Recupera os dados do cavalete.
        /// </summary>
        /// <param name="idCavalete"></param>
        /// <returns></returns>
        Entidades.Cavalete ObterCavalete(int idCavalete);

        /// <summary>
        /// Salva os dados do cavalete.
        /// </summary>
        /// <param name="cavalete"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarCavalete(Entidades.Cavalete cavalete);

        /// <summary>
        /// Apaga os dados do cavalete.
        /// </summary>
        /// <param name="cavalete"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarCavalete(Entidades.Cavalete cavalete);
    }
}
