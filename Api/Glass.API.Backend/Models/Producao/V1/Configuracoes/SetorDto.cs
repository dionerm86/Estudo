// <copyright file="SetorDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula os dados dos setores.
    /// </summary>
    [DataContract(Name = "Setor")]
    public class SetorDto : IdNomeDto
    {
        /// <summary>
        /// Obtém ou define a ordem de exibição do setor.
        /// </summary>
        [DataMember]
        [JsonProperty("ordem")]
        public int Ordem { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o setor pertence a algum roteiro.
        /// </summary>
        [DataMember]
        [JsonProperty("pertencenteARoteiro")]
        public bool PertencenteARoteiro { get; set; }
    }
}
