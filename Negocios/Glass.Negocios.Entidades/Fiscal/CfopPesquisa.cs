namespace Glass.Fiscal.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados da pesquisa dos Cfop.
    /// </summary>
    public class CfopPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador do Cfop.
        /// </summary>
        public int IdCfop { get; set; }

        /// <summary>
        /// Código interno.
        /// </summary>
        public string CodInterno { get; set; }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Identificador do tipo associado.
        /// </summary>
        public int? IdTipoCfop { get; set; }

        /// <summary>
        /// Descrição do tipo associado.
        /// </summary>
        public string Tipo { get; set; }

        /// <summary>
        /// Tipo de mercadoria associada.
        /// </summary>
        public Data.Model.TipoMercadoria? TipoMercadoria { get; set; }

        /// <summary>
        /// Alterar estoque de terceiros.
        /// </summary>
        public bool AlterarEstoqueTerceiros { get; set; }

        /// <summary>
        /// Alterar estoque do cliente.
        /// </summary>
        public bool AlterarEstoqueCliente { get; set; }

        /// <summary>
        /// Observação.
        /// </summary>
        public string Obs { get; set; }

        #endregion
    }
}
