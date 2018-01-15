namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados do registro da pesquisa do relacionamento
    /// entre módulo e funcionário.
    /// </summary>
    public class FuncModuloPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador do funcionário.
        /// </summary>
        public int IdFunc { get; set; }

        /// <summary>
        /// Identificador do módulo.
        /// </summary>
        public int IdModulo { get; set; }

        /// <summary>
        /// Descrição do módulo.
        /// </summary>
        public string Modulo { get; set; }

        /// <summary>
        /// Grupo do módulo.
        /// </summary>
        public string GrupoModulo { get; set; }

        /// <summary>
        /// Identifica se é para permitir.
        /// </summary>
        public bool Permitir { get; set; }

        #endregion
    }
}
