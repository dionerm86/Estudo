// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Volumes;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Volumes.V1.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de volumes.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaVolumes(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do volume.
        /// </summary>
        [JsonProperty("idVolume")]
        public int? IdVolume { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define as situações do volume.
        /// </summary>
        [JsonProperty("situacoesVolume")]
        public IEnumerable<Data.Model.Volume.SituacaoVolume> SituacoesVolume { get; set; }
    }
}
