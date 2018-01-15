using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio de unidades.
    /// </summary>
    public interface IUnidadesFluxo
    {
        #region Unidade Medida

        /// <summary>
        /// Pesquisa as unidades de medida.
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="descricao"></param>
        /// <returns></returns>
        IList<Entidades.UnidadeMedida> PesquisarUnidadesMedida(string codigo, string descricao);

        /// <summary>
        /// Recupera os descritores das unidades de medida do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemUnidadesMedida();

        /// <summary>
        /// Recupera os dados da unidade de medida.
        /// </summary>
        /// <param name="idUnidadeMedida"></param>
        /// <returns></returns>
        Entidades.UnidadeMedida ObtemUnidadeMedida(int idUnidadeMedida);

        /// <summary>
        /// Salva os dados da unidade de medida.
        /// </summary>
        /// <param name="unidadeMedida"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarUnidadeMedida(Entidades.UnidadeMedida unidadeMedida);

        /// <summary>
        /// Apaga os dados da unidade de medida.
        /// </summary>
        /// <param name="unidadeMedida"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarUnidadeMedida(Entidades.UnidadeMedida unidadeMedida);

        #endregion
    }
}
