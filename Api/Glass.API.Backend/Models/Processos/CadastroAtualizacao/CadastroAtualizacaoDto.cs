// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.CadastroAtualizacao;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Processos.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um processo de etiqueta.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o código do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public string Codigo
        {
            get { return this["codigo"] as string; }
            set { this["codigo"] = value; }
        }

        /// <summary>
        /// Obtém ou define o código do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao
        {
            get { return this["descricao"] as string; }
            set { this["descricao"] = value; }
        }

        /// <summary>
        /// Obtém ou define o código do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("idAplicacao")]
        public int? IdAplicacao
        {
            get { return this["idAplicacao"] as int?; }
            set { this["idAplicacao"] = value; }
        }

        /// <summary>
        /// Obtém ou define o código do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("destacarNaEtiqueta")]
        public bool DestacarNaEtiqueta
        {
            get { return (this["destacarNaEtiqueta"] as bool?).GetValueOrDefault(); }
            set { this["destacarNaEtiqueta"] = value; }
        }

        /// <summary>
        /// Obtém ou define o código do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarFormaInexistente")]
        public bool GerarFormaInexistente
        {
            get { return (this["gerarFormaInexistente"] as bool?).GetValueOrDefault(); }
            set { this["gerarFormaInexistente"] = value; }
        }

        /// <summary>
        /// Obtém ou define o código do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarArquivoDeMesa")]
        public bool GerarArquivoDeMesa
        {
            get { return (this["gerarArquivoDeMesa"] as bool?).GetValueOrDefault(); }
            set { this["gerarArquivoDeMesa"] = value; }
        }

        /// <summary>
        /// Obtém ou define o número de dias úteis para calcular data de entrega do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroDiasUteisDataEntrega")]
        public int NumeroDiasUteisDataEntrega
        {
            get { return (this["numeroDiasUteisDataEntrega"] as int?).GetValueOrDefault(); }
            set { this["numeroDiasUteisDataEntrega"] = value; }
        }

        /// <summary>
        /// Obtém ou define o tipo de processo do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoProcesso")]
        public Data.Model.EtiquetaTipoProcesso? TipoProcesso
        {
            get { return this["tipoProcesso"] as Data.Model.EtiquetaTipoProcesso?; }
            set { this["tipoProcesso"] = value; }
        }

        /// <summary>
        /// Obtém ou define os tipos de pedido do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("tiposPedidos")]
        public IEnumerable<Data.Model.Pedido.TipoPedidoEnum> TiposPedidos
        {
            get { return (this["tiposPedidos"] as JArray).Values<int>().Cast<Data.Model.Pedido.TipoPedidoEnum>(); }
            set { this["tiposPedidos"] = value; }
        }

        /// <summary>
        /// Obtém ou define a situação do processo de etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public Situacao Situacao
        {
            get { return (this["situacao"] as Situacao?).GetValueOrDefault(); }
            set { this["situacao"] = value; }
        }
    }
}
