// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Cfops.NaturezasOperacao;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro da lista de natureza de operação.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaNaturezasOperacao(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do CFOP.
        /// </summary>
        [JsonProperty("idCfop")]
        public int? IdCfop { get; set; }
    }
}
