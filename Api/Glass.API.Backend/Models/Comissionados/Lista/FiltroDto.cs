// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Comissionados;
using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Comissionados.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de comissionados.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaComissionados(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o nome do comissionado.
        /// </summary>
        [JsonProperty("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Obtém ou define a situação do comissionado.
        /// </summary>
        [JsonProperty("situacao")]
        public Situacao? Situacao { get; set; }
    }
}
