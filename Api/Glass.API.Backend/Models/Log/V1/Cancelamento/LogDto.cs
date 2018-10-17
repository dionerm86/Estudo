// <copyright file="LogDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Log.V1.Cancelamento
{
    /// <summary>
    /// Classe que encapsula os dados de um log de cancelamento.
    /// </summary>
    [DataContract]
    public class LogDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="LogDto"/>.
        /// </summary>
        /// <param name="log">A model de log de cancelamento.</param>
        internal LogDto(LogCancelamento log)
        {
            this.Id = log.IdLogCancelamento;
            this.NumeroEvento = (int)log.NumEvento;
            this.Cancelamento = new DataFuncionarioDto
            {
                Data = log.DataCanc,
                Funcionario = log.NomeFuncCanc,
            };

            this.Campo = log.Campo;
            this.Valor = log.Valor;
            this.Motivo = log.Motivo;
            this.Manual = log.CancelamentoManual;
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
        [JsonProperty("cancelamento")]
        public DataFuncionarioDto Cancelamento { get; set; }

        /// <summary>
        /// Obtém ou define o campo alterado.
        /// </summary>
        [DataMember]
        [JsonProperty("campo")]
        public string Campo { get; set; }

        /// <summary>
        /// Obtém ou define o valor do campo.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public string Valor { get; set; }

        /// <summary>
        /// Obtém ou define a referência do log.
        /// </summary>
        [DataMember]
        [JsonProperty("referencia")]
        public string Referencia { get; set; }

        /// <summary>
        /// Obtém ou define o motivo do cancelamento.
        /// </summary>
        [DataMember]
        [JsonProperty("motivo")]
        public string Motivo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o cancelamento foi manual.
        /// </summary>
        [DataMember]
        [JsonProperty("manual")]
        public bool Manual { get; set; }
    }
}
