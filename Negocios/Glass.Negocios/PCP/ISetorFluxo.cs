using System.Collections.Generic;

namespace Glass.PCP.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio dos setores de produção.
    /// </summary>
    public interface ISetorFluxo
    {
        #region Setor

        /// <summary>
        /// Pequisa os setores do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.Setor> PesquisarSetores();

        /// <summary>
        /// Recupera os descritores dos setores do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.SetorDescritor> ObtemSetores();

        /// <summary>
        /// Recupera os dados do setor.
        /// </summary>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        Entidades.Setor ObtemSetor(int idSetor);

        /// <summary>
        /// Salva os dados do setor.
        /// </summary>
        /// <param name="setor"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarSetor(Entidades.Setor setor);

        /// <summary>
        /// Apaga os dados do setor.
        /// </summary>
        /// <param name="setor"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarSetor(Entidades.Setor setor);

        /// <summary>
        /// Método usado para alterar a posição do setor.
        /// </summary>
        /// <param name="idSetor"></param>
        /// <param name="acima"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult AlterarPosicao(int idSetor, bool acima);

        #endregion
    }
}
