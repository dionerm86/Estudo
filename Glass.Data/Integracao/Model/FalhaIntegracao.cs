// <copyright file="FalhaIntegracao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;

namespace Glass.Data.Model
{
    /// <summary>
    /// Representa uma falha de integração.
    /// </summary>
    [PersistenceClass("falha_integracao")]
    public class FalhaIntegracao : Colosoft.Data.BaseModel
    {
        /// <summary>
        /// Obtém ou define o identificador da falha.
        /// </summary>
        [PersistenceProperty("IdFalhaIntegracao", PersistenceParameterType.IdentityKey)]
        public int IdFalhaIntegracao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do evento associado.
        /// </summary>
        [PersistenceProperty("IdEventoItemIntegracao")]
        [PersistenceForeignKey(typeof(EventoItemIntegracao), nameof(EventoItemIntegracao.IdEventoItemIntegracao))]
        public int? IdEventoItemIntegracao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do falha pai.
        /// </summary>
        [PersistenceProperty("IdFalhaIntegracaoPai")]
        [PersistenceForeignKey(typeof(FalhaIntegracao), nameof(FalhaIntegracao.IdFalhaIntegracao))]
        public int? IdFalhaIntegracaoPai { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de falha.
        /// </summary>
        [PersistenceProperty("Tipo")]
        public string Tipo { get; set; }

        /// <summary>
        /// Obtém ou define a mensagem da falha.
        /// </summary>
        [PersistenceProperty("Mensagem")]
        public string Mensagem { get; set; }

        /// <summary>
        /// Obtém ou define a pilha de chamada da falha.
        /// </summary>
        [PersistenceProperty("PilhaChamada")]
        public string PilhaChamada { get; set; }
    }
}
