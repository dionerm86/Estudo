// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Liberacoes;
using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System;

namespace Glass.API.Backend.Models.Liberacoes.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de clientes.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaLiberacoes(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da liberação.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o número da NFe.
        /// </summary>
        [JsonProperty("numeroNfe ")]
        public int? NumeroNfe { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do usuário que liberou o pedido.
        /// </summary>
        [JsonProperty("idFuncionario")]
        public int? IdFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente da liberação.
        /// </summary>
        [JsonProperty("idCliente")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente da liberação.
        /// </summary>
        [JsonProperty("nomeCliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define a data de início do cadastro da liberação.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final do cadastro da liberação.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define a situação da liberação.
        /// </summary>
        [JsonProperty("situacao")]
        public int Situacao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define a data de início do cancelamento da liberação.
        /// </summary>
        [JsonProperty("periodoCancelamentoInicio")]
        public DateTime? PeriodoCancelamentoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final do cancelamento da liberação.
        /// </summary>
        [JsonProperty("periodoCancelamentoFim ")]
        public DateTime? PeriodoCancelamentoFim { get; set; }

        /// <summary>
        /// Obtém ou define se é para buscar liberações com ou sem nota fiscal.
        /// </summary>
        [JsonProperty("liberacaoComSemNotaFiscal")]
        public int LiberacaoComSemNotaFiscal { get; set; }
    }
}
