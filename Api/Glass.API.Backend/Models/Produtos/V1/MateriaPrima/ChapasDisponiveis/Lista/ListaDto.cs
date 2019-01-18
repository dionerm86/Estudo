// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.MateriaPrima.ChapasDisponiveis.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de chapas disponíveis.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="chapasDisponiveis">As chapas disponíveis que serão retornadas.</param>
        public ListaDto(Data.RelModel.ChapasDisponiveis chapasDisponiveis)
        {
            this.Cor = chapasDisponiveis.Cor;
            this.Espessura = chapasDisponiveis.Espessura;
            this.Fornecedor = chapasDisponiveis.Fornecedor;
            this.NumeroNotaFiscal = chapasDisponiveis.NumeroNfe;
            this.Lote = chapasDisponiveis.Lote;
            this.Produto = chapasDisponiveis.Produto;
            this.CodigoEtiqueta = chapasDisponiveis.Etiqueta;
        }

        /// <summary>
        /// Obtém ou define a cor da chapa.
        /// </summary>
        [DataMember]
        [JsonProperty("cor")]
        public string Cor { get; set; }

        /// <summary>
        /// Obtém ou define a espessura da chapa.
        /// </summary>
        [DataMember]
        [JsonProperty("espessura")]
        public int Espessura { get; set; }

        /// <summary>
        /// Obtém ou define o fornecedor da chapa.
        /// </summary>
        [DataMember]
        [JsonProperty("fornecedor")]
        public string Fornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o número da nota fiscal da chapa.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroNotaFiscal")]
        public int NumeroNotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define o lota da chapa.
        /// </summary>
        [DataMember]
        [JsonProperty("lote")]
        public string Lote { get; set; }

        /// <summary>
        /// Obtém ou define o produto da chapa.
        /// </summary>
        [DataMember]
        [JsonProperty("produto")]
        public string Produto { get; set; }

        /// <summary>
        /// Obtém ou define o código da etiqueta.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoEtiqueta")]
        public string CodigoEtiqueta { get; set; }
    }
}
