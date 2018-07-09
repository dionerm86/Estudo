// <copyright file="ConversorDadosAdicionaisParaValidacao.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using System;
using System.Text;

namespace Glass.API.Backend.Helper
{
    /// <summary>
    /// Classe com métodos para conversão dos 'dados adicionais para validação' em uma string utilizável (JSON).
    /// </summary>
    internal class ConversorDadosAdicionaisParaValidacao
    {
        /// <summary>
        /// Converte os dados adicionais em um JSON utilizável, se possível.
        /// </summary>
        /// <param name="dadosAdicionaisValidacao">A string enviada para o serviço.</param>
        /// <returns>Uma string em formato JSON com os dados adicionais para validação.</returns>
        public string ConverterDadosCodificados(string dadosAdicionaisValidacao)
        {
            if (string.IsNullOrWhiteSpace(dadosAdicionaisValidacao))
            {
                return null;
            }

            var base64 = Convert.FromBase64String(dadosAdicionaisValidacao);
            var unicodeString = Encoding.UTF8.GetString(base64);
            var objetoSerializado = Uri.UnescapeDataString(unicodeString);

            return objetoSerializado;
        }
    }
}
