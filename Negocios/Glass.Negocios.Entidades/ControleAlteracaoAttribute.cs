using System;

namespace Glass.Negocios
{
    /// <summary>
    /// Assinatura das classe que cria o identificador da entidade para o log.
    /// </summary>
    public interface ILogIdCreator
    {
        /// <summary>
        /// Cria o id para o log.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Create(Colosoft.Business.IEntity entity);
    }

    /// <summary>
    /// Implementação básica.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class LogIdCreator<TEntity> : ILogIdCreator where TEntity : Colosoft.Business.IEntity
    {
        /// <summary>
        /// Cria o identificador para o log.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public abstract int Create(TEntity entity);

        /// <summary>
        /// Cria o identificador para o log.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int ILogIdCreator.Create(Colosoft.Business.IEntity entity)
        {
            return Create((TEntity)entity);
        }
    }

    /// <summary>
    /// Atribute usado para identificar que a entidade possui controle de alteração.
    /// </summary>
    public class ControleAlteracaoAttribute : Attribute
    {
        #region Variaveis Locais

        private bool _logFilhoAoInserir = true;

        #endregion

        #region Propriedades

        /// <summary>
        /// Tabela associada ao log de cancelamento.
        /// </summary>
        public Data.Model.LogCancelamento.TabelaCancelamento? TabelaCancelamento { get; private set; }

        /// <summary>
        /// Tabela associada ao log de alteração.
        /// </summary>
        public Data.Model.LogAlteracao.TabelaAlteracao? TabelaAlteracao { get; private set; }

        /// <summary>
        /// Tabela associada ao log de exclusão.
        /// </summary>
        public Data.Model.LogAlteracao.TabelaAlteracao? TabelaExclusao { get; private set; }

        /// <summary>
        /// Deverá ser inserido log ao inserir?
        /// </summary>
        public bool LogAoInserir { get; private set; }

        /// <summary>
        /// Deverá ser inserido log dos filhos ao inserir?
        /// </summary>
        public bool LogFilhoAoInserir { get { return _logFilhoAoInserir; } }

        /// <summary>
        /// Tipo da classe responsável por criar o identificador do log.
        /// </summary>
        public Type LogIdCreatorType { get; set; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public ControleAlteracaoAttribute()
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="tabelaAlteracao"></param>
        public ControleAlteracaoAttribute(Data.Model.LogAlteracao.TabelaAlteracao tabelaAlteracao)
            : this(tabelaAlteracao, false)
        {
        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="tabelaAlteracao"></param>
        public ControleAlteracaoAttribute(Data.Model.LogAlteracao.TabelaAlteracao tabelaAlteracao, bool logAoInserir)
        {
            TabelaAlteracao = tabelaAlteracao;
            LogAoInserir = logAoInserir;
        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="tabelaAlteracao"></param>
        public ControleAlteracaoAttribute(Data.Model.LogAlteracao.TabelaAlteracao tabelaAlteracao, bool logAoInserir, bool logFilhoAoInserir)
        {
            TabelaAlteracao = tabelaAlteracao;
            LogAoInserir = logAoInserir;
            _logFilhoAoInserir = logFilhoAoInserir;
        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="tabelaAlteracao"></param>
        /// <param name="logIdCreatorType"></param>
        public ControleAlteracaoAttribute(Data.Model.LogAlteracao.TabelaAlteracao tabelaAlteracao, Type logIdCreatorType)
        {
            TabelaAlteracao = tabelaAlteracao;
            LogIdCreatorType = logIdCreatorType;
        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="tabelaCancelamento"></param>
        public ControleAlteracaoAttribute(
            Data.Model.LogCancelamento.TabelaCancelamento tabelaCancelamento)
        {
            TabelaCancelamento = tabelaCancelamento;
        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="tabelaAlteracao"></param>
        /// <param name="tabelaCancelamento"></param>
        public ControleAlteracaoAttribute(
            Data.Model.LogAlteracao.TabelaAlteracao tabelaAlteracao,
            Data.Model.LogCancelamento.TabelaCancelamento tabelaCancelamento)
        {
            TabelaAlteracao = tabelaAlteracao;
            TabelaCancelamento = tabelaCancelamento;
        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="tabelaAlteracao"></param>
        /// <param name="tabelaExclusao"></param>
        public ControleAlteracaoAttribute(
            Data.Model.LogAlteracao.TabelaAlteracao tabelaAlteracao,
            Data.Model.LogAlteracao.TabelaAlteracao tabelaExclusao)
        {
            TabelaAlteracao = tabelaAlteracao;
            TabelaExclusao = tabelaExclusao;
        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="tabela"></param>
        /// <param name="tabelaCancelamento"></param>
        /// <param name="tabelaExclusao"></param>
        public ControleAlteracaoAttribute(
            Data.Model.LogAlteracao.TabelaAlteracao tabela,
            Data.Model.LogCancelamento.TabelaCancelamento tabelaCancelamento,
            Data.Model.LogAlteracao.TabelaAlteracao tabelaExclusao)
        {
            TabelaAlteracao = tabela;
            TabelaCancelamento = tabelaCancelamento;
            TabelaExclusao = tabelaExclusao;
        }

        #endregion
    }
}
