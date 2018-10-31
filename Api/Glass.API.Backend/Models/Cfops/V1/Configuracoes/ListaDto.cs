// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Configuracoes;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.Configuracoes
{
    /// <summary>
    /// Classe que encapsula as configurações para a tela de listagem de CFOP's.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        internal ListaDto()
        {
            this.ControlarEstoqueVidrosClientes = EstoqueConfig.ControlarEstoqueVidrosClientes;
        }

        /// <summary>
        /// Obtém ou define um valor que indica se a empresa controla o estoque de vidros dos clientes.
        /// </summary>
        [DataMember]
        [JsonProperty("controlarEstoqueVidrosClientes")]
        public bool ControlarEstoqueVidrosClientes { get; set; }
    }
}
