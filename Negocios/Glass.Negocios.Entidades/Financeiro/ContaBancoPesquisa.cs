namespace Glass.Financeiro.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados da pesquisa da conta bancária.
    /// </summary>
    public class ContaBancoPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador da conta do banco.
        /// </summary>
        public int IdContaBanco { get; set; }

        /// <summary>
        /// Identificador da loja.
        /// </summary>
        public int IdLoja { get; set; }

        /// <summary>
        /// Nome da loja associada.
        /// </summary>
        public string Loja { get; set; }

        /// <summary>
        /// Nome da conta.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Código do banco associado.
        /// </summary>
        public int? CodBanco { get; set; }

        /// <summary>
        /// Agencia.
        /// </summary>
        public string Agencia { get; set; }

        /// <summary>
        /// Número da conta.
        /// </summary>
        public string Conta { get; set; }

        /// <summary>
        /// Nome do titular da conta.
        /// </summary>
        public string Titular { get; set; }

        /// <summary>
        /// Código do convencia.
        /// </summary>
        public string CodConvenio { get; set; }

        /// <summary>
        /// Situacação da conta.
        /// </summary>
        public Glass.Situacao Situacao { get; set; }

        /// <summary>
        /// Quantidade de movimentações associadas com a conta.
        /// </summary>
        public int QtdeMovimentacoes { get; set; }

        #endregion
    }
}
