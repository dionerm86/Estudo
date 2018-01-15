using System.Collections.Generic;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Representa a classe que armazena o registro da pesquisa
    /// dos planos de contas.
    /// </summary>
    public class PlanoContasPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador da conta.
        /// </summary>
        public int IdConta { get; set; }

        /// <summary>
        /// Identificador da conta no grupo.
        /// </summary>
        public int IdContaGrupo { get; set; }

        /// <summary>
        /// Identificador do grupo associado com a conta.
        /// </summary>
        public int IdGrupo { get; set; }

        /// <summary>
        /// Identificador da categoria associado com o grupo.
        /// </summary>
        public int IdCategoriaConta { get; set; }

        /// <summary>
        /// Descrição do plano de contas.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Ecibir DRE?
        /// </summary>
        public bool ExibirDre { get; set; }

        /// <summary>
        /// Situação do plano de contas.
        /// </summary>
        public Glass.Situacao Situacao { get; set; }

        /// <summary>
        /// Descrição do grupo associado.
        /// </summary>
        public string Grupo { get; set; }

        /// <summary>
        /// Descrição da categoria associada.
        /// </summary>
        public string Categoria { get; set; }

        /// <summary>
        /// Identifica se é um grupo de contas do sistema.
        /// </summary>
        public bool PlanoContasSistema
        {
            get { return !(new List<string>(Glass.Data.Helper.UtilsPlanoConta.GetGruposSistema.Split(',')).Contains(IdGrupo.ToString())); }
        }

        #endregion
    }
}
