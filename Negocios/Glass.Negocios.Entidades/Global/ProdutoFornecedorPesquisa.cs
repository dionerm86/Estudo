using System;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados do resultado da pesquisa de produtos do fornecedor.
    /// </summary>
    public class ProdutoFornecedorPesquisa
    {
        #region Propriedades

        /// <summary>
        /// Identificador do relacionamento.
        /// </summary>
        public int IdProdFornec { get; set; }

        /// <summary>
        /// Identificador do produto associado.
        /// </summary>
        public int IdProd { get; set; }

        /// <summary>
        /// Identificador do fornecedor associado.
        /// </summary>
        public int IdFornec { get; set; }

        /// <summary>
        /// Código do produto no fornecedor.
        /// </summary>
        public string CodFornec { get; set; }

        /// <summary>
        /// Prazo de entrega em dias.
        /// </summary>
        public int PrazoEntregaDias { get; set; }

        /// <summary>
        /// Data de vigencia.
        /// </summary>
        public DateTime? DataVigencia { get; set; }

        /// <summary>
        /// Custo.
        /// </summary>
        public decimal CustoCompra { get; set; }

        /// <summary>
        /// Codigo interno do produto
        /// </summary>
        public string CodInternoProd { get; set; }

        /// <summary>
        /// Descrição do produto.
        /// </summary>
        public string DescricaoProduto { get; set; }

        /// <summary>
        /// Nome fantasia do fornecedor.
        /// </summary>
        public string NomeFantasiaFornecedor { get; set; }

        /// <summary>
        /// Razão social do fornecedor.
        /// </summary>
        public string RazaoSocialFornecedor { get; set; }

        /// <summary>
        /// Nome do fornecedor.
        /// </summary>
        public string NomeFornecedor
        {
            get
            {
                return !string.IsNullOrEmpty(NomeFantasiaFornecedor) ? NomeFantasiaFornecedor :
                    !String.IsNullOrEmpty(RazaoSocialFornecedor) ? RazaoSocialFornecedor : "";
            }
        }

        #endregion
    }
}
