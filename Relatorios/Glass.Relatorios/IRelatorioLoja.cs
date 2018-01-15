namespace Glass.Relatorios
{
    /// <summary>
    /// Assinatura do relatórios que possuem associação com a loja.
    /// </summary>
    public interface IRelatorioLoja : Colosoft.Reports.IReportDocument
    {
        #region Propriedades

        /// <summary>
        /// Identificador da loja associada.
        /// </summary>
        int IdLoja { get; }

        #endregion
    }
}
