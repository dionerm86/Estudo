// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Turnos.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de turnos.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="turno">O turno que será retornado.</param>
        public ListaDto(Global.Negocios.Entidades.Turno turno)
        {
            this.Id = turno.IdTurno;
            this.Nome = turno.Descricao;
            this.Inicio = turno.Inicio;
            this.Termino = turno.Termino;
            this.Sequencia = new IdNomeDto()
            {
                Id = (int)turno.NumSeq,
                Nome = Colosoft.Translator.Translate(turno.NumSeq).Format(),
            };
        }

        /// <summary>
        /// Obtém ou define o início do turno.
        /// </summary>
        [DataMember]
        [JsonProperty("inicio")]
        public string Inicio { get; set; }

        /// <summary>
        /// Obtém ou define o término do turno.
        /// </summary>
        [DataMember]
        [JsonProperty("termino")]
        public string Termino { get; set; }

        /// <summary>
        /// Obtém ou define a sequência do turno.
        /// </summary>
        [DataMember]
        [JsonProperty("sequencia")]
        public IdNomeDto Sequencia { get; set; }
    }
}
