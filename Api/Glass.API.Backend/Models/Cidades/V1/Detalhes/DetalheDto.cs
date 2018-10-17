// <copyright file="DetalheDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cidades.V1.Detalhes
{
    /// <summary>
    /// Classe que encapsula os dados de cidades.
    /// </summary>
    [DataContract(Name = "Cidade")]
    public class DetalheDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="DetalheDto"/>.
        /// </summary>
        /// <param name="cidade">A model de cidade.</param>
        internal DetalheDto(Cidade cidade)
        {
            this.Id = cidade.IdCidade;
            this.Nome = cidade.NomeCidade;
            this.CodigoIbge = cidade.CodIbgeCidade;
            this.Uf = cidade.NomeUf;
        }

        /// <summary>
        /// Obtém ou define o código IBGE da cidade.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoIbge")]
        public string CodigoIbge { get; set; }

        /// <summary>
        /// Obtém ou define a UF da cidade.
        /// </summary>
        [DataMember]
        [JsonProperty("uf")]
        public string Uf { get; set; }
    }
}
