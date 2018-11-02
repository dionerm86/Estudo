// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Projetos.ModelosProjeto;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Projetos.V1.ModelosProjeto.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de modelos de projeto.
    /// </summary>
    [DataContract(Name = "Filtro")]
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaModelosProjeto(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o código do modelo de projeto.
        /// </summary>
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do modelo de projeto.
        /// </summary>
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do grupo de modelo de projeto.
        /// </summary>
        [JsonProperty("idGrupoModelo")]
        public int? IdGrupoModelo { get; set; }

        /// <summary>
        /// Obtém ou define o período inicial de cadastro do modelo de projeto.
        /// </summary>
        [JsonProperty("situacao")]
        public Data.Model.ProjetoModelo.SituacaoEnum? Situacao { get; set; }
    }
}
