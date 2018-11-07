// <copyright file="ParametroOperacaoIntegracaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Integracao.V1.Integradores.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um parâmetro para a tela de listagem.
    /// </summary>
    [DataContract(Name = "ParametroOperacaoIntegracao")]
    public class ParametroOperacaoIntegracaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ParametroOperacaoIntegracaoDto"/>.
        /// </summary>
        /// <param name="parametro">Parâmetro que será usado como base.</param>
        public ParametroOperacaoIntegracaoDto(Glass.Integracao.ParametroOperacaoIntegracao parametro)
        {
            this.Nome = parametro.Nome;
            this.Descricao = parametro.Descricao;
            this.Tipo = parametro.Tipo?.Name;
            this.ValorPadrao = parametro.ValorPadrao;
        }

        /// <summary>
        /// Obtém o nome do parâmetro.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; }

        /// <summary>
        /// Obtém a descrição do parâmetro.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get;}

        /// <summary>
        /// Obtém o tipo do parâmetro.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public string Tipo { get; }

        /// <summary>
        /// Obtém o valor padrão do parâmetro.
        /// </summary>
        [DataMember]
        [JsonProperty("valorPadrao")]
        public object ValorPadrao { get; set; }
    }
}
