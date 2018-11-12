// <copyright file="TipoItemHistorico.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.Integracao.Historico
{
    /// <summary>
    /// Possíveis tipos de item de histórico.
    /// </summary>
    public enum TipoItemHistorico
    {
        /// <summary>
        /// Evento informando que a integração está sendo feita.
        /// </summary>
        Integrando = 1,

        /// <summary>
        /// Indica que o item foi integrado.
        /// </summary>
        Integrado = 2,

        /// <summary>
        /// Indica um item informativo.
        /// </summary>
        Informativo = 3,

        /// <summary>
        /// Indica um item de falha.
        /// </summary>
        Falha = 4,

        /// <summary>
        /// Indica um item de atenção.
        /// </summary>
        [Description("Atenção")]
        Atencao = 5,
    }
}
