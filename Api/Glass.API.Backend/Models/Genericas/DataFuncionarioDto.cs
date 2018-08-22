// <copyright file="DataFuncionarioDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas
{
    /// <summary>
    /// Classe que encapsula data e nome do funcionário que realizou uma operação no sistema.
    /// </summary>
    [DataContract(Name = "DataFuncionario")]
    public class DataFuncionarioDto : DataDto
    {
        /// <summary>
        /// Obtém ou define o nome do funcionário que realizou a operação.
        /// </summary>
        [DataMember]
        [JsonProperty("funcionario")]
        public string Funcionario { get; set; }
    }
}
