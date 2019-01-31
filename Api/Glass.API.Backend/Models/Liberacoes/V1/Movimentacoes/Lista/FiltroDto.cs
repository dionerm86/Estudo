// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Liberacoes.Movimentacoes;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;

namespace Glass.API.Backend.Models.Liberacoes.V1.Movimentacoes.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro da lista de movimentações de liberações.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaMovimentacoesLiberacoes(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do cliente.
        /// </summary>
        [JsonProperty("idCliente")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        [JsonProperty("nomeCliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do funcionário.
        /// </summary>
        [JsonProperty("idFuncionario")]
        public int? IdFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial do cadastro.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final do cadastro.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define a situação da liberação.
        /// </summary>
        [JsonProperty("situacao")]
        public int? Situacao { get; set; }
    }
}
