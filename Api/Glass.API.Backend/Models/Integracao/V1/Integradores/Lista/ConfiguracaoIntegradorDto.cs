// <copyright file="ConfiguracaoIntegradorDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Integracao.V1.Integradores.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de uma configuraçãoo do integrador para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Integrador")]
    public class ConfiguracaoIntegradorDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ConfiguracaoIntegradorDto"/>.
        /// </summary>
        /// <param name="nome">Nome da configuração.</param>
        /// <param name="valor">Valor da configuração.</param>
        public ConfiguracaoIntegradorDto(string nome, object valor)
        {
            this.Nome = nome;

            var convertible = valor as IConvertible;

            if (convertible != null)
            {
                this.Valor = convertible.ToString(System.Globalization.CultureInfo.CurrentCulture);
            }
            else
            {
                this.Valor = valor?.ToString();
            }
        }

        /// <summary>
        /// Obtém o nome da configuração.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; }

        /// <summary>
        /// Obtém o valor do parâmetro.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public string Valor { get; }
    }
}
