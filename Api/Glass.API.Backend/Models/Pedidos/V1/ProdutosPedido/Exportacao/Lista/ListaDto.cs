// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.ProdutosPedido.Exportacao.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um produto para a lista de produtos de pedido de exportação.
    /// </summary>
    [DataContract(Name = "ProdutoPedidoExportacao")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="produtoPedido">A model de produtos de pedido.</param>
        internal ListaDto(Data.Model.ProdutosPedido produtoPedido)
        {
            this.Id = (int)produtoPedido.IdProdPed;
            this.Produto = new ProdutoDto
            {
                Codigo = produtoPedido.CodInterno,
                Descricao = produtoPedido.DescrProduto,
            };

            this.Beneficiamentos = produtoPedido.DescrBeneficiamentos;
            this.Quantidade = (decimal)produtoPedido.Qtde;
            this.Largura = produtoPedido.Largura;
            this.Altura = (decimal)produtoPedido.Altura;
            this.TotalMetroQuadrado = (decimal)produtoPedido.TotM;
            this.Total = produtoPedido.Total;
        }

        /// <summary>
        /// Obtém ou define os dados do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("produto")]
        public ProdutoDto Produto { get; set; }

        /// <summary>
        /// Obtém ou define os beneficiamentos do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("beneficiamentos")]
        public string Beneficiamentos { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade referente ao produto.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidade")]
        public decimal Quantidade { get; set; }

        /// <summary>
        /// Obtém ou define a largura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public int Largura { get; set; }

        /// <summary>
        /// Obtém ou define a altura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public decimal Altura { get; set; }

        /// <summary>
        /// Obtém ou define o total em metros quadrados do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("totalMetroQuadrado")]
        public decimal TotalMetroQuadrado { get; set; }

        /// <summary>
        /// Obtém ou define o valor total do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total { get; set; }
    }
}