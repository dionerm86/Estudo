using System.Collections.Generic;

namespace Glass.Projeto.Negocios
{
    public interface IFlagArqMesaFluxo
    {
        /// <summary>
        /// Busca os flags do sistema
        /// </summary>
        /// <returns></returns>
        IList<Entidades.FlagArqMesa> PesquisarFlag();

        /// <summary>
        /// Recupera os dados do flag
        /// </summary>
        /// <param name="idFlag"></param>
        /// <returns></returns>
        Entidades.FlagArqMesa ObtemFlag(int IdFlagArqMesa);

        /// <summary>
        /// Recupera um lista de flags
        /// </summary>
        /// <param name="idsFlags"></param>
        /// <returns></returns>
        IList<Entidades.FlagArqMesa> ObtemFlags(int[] idsFlags);

        /// <summary>
        /// Recupera os descritores dos flags do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemFlagsArqMesa();

        /// <summary>
        /// Recupera os descritores dos flags do arquivo calcEngine.
        /// </summary>
        /// <param name="idArquivoCalcEngine"></param>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemFlagsArqMesaArqCalcengine(int? idArquivoMesaCorte);

        /// <summary>
        /// Salva os dados de um flag
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarFlagArqMesa(Entidades.FlagArqMesa flag);

        /// <summary>
        /// Apaga os dados do flag.
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarFlagArqMesa(Entidades.FlagArqMesa flag);
    }
}
