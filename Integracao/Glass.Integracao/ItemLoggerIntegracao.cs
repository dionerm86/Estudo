// <copyright file="ItemLoggerIntegracao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Colosoft;
using Colosoft.Logging;
using System;

namespace Glass.Integracao
{
    /// <summary>
    /// Representa um item do logger de integração.
    /// </summary>
    public class ItemLoggerIntegracao
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ItemLoggerIntegracao"/>.
        /// </summary>
        /// <param name="categoria">Categoria do item.</param>
        /// <param name="prioridade">Prioridade do item.</param>
        /// <param name="mensagem">Mensagem do item.</param>
        /// <param name="erro">Descrição do erro.</param>
        /// <param name="pilhaChamada">Pilha de chamada do erro.</param>
        public ItemLoggerIntegracao(Category categoria, Priority prioridade, IMessageFormattable mensagem, string erro, string pilhaChamada)
        {
            this.Categoria = categoria;
            this.Prioridade = prioridade;
            this.Mensagem = mensagem;
            this.Erro = erro;
            this.PilhaChamada = pilhaChamada;
        }

        /// <summary>
        /// Obtém a data de criação do item.
        /// </summary>
        public DateTime DataCriacao { get; } = DateTime.Now;

        /// <summary>
        /// Obtém a categoria do item.
        /// </summary>
        public Category Categoria { get; }

        /// <summary>
        /// Obtém a prioridade do item.
        /// </summary>
        public Priority Prioridade { get; }

        /// <summary>
        /// Obtém a mensasgem do item.
        /// </summary>
        public IMessageFormattable Mensagem { get; }

        /// <summary>
        /// Obtém a descrição do erro.
        /// </summary>
        public string Erro { get; }

        /// <summary>
        /// Obtém a pilha de chamada.
        /// </summary>
        public string PilhaChamada { get; }

    }
}
