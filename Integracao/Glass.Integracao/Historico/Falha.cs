// <copyright file="Falha.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;

namespace Glass.Integracao.Historico
{
    /// <summary>
    /// Representa uma falha.
    /// </summary>
    public class Falha
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="Falha"/>.
        /// </summary>
        /// <param name="tipo">Tipo da falha.</param>
        /// <param name="mensagem">Mensagem da falha.</param>
        /// <param name="pilhaChamada">Pilha de chamada.</param>
        /// <param name="falhaInterna">Falha interna.</param>
        public Falha(string tipo, string mensagem, string pilhaChamada, Falha falhaInterna = null)
        {
            this.Tipo = tipo;
            this.Mensagem = mensagem;
            this.PilhaChamada = pilhaChamada;
            this.FalhaInterna = falhaInterna;
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="Falha"/>
        /// com base nos dados da exception.
        /// </summary>
        /// <param name="exception">Erro que será usado como base para criar a falha.</param>
        public Falha(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            this.Tipo = exception.GetType().Name;
            this.Mensagem = exception.Message;
            this.PilhaChamada = exception.StackTrace;
            if (exception.InnerException != null)
            {
                this.FalhaInterna = new Falha(exception.InnerException);
            }
        }

        /// <summary>
        /// Obtém o tipo de falha.
        /// </summary>
        public string Tipo { get; }

        /// <summary>
        /// Obtém a mensagem da falha.
        /// </summary>
        public string Mensagem { get; }

        /// <summary>
        /// Obtém a pilha de chamada da falha.
        /// </summary>
        public string PilhaChamada { get; }

        /// <summary>
        /// Obtém os dados da falha interna.
        /// </summary>
        public Falha FalhaInterna { get; }
    }
}
