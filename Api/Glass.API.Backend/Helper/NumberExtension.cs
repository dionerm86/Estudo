// <copyright file="NumberExtension.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;

namespace Glass.API.Backend.Helper
{
    /// <summary>
    /// Classe com métodos de extensão para números.
    /// </summary>
    internal static class NumberExtension
    {
        /// <summary>
        /// Arredonda um número para casas decimais.
        /// </summary>
        /// <param name="numero">O número que será arredondado.</param>
        /// <param name="casas">Número de casas decimais para o arredondamento.</param>
        /// <returns>O número arredondado.</returns>
        public static decimal Arredondar(this decimal numero, int casas)
        {
            var ajuste = 1 / (decimal)Math.Pow(10, casas + 1);
            return Math.Round(numero + ajuste, casas);
        }

        /// <summary>
        /// Arredonda um número para casas decimais.
        /// </summary>
        /// <param name="numero">O número que será arredondado.</param>
        /// <param name="casas">Número de casas decimais para o arredondamento.</param>
        /// <returns>O número arredondado.</returns>
        public static float Arredondar(this float numero, int casas)
        {
            var arredondado = Arredondar((decimal)numero, casas);
            return decimal.ToSingle(arredondado);
        }

        /// <summary>
        /// Arredonda um número para casas decimais.
        /// </summary>
        /// <param name="numero">O número que será arredondado.</param>
        /// <param name="casas">Número de casas decimais para o arredondamento.</param>
        /// <returns>O número arredondado.</returns>
        public static double Arredondar(this double numero, int casas)
        {
            var arredondado = Arredondar((decimal)numero, casas);
            return decimal.ToDouble(arredondado);
        }
    }
}
