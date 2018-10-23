// <copyright file="FuncoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Setores.Lista
{
    /// <summary>
    /// Classe que encapsula funções do setor.
    /// </summary>
    [DataContract(Name = "Funcoes")]
    public class FuncoesDto
    {
        /// <summary>
        /// Obtém ou define o tipo do Setor.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public IdNomeDto Tipo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o setor é de corte.
        /// </summary>
        [DataMember]
        [JsonProperty("corte")]
        public bool Corte { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o setor é de forno.
        /// </summary>
        [DataMember]
        [JsonProperty("forno")]
        public bool Forno { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o setor é de laminado.
        /// </summary>
        [DataMember]
        [JsonProperty("laminado")]
        public bool Laminado { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o setor é de entrada de estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("entradaEstoque")]
        public bool EntradaEstoque { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se é gerencidada fornada no setor.
        /// </summary>
        [DataMember]
        [JsonProperty("gerenciarFornada")]
        public bool GerenciarFornada { get; set; }
    }
}
