// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.ListaVolumes
{
    /// <summary>
    /// Classe que encapsula os dados de permissão do pedido.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se pode gerar volume para o pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarVolume")]
        public bool GerarVolume { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido exibir o relatório de volume do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirRelatorioVolume")]
        public bool ExibirRelatorioVolume { get; set; }
    }
}
