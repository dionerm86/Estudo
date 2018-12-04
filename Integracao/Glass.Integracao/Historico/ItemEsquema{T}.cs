// <copyright file="ItemEsquema{T}.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Integracao.Historico
{
    /// <summary>
    /// Representa um item do esquema de histório associado do tipo <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Tipo que o item representa.</typeparam>
    public class ItemEsquema<T> : ItemEsquema
    {
        private readonly Func<T, IEnumerable<object>> provedorIdentificadores;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ItemEsquema{T}"/>.
        /// </summary>
        /// <param name="id">Identificador do item do esquema.</param>
        /// <param name="nome">Nome do item.</param>
        /// <param name="descricao">Descrição do item.</param>
        /// <param name="identificadores">Identicadores do item.</param>
        /// <param name="provedorIdentificadores">Provedor dos identificadores associado com tipo <typeparamref name="T"/>.</param>
        public ItemEsquema(
            int id,
            string nome,
            string descricao,
            IEnumerable<IdentificadorItemEsquema> identificadores,
            Func<T, IEnumerable<object>> provedorIdentificadores)
            : base(id, nome, descricao, identificadores, (type) => type == typeof(T))
        {
            if (provedorIdentificadores == null)
            {
                throw new ArgumentNullException(nameof(provedorIdentificadores));
            }

            this.provedorIdentificadores = provedorIdentificadores;
        }

        /// <summary>
        /// Cria um item do histório com base no esquema.
        /// </summary>
        /// <param name="referencia">Referência de onde serão recuperados os dados para gerar o item do histórico.</param>
        /// <param name="tipo">Tipo do item do histórico.</param>
        /// <param name="mensagem">Mensagem.</param>
        /// <returns>Item do histórico.</returns>
        public Item CriarItemHistorico(T referencia, TipoItemHistorico tipo, string mensagem)
        {
            var identificadores = this.provedorIdentificadores(referencia).ToArray();
            return new Item(this, tipo, mensagem, identificadores, DateTime.Now);
        }

        /// <summary>
        /// Cria um item do histórico para falha com base no esquema.
        /// </summary>
        /// <param name="referencia">Referência de onde serão recuperados os dados para gerar o item do histórico.</param>
        /// <param name="mensagem">Mensagem.</param>
        /// <param name="falha">Falha associada.</param>
        /// <returns>Item do histórico.</returns>
        public Item CriarItemHistorico(T referencia, string mensagem, Falha falha)
        {
            var identificadores = this.provedorIdentificadores(referencia).ToArray();
            return new Item(this, mensagem, falha, identificadores, DateTime.Now);
        }
    }
}
