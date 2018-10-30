// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.PCP.Negocios.Entidades;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Roteiros.ClassificacoesRoteiro.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de classificações de roteiro.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="classificacao">O roteiro que será retornado.</param>
        public ListaDto(ClassificacaoRoteiroProducao classificacao)
        {
            this.Id = classificacao.IdClassificacaoRoteiroProducao;
            this.Nome = classificacao.Descricao;
            this.CapacidadeDiaria = classificacao.CapacidadeDiaria;
            this.MetaDiaria = classificacao.MetaDiaria;
        }

        /// <summary>
        /// Obtém ou define a capacidade diária.
        /// </summary>
        [DataMember]
        [JsonProperty("capacidadeDiaria")]
        public decimal CapacidadeDiaria { get; set; }

        /// <summary>
        /// Obtém ou define a meta diária.
        /// </summary>
        [DataMember]
        [JsonProperty("metaDiaria")]
        public decimal MetaDiaria { get; set; }
    }
}
