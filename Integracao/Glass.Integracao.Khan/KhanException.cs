// <copyright file="KhanException.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Runtime.Serialization;

namespace Glass.Integracao.Khan
{
    /// <summary>
    /// Representa o erro base da integração com a Khan.
    /// </summary>
    [Serializable]
    public class KhanException : IntegracaoException
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="KhanException"/>.
        /// </summary>
        /// <param name="message">Mensagem do erro.</param>
        public KhanException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="KhanException"/>.
        /// </summary>
        /// <param name="message">Mensagem do erro.</param>
        /// <param name="innerException">Erro interno.</param>
        public KhanException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="KhanException"/>.
        /// </summary>
        /// <param name="info">Informações da serialização.</param>
        /// <param name="context">Contexto para serialização.</param>
        protected KhanException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
