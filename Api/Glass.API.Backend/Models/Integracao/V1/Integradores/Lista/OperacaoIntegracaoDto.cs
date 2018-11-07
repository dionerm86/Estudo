// <copyright file="OperacaoIntegracaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Integracao.V1.Integradores.Lista
{
    /// <summary>
    /// Classe que encapsula os dados da operação de integração para a tela de listagem.
    /// </summary>
    [DataContract(Name = "OperacaoIntegracao")]
    public class OperacaoIntegracaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="OperacaoIntegracaoDto"/>.
        /// </summary>
        /// <param name="operacao">Operação que será encapsulada.</param>
        public OperacaoIntegracaoDto(Glass.Integracao.OperacaoIntegracao operacao)
        {
            this.Nome = operacao.Nome;
            this.Descricao = operacao.Descricao;
            this.Parametros = operacao.Parametros.Select(f => new ParametroOperacaoIntegracaoDto(f)).ToList();
        }

        /// <summary>
        /// Obtém o nome da operação.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; }

        /// <summary>
        /// Obtém a descrição da operação.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; }

        /// <summary>
        /// Obtém os parâmetros da operação.
        /// </summary>
        [DataMember]
        [JsonProperty("parametros")]
        public IEnumerable<ParametroOperacaoIntegracaoDto> Parametros { get; }
    }
}
