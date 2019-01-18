// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.RelModel;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.MateriaPrima.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de chapa de matéria prima para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="chapaMateriaPrima">A model de chapa de matéria prima.</param>
        internal ListaDto(PosicaoMateriaPrimaChapa chapaMateriaPrima)
        {
            this.Produto = new ProdutoDto
            {
                Codigo = chapaMateriaPrima.CodInterno,
                Descricao = chapaMateriaPrima.Descricao,
            };

            this.Fornecedor = chapaMateriaPrima.NomeFornecedor;
            this.Altura = chapaMateriaPrima.Altura;
            this.Largura = chapaMateriaPrima.Largura;
            this.Quantidade = (int)chapaMateriaPrima.QtdeChapa;
            this.TotalMetroQuadrado = chapaMateriaPrima.TotalM2Chapa;
        }

        /// <summary>
        /// Obtém ou define os dados referentes ao produto.
        /// </summary>
        [DataMember]
        [JsonProperty("produto")]
        public ProdutoDto Produto { get; set; }

        /// <summary>
        /// Obtém ou define o fornecedor da matéria prima.
        /// </summary>
        [DataMember]
        [JsonProperty("fornecedor")]
        public string Fornecedor { get; set; }

        /// <summary>
        /// Obtém ou define a altura da matéria prima.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public decimal? Altura { get; set; }

        /// <summary>
        /// Obtém ou define a largura da matéria prima.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public decimal? Largura { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade da matéria prima.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidade")]
        public int? Quantidade { get; set; }

        /// <summary>
        /// Obtém ou define o total em metros quadrados da matéria prima.
        /// </summary>
        [DataMember]
        [JsonProperty("totalMetroQuadrado")]
        public decimal? TotalMetroQuadrado { get; set; }
    }
}
