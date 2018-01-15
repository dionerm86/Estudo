using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio do fornecedor.
    /// </summary>
    public interface IFornecedorFluxo
    {
        #region Fornecedor

        /// <summary>
        /// Pesquisa os fornecedores.
        /// </summary>
        IList<Entidades.FornecedorPesquisa> PesquisarFornecedores
            (int? idFornec, string nomeFornec, Data.Model.SituacaoFornecedor? situacao,
             string cnpj, bool comCredito, Data.Model.TipoPessoa? tipoPessoa, int idPlanoConta, int idTipoPagto,
             string endereco, string vendedor);

        /// <summary>
        /// Recupera os descritores dos fornecedores do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemFornecedores();

        /// <summary>
        /// Recupera os dados do fornecedor.
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        Entidades.Fornecedor ObtemFornecedor(int idFornec);

        /// <summary>
        /// Recupera os detalhes do fornecedor.
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        Entidades.FornecedorPesquisa ObtemDetalhesFornecedor(int idFornec);

        /// <summary>
        /// Salva os dados do fornecedor.
        /// </summary>
        /// <param name="fornecedor"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarFornecedor(Entidades.Fornecedor fornecedor);

        /// <summary>
        /// Apaga os dados do fornecedor.
        /// </summary>
        /// <param name="fornecedor"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarFornecedor(Entidades.Fornecedor fornecedor);

        #endregion

        #region Produtos Fornecedor

        /// <summary>
        /// Pesquisa os produtos do fornecedores.
        /// </summary>
        /// <param name="idFornec"></param>
        /// <param name="idProdFornec">Identificador do ProdutoFornecedor.</param>
        /// <param name="idProd">Identificador do produto.</param>
        /// <param name="codigoProduto">Código do produto.</param>
        /// <param name="exibirSemDataVigencia">Identifica se é para exibir sem a data de vigência.</param>
        /// <param name="descricaoProduto">Código do produto que será pesquisado.</param>
        /// <returns></returns>
        IList<Entidades.ProdutoFornecedorPesquisa> PesquisarProdutosFornecedor
            (int? idFornec, int? idProd, bool? exibirSemDataVigencia, 
             string codigoProduto, string descricaoProduto);

        /// <summary>
        /// Recupera a relação dos descritores os produtod dos fornecedores.
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemProdutosFornecedor(int idFornec);

        /// <summary>
        /// Recupera os dados do produto do fornecedor.
        /// </summary>
        /// <param name="idProdFornec"></param>
        /// <returns></returns>
        Entidades.ProdutoFornecedor ObtemProdutoFornecedor(int idProdFornec);

        /// <summary>
        /// Salva os dados do produto do fornecedor.
        /// </summary>
        /// <param name="produtoFornecedor"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarProdutoFornecedor(Entidades.ProdutoFornecedor produtoFornecedor);

        /// <summary>
        /// Apaga os dados do produto do fornecedor.
        /// </summary>
        /// <param name="produtoFornecedor"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarProdutoFornecedor(Entidades.ProdutoFornecedor produtoFornecedor);

        #endregion
    }
}
