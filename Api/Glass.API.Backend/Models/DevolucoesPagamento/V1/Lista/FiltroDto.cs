// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.DevolucoesPagamento;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;

namespace Glass.API.Backend.Models.DevolucoesPagamento.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os itens de filtro para a lista de devoluções de pagamentos.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaDevolucoesPagamento(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do cliente referente a devolução do pagamento.
        /// </summary>
        [JsonProperty("idCliente")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente referente a devolução do pagamento.
        /// </summary>
        [JsonProperty("nomeCliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define o período inicial do cadastro da devolução do pagamento.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define o período final do cadastro da devolução do pagamento.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }
    }
}