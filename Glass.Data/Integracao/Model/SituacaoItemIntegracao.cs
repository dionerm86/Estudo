// <copyright file="SituacaoItemIntegracao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.Data.Model
{
    /// <summary>
    /// Possíveis situações do item da integração.
    /// </summary>
    public enum SituacaoItemIntegracao
    {
        /// <summary>
        /// Indica que o item foi intefrado.
        /// </summary>
        Integrado = 1,

        /// <summary>
        /// Indica que teve uma falha na integração do item.
        /// </summary>
        Falha = 2,

        /// <summary>
        /// Indica que o item ainda está sendo integrado.
        /// </summary>
        Integrando = 3,
    }
}
