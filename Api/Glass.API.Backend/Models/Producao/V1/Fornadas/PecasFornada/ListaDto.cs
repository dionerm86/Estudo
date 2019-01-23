// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Fornadas.PecasFornada
{
    /// <summary>
    /// Classe que encapsula os dados de um item para a tela de listagem de peças da fornada.
    /// </summary>
    [DataContract(Name = "Pecas")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="produtoPedidoProducao">A model de produtos de pedido de produção preenchida.</param>
        public ListaDto(ProdutoPedidoProducao produtoPedidoProducao)
        {
            this.IdFornada = (int)produtoPedidoProducao.IdProdPedProducao;
            this.Produto = new ProdutoDto
            {
                Codigo = produtoPedidoProducao.CodInterno,
                Nome = produtoPedidoProducao.DescrProduto,
            };

            this.CodigoEtiqueta = produtoPedidoProducao.NumEtiqueta;
            this.Medidas = new MedidasDto
            {
                Altura = (decimal)produtoPedidoProducao.Altura,
                Largura = produtoPedidoProducao.Largura,
            };
        }

        /// <summary>
        /// Obtém ou define o identificador da fornada.
        /// </summary>
        [DataMember]
        [JsonProperty("idFornada")]
        public int? IdFornada { get; set; }

        /// <summary>
        /// Obtém ou define os dados referentes ao produto (peça da fornada).
        /// </summary>
        [DataMember]
        [JsonProperty("produto")]
        public ProdutoDto Produto { get; set; }

        /// <summary>
        /// Obtém ou define o código da etiqueta da peça da fornada.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoEtiqueta")]
        public string CodigoEtiqueta { get; set; }

        /// <summary>
        /// Obtém ou define os dados referentes as medidas da peça da fornada.
        /// </summary>
        [DataMember]
        [JsonProperty("medidas")]
        public MedidasDto Medidas { get; set; }
    }
}