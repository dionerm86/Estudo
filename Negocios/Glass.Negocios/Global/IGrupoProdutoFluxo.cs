using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócios do grupo de produtos.
    /// </summary>
    public interface IGrupoProdutoFluxo
    {
        #region TipoCalculoGrupoProd

        /// <summary>
        /// Retorna uma lista com os tipos de cálculo.
        /// </summary>
        /// <param name="exibirDecimal">Identifica se é para exibir os tipos de calculo com decimal.</param>
        /// <param name="notaFiscal">Identifica se é para exibir os tipos de calculo de nota fiscal.</param>
        /// <returns></returns>
        IEnumerable<Data.Model.TipoCalculoGrupoProd> ObtemTiposCalculo(bool exibirDecimal, bool notaFiscal);

        /// <summary>
        /// Recupera o tipo de calculo para o grupo de produto.
        /// </summary>
        /// <param name="idGrupoProd">Identificador do grupo de produtos.</param>
        /// <param name="idSubgrupoProd">Identificador do subgrupo de produtos.</param>
        /// <returns></returns>
        Data.Model.TipoCalculoGrupoProd ObtemTipoCalculo(int idGrupoProd, int? idSubgrupoProd);

        #endregion

        #region GrupoProd
        
        /// <summary>
        /// Pesquisa os grupos de produto do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.GrupoProd> PesquisarGruposProduto();

        /// <summary>
        /// Recupera os descritores dos grupos de produto.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemGruposProduto();

        /// <summary>
        /// Recupera os dados do grupo de produtos.
        /// </summary>
        /// <param name="idGrupoProd"></param>
        /// <returns></returns>
        Entidades.GrupoProd ObtemGrupoProduto(int idGrupoProd);

        /// <summary>
        /// Apaga os dados do grupo de produtos.
        /// </summary>
        /// <param name="grupoProd"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarGrupoProduto(Entidades.GrupoProd grupoProd);

        /// <summary>
        /// Apaga os dados do grupo de produtos.
        /// </summary>
        /// <param name="grupoProd"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarGrupoProduto(Entidades.GrupoProd grupoProd);

        #endregion

        #region SubgrupoProd

        /// <summary>
        /// Pesquisa os subgrupos de produtos.
        /// </summary>
        /// <param name="idGrupoProd">Identificador do grupo que será usado como filtro.</param>
        /// <returns></returns>
        IList<Entidades.SubgrupoProdPesquisa> PesquisarSubgruposProduto(int? idGrupoProd);

        /// <summary>
        /// Recupera os descritores do subgrupos.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemSubgruposProduto();

        /// <summary>
        /// Recupera os descritores dos subgrupos de produtos associados ao grupo informado.
        /// </summary>
        /// <param name="idGrupoProd"></param>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemSubgruposProduto(int idGrupoProd);

        /// <summary>
        /// Recupera os descritores dos subgrupos de produtos do grupo Vidro.
        /// </summary>
        IList<Colosoft.IEntityDescriptor> ObterSubgruposVidro();

        /// <summary>
        /// Recupera os descritores dos subgrupos de produtos do grupo vidro e mão de obra, conforme solicitação do chamado 45497.
        /// </summary>
        IList<Colosoft.IEntityDescriptor> ObterSubgruposClassificacaoRoteiroProducao();

        /// <summary>
        /// Recupera os dados do subgrupo.
        /// </summary>
        /// <param name="idSubgrupoProd"></param>
        /// <returns></returns>
        Entidades.SubgrupoProd ObtemSubgrupoProduto(int idSubgrupoProd);

        /// <summary>
        /// Salva os dados do subgrupo.
        /// </summary>
        /// <param name="subgrupoProduto"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarSubgrupoProduto(Entidades.SubgrupoProd subgrupoProduto);

        /// <summary>
        /// Apaga os dados do subgrupo.
        /// </summary>
        /// <param name="subgrupoProduto"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarSubgrupoProduto(Entidades.SubgrupoProd subgrupoProduto);

        // <summary>
        ///  Recupera os descritores dos subgrupos de produtos associados ao grupos informados.
        /// </summary>
        /// <param name="idsGrupoProds"></param>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemSubgruposProduto(string idsGrupoProds);

        #endregion
    }
}
