// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Exportacao;
using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Exportacao.Lista.V1
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de exportações de pedido.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoExportacaoPedidos(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da listagem de exportações de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido da listagem de exportações de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define a situação da listagem de exportações de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public PedidoExportacao.SituacaoExportacaoEnum Situacao { get; set; }

        /// <summary>
        /// Obtém ou define o período do inicio das exportações de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define o período do fim das exportações de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

    }
}
