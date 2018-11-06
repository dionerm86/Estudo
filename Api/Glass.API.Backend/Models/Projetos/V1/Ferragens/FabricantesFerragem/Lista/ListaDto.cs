// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Projeto.Negocios.Entidades;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Projetos.V1.Ferragens.FabricantesFerragem.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de fabricantes de ferragem.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="fabricante">O fabricante de ferradem que será retornado.</param>
        public ListaDto(FabricanteFerragemPesquisa fabricante)
        {
            this.Id = fabricante.IdFabricanteFerragem;
            this.Nome = fabricante.Nome;
            this.Site = fabricante.Sitio;
        }

        /// <summary>
        /// Obtém ou define o site do fabricante de ferragem.
        /// </summary>
        [DataMember]
        [JsonProperty("site")]
        public string Site { get; set; }
    }
}
