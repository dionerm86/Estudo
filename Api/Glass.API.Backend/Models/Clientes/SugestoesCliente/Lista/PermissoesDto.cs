// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Clientes.SugestoesCliente.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão da sugestão do cliente.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se a sugestão do cliente pode ser inativado.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelar")]
        public bool Cancelar { get; set; }
    }
}
