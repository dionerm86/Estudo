// <copyright file="MonitorEventosEntitidade.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Integracao
{
    /// <summary>
    /// Classe que representa o monitor de eventos de entidades.
    /// </summary>
    /// <typeparam name="TEntidade">Tipo da entidade do monitor.</typeparam>
    public abstract class MonitorEventosEntitidade<TEntidade> : MonitorEventos
        where TEntidade : Colosoft.Business.IEntity
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="MonitorEventosEntitidade{TEntidade}"/>.
        /// </summary>
        /// <param name="domainEvents">Domínio de eventos.</param>
        protected MonitorEventosEntitidade(Colosoft.Domain.IDomainEvents domainEvents)
            : base(domainEvents)
        {
            domainEvents.GetEvent<Colosoft.Business.EntityPersistingEvent>()
                .Subscribe(this.EntityPersisting);
        }

        private void EntityPersisting(Colosoft.Business.EntityPersistingEventArgs e)
        {
            if (e.Entity is TEntidade && e.Entity.IsChanged)
            {
                var action = e.AfterSession.LastOrDefault() ?? e.Session.LastOrDefault();

                Colosoft.Data.PersistenceActionCallback handler = null;

                handler = (action1, result1) =>
                {
                    action.Callback -= handler;

                    if (result1.Success)
                    {
                        this.EntidadeAtualizada((TEntidade)e.Entity);
                    }
                };

                action.Callback += handler;
            }
        }

        /// <summary>
        /// Método acionado quando a entidade for atualizada.
        /// </summary>
        /// <param name="entidade">Entidade atualizada.</param>
        protected abstract void EntidadeAtualizada(TEntidade entidade);
    }
}
