// <copyright file="CadastroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;


namespace Glass.API.Backend.Models.Clientes.SugestoesCliente.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro da sugestão de cliente.
    /// </summary>
    [DataContract(Name = "Cadastro")]
    public class CadastroDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica a data de cadastro da sugestão.
        /// </summary>
        [DataMember]
        [JsonProperty("data")]
        public DateTime DataCadastro { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica o nome do funcionario que cadastrou a sugestão.
        /// </summary>
        [DataMember]
        [JsonProperty("funcionario")]
        public string NomeFuncionario { get; set; }
    }
}
