// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Seguradoras.V1.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de seguradoras.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="seguradora">A seguradora que será retornada.</param>
        public ListaDto(Fiscal.Negocios.Entidades.Seguradora seguradora)
        {
            this.Id = seguradora.IdSeguradora;
            this.Nome = seguradora.NomeSeguradora;
            this.Cnpj = seguradora.CNPJ;
        }

        /// <summary>
        /// Obtém ou define o CNPJ da seguradora.
        /// </summary>
        [DataMember]
        [JsonProperty("cnpj")]
        public string Cnpj { get; set; }
    }
}
