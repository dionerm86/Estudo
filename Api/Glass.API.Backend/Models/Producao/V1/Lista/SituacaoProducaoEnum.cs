// <copyright file="SituacaoProducaoEnum.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Enumerador com as situações de produção para uma peça.
    /// </summary>
    public enum SituacaoProducao
    {
        /// <summary>
        /// Apenas as peças pendentes.
        /// </summary>
        [Description("Pendente")]
        Pendente = 3,

        /// <summary>
        /// Apenas as peças prontas.
        /// </summary>
        [Description("Pronta")]
        Pronta = 4,

        /// <summary>
        /// Apenas as peças entregues.
        /// </summary>
        [Description("Entregue")]
        Entregue = 5,

        /// <summary>
        /// Apenas as peças com perda.
        /// </summary>
        [Description("Perda")]
        Perda = 2,
    }
}
