// <copyright file="LogDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Log.V1.Alteracao
{
    /// <summary>
    /// Classe que encapsula os dados de um log de alteração.
    /// </summary>
    [DataContract]
    public class LogDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="LogDto"/>.
        /// </summary>
        /// <param name="log">A model de log de alteração.</param>
        internal LogDto(LogAlteracao log)
        {
            this.Id = log.IdLog;
            this.NumeroEvento = (int)log.NumEvento;
            this.Alteracao = new DataFuncionarioDto
            {
                Data = log.DataAlt,
                Funcionario = log.NomeFuncAlt,
            };

            this.Campo = log.Campo;
            this.ValorAnterior = log.ValorAnterior;
            this.ValorAtual = log.ValorAtual;
            this.Referencia = log.Referencia;
        }

        /// <summary>
        /// Obtém ou define o identificador do log.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o número do evento de log.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroEvento")]
        public int NumeroEvento { get; set; }

        /// <summary>
        /// Obtém ou define os dados de criação do log.
        /// </summary>
        [DataMember]
        [JsonProperty("alteracao")]
        public DataFuncionarioDto Alteracao { get; set; }

        /// <summary>
        /// Obtém ou define o campo alterado.
        /// </summary>
        [DataMember]
        [JsonProperty("campo")]
        public string Campo { get; set; }

        /// <summary>
        /// Obtém ou define o valor anterior do campo.
        /// </summary>
        [DataMember]
        [JsonProperty("valorAnterior")]
        public string ValorAnterior { get; set; }

        /// <summary>
        /// Obtém ou define o valor atual do campo.
        /// </summary>
        [DataMember]
        [JsonProperty("valorAtual")]
        public string ValorAtual { get; set; }

        /// <summary>
        /// Obtém ou define a referência do log.
        /// </summary>
        [DataMember]
        [JsonProperty("referencia")]
        public string Referencia { get; set; }
    }
}
