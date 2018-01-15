using System.Collections.Generic;
using GDA;

namespace Glass.PCP.Negocios
{
    public interface IVolumeFluxo
    {
        /// <summary>
        /// Realiza a expedicao de um volume
        /// </summary>
        /// <param name="codEtiquetaVolume"></param>
        /// <param name="idLiberarPedido"></param>
        /// <param name="balcao"></param>
        /// <returns></returns>
        string MarcaExpedicaoVolume(GDASession sessao, string codEtiquetaVolume, int idLiberarPedido, bool balcao);

        /// <summary>
        /// Estorna a expedição balcão de um volume
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idVolume"></param>
        void EstornaExpedicaoVolume(GDASession sessao, uint idVolume);
    }

    /// <summary>
    /// Assinatura do fluxo de negocios da expedição
    /// </summary>
    public interface IExpedicaoFluxo
    {
        #region Exp. Balcão

        /// <summary>
        /// Recupera os dados para a expedição balcão
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <param name="idPedido"></param>
        /// <param name="visualizar">1 - Expedidos, 2 Não Expedidos</param>
        /// <returns></returns>
        Entidades.ExpBalcao BuscaParaExpBalcao(int idLiberarPedido, int idPedido, string visualizar);

        /// <summary>
        /// Realiza a leitura da expedição
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="idLiberarPedido"></param>
        /// <param name="numEtiqueta"></param>
        /// <param name="idPedidoExp"></param>
        void EfetuaLeitura(int idFunc, int idLiberarPedido, string numEtiqueta, int? idPedidoExp);

        /// <summary>
        /// Estorna itens de uma expedição de liberação
        /// </summary>
        /// <param name="itens"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult EstornaLiberacao(Dictionary<int, int> itens);

        #endregion

        #region Exp. Carregamento

        #endregion
    }
}
