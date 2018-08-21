// <copyright file="DadosEntradaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Obras.Filtro
{
    /// <summary>
    /// Classe com os dados de entrada para o método de filtro de obras.
    /// </summary>
    [DataContract(Name = "DadosEntrada")]
    public class DadosEntradaDto
    {
        /// <summary>
        /// Obtém ou define o identificador da obra para pesquisa.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define a descrição da obra para pesquisa.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente da obra para pesquisa.
        /// </summary>
        [DataMember]
        [JsonProperty("idCliente")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define os pedidos que serão ignorados da obra.
        /// </summary>
        [DataMember]
        [JsonProperty("idsPedidosIgnorar")]
        public int[] IdsPedidosIgnorar { get; set; }

        /// <summary>
        /// Obtém ou define a situação da obra para pesquisa.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public Obra.SituacaoObra? Situacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se devem ser buscadas obras que geraram crédito ou foram pagamento antecipado.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoObras")]
        public TipoObrasFiltradas? TipoObras { get; set; }
    }
}
