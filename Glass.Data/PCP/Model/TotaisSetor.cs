namespace Glass.Data
{
    /// <summary>
    /// Armazena os totais do setor.
    /// </summary>
    public class TotaisSetor
    {
        #region Propriedades

        /// <summary>
        /// Identificador do setor.
        /// </summary>
        public int IdSetor { get; set; }

        /// <summary>
        /// Descrição do setor.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Total das peças que devem passar no setor.
        /// </summary>
        public int TotalPecas { get; set; }

        /// <summary>
        /// Total em m² das peas que devem passsar no setor.
        /// </summary>
        public double TotalPecasM2 { get; set; }

        /// <summary>
        /// Total de peças que devem passar neste setor 
        /// considerando apenas peças já lidas no setor anterior à este
        /// </summary>
        public int TotalPecasMomento { get; set; }

        /// <summary>
        /// Total em m² que devem passar neste setor 
        /// considerando apenas peças já lidas no setor anterior à este
        /// </summary>
        public double TotalPecasMomentoM2 { get; set; }

        #endregion
    }
}
