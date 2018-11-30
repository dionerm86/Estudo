// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Estoques.V1.Movimentacoes.Reais
{
    /// <summary>
    /// Classe que encapsula os dados de permissão do estoque.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se a movimentação poderá ser excluida.
        /// </summary>
        [DataMember]
        [JsonProperty("excluir")]
        public bool Excluir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o log de alterações poderá ser acessado.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }
    }
}
