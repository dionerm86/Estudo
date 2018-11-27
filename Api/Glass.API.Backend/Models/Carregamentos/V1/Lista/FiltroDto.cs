// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Carregamentos;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;

namespace Glass.API.Backend.Models.Carregamentos.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de carregamentos.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaCarregamentos(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do carregamento.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da rota.
        /// </summary>
        [JsonProperty("idRota")]
        public int? IdRota { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do motorista.
        /// </summary>
        [JsonProperty("idMotorista")]
        public int? IdMotorista { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da ordem de carga.
        /// </summary>
        [JsonProperty("idOrdemCarga")]
        public int? IdOrdemCarga { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define a placa do veículo.
        /// </summary>
        [JsonProperty("placa")]
        public string Placa { get; set; }

        /// <summary>
        /// Obtém ou define a situação do carregamento.
        /// </summary>
        [JsonProperty("situacaoCarregamento")]
        public Data.Model.Carregamento.SituacaoCarregamentoEnum? SituacaoCarregamento { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial para o filtro de período da previsão de saída.
        /// </summary>
        [JsonProperty("periodoPrevisaoSaidaInicio")]
        public DateTime? PeriodoPrevisaoSaidaInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final para o filtro de período da previsão de saída.
        /// </summary>
        [JsonProperty("periodoPrevisaoSaidaFim")]
        public DateTime? PeriodoPrevisaoSaidaFim { get; set; }
    }
}
