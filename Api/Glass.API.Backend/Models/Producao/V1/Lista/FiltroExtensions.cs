// <copyright file="FiltroExtensions.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Classe com métodos de extensão para o filtro da tela.
    /// </summary>
    internal static class FiltroExtensions
    {
        /// <summary>
        /// Recupera uma string com os inteiros separados por vírgula, caso a lista esteja preenchida.
        /// </summary>
        /// <param name="lista">A lista com os números.</param>
        /// <returns>Uma string com os números separados por vírgula.</returns>
        public static string ObterComoString(this IEnumerable<int> lista)
        {
            return lista != null && lista.Any()
                ? string.Join(",", lista)
                : null;
        }

        /// <summary>
        /// Retorna uma data formatada para uso nos métodos de busca.
        /// </summary>
        /// <param name="data">A data que será formatada.</param>
        /// <returns>Uma string com a data formatada.</returns>
        public static string FormatarData(this DateTime? data)
        {
            return data.HasValue
                ? data.Value.ToString("dd/MM/yyyy")
                : null;
        }
    }
}
