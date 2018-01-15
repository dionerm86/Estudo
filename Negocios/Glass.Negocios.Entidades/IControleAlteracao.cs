using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass
{
    /// <summary>
    /// Classe com métodos de extensão para o controle de alteração.
    /// </summary>
    public static class ControleAlteracaoExtensions
    {
        /// <summary>
        /// Registra na sessão de persistencia informações sobre o cancelamento da entidade.
        /// </summary>
        /// <param name="entidade"></param>
        /// <param name="session"></param>
        public static void RegistrarCancelamento(this Colosoft.Business.IEntity entidade, Colosoft.Data.IPersistenceSession session)
        {
            var controleAlteracao = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Negocios.IControleAlteracao>();
            controleAlteracao.RegistraExclusao(session, entidade);
        }

        /// <summary>
        /// Configura para gerar o log para a propriedade da entidade.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="loader"></param>
        /// <param name="description"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static Colosoft.Business.EntityLoader<TEntity, TModel>.FluentEntityLoader Log<TEntity, TModel>
            (this Colosoft.Business.EntityLoader<TEntity, TModel>.FluentEntityLoader fluentEntityLoader,
             string childName, string description)
            where TEntity : class, Colosoft.Business.IEntity<TModel>
            where TModel : class, Colosoft.Data.IModel
        {
            var config = Glass.Negocios.ControleAlteracaoEntityLoaderConfiguration.ObtemConfiguracao(fluentEntityLoader.Loader);

            var entry = new Glass.Negocios.ControleAlteracaoEntityLoaderConfigurationEntry
                (childName, description);

            config.Entries.Add(entry);

            return fluentEntityLoader;
        }
    }
}

namespace Glass.Negocios
{
    /// <summary>
    /// Configuração para o loader da entidade com os dados do controle de alteração.
    /// </summary>
    public class ControleAlteracaoEntityLoaderConfiguration
    {
        #region Constantes

        private const string Key = "ControleAlteracao";

        #endregion

        #region Variáveis Locais

        private List<ControleAlteracaoEntityLoaderConfigurationEntry> _entries = new List<ControleAlteracaoEntityLoaderConfigurationEntry>();

        #endregion

        #region Propriedades

        /// <summary>
        /// Entradas.
        /// </summary>
        public List<ControleAlteracaoEntityLoaderConfigurationEntry> Entries
        {
            get { return _entries; }
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Verifica se existem a configuração no loader.
        /// </summary>
        /// <param name="loader"></param>
        /// <returns></returns>
        public static bool PossuiConfiguracao(Colosoft.Business.IEntityLoader loader)
        {
            return loader.CustomParameters.ContainsKey(Key);
        }

        /// <summary>
        /// Recupera a configuração.
        /// </summary>
        /// <param name="loader"></param>
        /// <returns></returns>
        public static ControleAlteracaoEntityLoaderConfiguration ObtemConfiguracao(Colosoft.Business.IEntityLoader loader)
        {
            ControleAlteracaoEntityLoaderConfiguration config = null;
            if (!PossuiConfiguracao(loader))
            {
                config = new ControleAlteracaoEntityLoaderConfiguration();
                loader.CustomParameters[Key] = config;
            }
            else
                config = (ControleAlteracaoEntityLoaderConfiguration)loader.CustomParameters[Key];

            return config;
        }

        #endregion
    }

    /// <summary>
    /// Representa uma entrada da configuração.
    /// </summary>
    public class ControleAlteracaoEntityLoaderConfigurationEntry
    {
        #region Variáveis Locais

        private string _childName;
        private string _description;

        #endregion

        #region Propriedades

        /// <summary>
        /// Nome da filho.
        /// </summary>
        public string ChildName
        {
            get { return _childName; }
        }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="childName"></param>
        /// <param name="description"></param>
        public ControleAlteracaoEntityLoaderConfigurationEntry(string childName, string description)
        {
            _childName = childName;
            _description = description;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Recupera o valor.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Colosoft.Business.IEntity GetValue(Colosoft.Business.IEntityLoader loader, Colosoft.Business.IEntity entity)
        {
            var childAccessor = loader.GetChildrenAccessors().FirstOrDefault(f => f.Name == ChildName);

            if (childAccessor == null)
                throw new NotSupportedException(
                    string.Format("Filho '{0}' da entidade '{1}' não foi encontrado", ChildName, loader.DataModelType.Name));


            return childAccessor.Get(entity);
        }

        #endregion
    }

    /// <summary>
    /// Assinatura da classe responsável por controlar as alterações
    /// das entidades de negócio do sistema.
    /// </summary>
    public interface IControleAlteracao
    {
        #region Métodos

        /// <summary>
        /// Registra o número do evento de alteração para um registro de uma tabela
        /// </summary>
        int ObterNumeroEventoAlteracao(Data.Model.LogAlteracao.TabelaAlteracao tabela, int idRegistroAlt);

        /// <summary>
        /// Registra na sessão de persistencia a operação para salvar
        /// as alterações da entidade.
        /// </summary>
        /// <param name="sessao">Sessão de persistencia.</param>
        /// <param name="entidade">Entidade com os dados alterados.</param>
        void RegistraAlteracoes(Colosoft.Data.IPersistenceSession sessao, Colosoft.Business.IEntity entidade);

        /// <summary>
        /// Registra na sessão de persistencia o log da exclusão da entidade.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="entidade"></param>
        void RegistraExclusao(Colosoft.Data.IPersistenceSession sessao, Colosoft.Business.IEntity entidade);

        /// <summary>
        /// Registra que é para ignorar o log de exclusão da entidade.
        /// </summary>
        /// <param name="entidade"></param>
        void IgnoreLogExclusao(Colosoft.Business.IEntity entidade);

        #endregion
    }
}
