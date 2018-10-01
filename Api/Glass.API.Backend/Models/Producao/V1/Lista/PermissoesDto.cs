// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Lista
{
    /// <summary>
    /// Classe que encapsula as permissões para uma peça.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se a última leitura pode ser desfeita na peça.
        /// </summary>
        [DataMember]
        [JsonProperty("desfazerLeitura")]
        public bool DesfazerLeitura { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o relatório de pedido pode ser impresso para a peça.
        /// </summary>
        [DataMember]
        [JsonProperty("relatorioPedido")]
        public bool RelatorioPedido { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a peça pode ser parada na produção.
        /// </summary>
        [DataMember]
        [JsonProperty("pararPecaProducao")]
        public bool PararPecaProducao { get; set; }
    }
}
