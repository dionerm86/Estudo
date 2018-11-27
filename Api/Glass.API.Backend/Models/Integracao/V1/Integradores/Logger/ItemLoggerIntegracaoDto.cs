// <copyright file="ItemLoggerIntegracaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Integracao.V1.Integradores.Logger
{
    /// <summary>
    /// Classe que encapsula o item o logger de integração.
    /// </summary>
    [DataContract(Name = "LoggerIntegracao")]
    public class ItemLoggerIntegracaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ItemLoggerIntegracaoDto"/>.
        /// </summary>
        /// <param name="item">Item que será encapsulado.</param>
        public ItemLoggerIntegracaoDto(Glass.Integracao.ItemLoggerIntegracao item)
        {
            this.DataCriacao = item.DataCriacao;
            this.Categoria = item.Categoria.ToString();
            this.Prioridade = item.Prioridade.ToString();
            this.Mensagem = item.Mensagem?.Format(System.Globalization.CultureInfo.CurrentCulture);
            this.Erro = item.Erro;
            this.PilhaChamada = item.PilhaChamada;
        }

        /// <summary>
        /// Obtém a data de criação do item.
        /// </summary>
        [DataMember]
        [JsonProperty("dataCriacao")]
        public DateTime DataCriacao { get; }

        /// <summary>
        /// Obtém a cateogria do item.
        /// </summary>
        [DataMember]
        [JsonProperty("categoria")]
        public string Categoria { get; }

        /// <summary>
        /// Obtém a prioridade do item.
        /// </summary>
        [DataMember]
        [JsonProperty("prioridade")]
        public string Prioridade { get; }

        /// <summary>
        /// Obtém a mensagem do item.
        /// </summary>
        [DataMember]
        [JsonProperty("mensagem")]
        public string Mensagem { get; }

        /// <summary>
        /// Obtém o erro do item.
        /// </summary>
        [DataMember]
        [JsonProperty("erro")]
        public string Erro { get; }

        /// <summary>
        /// Obtém a pilha de chamada.
        /// </summary>
        [DataMember]
        [JsonProperty("pilhaChamada")]
        public string PilhaChamada { get; }
    }
}
