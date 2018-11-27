// <copyright file="SituacaoJobIntegracao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.Integracao
{
    /// <summary>
    /// Possíveis situações do Job de integração.
    /// </summary>
    public enum SituacaoJobIntegracao
    {
        /// <summary>
        /// Identifica que o job ainda não foi iniciado.
        /// </summary>
        [Description("Não iniciado")]
        NaoIniciado,

        /// <summary>
        /// Identifica que o job está executando.
        /// </summary>
        Executando,

        /// <summary>
        /// Identifica que o job foi executado.
        /// </summary>
        Executado,

        /// <summary>
        /// Identifica que o job foi executa com falha.
        /// </summary>
        Falha,
    }
}
