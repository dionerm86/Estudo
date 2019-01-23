// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.MateriaPrima.Posicao.Lista
{
    /// <summary>
    /// Classe que encapsula os dados das permissões concebidas a uma posição de matéria prima.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se serão exibidas as chapas para a posição de matéria prima.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirChapas")]
        public bool ExibirChapas { get; set; }
    }
}
