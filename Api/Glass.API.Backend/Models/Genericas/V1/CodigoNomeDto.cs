// <copyright file="CodigoNomeDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas.V1
{
    /// <summary>
    /// Classe que encapsula dados básicos (código e nome) de um item.
    /// </summary>
    [DataContract(Name = "CodigoNome")]
    public class CodigoNomeDto
    {
        /// <summary>
        /// Obtém ou define o código do item.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        /// <summary>
        /// Obtém ou define o nome do item.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Tenta realizar a conversão dos parâmetros passados na classe CodigoNomeDto.
        /// </summary>
        /// <param name="codigo">O código do item.</param>
        /// <param name="nome">O nome do item.</param>
        /// <returns>Retorna a classe CodigoNomeDto preenchida com os parâmetros passados.</returns>
        internal static CodigoNomeDto TentarConverter(string codigo, string nome)
        {
            return string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(nome)
                ? null
                : new CodigoNomeDto
                {
                    Codigo = codigo,
                    Nome = nome,
                };
        }
    }
}
