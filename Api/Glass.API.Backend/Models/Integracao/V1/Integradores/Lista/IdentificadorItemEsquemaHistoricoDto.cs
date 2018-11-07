// <copyright file="IdentificadorItemHistoricoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Integracao.V1.Integradores.Lista
{
    /// <summary>
    /// Classe que encapsula os dados do identificador do item do esquema do histórico.
    /// </summary>
    [DataContract(Name = "IdentificadorItemEsquemaHistorico")]
    public class IdentificadorItemEsquemaHistoricoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="IdentificadorItemEsquemaHistoricoDto"/>.
        /// </summary>
        /// <param name="identificador">Identificador cujo os dados serão encapsulados.</param>
        public IdentificadorItemEsquemaHistoricoDto(Glass.Integracao.Historico.IdentificadorItemEsquema identificador)
        {
            this.Nome = identificador.Nome;
            this.Tipo = identificador.Tipo?.Name;
        }

        /// <summary>
        /// Obtém o nome do identificador.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; }

        /// <summary>
        /// Obtém o tipo do identificador.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public string Tipo { get; }
    }
}
