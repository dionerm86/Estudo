using System;
using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio dos funcionarios.
    /// </summary>
    public interface IFuncionarioFluxo
    {
        #region Funcionário

        /// <summary>
        /// Recupera os funcionários ativos que são vendedores ou estão 
        /// associados como vendedores para os clientes.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemFuncionariosAtivosAssociadosAClientes();

        /// <summary>
        /// Recupera os descritores dos funcionários ativos
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemFuncionariosAtivos();

        /// <summary>
        /// Recupera os dados dos funcionarios pelos nomes informados.
        /// </summary>
        /// <param name="nomes"></param>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemFuncionarios(IEnumerable<string> nomes);

        /// <summary>
        /// Recupera os descritores dos funcionários que são vendedores.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemVendedores();

        /// <summary>
        /// Recupera os descritores dos funcionários que devem aparecer no filtro de funcionário da listagem de sugestão de cliente.
        /// </summary>
        IList<Colosoft.IEntityDescriptor> ObterFuncionariosSugestao();

        /// <summary>
        /// Cria uma nova instancia do funcionario.
        /// </summary>
        /// <returns></returns>
        Entidades.Funcionario CriarFuncionario();

        /// <summary>
        /// Pesquisa os funcionários.
        /// </summary>
        /// <param name="idLoja">Identificador da loja.</param>
        /// <param name="nomeFuncionario">Nome do funcionário que será usado no filtro.</param>
        /// <param name="situacao">Situação do funcionário.</param>
        /// <param name="apenasRegistrados">Identifica se é para recupera apenas registrados.</param>
        /// <param name="idTipoFunc">Identificador do tipo de funcionário.</param>
        /// <param name="idSetor">Identificador do setor.</param>
        /// <param name="dataNascInicio"></param>
        /// <param name="dataNascFim"></param>
        /// <returns></returns>
        IList<Entidades.FuncionarioPesquisa> PesquisarFuncionarios(
            int? idLoja, string nomeFuncionario, Glass.Situacao? situacao,
            bool apenasRegistrados, int? idTipoFunc, int? idSetor,
            DateTime? dataNascInicio, DateTime? dataNascFim);

        /// <summary>
        /// Recupera os dados do funcionário.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        Entidades.Funcionario ObtemFuncionario(int idFunc);

        /// <summary>
        /// Recupera descritores de vários funcionários
        /// </summary>
        /// <param name="idsFunc"></param>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemFuncionario(IEnumerable<int> idsFunc);

        /// <summary>
        /// Salva os dados do funcionário.
        /// </summary>
        /// <param name="funcionario"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarFuncionario(Entidades.Funcionario funcionario);

        /// <summary>
        /// Apaga os dados do funcionário.
        /// </summary>
        /// <param name="funcionario"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarFuncionario(Entidades.Funcionario funcionario);

        #endregion

        #region Tipo de Funcionário

        /// <summary>
        /// Recupera os tipos de funcionarios do sistema.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.TipoFuncionario> PesquisarTiposFuncionario();

        /// <summary>
        /// Recupera os tipos de funcionarios cadastrados no sistema.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemTiposFuncionario();

        /// <summary>
        /// Recupera os tipos de funcionarios cadastrados no sistema para serem usados no ControleUsuarios.aspx
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemTiposFuncionarioParaControleUsuarios();

        /// <summary>
        /// Recupera os tipos de funcionario.
        /// </summary>
        /// <param name="incluirSetor">Identifica se é para incluir os setores no resultado.</param>
        /// <param name="removerMarcadorProducaoSemSetor">Identifica se é para remover o marcador de produção sem setor.</param>
        /// <returns></returns>
        IList<Entidades.TipoFuncSetor> ObtemTiposFuncionarioSetor(bool incluirSetor, bool removerMarcadorProducaoSemSetor);

        /// <summary>
        /// Recupera o tipo de funcionário pelo identificador informado.
        /// </summary>
        /// <param name="idTipoFuncionario"></param>
        /// <returns></returns>
        Entidades.TipoFuncionario ObtemTipoFuncionario(int idTipoFuncionario);

        /// <summary>
        /// Salva os dados da entidade.
        /// </summary>
        /// <param name="tipoFuncionario"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarTipoFuncionario(Entidades.TipoFuncionario tipoFuncionario);

        /// <summary>
        /// Apaga o tipo de funcionario.
        /// </summary>
        /// <param name="idTipoFuncionario"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarTipoFuncionario(int idTipoFuncionario);

        #endregion

        #region FuncModulo

        /// <summary>
        /// Recupera os módulos do funcionário.
        /// </summary>
        /// <param name="idFunc">Identificador do funcionário.</param>
        /// <returns></returns>
        IList<Entidades.FuncModuloPesquisa> ObtemModulosFuncionario(int idFunc);

        #endregion

        #region Departamento

        /// <summary>
        /// Pesquisa os departamentos do sistema.
        /// </summary>
        /// <param name="idDepartamento">Identificador do departamento que será pesquisado.</param>
        /// <param name="nome">Nome do departamento.</param>
        /// <returns></returns>
        IList<Entidades.Departamento> PesquisarDepartamentos(int? idDepartamento, string nome);

        /// <summary>
        /// Recupera os descritores dos departamentos.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObteDepartamentos();

        /// <summary>
        /// Recupera os dados do departamento.
        /// </summary>
        /// <param name="idDepartamento"></param>
        /// <returns></returns>
        Entidades.Departamento ObtemDepartamento(int idDepartamento);

        /// <summary>
        /// Salva os dados do departamento.
        /// </summary>
        /// <param name="departamento"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarDepartamento(Entidades.Departamento departamento);

        /// <summary>
        /// Apaga os dados do departamento.
        /// </summary>
        /// <param name="departamento"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarDepartamento(Entidades.Departamento departamento);

        /// <summary>
        /// Recupera os descritores dos funcionário associados ao departamento informado.
        /// </summary>
        /// <param name="idDepartamento">Identificador do departamento.</param>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemFuncionariosPorDepartamento(int idDepartamento);

        /// <summary>
        /// Recupera os descritores dos funcionário que estão fora do departamento informado.
        /// </summary>
        /// <param name="idDepartamento">Identificador do departamento.</param>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemFuncionariosForaDepartamento(int idDepartamento);

        /// <summary>
        /// Adiciona o funcionário ao departamento.
        /// </summary>
        /// <param name="idFunc">Identificador do funcionário.</param>
        /// <param name="idDepartamento">Identificador do departamento.</param>
        /// <returns></returns>
        Colosoft.Business.SaveResult AdicionarFuncionarioDepartamento(int idFunc, int idDepartamento);

        /// <summary>
        /// Apaga o funcionário do departamento.
        /// </summary>
        /// <param name="idFunc">Identificador do funcionário.</param>
        /// <param name="idDepartamento">Identificador do departamento.</param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarFuncionarioDepartamento(int idFunc, int idDepartamento);

        #endregion
    }
}
