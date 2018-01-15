using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de origem de Troca/Desconto.
    /// </summary>
    public interface IOrigemTrocaDescontoFluxo
    {
        #region OrigemTrocaDesconto

        /// <summary>
        /// Pesquisa as origens de Troca/Desconto.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.OrigemTrocaDescontoPesquisa> PesquisarOrigensTrocaDesconto();

        /// <summary>
        /// Recupera os descritores das origens de Troca/Desconto.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemOrigensTrocaDesconto();

        /// <summary>
        /// Recupera a origem de Troca/Desconto.
        /// </summary>
        /// <param name="idOrigemTrocaDesconto"></param>
        /// <returns></returns>
        Entidades.OrigemTrocaDesconto ObtemOrigemTrocaDesconto(int idOrigemTrocaDesconto);

        /// <summary>
        /// Salva os dados da origem.
        /// </summary>
        /// <param name="origemTrocaDesconto"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarOrigemTrocaDesconto(Entidades.OrigemTrocaDesconto origemTrocaDesconto);

        /// <summary>
        /// Apaga os dados da origem.
        /// </summary>
        /// <param name="origemTrocaDesconto"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarOrigemTrocaDesconto(Entidades.OrigemTrocaDesconto origemTrocaDesconto);

        #endregion
    }
}
