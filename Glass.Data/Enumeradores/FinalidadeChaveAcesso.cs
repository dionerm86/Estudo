// <copyright file="FinalidadeChaveAcesso.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.Data.Model
{
    /// <summary>
    /// Enumerador de finalidades de chaves de acesso associados ao CT-e.
    /// </summary>
    public enum FinalidadeChaveAcesso
    {
        /// <summary>
        /// Chave de acesso associada a um CT-e comum.
        /// </summary>
        [Description("Chave de Acesso Normal")]
        Normal = 0,

        /// <summary>
        /// Chave de acesso associada a um CT-e já emitido.
        /// Utilizado para CT-e's de Substituição/Anulação.
        /// </summary>
        [Description("CT-e Original")]
        CTeOriginal,

        /// <summary>
        /// Chave de acesso referente a uma NF-e já emitida de anulação de serviço de transporte.
        /// Utilizado para CT-e's de Substituição/Anulação.
        /// </summary>
        [Description("NF-e de Anulação")]
        NFeAnulacao,

        /// <summary>
        /// Chave de acesso referente a um CT-e já emitido de anulação de serviço de transporte.
        /// Utilizado para CT-e's de Substituição/Anulação.
        /// </summary>
        [Description("CTe de Anulação")]
        CTeAnulacao,
    }
}
