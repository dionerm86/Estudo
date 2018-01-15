using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio dos departamentos.
    /// </summary>
    public interface IDepartamentoFluxo
    {
        #region Métodos

        /// <summary>
        /// Pesquisa os departamentos do sistema.
        /// </summary>
        /// <param name="idDepartamento">Identificador do departamento.</param>
        /// <param name="nome">Nome que será usado na pesquisa.</param>
        /// <param name="pageSize"></param>
        /// <param name="sortExpression"></param>
        /// <returns></returns>
        IList<Entidades.Departamento> PesquisaDepartamentos(int idDepartamento, string nome, int pageSize, string sortExpression);

        /// <summary>
        /// Recupera os descritores dos departamentos do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemDepartamentos();

        /// <summary>
        /// Recupera os dados do departamento pelo identificador informado.
        /// </summary>
        /// <param name="idDepartamento"></param>
        /// <returns></returns>
        Entidades.Departamento ObtemDepartamento(int idDepartamento);

        /// <summary>
        /// Salva os dados do departamento.
        /// </summary>
        /// <param name="departamento"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult Salvar(Entidades.Departamento departamento);

        /// <summary>
        /// Apaga os dados do departamento.
        /// </summary>
        /// <param name="idDepartamento"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult Apagar(int idDepartamento);

        #endregion
    }
}
