// <copyright file="TipoObrasFiltradas.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

namespace Glass.API.Backend.Models.Obras.V1.Filtro
{
    /// <summary>
    /// Enumerador com os tipos de obras que podem ser filtradas.
    /// </summary>
    public enum TipoObrasFiltradas
    {
        /// <summary>
        /// Retornar todas as obras.
        /// </summary>
        Todas,

        /// <summary>
        /// Retornar apenas obras que geraram crédito.
        /// </summary>
        GerarCredito,

        /// <summary>
        /// Retornar apenas obras que são pagamentos antecipados.
        /// </summary>
        PagamentoAntecipado,
    }
}
