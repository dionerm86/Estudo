using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Negocios.Componentes
{
    /// <summary>
    /// Implementação do controle de alteração.
    /// </summary>
    public class ControleAlteracao : IControleAlteracao
    {
        #region Local Variables

        /// <summary>
        /// Nome do parametro usado para ignorar o log de exclusão da entidade.
        /// </summary>
        private const string IgnoreExclusaoParameterName = "IGNORE-DELETE-LOG";

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Recupera os dados atuais da entidade.
        /// </summary>
        /// <param name="entidade"></param>
        /// <returns></returns>
        private static object ObtemDadosAtuais(Colosoft.Business.IEntity entidade)
        {
            var ts = Colosoft.Query.TypeBindStrategyCache.GetItem(entidade.Loader.DataModelType,
                (t) => new Colosoft.Query.QueryResultObjectCreator(t));

            var consulta = SourceContext.Instance.CreateQuery()
                .From(new Colosoft.Query.EntityInfo(entidade.Loader.DataModelTypeName.FullName));

            object atual = null;

            if (entidade.Loader.HasUid)
                // Carrega o banco os dados atuais da model
                consulta.Where(string.Format("{0}=?id", entidade.Loader.UidPropertyName))
                    .Add("?id", entidade.Uid);
            else
            {
                consulta.Where(string.Join(" AND ",
                    entidade.Loader.KeysPropertyNames.Select(f => string.Format("{0}=?{0}", f)).ToArray()));

                foreach (var i in entidade.Loader.GetInstanceKeysValues(entidade))
                    consulta.Add(string.Format("?{0}", i.Item1), i.Item2); 
            }
                
            atual = consulta.Execute()
                .Select(f =>
                {
                    var obj = ts.Create();
                    ts.Bind(f, Colosoft.Query.BindStrategyMode.All, ref obj);
                    return obj;
                }).FirstOrDefault();

            return atual;
        }

        #endregion

        #region Método Públicos

        /// <summary>
        /// Registra na sessão de persistencia a operação para salvar
        /// as alterações da entidade.
        /// </summary>
        /// <param name="sessao">Sessão de persistencia.</param>
        /// <param name="entidade">Entidade com os dados alterados.</param>
        public void RegistraAlteracoes(Colosoft.Data.IPersistenceSession sessao, Colosoft.Business.IEntity entidade)
        {
            var attribute = entidade.GetType()
                .GetCustomAttributes(typeof(ControleAlteracaoAttribute), false)
                .FirstOrDefault() as ControleAlteracaoAttribute;

            var id = new Lazy<int>(() =>
            {
                if (entidade.HasUid)
                    return entidade.Uid;
                else if (attribute.LogIdCreatorType != null)
                {
                    var idCreator = Activator.CreateInstance(attribute.LogIdCreatorType) as ILogIdCreator;
                    return idCreator.Create(entidade);
                }

                return 0;
            });

            // Verifica se está atualizando a entidade
            if (attribute != null && attribute.TabelaAlteracao.HasValue && (entidade.ExistsInStorage || attribute.LogAoInserir))
            {
                dynamic entidade2 = entidade;
                object atual = ObtemDadosAtuais(entidade);

                bool criou = false;

                if (atual == null && attribute.LogAoInserir)
                {
                    dynamic e = entidade.Loader.Create(null, Colosoft.Business.EntityTypeManager.Instance, SourceContext.Instance);
                    atual = e.DataModel;
                    criou = atual != null;
                }

                if (atual == null)
                    return;

                string referencia = attribute.LogAoInserir && criou && entidade.Loader.HasFindName ?
                    entidade.Loader.GetInstanceFindName(entidade2) :
                    null;

                var logs = Data.DAL.LogAlteracaoDAO.Instance.ObtemLogs(Glass.Data.Helper.UserInfo.GetUserInfo.CodUser,
                    attribute.TabelaAlteracao.Value, id.Value, atual, entidade2.DataModel, referencia);

                foreach (Data.Model.LogAlteracao log in logs)
                {
                    if (log.IdLog == 0)
                    {
                        // Define o novo identificador virtual
                        log.IdLog = Colosoft.Business.EntityTypeManager.Instance.GenerateInstanceUid(typeof(Data.Model.LogAlteracao));
                    }

                    // Adiciona a entrada do log para inserir no banco
                    sessao.Insert(log, (string[])null);
                }
            }

            // Verifica se possui configuração de log
            if (ControleAlteracaoEntityLoaderConfiguration.PossuiConfiguracao(entidade.Loader) && (entidade.ExistsInStorage || (attribute != null && attribute.LogFilhoAoInserir)))
            {
                var config = ControleAlteracaoEntityLoaderConfiguration.ObtemConfiguracao(entidade.Loader);

                foreach (var entry in config.Entries)
                {
                    // Recupera o valor da entidade
                    var value = entry.GetValue(entidade.Loader, entidade);

                    // Verifica se é uma lista de filho
                    if (value is Colosoft.Business.IEntityChildrenList)
                    {
                        var children = (Colosoft.Business.IEntityChildrenList)value;

                        if (!children.IsLazyLoadState)
                        {
                            if (children.IsChanged)
                            {
                                // Recupera os loader da entidades filhas
                                var entityLoader =
                                    children.GetNewItems()
                                        .Union(children.GetChangedItems()
                                            .Union(children.GetRemovedItems()))
                                    .Select(f => f.Loader)
                                    .FirstOrDefault(f => f != null);

                                if (entityLoader == null) continue;

                                IEnumerable<string> changedItems = null;
                                if (children.GetChangedItems().Any())
                                {
                                    // Cria uma consulta para recupera os filhos alterados
                                    var query = SourceContext.Instance.CreateQuery()
                                        .From(new Colosoft.Query.EntityInfo(entityLoader.DataModelType.FullName))
                                        .Select(string.Join(",", entityLoader.FindNameProperties));

                                    if (entityLoader.HasUid)
                                    {
                                        // Consulta pelos ids
                                        var ids = string.Join(",", children.GetChangedItems().Select(f => entityLoader.GetInstanceUid(f)));
                                        query.Where(string.Format("{0} IN ({1})", entityLoader.UidPropertyName, ids));
                                    }
                                    else
                                    {
                                        var count = 0;

                                        // Consulta pelas alterações

                                        foreach (var i in children.GetChangedItems())
                                        {
                                            var container = new Colosoft.Query.ConditionalContainer();

                                            // Recupera os valores das chaves da instancia.
                                            var keys = entityLoader.GetInstanceKeysValues(i);
                                            foreach (var k in keys)
                                            {
                                                container
                                                    .And(string.Format("{0}=?v{1}", k.Item1, count))
                                                    .Add(string.Format("?v{0}", count++), k.Item2);
                                            }

                                            query.WhereClause.Or(container);
                                        }
                                    }

                                    var properties = entityLoader.DataModelType.GetProperties().Where(f => (entityLoader.FindNameProperties.Contains(f.Name))).ToArray();
                                    properties = entityLoader.FindNameProperties.Select(f => properties.First(x => x.Name == f)).ToArray();

                                    // Recupera os itens alterados
                                    changedItems = query.Execute()
                                        .Select(f => entityLoader.GetInstanceFindName(
                                            properties.Select(x => Colosoft.Reflection.TypeConverter.Get(x.PropertyType, f.GetValue(x.Name))).ToArray()))
                                        .ToList();
                                }
                                else
                                    changedItems = new string[0];

                                var anterior = changedItems.Any() ? string.Format("-> Alterados:\r\n{0}", string.Join("\r\n", changedItems)) : "";

                                var newItems = children.GetNewItems().Any() ? children.GetNewItems().Select(f => f.FindName).ToArray() : new string[0];
                                changedItems = children.GetChangedItems().Select(f => f.FindName).Where(f => !newItems.Contains(f)).ToList();

                                /*var atual = string.Join("\r\n", new[] 
                                {
                                    newItems.Any() ? string.Format("-> Novos:\r\n{0}", string.Join("\r\n", newItems)) : "",
                                    changedItems.Any() ? string.Format("-> Alterados:\r\n{0}", string.Join("\r\n", changedItems)) : "",
                                    children.GetRemovedItems().Any() ? string.Format("-> Removidos:\r\n{0}", string.Join("\r\n", children.GetRemovedItems().Select(f => f.FindName))) : "" 
                                }.Where(f => !string.IsNullOrEmpty(f)));*/
                                /* Chamado 18215. */
                                var atual = string.Join("\r\n", new[] 
                                {
                                    newItems.Any() ?
                                        string.Format(entry.ChildName == "FormasPagto" ? "-> Removidos:\r\n{0}" : "-> Novos:\r\n{0}",
                                        string.Join("\r\n", newItems)) : "",
                                    changedItems.Any() ?
                                        string.Format("-> Alterados:\r\n{0}", string.Join("\r\n", changedItems)) : "",
                                    children.GetRemovedItems().Any() ?
                                        string.Format(entry.ChildName == "FormasPagto" ? "-> Novos:\r\n{0}" : "-> Removidos:\r\n{0}",
                                        string.Join("\r\n", children.GetRemovedItems().Select(f => f.FindName))) : "" 
                                }.Where(f => !string.IsNullOrEmpty(f)));

                                var log = new Data.Model.LogAlteracao();
                                // Define o novo identificador virtual
                                log.IdLog = Colosoft.Business.EntityTypeManager.Instance.GenerateInstanceUid(typeof(Data.Model.LogAlteracao));

                                log.Tabela = (int)attribute.TabelaAlteracao.Value;
                                log.IdRegistroAlt = id.Value;
                                log.NumEvento = Glass.Data.DAL.LogAlteracaoDAO.Instance.GetNumEvento(attribute.TabelaAlteracao.Value, id.Value); ;
                                log.Campo = entry.Description;
                                log.DataAlt = DateTime.Now;
                                log.IdFuncAlt = Glass.Data.Helper.UserInfo.GetUserInfo.CodUser;
                                log.ValorAnterior = anterior;
                                log.ValorAtual = atual;

                                // Adiciona a entrada do log para inserir no banco
                                sessao.Insert(log, (string[])null);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Registra na sessão de persistencia o log da exclusão da entidade.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="entidade"></param>
        public void RegistraExclusao(Colosoft.Data.IPersistenceSession sessao, Colosoft.Business.IEntity entidade)
        {
            // Verifica se está apagando uma entidade existente
            if (entidade.ExistsInStorage && !entidade.InstanceState.Parameters.Contains(IgnoreExclusaoParameterName))
            {
                var cancelamentoAttribute = entidade.GetType()
                    .GetCustomAttributes(typeof(ControleAlteracaoAttribute), false)
                    .FirstOrDefault() as ControleAlteracaoAttribute;

                if (cancelamentoAttribute != null && cancelamentoAttribute.TabelaCancelamento.HasValue)
                {
                    dynamic entidade2 = entidade;

                    var logs = Data.DAL.LogCancelamentoDAO.Instance.ObtemLogs(Glass.Data.Helper.UserInfo.GetUserInfo.CodUser,
                        cancelamentoAttribute.TabelaCancelamento.Value, entidade.Uid, entidade2.DataModel, null, false);

                    foreach (Data.Model.LogCancelamento log in logs)
                    {
                        if (log.IdLogCancelamento == 0)
                        {
                            // Define o novo identificador virtual
                            log.IdLogCancelamento = Colosoft.Business.EntityTypeManager.Instance.GenerateInstanceUid(typeof(Data.Model.LogCancelamento));
                        }

                        // Adiciona a entrada do log para inserir no banco
                        sessao.Insert(log, (string[])null);
                    }
                }
            }
        }

        /// <summary>
        /// Registra que é para ignorar o log de exclusão da entidade.
        /// </summary>
        /// <param name="entidade"></param>
        public void IgnoreLogExclusao(Colosoft.Business.IEntity entidade)
        {
            entidade.Require("entidade").NotNull();
            entidade.InstanceState.Parameters[IgnoreExclusaoParameterName] = true;
        }

        #endregion
    }
}
