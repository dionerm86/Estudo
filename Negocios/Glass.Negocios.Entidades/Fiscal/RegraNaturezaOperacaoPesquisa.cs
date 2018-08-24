namespace Glass.Fiscal.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados da pesquisa das regras de natureza de operação.
    /// </summary>
    public class RegraNaturezaOperacaoPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador da regra.
        /// </summary>
        public int IdRegraNaturezaOperacao { get; set; }

        /// <summary>
        /// Identificador da loja.
        /// </summary>
        public int? IdLoja { get; set; }

        /// <summary>
        /// Nome fantasia da loja.
        /// </summary>
        public string NomeFantasiaLoja { get; set; }

        /// <summary>
        /// Razão social da loja.
        /// </summary>
        public string RazaoSocialLoja { get; set; }

        /// <summary>
        /// Nome da loja.
        /// </summary>
        public string NomeLoja
        {
            get { return string.IsNullOrEmpty(NomeFantasiaLoja) ? NomeFantasiaLoja : RazaoSocialLoja; }
        }

        /// <summary>
        /// Descrição do tipo de cliente.
        /// </summary>
        public string DescricaoTipoCliente { get; set; }

        /// <summary>
        /// Descrição do grupo de produtos.
        /// </summary>
        public string DescricaoGrupoProduto { get; set; }

        /// <summary>
        /// Descrição do subgrupo de produtos.
        /// </summary>
        public string DescricaoSubgrupoProduto { get; set; }

        /// <summary>
        /// Descrição da cor do vidro.
        /// </summary>
        public string DescricaoCorVidro { get; set; }

        /// <summary>
        /// Descrição da cor do aluminio.
        /// </summary>
        public string DescricaoCorAluminio { get; set; }

        /// <summary>
        /// Descrição da cor da ferragem.
        /// </summary>
        public string DescricaoCorFerragem { get; set; }

        /// <summary>
        /// Espessura.
        /// </summary>
        public float? Espessura { get; set; }

        public string UfDest { get; set; }

        /// <summary>
        /// Descrição da natureza de operaçao producao intraestadual.
        /// </summary>
        public string DescricaoNaturezaOperacaoProducaoIntra { get; set; }

        /// <summary>
        /// Descrição Natureza Operação Revenda - Intraestadual.
        /// </summary>
        public string DescricaoNaturezaOperacaoRevendaIntra { get; set; }

        /// <summary>
        /// Descrição da natureza de operação de produção interestadual.
        /// </summary>
        public string DescricaoNaturezaOperacaoProducaoInter { get; set; }

        /// <summary>
        /// Descrição da natureza de operação de revenda interestadual.
        /// </summary>
        public string DescricaoNaturezaOperacaoRevendaInter { get; set; }

        /// <summary>
        /// Descrição Natureza Operação Produção ST - Intraestadual.
        /// </summary>
        public string DescricaoNaturezaOperacaoProducaoStIntra { get; set; }

        /// <summary>
        /// Descrição Natureza Operação Produção ST - Intraestadual.
        /// </summary>
        public string DescricaoNaturezaOperacaoRevendaStIntra { get; set; }

        /// <summary>
        /// Descrição Natureza Operação Produção ST - Interestadual.
        /// </summary>
        public string DescricaoNaturezaOperacaoProducaoStInter { get; set; }

        /// <summary>
        /// Descrição Natureza Operação Produção ST - Intrerstadual.
        /// </summary>
        public string DescricaoNaturezaOperacaoRevendaStInter { get; set; }

        #endregion
    }
}
