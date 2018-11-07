// <copyright file="ItemEsquemaHistoricoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Integracao.V1.Integradores.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um item do esquema do histórico.
    /// </summary>
    [DataContract(Name = "ItemEsquemaHistorico")]
    public class ItemEsquemaHistoricoDto : Genericas.V1.IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ItemEsquemaHistoricoDto"/>.
        /// </summary>
        /// <param name="item">Item cujo os dados serão encapsulados.</param>
        public ItemEsquemaHistoricoDto(Glass.Integracao.Historico.ItemEsquema item)
        {
            this.Id = item.Id;
            this.Nome = item.Nome;
            this.Descricao = item.Descricao;
            this.Identificadores = item.Identificadores.Select(f => new IdentificadorItemEsquemaHistoricoDto(f));
        }

        /// <summary>
        /// Obtém a descrição do item.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; }

        /// <summary>
        /// Obtém os identificadores do item.
        /// </summary>
        [DataMember]
        [JsonProperty("identificadores")]
        public IEnumerable<IdentificadorItemEsquemaHistoricoDto> Identificadores { get; }
    }
}
