// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ConhecimentosTransporte.V1.Veiculos.Proprietarios.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de proprietários de veículos.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="proprietarioVeiculo">O proprietário de veículo que será retornado.</param>
        public ListaDto(Data.Model.Cte.ProprietarioVeiculo proprietarioVeiculo)
        {
            this.Id = (int)proprietarioVeiculo.IdPropVeic;
            this.Nome = proprietarioVeiculo.Nome;
            this.Rntrc = proprietarioVeiculo.RNTRC;
            this.InscricaoEstadual = proprietarioVeiculo.IE;
            this.Uf = proprietarioVeiculo.UF;
            this.Tipo = proprietarioVeiculo.TipoProp.ToString();
        }

        /// <summary>
        /// Obtém ou define o nome do proprietário do veiculo.
        /// </summary>
        [JsonProperty("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Obtém ou define o Registro Nacional de Trasporte Rodoviário de Cargas do proprietário.
        /// </summary>
        [JsonProperty("rntrc")]
        public string Rntrc { get; set; }

        /// <summary>
        /// Obtém ou define a inscrição estadual do proprietário.
        /// </summary>
        [JsonProperty("inscricaoEstadual")]
        public string InscricaoEstadual { get; set; }

        /// <summary>
        /// Obtém ou define a unidade federal do proprietário.
        /// </summary>
        [JsonProperty("uf")]
        public string Uf { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do proprietário.
        /// </summary>
        [JsonProperty("tipo")]
        public string Tipo { get; set; }
    }
}