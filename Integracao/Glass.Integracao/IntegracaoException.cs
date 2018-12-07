// <copyright file="IntegracaoException.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Runtime.Serialization;

namespace Glass.Integracao
{
    /// <summary>
    /// Representa o erro base da integração.
    /// </summary>
    [Serializable]
    public class IntegracaoException : Exception
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="IntegracaoException"/>.
        /// </summary>
        /// <param name="message">Mensagem do erro.</param>
        public IntegracaoException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="IntegracaoException"/>.
        /// </summary>
        /// <param name="message">Mensagem do erro.</param>
        /// <param name="innerException">Erro interno.</param>
        public IntegracaoException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="IntegracaoException"/>.
        /// </summary>
        /// <param name="info">Informações da serialização.</param>
        /// <param name="context">Contexto para serialização.</param>
        protected IntegracaoException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
