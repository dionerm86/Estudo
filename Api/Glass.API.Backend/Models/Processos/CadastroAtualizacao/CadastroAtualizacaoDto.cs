// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Processos.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um processo de etiqueta.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto
    {
        /// <summary>
        /// Obtém ou define o código do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        /// <summary>
        /// Obtém ou define o código do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define o código do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("idAplicacao")]
        public int? IdAplicacao { get; set; }

        /// <summary>
        /// Obtém ou define o código do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("destacarNaEtiqueta")]
        public bool? DestacarNaEtiqueta { get; set; }

        /// <summary>
        /// Obtém ou define o código do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarFormaInexistente")]
        public bool? GerarFormaInexistente { get; set; }

        /// <summary>
        /// Obtém ou define o código do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarArquivoDeMesa")]
        public bool? GerarArquivoDeMesa { get; set; }

        /// <summary>
        /// Obtém ou define o número de dias úteis para calcular data de entrega do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroDiasUteisDataEntrega")]
        public int? NumeroDiasUteisDataEntrega { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de processo do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoProcesso")]
        public Data.Model.EtiquetaTipoProcesso? TipoProcesso { get; set; }

        /// <summary>
        /// Obtém ou define os tipos de pedido do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("tiposPedidos")]
        public IEnumerable<Data.Model.Pedido.TipoPedidoEnum> TiposPedidos { get; set; }

        /// <summary>
        /// Obtém ou define a situação do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public Situacao? Situacao { get; set; }
    }
}
