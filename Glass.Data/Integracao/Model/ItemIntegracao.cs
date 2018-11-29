// <copyright file="ItemIntegracao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using System;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa um item da integração.
    /// </summary>
    [PersistenceClass("item_integracao")]
    public class ItemIntegracao : Colosoft.Data.BaseModel
    {
        /// <summary>
        /// Obtém ou define o identificador do item de integração.
        /// </summary>
        [PersistenceProperty("IdItemIntegracao", PersistenceParameterType.IdentityKey)]
        public int IdItemIntegracao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do esquema do histórico.
        /// </summary>
        [PersistenceProperty("IdEsquemaHistorico")]
        public int IdEsquemaHistorico { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do item do esquema do histórico.
        /// </summary>
        [PersistenceProperty("IdItemEsquemaHistorico")]
        public int IdItemEsquemaHistorico { get; set; }

        /// <summary>
        /// Obtém ou define o primeiro identificador inteiro do item.
        /// </summary>
        [PersistenceProperty("IdInteiro1")]
        public int IdInteiro1 { get; set; }

        /// <summary>
        /// Obtém ou define o segundo identificador inteiro do item.
        /// </summary>
        [PersistenceProperty("IdInteiro2")]
        public int IdInteiro2 { get; set; }

        /// <summary>
        /// Obtém ou define o identificador textual do item.
        /// </summary>
        [PersistenceProperty("IdTextual")]
        public string IdTextual { get; set; }

        /// <summary>
        /// Obtém ou define a situação do item.
        /// </summary>
        [PersistenceProperty("Situacao")]
        public SituacaoItemIntegracao Situacao { get; set; }

        /// <summary>
        /// Obtém ou define da data da última atualização.
        /// </summary>
        [PersistenceProperty("UltimaAtualizacao")]
        public DateTime UltimaAtualizacao { get; set; }
    }
}
