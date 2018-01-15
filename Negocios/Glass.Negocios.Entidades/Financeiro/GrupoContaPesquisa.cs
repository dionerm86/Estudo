using System.Collections.Generic;

namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Repreesnta os dados do resultado da pesquisa do grupo de conta.
    /// </summary>
    public class GrupoContaPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador do grupo.
        /// </summary>
        public int IdGrupo { get; set; }

        /// <summary>
        /// Identificador da categoria de conta.
        /// </summary>
        public int IdCategoriaConta { get; set; }

        /// <summary>
        /// Nome da categoria associada.
        /// </summary>
        public string Categoria { get; set; }

        /// <summary>
        /// Descrição do grupo.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Situação do grupo.
        /// </summary>
        public Glass.Situacao Situacao { get; set; }

        /// <summary>
        /// Ponto de equilibrio.
        /// </summary>
        public bool PontoEquilibrio { get; set; }

        /// <summary>
        /// Número de sequencia.
        /// </summary>
        public int NumeroSequencia { get; set; }

        /// <summary>
        /// Identifica se é um grupo de conta do sistema.
        /// </summary>
        public bool GrupoContaSistema
        {
            get
            {
                return !(new List<string>(Glass.Data.Helper.UtilsPlanoConta.GetGruposSistema.Split(',')).Contains(IdGrupo.ToString()));
            }
        }

        #endregion
    }
}
