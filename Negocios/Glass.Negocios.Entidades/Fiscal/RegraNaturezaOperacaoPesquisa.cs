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

        /// <summary>
        /// Unidade Federativa de destino
        /// </summary>
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

        /// <summary>
        /// Identificador do tipo cliente.
        /// </summary>
        public int? IdTipoCliente { get; set; }

        /// <summary>
        /// Identificador do grupo de produto.
        /// </summary>
        public int? IdGrupoProd { get; set; }

        /// <summary>
        /// Identificador do subgrupo de produto.
        /// </summary>
        public int? IdSubgrupoProd { get; set; }

        /// <summary>
        /// Identificador da cor do vidro.
        /// </summary>
        public int? IdCorVidro { get; set; }

        /// <summary>
        /// Identificador da cor do vidro.
        /// </summary>
        public int? IdCorFerragem { get; set; }

        /// <summary>
        /// Identificador da cor do alumínio.
        /// </summary>
        public int? IdCorAluminio { get; set; }

        /// <summary>
        /// Identificador da natureza de operação de produção intraestadual.
        /// </summary>
        public int IdNaturezaOperacaoProdIntra { get; set; }

        /// <summary>
        /// Identificador da natureza de operação de revenda intraestadual.
        /// </summary>
        public int IdNaturezaOperacaoRevIntra { get; set; }

        /// <summary>
        /// Identificador da natureza de operação de produção interestadual.
        /// </summary>
        public int IdNaturezaOperacaoProdInter { get; set; }

        /// <summary>
        /// Identificador da natureza de operação de revenda interestadual.
        /// </summary>
        public int IdNaturezaOperacaoRevInter { get; set; }

        /// <summary>
        /// Identificador da natureza de operação de produção intraestadual com ST.
        /// </summary>
        public int? IdNaturezaOperacaoProdStIntra { get; set; }

        /// <summary>
        /// Identificador da natureza de operação de revenda intraestadual com ST.
        /// </summary>
        public int? IdNaturezaOperacaoRevStIntra { get; set; }

        /// <summary>
        /// Identificador da natureza de operação de produção interestadual com ST.
        /// </summary>
        public int? IdNaturezaOperacaoProdStInter { get; set; }

        /// <summary>
        /// Identificador da natureza de operação de revenda interestadual com ST.
        /// </summary>
        public int? IdNaturezaOperacaoRevStInter { get; set; }

        #endregion
    }
}
