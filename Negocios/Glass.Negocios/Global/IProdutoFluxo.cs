using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de produtos.
    /// </summary>
    public interface IProdutoFluxo
    {
        #region Genero Produto

        /// <summary>
        /// Recuper aos generos de produto.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.GeneroProduto> ObtemGenerosProduto();

        /// <summary>
        /// Recupera os dados do genero do produto.
        /// </summary>
        /// <param name="idGeneroProduto"></param>
        /// <returns></returns>
        Entidades.GeneroProduto ObtemGeneroProduto(int idGeneroProduto);

        #endregion

        #region Produto

        /// <summary>
        /// Cria a instancia de um novo produto.
        /// </summary>
        /// <returns></returns>
        Entidades.Produto CriarProduto();

        /// <summary>
        /// Pesquisa os produtos.
        /// </summary>
        /// <param name="codInterno">Código interno do produto.</param>
        /// <param name="descricao">Descrição.</param>
        /// <param name="situacao">Situação.</param>
        /// <param name="idLoja">Identificador da loja.</param>
        /// <param name="idFornec"></param>
        /// <param name="nomeFornecedor">Nome do fornecedor.</param>
        /// <param name="idGrupo">Identificador do grupo</param>
        /// <param name="idSubgrupo">Identificador do subgrupo.</param>
        /// <param name="tipoNegociacao">Tipo de negociação.</param>
        /// <param name="apenasProdutosEstoqueBaixa">Identifica se é para recuperar apenas produtos com baixa no estoque.</param>
        /// <param name="agruparEstoqueLoja">Idenifica se é para agrupar pelo estoque da loja.</param>
        /// <param name="alturaInicio">Altura de inicio.</param>
        /// <param name="alturaFim">Altura final.</param>
        /// <param name="larguraInicio">Largura de início.</param>
        /// <param name="larguraFim">Largura final.</param>
        /// <param name="ordenacao">Ordenação inicial da lista.</param>
        /// <returns></returns>
        IList<Entidades.ProdutoPesquisa> PesquisarProdutos(
            string codInterno, string descricao, Glass.Situacao? situacao, int? idLoja, int? idFornec, 
            string nomeFornecedor, string idGrupo, string idSubgrupo, TipoNegociacaoProduto? tipoNegociacao,
            bool apenasProdutosEstoqueBaixa, bool agruparEstoqueLoja, decimal? alturaInicio, decimal? alturaFim, 
            decimal? larguraInicio, decimal? larguraFim, string ordenacao);

        /// <summary>
        /// Pesquisa os produtos.
        /// </summary>
        /// <param name="codInterno">Código interno do produto.</param>
        /// <param name="descricao">Descrição.</param>
        /// <param name="situacao">Situação.</param>
        /// <param name="idFornec"></param>
        /// <param name="nomeFornecedor">Nome do fornecedor.</param>
        /// <param name="idGrupo">Identificador do grupo</param>
        /// <param name="idSubgrupo">Identificador do subgrupo.</param>
        /// <param name="tipoNegociacao">Tipo de negociação.</param>
        /// <param name="apenasProdutosEstoqueBaixa">Identifica se é para recuperar apenas produtos com baixa no estoque.</param>
        /// <param name="ordenacao">Ordenação inicial da lista.</param>
        /// <returns></returns>
        IList<Entidades.ProdutoListagemItem> PesquisarListagemProdutos(
            string codInterno, string descricao, Glass.Situacao? situacao, int? idFornec,
            string nomeFornecedor, string idsGrupos, string idsSubgrupos, TipoNegociacaoProduto? tipoNegociacao,
            bool apenasProdutosEstoqueBaixa, string ordenacao);
       
        /// <summary>
        /// Recupera os dados da ficha do produto.
        /// </summary>
        /// <param name="idProd">Identificador do produto.</param>
        /// <returns></returns>
        Entidades.FichaProduto ObtemFichaProduto(int idProd);

        /// <summary>
        /// Recupera as fichas dos produtos que se enquadram no filtro aplicado.
        /// </summary>
        /// <param name="idFornec">Identificador do fornecedor.</param>
        /// <param name="nomeFornec">Nome do fornecedor.</param>
        /// <param name="idGrupoProd">Identificador do grupo de produtos.</param>
        /// <param name="idSubgrupoProd">Identificador do subgrupo de produtos.</param>
        /// <param name="codInterno">Código interno do produto.</param>
        /// <param name="descricao">Descrção do produto</param>
        /// <param name="tipoNegociacao">Tipo de negociacao do produto.</param>
        /// <param name="situacao">Situação.</param>
        /// <param name="apenasProdutosEstoqueBaixa"></param>
        /// <param name="alturaInicio"></param>
        /// <param name="alturaFim"></param>
        /// <param name="larguraInicio"></param>
        /// <param name="larguraFim"></param>
        /// <param name="ordenacao"></param>
        /// <returns></returns>
        IList<Entidades.FichaProduto> ObtemFichasProdutos(
            int? idFornec, string nomeFornec, int? idGrupoProd, int? idSubgrupoProd, string codInterno, string descricao,
            TipoNegociacaoProduto tipoNegociacao, Glass.Situacao? situacao, bool apenasProdutosEstoqueBaixa, 
            decimal alturaInicio, decimal alturaFim, decimal larguraInicio, decimal larguraFim, string ordenacao);

        /// <summary>
        /// Recupera descritor de todos os produtos.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemProdutos();

        /// <summary>
        /// Recupera os produtos pelos identificadores informados.
        /// </summary>
        /// <param name="idProds">Identificadores dos produtos que devem ser recuperados.</param>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemProdutos(IEnumerable<int> idProds);

        /// <summary>
        /// Recupera os dados do produto.
        /// </summary>
        /// <param name="idProd"></param>
        /// <returns></returns>
        Entidades.Produto ObtemProduto(int idProd);

        /// <summary>
        /// Recupera os dados do produto.
        /// </summary>
        Entidades.Produto ObterProduto(string codInternoProd);

        /// <summary>
        /// Salva os dados do produto.
        /// </summary>
        /// <param name="produto"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarProduto(Entidades.Produto produto);

        /// <summary>
        /// Apaga os dados do produto.
        /// </summary>
        /// <param name="produto">Instancia do produto que será apagada.</param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarProduto(Entidades.Produto produto);

        #endregion
    }
}
