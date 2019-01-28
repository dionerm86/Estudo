// <copyright file="TipoSituacaoProducao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Enumerador com os tipos de situações de produção para um setor.
    /// </summary>
    public enum TipoSituacaoProducao
    {
        /// <summary>
        /// Apenas os produtos do setor atual.
        /// </summary>
        [Description("Apenas os produtos do setor atual")]
        ApenasProdutosSetorAtual,

        /// <summary>
        /// Apenas produtos que ainda não passaram por este setor.
        /// </summary>
        [Description("Apenas produtos que ainda não passaram por este setor")]
        ApenasProdutosQueAindaNaoPassaramPeloSetor,

        /// <summary>
        /// Incluir produtos que já passaram por este setor.
        /// </summary>
        [Description("Incluir produtos que já passaram por este setor")]
        IncluirProdutosQuePassaramPeloSetor,

        /// <summary>
        /// Apenas produtos disponíveis para leitura neste setor no momento.
        /// </summary>
        [Description("Apenas produtos disponíveis para leitura neste setor no momento")]
        ApenasProdutosDisponiveisParaLeituraNoSetor,
    }
}
