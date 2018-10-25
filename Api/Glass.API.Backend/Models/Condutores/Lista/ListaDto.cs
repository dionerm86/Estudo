// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Condutores.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de processos.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="condutor">O condutor que será retornado.</param>
        public ListaDto(Data.Model.Condutores condutor)
        {
            this.IdCondutor = condutor.IdCondutor;
            this.Nome = condutor.Nome;
            this.Cpf = condutor.Cpf;
        }

        [JsonProperty("id")]
        public int IdCondutor { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("cpf")]
        public string Cpf { get; set; }
    }
}
