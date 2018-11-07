// <copyright file="LoggerIntegracaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Integracao.V1.Integradores.Logger
{
    /// <summary>
    /// Classe que encapsula os dados do logger de integração.
    /// </summary>
    [DataContract(Name = "LoggerIntegracao")]
    public class LoggerIntegracaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="LoggerIntegracaoDto"/>.
        /// </summary>
        /// <param name="logger">Logger que será encapsulado.</param>
        public LoggerIntegracaoDto(Glass.Integracao.LoggerIntegracao logger)
        {
            this.Itens = logger.Itens.Select(f => new ItemLoggerIntegracaoDto(f));
        }

        /// <summary>
        /// Obtém os itens do logger.
        /// </summary>
        [DataMember]
        [JsonProperty("itens")]
        public IEnumerable<ItemLoggerIntegracaoDto> Itens { get; }
    }
}
