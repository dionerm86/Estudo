using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio das etiquetas.
    /// </summary>
    public interface IEtiquetaFluxo
    {
        #region EtiquetaProcesso

        /// <summary>
        /// Pesquisa os processos de etiqueta.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.EtiquetaProcessoPesquisa> PesquisarEtiquetaProcessos();

        /// <summary>
        /// Recupera os descritores dos EtiquetaProcessos.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemEtiquetaProcessos();

        /// <summary>
        /// Recupera os dados do processo.
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <returns></returns>
        Entidades.EtiquetaProcesso ObtemEtiquetaProcesso(int idProcesso);

        /// <summary>
        /// Salva os dados do EtiquetaProcesso.
        /// </summary>
        /// <param name="etiquetaProcesso"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarEtiquetaProcesso(Entidades.EtiquetaProcesso etiquetaProcesso);

        /// <summary>
        /// Apagar os dados do EtiquetaProcesso.
        /// </summary>
        /// <param name="etiquetaProcesso"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarEtiquetaProcesso(Entidades.EtiquetaProcesso etiquetaProcesso);

        #endregion

        #region EtiquetaAplicacao

        /// <summary>
        /// Pesquisa as aplicações de etiqueta.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.EtiquetaAplicacao> PesquisarEtiquetaAplicacoes();

        /// <summary>
        /// Recupera os descritores das EtiquetaAplicacao.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemEtiquetaAplicacoes();

        /// <summary>
        /// Recupera os dados da EtiquetaAplicação.
        /// </summary>
        /// <param name="idAplicacao"></param>
        /// <returns></returns>
        Entidades.EtiquetaAplicacao ObtemEtiquetaAplicacao(int idAplicacao);
        
        /// <summary>
        /// Salva os dados da EtiquetaAplicacao.
        /// </summary>
        /// <param name="etiquetaAplicacao"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarEtiquetaAplicacao(Entidades.EtiquetaAplicacao etiquetaAplicacao);

        /// <summary>
        /// Apaga os dados da EtiquetaAplicacao.
        /// </summary>
        /// <param name="etiquetaAplicacao"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarEtiquetaAplicacao(Entidades.EtiquetaAplicacao etiquetaAplicacao);

        #endregion
    }
}
