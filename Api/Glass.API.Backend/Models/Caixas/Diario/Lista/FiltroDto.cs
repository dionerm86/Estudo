// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Caixas.Diario;
using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.Diario.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de movimentações do caixa diário.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaCaixaDiario(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("idLoja")]
        public int IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do funcionário.
        /// </summary>
        [JsonProperty("idFuncionario")]
        public int? IdFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define a data das movimentações.
        /// </summary>
        [JsonProperty("data")]
        public DateTime Data { get; set; }
    }
}
