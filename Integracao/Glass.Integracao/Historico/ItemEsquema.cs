﻿// <copyright file="ItemEsquema.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Glass.Integracao.Historico
{
    /// <summary>
    /// Representa um item do esquema do histórico.
    /// </summary>
    public class ItemEsquema
    {
        private readonly Predicate<Type> verificadorCompatibilidade;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ItemEsquema"/>.
        /// </summary>
        /// <param name="id">Identificador do item do esquema.</param>
        /// <param name="nome">Nome do item.</param>
        /// <param name="descricao">Descrição do item.</param>
        /// <param name="identificadores">Identicadores do item.</param>
        /// <param name="verificadorCompatibilidade">Predicado que defin se o item é compatível com o tipo informado.</param>
        public ItemEsquema(
            int id,
            string nome,
            string descricao,
            IEnumerable<IdentificadorItemEsquema> identificadores,
            Predicate<Type> verificadorCompatibilidade)
        {
            this.Id = id;
            this.Nome = nome;
            this.Descricao = descricao;
            this.Identificadores = identificadores;
            this.verificadorCompatibilidade = verificadorCompatibilidade;
        }

        /// <summary>
        /// Obtém o identificador do item.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Obtém o nome do item.
        /// </summary>
        public string Nome { get; }

        /// <summary>
        /// Obtém a descrição do item.
        /// </summary>
        public string Descricao { get; }

        /// <summary>
        /// Obtém os identificadores do item.
        /// </summary>
        public IEnumerable<IdentificadorItemEsquema> Identificadores { get; }

        /// <summary>
        /// Obtém o esquema associado.
        /// </summary>
        public Esquema Esquema { get; internal set; }

        /// <summary>
        /// Verifica se o item é compatível com o tipo <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Tipo que será comparado para verificar a compatibilidade.</typeparam>
        /// <returns>True se o item for compatível com o tipo informdo.</returns>
        public bool Compativel<T>() =>
            this.verificadorCompatibilidade == null && this.verificadorCompatibilidade(typeof(T));

        /// <summary>
        /// Cria um item do histórico com base no esquema.
        /// </summary>
        /// <param name="identificadores">Identificadores que representa o item.</param>
        /// <param name="tipo">Tipo do item que será criado.</param>
        /// <param name="mensagem">Mensagem associada.</param>
        /// <returns>Item do histórico.</returns>
        public Item CriarItemHistorico(IEnumerable<object> identificadores, TipoItemHistorico tipo, string mensagem)
        {
            if (identificadores == null)
            {
                throw new ArgumentNullException(nameof(identificadores));
            }

            return new Item(this, tipo, mensagem, identificadores, DateTime.Now);
        }

        /// <summary>
        /// Cria um item do histórico para falha com base no esquema.
        /// </summary>
        /// <param name="identificadores">Identificadores que representa o item.</param>
        /// <param name="mensagem">Mensagem.</param>
        /// <param name="falha">Falha associada.</param>
        /// <returns>Item do histórico.</returns>
        public Item CriarItemHistorico(IEnumerable<object> identificadores, string mensagem, Falha falha)
        {
            if (identificadores == null)
            {
                throw new ArgumentNullException(nameof(identificadores));
            }

            return new Item(this, mensagem, falha, identificadores, DateTime.Now);
        }
    }
}
