// <copyright file="MonitorEventos.cs" company="Sync Softwares">
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
    /// Representa o monitor base.
    /// </summary>
    public abstract class MonitorEventos : IDisposable
    {
        private readonly Colosoft.Domain.IDomainEvents domainEvents;
        private readonly List<IDisposable> tokens = new List<IDisposable>();

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="MonitorEventos"/>.
        /// </summary>
        /// <param name="domainEvents">Eventos de domínio.</param>
        protected MonitorEventos(Colosoft.Domain.IDomainEvents domainEvents)
        {
            this.domainEvents = domainEvents;
        }

        /// <summary>
        /// Adiciona o token para o monitor.
        /// </summary>
        /// <param name="token">Token que será adicionado.</param>
        /// <typeparam name="T">Tipo do evento de domínio.</typeparam>
        public void AdicionarToken<T>(Colosoft.Domain.SubscriptionToken token)
            where T : Colosoft.Domain.DomainEventBase, new()
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            this.tokens.Add(new TokenInfo<T>(this.domainEvents, token));
        }

        /// <summary>
        /// Libera a instância.
        /// </summary>
        /// <param name="disposing">Identifica se esta sendo liberado.</param>
        protected virtual void Dispose(bool disposing)
        {
            foreach (var token in this.tokens)
            {
                token.Dispose();
            }

            this.tokens.Clear();
        }

        /// <summary>
        /// Libera a instância.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private sealed class TokenInfo<T> : IDisposable
            where T : Colosoft.Domain.DomainEventBase, new()
        {
            private readonly Colosoft.Domain.SubscriptionToken token;
            private readonly Colosoft.Domain.IDomainEvents domainEvents;

            public TokenInfo(Colosoft.Domain.IDomainEvents domainEvents, Colosoft.Domain.SubscriptionToken token)
            {
                this.domainEvents = domainEvents;
                this.token = token;
            }

            public void Dispose()
            {
                this.domainEvents.GetEvent<T>().Unsubscribe(this.token);
                GC.SuppressFinalize(this);
            }
        }
    }
}
