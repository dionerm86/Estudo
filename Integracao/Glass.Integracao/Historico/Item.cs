// <copyright file="Item.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;

namespace Glass.Integracao.Historico
{
    /// <summary>
    /// Representa um item do histórico.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="Item"/>.
        /// </summary>
        /// <param name="itemEsquema">Item do esquema.</param>
        /// <param name="tipo">Tipo do histórico.</param>
        /// <param name="mensagem">Mensagem.</param>
        /// <param name="identificadores">Identificadores associados.</param>
        /// <param name="data">Data de criação do item.</param>
        public Item(
            ItemEsquema itemEsquema,
            TipoItemHistorico tipo,
            string mensagem,
            IEnumerable<object> identificadores,
            DateTime data)
        {
            this.ItemEsquema = itemEsquema;
            this.Tipo = tipo;
            this.Identificadores = identificadores;
            this.Mensagem = mensagem;
            this.Data = data;
        }

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="Item"/>
        /// com base nos dados da falha.
        /// </summary>
        /// <param name="itemEsquema">Item do esquema de histórico.</param>
        /// <param name="mensagem">Mensagem.</param>
        /// <param name="falha">Falha associada.</param>
        /// <param name="identificadores">Identificadores associados.</param>
        /// <param name="data">Data de criação do item.</param>
        public Item(
            ItemEsquema itemEsquema,
            string mensagem,
            Falha falha,
            IEnumerable<object> identificadores,
            DateTime data)
        {
            this.ItemEsquema = itemEsquema;
            this.Tipo = TipoItemHistorico.Falha;
            this.Mensagem = mensagem ?? falha?.Mensagem;
            this.Falha = falha;
            this.Identificadores = identificadores;
            this.Data = data;
        }

        /// <summary>
        /// Obtém o item do esquema associado.
        /// </summary>
        public ItemEsquema ItemEsquema { get; }

        /// <summary>
        /// Obtém o tipo do item.
        /// </summary>
        public TipoItemHistorico Tipo { get; }

        /// <summary>
        /// Obtém os identificadores que representa o item.
        /// </summary>
        public IEnumerable<object> Identificadores { get; }

        /// <summary>
        /// Obtém a mensagem do item.
        /// </summary>
        public string Mensagem { get; }

        /// <summary>
        /// Obtém a falha associada com o item.
        /// </summary>
        public Falha Falha { get; }

        /// <summary>
        /// Obtém a data que o item foi registrado.
        /// </summary>
        public DateTime Data { get; }
    }
}
