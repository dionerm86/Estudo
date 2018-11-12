// <copyright file="EsquemaHistoricoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Integracao.V1.Integradores.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um esquema do histórico.
    /// </summary>
    [DataContract(Name = "EsquemaHistorico")]
    public class EsquemaHistoricoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="EsquemaHistoricoDto"/>.
        /// </summary>
        /// <param name="esquema">Esquema cujos dados serão </param>
        public EsquemaHistoricoDto(Glass.Integracao.Historico.Esquema esquema)
        {
            this.Id = esquema.Id;
            this.Nome = esquema.Nome;
            this.Descricao = esquema.Descricao;
            this.Itens = esquema.Itens.Select(f => new ItemEsquemaHistoricoDto(f));
        }

        /// <summary>
        /// Obtém o identificador do esquema.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; }

        /// <summary>
        /// Obtém o nome do esquema.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; }

        /// <summary>
        /// Obtém a descrição do esquema.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; }

        /// <summary>
        /// Obtém os itens do esquema.
        /// </summary>
        [DataMember]
        [JsonProperty("itens")]
        public IEnumerable<ItemEsquemaHistoricoDto> Itens { get; }
    }
}
