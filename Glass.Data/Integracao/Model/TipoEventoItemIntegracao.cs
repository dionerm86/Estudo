// <copyright file="TipoEventoItemIntegracao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.Data.Model
{
    /// <summary>
    /// Possíveis tipo de evento de integração.
    /// </summary>
    public enum TipoEventoItemIntegracao
    {
        /// <summary>
        /// Evento informando que a integração está sendo feita.
        /// </summary>
        Integrando = 1,

        /// <summary>
        /// Evento de integração do item.
        /// </summary>
        ItemIntegrado = 2,

        /// <summary>
        /// Evento informativo.
        /// </summary>
        Informativo,

        /// <summary>
        /// Evento de falha de integração.
        /// </summary>
        Falha,
    }
}
