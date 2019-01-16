// <copyright file="TipoValorTabelaEnum.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System.ComponentModel;

namespace Glass.API.Backend.Models.Produtos.V1.PrecosTabelaCliente
{
    /// <summary>
    /// Enumerador com os tipos de valor de tabela.
    /// </summary>
    public enum TipoValorTabela
    {
        /// <summary>
        /// Padrão.
        /// </summary>
        [Description("Padrão")]
        Padrao = 0,

        /// <summary>
        /// Atacado.
        /// </summary>
        [Description("Atacado")]
        Atacado = 1,

        /// <summary>
        /// Balcão.
        /// </summary>
        [Description("Balcão")]
        Balcao = 2,

        /// <summary>
        /// Obra.
        /// </summary>
        [Description("Obra")]
        Obra = 3,
    }
}