namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados da pesquisa da Origem Troca/Desconto.
    /// </summary>
    public class OrigemTrocaDescontoPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador da origem.
        /// </summary>
        public int IdOrigemTrocaDesconto { get; set; }

        /// <summary>
        /// Descriçõa.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Situação.
        /// </summary>
        public Glass.Situacao Situacao { get; set; }

        /// <summary>
        /// Quantidade de contas a receber associadas.
        /// </summary>
        public int QtdeContasReceber { get; set; }

        #endregion
    }
}
