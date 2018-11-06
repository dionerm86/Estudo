// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Cfops.NaturezasOperacao.RegrasNaturezaOperacao;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de regras de natureza de operação.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaRegrasNaturezaOperacao(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador da loja.
        /// </summary>
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da natureza de operação.
        /// </summary>
        [JsonProperty("idNaturezaOperacao")]
        public int? IdNaturezaOperacao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do tipo do cliente.
        /// </summary>
        [JsonProperty("idTipoCliente")]
        public int? IdTipoCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do grupo de produto.
        /// </summary>
        [JsonProperty("idGrupoProduto")]
        public int? IdGrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do subgrupo de produto.
        /// </summary>
        [JsonProperty("idSubgrupoProduto")]
        public int? IdSubgrupoProduto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da cor de vidro.
        /// </summary>
        [JsonProperty("idCorVidro")]
        public int? IdCorVidro { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da cor de ferragem.
        /// </summary>
        [JsonProperty("idCorFerragem")]
        public int? IdCorFerragem { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da cor de alumínio.
        /// </summary>
        [JsonProperty("idCorAluminio")]
        public int? IdCorAluminio { get; set; }

        /// <summary>
        /// Obtém ou define a espessura.
        /// </summary>
        [JsonProperty("espessura")]
        public float? Espessura { get; set; }

        /// <summary>
        /// Obtém ou define as ufs de destino.
        /// </summary>
        [JsonProperty("ufsDestino")]
        public IEnumerable<string> UfsDestino { get; set; }
    }
}
