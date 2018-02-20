using System.Linq;

namespace Glass.Global.Negocios.Entidades
{
    /// <summary>
    /// Armazena os dados da ficha do produto.
    /// </summary>
    public class FichaProduto
    {
        #region Local Variables

        private string _beneficiamentosDescricao;
        private string _materiaPrimaQtde;

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do produto.
        /// </summary>
        public int IdProd { get; set; }

        /// <summary>
        /// Código interno do produto.
        /// </summary>
        public string CodInterno { get; set; }

        /// <summary>
        /// Descrição do produto.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Nome do fornecedor associado.
        /// </summary>
        public string NomeFornecedor { get; set; }

        /// <summary>
        /// Plano de conta contábil.
        /// </summary>
        public string PlanoContaContabil { get; set; }

        /// <summary>
        /// Altura do produto.
        /// </summary>
        public int Altura { get; set; }

        /// <summary>
        /// Largura do produto.
        /// </summary>
        public int Largura { get; set; }

        /// <summary>
        /// Descrição dos beneficiamentos associados ao produto.
        /// </summary>
        public string BeneficiamentosDescricao
        {
            get
            {
                if (_beneficiamentosDescricao == null)
                {
                    _beneficiamentosDescricao =
                        string.Join(", ", Microsoft.Practices.ServiceLocation.ServiceLocator
                            .Current.GetInstance<Global.Negocios.Entidades.IProdutoBeneficiamentosRepositorio>()
                            .ObtemDescricoes(IdProd)
                            .ToArray());
                }

                return _beneficiamentosDescricao;
            }
        }

        /// <summary>
        /// Descrição da matéria prima em relação a quantidade.
        /// </summary>
        public string MateriaPrimaQtde
        {
            get
            {
                if (_materiaPrimaQtde == null && QtdeBaixaEstoque > 0)
                {
                    _materiaPrimaQtde = 
                        string.Join(", ", Microsoft.Practices.ServiceLocation.ServiceLocator
                            .Current.GetInstance<Estoque.Negocios.Entidades.IProdutoBaixaEstoqueRepositorio>()
                            .ObtemDetalhesBaixasEstoque(IdProd)
                            .Select(f => 
                                string.Format("{0} x {1}", 
                                    f.DescricaoBaixa, 
                                    f.Qtde.ToString(Glass.Globalizacao.Cultura.CulturaSistema)))
                            .ToArray());
                }

                return _materiaPrimaQtde;
            }
        }

        /// <summary>
        /// Quantidade de baixas de estoque associadas.
        /// </summary>
        public int QtdeBaixaEstoque { get; set; }

        /// <summary>
        /// Descrição do grupo de produtos.
        /// </summary>
        public string Grupo { get; set; }

        /// <summary>
        /// Descrição do subgrupo de produtos.
        /// </summary>
        public string Subgrupo { get; set; }

        /// <summary>
        /// Situação do produto.
        /// </summary>
        public Glass.Situacao Situacao { get; set; }

        /// <summary>
        /// Tipo de mercadoria.
        /// </summary>
        public Data.Model.TipoMercadoria TipoMercadoria { get; set; }

        /// <summary>
        /// Área minima.
        /// </summary>
        public float AreaMinima { get; set; }

        /// <summary>
        /// Cor.
        /// </summary>
        public string Cor { get; set; }

        /// <summary>
        /// Espessura.
        /// </summary>
        public float Espessura { get; set; }

        /// <summary>
        /// Peso.
        /// </summary>
        public float Peso { get; set; }

        /// <summary>
        /// Forma.
        /// </summary>
        public string Forma { get; set; }

        /// <summary>
        /// Valor fiscal.
        /// </summary>
        public decimal ValorFiscal { get; set; }

        /// <summary>
        /// Valor Obra.
        /// </summary>
        public decimal ValorObra { get; set; }

        /// <summary>
        /// Valor atacado.
        /// </summary>
        public decimal ValorAtacado { get; set; }

        /// <summary>
        /// Valor de balcão.
        /// </summary>
        public decimal ValorBalcao { get; set; }

        /// <summary>
        /// Valor de reposição.
        /// </summary>
        public decimal ValorReposicao { get; set; }

        /// <summary>
        /// Valor de transferencia.
        /// </summary>
        public decimal ValorTransferencia { get; set; }

        /// <summary>
        /// Custo de fabricação base.
        /// </summary>
        public decimal Custofabbase { get; set; }

        /// <summary>
        /// Custom de compra.
        /// </summary>
        public decimal CustoCompra { get; set; }

        /// <summary>
        /// Valor da Aliquota ICMS.
        /// </summary>
        public string AliqICMS { get; set; }

        /// <summary>
        /// Valor da Aliquota IPI.
        /// </summary>
        public float AliqIPI { get; set; }

        /// <summary>
        /// Cst.
        /// </summary>
        public string Cst { get; set; }

        /// <summary>
        /// Csosn
        /// </summary>
        public string Csosn { get; set; }

        /// <summary>
        /// Cst IPI.
        /// </summary>
        public Data.Model.ProdutoCstIpi CstIpi { get; set; }

        /// <summary>
        /// Ncm.
        /// </summary>
        public string Ncm { get; set; }

        /// <summary>
        /// MVA.
        /// </summary>
        public string Mva { get; set; }

        /// <summary>
        /// GTIN Produto
        /// </summary>
        public string GTINProduto { get; set; }

        /// <summary>
        /// GTIN Unid. Tributação.
        /// </summary>
        public string GTINUnidTrib { get; set; }

        /// <summary>
        /// Identificador do genero do produto associado.
        /// </summary>
        public int IdGeneroProduto { get; set; }

        /// <summary>
        /// Genero do produto.
        /// </summary>
        public string GeneroProduto { get; set; }

        /// <summary>
        /// Unidade de medida.
        /// </summary>
        public string Unidade { get; set; }

        /// <summary>
        /// Unidade de tributação.
        /// </summary>
        public string UnidadeTrib { get; set; }

        /// <summary>
        /// Identifica se é para ativar a área minima.
        /// </summary>
        public bool AtivarAreaMinima { get; set; }

        /// <summary>
        /// Identificador do produto da baixa de estoque.
        /// </summary>
        public int? IdProdBaixaEstoqueFiscal { get; set; }

        /// <summary>
        /// Código interno da baixa de estoque fiscal.
        /// </summary>
        public string CodInternoBaixaEstoqueFiscal { get; set; }

        /// <summary>
        /// Descrição da baixa de estoque fiscal.
        /// </summary>
        public string DescricaoBaixaEstoqueFiscal { get; set; }

        #endregion
    }
}
