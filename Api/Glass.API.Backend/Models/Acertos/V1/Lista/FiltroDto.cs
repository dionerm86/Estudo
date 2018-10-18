// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Acertos;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Acertos.V1.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de acertos.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaAcertos(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do acerto.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da liberação de pedido.
        /// </summary>
        [JsonProperty("idLiberacao")]
        public int? IdLiberacao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente.
        /// </summary>
        [JsonProperty("idCliente")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial para filtro pela data de cadastro do acerto.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial para filtro pela data de cadastro do acerto.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da forma de pagamento.
        /// </summary>
        [JsonProperty("idFormaPagamento")]
        public int? IdFormaPagamento { get; set; }

        /// <summary>
        /// Obtém ou define o número da NF-e.
        /// </summary>
        [JsonProperty("numeroNfe")]
        public int? NumeroNfe { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se é para buscar ou não contas protestadas.
        /// </summary>
        [JsonProperty("protesto")]
        public Protesto Protesto { get; set; }
    }
}
