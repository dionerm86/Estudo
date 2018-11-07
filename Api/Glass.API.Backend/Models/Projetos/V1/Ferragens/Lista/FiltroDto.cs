// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Projetos.Ferragens;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Projetos.V1.Ferragens.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de ferragens.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaFerragens(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o código da ferragem.
        /// </summary>
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        /// <summary>
        /// Obtém ou define o nome da ferragem.
        /// </summary>
        [JsonProperty("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do fabricante da ferragem.
        /// </summary>
        [JsonProperty("idFabricante")]
        public int? IdFabricante { get; set; }
    }
}
