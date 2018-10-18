// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Transportadores;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Transportadores.V1.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de transportadores.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaTransportadores(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do transportador.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o nome do transportador.
        /// </summary>
        [JsonProperty("nome")]
        public string Nome { get; set; }
    }
}
