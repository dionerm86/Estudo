using System.Collections.Generic;

namespace Glass.Financeiro.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio do plano de contas.
    /// </summary>
    public interface IPlanoContasFluxo
    {
        #region Categoria de Contas

        /// <summary>
        /// Pesquisa as categorias de conta do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.CategoriaConta> PesquisarCategoriasConta();

        /// <summary>
        /// Recupera os descritores das categorias de conta cadastradas no sistema.
        /// </summary>
        /// <param name="ativas"></param>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemCategoriasConta(bool ativas = false);

        /// <summary>
        /// Recupera os descritores das categorias de conta sem as categorias do tipo Subtotal.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemCategoriasContaSemSubtotal();

        /// <summary>
        /// Recupera os dados da categoria de conta.
        /// </summary>
        /// <param name="idCategoriaConta">Identificador da categoria.</param>
        /// <returns></returns>
        Entidades.CategoriaConta ObtemCategoriaConta(int idCategoriaConta);

        /// <summary>
        /// Salva os dados da categoria de conta.
        /// </summary>
        /// <param name="categoriaConta"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarCategoriaConta(Entidades.CategoriaConta categoriaConta);

        /// <summary>
        /// Apaga a categoria de conta.
        /// </summary>
        /// <param name="categoriaConta"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarCategoriaConta(Entidades.CategoriaConta categoriaConta);

        /// <summary>
        /// Altera a posição da categoria de conta.
        /// </summary>
        /// <param name="idCategoriaConta">Identificador da categoria que será movimentada.</param>
        /// <param name="moverParaCima">Identifica se é para mover para cima.</param>
        /// <returns></returns>
        Colosoft.Business.SaveResult AlterarPosicaoCategoriaConta(int idCategoriaConta, bool moverParaCima);

        #endregion

        #region Grupo de Contas

        /// <summary>
        /// Pesquisa os grupos de conta do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.GrupoContaPesquisa> PesquisarGruposConta();

        /// <summary>
        /// Pequisa os grupos de conta ignorando os ponto de equilibrio.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.GrupoContaPesquisa> PesquisarGruposContaIgnoraPE();

        /// <summary>
        /// Recupera os descritores dos grupos de conta do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemGruposConta();

        /// <summary>
        /// Recupera os descritores dos grupos de conta que podem ser selecionados para cadastro.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemGruposContaCadastro();

        /// <summary>
        /// Recupera os grupos de contas associados com a categoria informada.
        /// </summary>
        /// <param name="idCategoriaConta">Identificador da categoria que será usada como filtro.</param>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemGruposContaPorCategoria(int idCategoriaConta);

        /// <summary>
        /// Recupera os dados do grupo de conta.
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        Entidades.GrupoConta ObtemGrupoConta(int idGrupo);

        /// <summary>
        /// Salva os dados do grupo de conta.
        /// </summary>
        /// <param name="grupoConta"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarGrupoConta(Entidades.GrupoConta grupoConta);

        /// <summary>
        /// Apaga os dados do grupo de conta.
        /// </summary>
        /// <param name="grupoConta"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarGrupoConta(Entidades.GrupoConta grupoConta);

        /// <summary>
        /// Altera a posição do grupo de conta.
        /// </summary>
        /// <param name="idGrupo">Identificador do grupo de conta.</param>
        /// <param name="moverParaCima">Identifica se é para mover o grupo para cima.</param>
        /// <returns></returns>
        Colosoft.Business.SaveResult AlterarPosicaoGrupoConta(int idGrupo, bool moverParaCima);

        #endregion

        #region Plano de Contas

        /// <summary>
        /// Pesquisa os planos de contas.
        /// </summary>
        /// <param name="idGrupo">Identificador do grupo para filtro.</param>
        /// <param name="situacao">Situação dos planos.</param>
        /// <returns></returns>
        IList<Entidades.PlanoContasPesquisa> PesquisarPlanosContas(int idGrupo, Glass.Situacao? situacao);

        /// <summary>
        /// Recupera os descritores dos planos de contas.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.PlanoContasDescritor> ObtemPlanosContas();

        /// <summary>
        /// Recupera os dados do plano de contas.
        /// </summary>
        /// <param name="idConta"></param>
        /// <returns></returns>
        Entidades.PlanoContas ObtemPlanoContas(int idConta);


        /// <summary>
        /// Recupera os planos de contas vinculados a um plano de conta contabil
        /// </summary>
        /// <param name="idPlanoContabil"></param>
        /// <returns></returns>
        IList<Entidades.PlanoContas> ObterPlanosContasVincularPlanoContas(int idContaContabil);

        /// <summary>
        /// Recupera o identificador do plano de contas.
        /// </summary>
        /// <param name="idContaGrupo">Identificador do plano dentro da grupo.</param>
        /// <param name="idGrupo">Identificador do grupo.</param>
        /// <returns></returns>
        int ObtemIdPlanoContas(int idContaGrupo, int idGrupo);

        /// <summary>
        /// Retorna o PlanoContas pela descrição se o mesmo for encontrado.
        /// </summary>
        /// <param name="descricao"></param>
        /// <returns></returns>
        Entidades.PlanoContas ObterPlanoContaPelaDescricao(string descricao);

        /// <summary>
        /// Busca o grupo pela descrição, se não encontrar, cria um novo com a descrição e o IdGrupo passado.
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <param name="descricao"></param>
        /// <returns></returns>
        Entidades.PlanoContas RecuperaOuCriaPlanoContas(int idGrupo, string descricao);

        /// <summary>
        /// Verifica se o plano de contas está em uso.
        /// </summary>
        /// <param name="idConta"></param>
        /// <param name="usarConfiguracao">Identifica se este método está sendo chamado da tela de configurações.</param>
        /// <returns></returns>
        bool PlanoContasEmUso(int idConta, bool usarConfiguracao);

        /// <summary>
        /// Salva os dados do plano de contas.
        /// </summary>
        /// <param name="planoContas"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarPlanoContas(Entidades.PlanoContas planoContas);

        /// <summary>
        /// Apaga o plano de contas.
        /// </summary>
        /// <param name="planoContas"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarPlanoContas(Entidades.PlanoContas planoContas);

        #endregion
    }
}
