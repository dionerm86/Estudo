// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Caixas.Geral;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.V1.Geral.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de movimentações do caixa geral.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaCaixaGeral(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da movimentação do caixa geral.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do funcionário.
        /// </summary>
        [JsonProperty("idFuncionario")]
        public int? IdFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial para filtro pela data de cadastro da movimentação.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data inicial para filtro pela data de cadastro da movimentação.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define o valor da movimentação.
        /// </summary>
        [JsonProperty("valor")]
        public decimal? Valor { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se é para buscar apenas movimentações em dinheiro.
        /// </summary>
        [JsonProperty("apenasDinheiro")]
        public bool ApenasDinheiro { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se é para buscar apenas movimentações em cheque.
        /// </summary>
        [JsonProperty("apenasCheque")]
        public bool ApenasCheque { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se é para buscar apenas movimentações entrada, exceto as de estorno.
        /// </summary>
        [JsonProperty("apenasEntradaExcetoEstorno")]
        public bool ApenasEntradaExcetoEstorno { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("tipo")]
        public Data.Model.CaixaGeral.TipoEnum? Tipo { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }
    }
}
