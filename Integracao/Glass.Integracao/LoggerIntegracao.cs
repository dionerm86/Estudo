// <copyright file="LoggerIntegracao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using Colosoft.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Integracao
{
    /// <summary>
    /// Representa o logger da integração.
    /// </summary>
    public class LoggerIntegracao : Colosoft.Logging.ILogger
    {
        private const int CapacidadeMaxima = 50;

        private readonly Queue<ItemLoggerIntegracao> itens = new Queue<ItemLoggerIntegracao>(CapacidadeMaxima);

        /// <summary>
        /// Obtém os itens do logger.
        /// </summary>
        public IEnumerable<ItemLoggerIntegracao> Itens => itens;

        /// <inheritdoc />
        public bool IsDebugEnabled => true;

        /// <inheritdoc />
        public bool IsErrorEnabled => true;

        /// <inheritdoc />
        public bool IsFatalEnabled => true;

        /// <inheritdoc />
        public bool IsInfoEnabled => true;

        /// <inheritdoc />
        public bool IsWarnEnabled => true;

        private void AdicionarItem(IMessageFormattable mensagem, Category categoria, string erro = null, string pilhaChamada = null, Priority prioridade = Priority.None)
        {
            if (this.itens.Count > CapacidadeMaxima)
            {
                this.itens.Dequeue();
            }

            this.itens.Enqueue(new ItemLoggerIntegracao(categoria, prioridade, mensagem, erro, pilhaChamada));
        }

        /// <inheritdoc />
        public void CriticalInfo(IMessageFormattable message)
        {
            this.AdicionarItem(message, Category.Exception);
        }

        /// <inheritdoc />
        public void CriticalInfo(IMessageFormattable module, IMessageFormattable message)
        {
            this.CriticalInfo(message);
        }

        /// <inheritdoc />
        public void Debug(IMessageFormattable message)
        {
            if (this.IsDebugEnabled)
            {
                this.AdicionarItem(message, Category.Debug);
            }
        }

        /// <inheritdoc />
        public void Error(IMessageFormattable message)
        {
            if (this.IsErrorEnabled)
            {
                this.AdicionarItem(message, Category.Exception);
            }
        }

        /// <inheritdoc />
        public void Error(IMessageFormattable message, Exception exception)
        {
            if (this.IsErrorEnabled)
            {
                this.AdicionarItem(message, Category.Exception, exception?.Message, exception?.StackTrace);
            }
        }

        /// <inheritdoc />
        public void Error(IMessageFormattable module, IMessageFormattable message)
        {
            this.Error(message);
        }

        /// <inheritdoc />
        public void Fatal(IMessageFormattable message)
        {
            if (this.IsFatalEnabled)
            {
                this.AdicionarItem(message, Category.Exception);
            }
        }

        /// <inheritdoc />
        public void Fatal(IMessageFormattable message, Exception exception)
        {
            if (this.IsFatalEnabled)
            {
                this.AdicionarItem(message, Category.Exception, exception?.Message, exception?.StackTrace);
            }
        }

        /// <inheritdoc />
        public void Info(IMessageFormattable message)
        {
            if (this.IsInfoEnabled)
            {
                this.AdicionarItem(message, Category.Info);
            }
        }

        /// <inheritdoc />
        public void SetLevel(string level)
        {
            // Ignora
        }

        /// <inheritdoc />
        public void Warn(IMessageFormattable message)
        {
            if (this.IsWarnEnabled)
            {
                this.AdicionarItem(message, Category.Warn);
            }
        }

        /// <inheritdoc />
        public bool Write(IMessageFormattable message, Exception exception, Priority priority)
        {
            this.AdicionarItem(message, Category.Exception, exception?.Message, exception?.StackTrace, priority);
            return true;
        }

        /// <inheritdoc />
        public bool Write(IMessageFormattable message, Category category, Priority priority)
        {
            this.AdicionarItem(message, category, null, null, priority);
            return true;
        }
    }
}
