namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados da pesquisa da configuração do beneficiamento.
    /// </summary>
    public class BenefConfigPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador do configuração do beneficiamento.
        /// </summary>
        public int IdBenefConfig { get; set; }

        /// <summary>
        /// Nome do beneficiamento.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Descrição do beneficiamento.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Tipo de controle do beneficiamento.
        /// </summary>
        public Glass.Data.Model.TipoControleBenef TipoControle { get; set; }

        /// <summary>
        /// Tipo de calculo do beneficiamento.
        /// </summary>
        public Glass.Data.Model.TipoCalculoBenef TipoCalculo { get; set; }

        /// <summary>
        /// Idenificador da etiqueta de aplicação associada.
        /// </summary>
        public int? IdAplicacao { get; set; }

        /// <summary>
        /// Nome da etiqueta de aplicação associada.
        /// </summary>
        public string CodAplicacao { get; set; }

        /// <summary>
        /// Identificador da etiqueta de processo associada.
        /// </summary>
        public int? IdProcesso { get; set; }

        /// <summary>
        /// Código interno da etiqueta de processo associada.
        /// </summary>
        public string CodProcesso { get; set; }

        /// <summary>
        /// Cobrança opcional.
        /// </summary>
        public bool CobrancaOpcional { get; set; }

         /// <summary>
        /// Identifica se não é para exibir descrição do beneficiamento na etiqueta.
        /// </summary>
        public bool NaoExibirEtiqueta { get; set; }

        /// <summary>
        /// Situação.
        /// </summary>
        public Glass.Situacao Situacao { get; set; }

        /// <summary>
        /// Tipo do beneficiamento.
        /// </summary>
        public Glass.Data.Model.TipoBenef TipoBenef { get; set; }
        
        #endregion
    }
}
