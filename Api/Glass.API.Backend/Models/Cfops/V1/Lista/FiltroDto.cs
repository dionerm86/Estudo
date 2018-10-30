// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Cfops;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;

namespace Glass.API.Backend.Models.Cfops.V1.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de CFOP's.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaCfops(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o código do CFOP.
        /// </summary>
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do CFOP.
        /// </summary>
        [JsonProperty("descricao")]
        public string Descricao { get; set; }
    }
}
