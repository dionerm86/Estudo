// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ImpressoesEtiquetas.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão da tela de listagem de impressões de etiquetas.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se a impressão de etiqueta pode ser impressa.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimir")]
        public bool Imprimir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se poderá baixar o arquivo de otimização da impressão de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("baixarArquivoOtimizacao")]
        public bool BaixarArquivoOtimizacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se poderá abrir a impressão de etiqueta no e-Cutter.
        /// </summary>
        [DataMember]
        [JsonProperty("abrirECutter")]
        public bool AbrirECutter { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a impressão de etiqueta poderá ser cancelada.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelar")]
        public bool Cancelar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a impressão de etiqueta possui log de cancelamento.
        /// </summary>
        [DataMember]
        [JsonProperty("logCancelamento")]
        public bool LogCancelamento { get; set; }
    }
}
