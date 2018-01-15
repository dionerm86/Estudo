namespace Glass
{
    /// <summary>
    /// Atribute usado para exporta os relatórios do sistema.
    /// </summary>
    public class ExportaRelatorioAttribute : System.ComponentModel.Composition.ExportAttribute
    {
        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="nome"></param>
        public ExportaRelatorioAttribute(string nome)
            : base(string.Format("{0}.{1}", typeof(Colosoft.Reports.IReportDocument).FullName, nome), typeof(Colosoft.Reports.IReportDocument))
        {
        }

        #endregion
    }
}
