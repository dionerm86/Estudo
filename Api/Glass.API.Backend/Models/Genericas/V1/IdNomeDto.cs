// <copyright file="IdNomeDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas.V1
{
    /// <summary>
    /// Classe que encapsula dados básicos (ID e nome) de um item.
    /// </summary>
    [DataContract(Name = "IdNome")]
    public class IdNomeDto : IdDto
    {
        /// <summary>
        /// Obtém ou define o nome do item.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Tenta realizar a conversão dos parâmetros passados na classe IdNomeDto.
        /// </summary>
        /// <param name="id">O identificador do item.</param>
        /// <param name="nome">O nome do item.</param>
        /// <returns>Retorna a classe IdNomeDto preenchida com os parâmetros passados.</returns>
        internal static IdNomeDto TentarConverter(int? id, string nome)
        {
            return !id.HasValue || string.IsNullOrWhiteSpace(nome)
                ? null
                : new IdNomeDto
                {
                    Id = id.Value,
                    Nome = nome,
                };
        }
    }
}
