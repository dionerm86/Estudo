// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Sinais;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Sinais.V1.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de sinais.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaSinais(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do sinal.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente.
        /// </summary>
        [JsonProperty("idCliente")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial para filtro pela data de cadastro do sinal.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial para filtro pela data de cadastro do sinal.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o sinal é do tipo pagamento antecipado de pedido.
        /// </summary>
        [JsonProperty("pagamentoAntecipado")]
        public bool PagamentoAntecipado { get; set; }

        /// <summary>
        /// Obtém ou define a ordenação a ser usada na listagem.
        /// </summary>
        [JsonProperty("ordenacaoFiltro")]
        public int OrdenacaoFiltro { get; set; }
    }
}
