namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados do registro da pesquisa de destinatário.
    /// </summary>
    public class DestinatarioPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador do destinatário.
        /// </summary>
        public int IdDestinatario { get; set; }

        /// <summary>
        /// Nome.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Função.
        /// </summary>
        public string Funcao { get; set; }

        #endregion
    }
}
