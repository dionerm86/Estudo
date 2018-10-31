// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Funcionarios;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;

namespace Glass.API.Backend.Models.Funcionarios.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de funcionários.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaFuncionarios(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o nome do funcionario.
        /// </summary>
        [JsonProperty("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Obtém ou define a situação do funcionario.
        /// </summary>
        [JsonProperty("situacao")]
        public Situacao? Situacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se apenas funcionários registrados devem ser retornados.
        /// </summary>
        [JsonProperty("apenasRegistrados")]
        public bool ApenasRegistrados { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do tipo de funcionario.
        /// </summary>
        [JsonProperty("idTipoFuncionario")]
        public int? IdTipoFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do setor do funcionário.
        /// </summary>
        [JsonProperty("idSetor")]
        public int? IdSetor { get; set; }

        /// <summary>
        /// Obtém ou define a data de nascimento inicial do funcionario.
        /// </summary>
        [JsonProperty("periodoDataNascimentoInicio")]
        public DateTime? PeriodoDataNascimentoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data de nascimento final do funcionario.
        /// </summary>
        [JsonProperty("periodoDataNascimentoFim")]
        public DateTime? PeriodoDataNascimentoFim { get; set; }
    }
}
