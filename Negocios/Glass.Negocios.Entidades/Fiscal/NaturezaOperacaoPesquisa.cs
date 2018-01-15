namespace Glass.Fiscal.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados da pequisa de natureza de operação.
    /// </summary>
    public class NaturezaOperacaoPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador da natureza de operação.
        /// </summary>
        public int IdNaturezaOperacao { get; set; }

        /// <summary>
        /// Identificador do CFOP associado.
        /// </summary>
        public int IdCfop { get; set; }

        /// <summary>
        /// Código interno.
        /// </summary>
        public string CodInterno { get; set; }

        /// <summary>
        /// Identifica se calcula o ICMS.
        /// </summary>
        public bool CalcIcms { get; set; }

        /// <summary>
        /// Identifica se calcula o ICMSST.
        /// </summary>
        public bool CalcIcmsSt { get; set; }

        /// <summary>
        /// Identifica se calcula o IPI.
        /// </summary>
        public bool CalcIpi { get; set; }

        /// <summary>
        /// Identifica se calcula o PIS.
        /// </summary>
        public bool CalcPis { get; set; }

        /// <summary>
        /// Identifica se calcula o confis.
        /// </summary>
        public bool CalcCofins { get; set; }

        /// <summary>
        /// IPI Integra Base de calculo ICMS.
        /// </summary>
        public bool IpiIntegraBcIcms { get; set; }

        /// <summary>
        /// Altera o estoque fiscal.
        /// </summary>
        public bool AlterarEstoqueFiscal { get; set; }

        /// <summary>
        /// CST ICMS.
        /// </summary>
        public string CstIcms { get; set; }

        /// <summary>
        /// Percentual redução base calculo ICMS.
        /// </summary>
        public float PercReducaoBcIcms { get; set; }

        /// <summary>
        /// CST IPI.
        /// </summary>
        public int? CstIpi { get; set; }

        /// <summary>
        /// CST PIS Cofins.
        /// </summary>
        public int? CstPisCofins { get; set; }

        /// <summary>
        /// CSON.
        /// </summary>
        public string Csosn { get; set; }

        /// <summary>
        /// Código interno do CFOP.
        /// </summary>
        public string CodCfop { get; set; }

        /// <summary>
        /// Descrição do CFOP.
        /// </summary>
        public string DescricaoCfop { get; set; }

        #endregion
    }
}
