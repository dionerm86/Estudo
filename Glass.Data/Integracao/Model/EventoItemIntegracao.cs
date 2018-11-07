// <copyright file="EventoItemIntegracao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using System;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa um evento do item de integração.
    /// </summary>
    [PersistenceClass("evento_item_integracao")]
    public class EventoItemIntegracao : Colosoft.Data.BaseModel
    {
        /// <summary>
        /// Obtém ou define o identificador do evento do item de integração.
        /// </summary>
        [PersistenceProperty("IdEventoItemIntegracao", PersistenceParameterType.IdentityKey)]
        public int IdEventoItemIntegracao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do item de integração.
        /// </summary>
        [PersistenceProperty("IdItemIntegracao")]
        [PersistenceForeignKey(typeof(ItemIntegracao), nameof(ItemIntegracao.IdItemIntegracao))]
        public int IdItemIntegracao { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do evento.
        /// </summary>
        [PersistenceProperty("Tipo")]
        public TipoEventoItemIntegracao Tipo { get; set; }

        /// <summary>
        /// Obtém ou define a mensagem do evento.
        /// </summary>
        [PersistenceProperty("Mensagem")]
        public string Mensagem { get; set; }

        /// <summary>
        /// Obtém ou define a data que o evento ocorreu.
        /// </summary>
        [PersistenceProperty("Data")]
        public DateTime Data { get; set; }
    }
}
