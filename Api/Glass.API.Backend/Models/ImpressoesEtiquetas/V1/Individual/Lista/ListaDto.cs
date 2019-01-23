// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ImpressoesEtiquetas.V1.Individual.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de uma impressão individual de etiquetas.
    /// </summary>
    [DataContract(Name = "ImpressaoEtiquetaIndividual")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="produtoImpressaoIndividualEtiqueta">A model de produto que será utilizada na listagem de impressão individual de etiquetas.</param>
        internal ListaDto(ProdutosPedidoEspelho produtoImpressaoIndividualEtiqueta)
        {
            this.IdPedido = (int)produtoImpressaoIndividualEtiqueta.IdPedido;
            this.IdAmbientePedido = (int)produtoImpressaoIndividualEtiqueta.IdAmbientePedido;
            this.IdProdutoPedido = (int)produtoImpressaoIndividualEtiqueta.IdProdPed;
            this.IdProdutoNotaFiscal = produtoImpressaoIndividualEtiqueta.IdProdNf;
            this.NumeroNotaFiscal = produtoImpressaoIndividualEtiqueta.NumeroNFe;
            this.Produto = produtoImpressaoIndividualEtiqueta.DescrProduto;
            this.Processo = produtoImpressaoIndividualEtiqueta.CodProcesso;
            this.Aplicacao = produtoImpressaoIndividualEtiqueta.CodAplicacao;
            this.Medidas = new MedidasDto
            {
                Altura = (decimal)produtoImpressaoIndividualEtiqueta.Altura,
                Largura = produtoImpressaoIndividualEtiqueta.Largura,
            };

            this.Quantidade = new QuantidadeDto
            {
                Total = (decimal)produtoImpressaoIndividualEtiqueta.Qtde,
                Impressa = produtoImpressaoIndividualEtiqueta.QtdImpresso,
            };

            this.CorLinha = this.ObterCorLinha(produtoImpressaoIndividualEtiqueta);
        }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do ambiente do pedido.
        /// </summary>
        [JsonProperty("idAmbientePedido")]
        public int? IdAmbientePedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do produto do pedido.
        /// </summary>
        [JsonProperty("idProdutoPedido")]
        public int? IdProdutoPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do produto da nota fiscal.
        /// </summary>
        [JsonProperty("idProdutoNotaFiscal")]
        public int? IdProdutoNotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define o número da nota fiscal.
        /// </summary>
        [JsonProperty("numeroNotaFiscal")]
        public int? NumeroNotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [JsonProperty("produto")]
        public string Produto { get; set; }

        /// <summary>
        /// Obtém ou define o processo associado ao produto.
        /// </summary>
        [JsonProperty("processo")]
        public string Processo { get; set; }

        /// <summary>
        /// Obtém ou define a aplicação associada ao produto.
        /// </summary>
        [DataMember]
        [JsonProperty("aplicacao")]
        public string Aplicacao { get; set; }

        /// <summary>
        /// Obtém ou define os dados referentes as medidas.
        /// </summary>
        [JsonProperty("medidas")]
        public MedidasDto Medidas { get; set; }

        /// <summary>
        /// Obtém ou define os dados referentes as quantidades.
        /// </summary>
        [JsonProperty("quantidade")]
        public QuantidadeDto Quantidade { get; set; }

        /// <summary>
        /// Obtém ou define a cor da linha referente a impressão individual de etiqueta atual.
        /// </summary>
        [JsonProperty("corLinha")]
        public string CorLinha { get; set; }

        private string ObterCorLinha(ProdutosPedidoEspelho produtoImpressaoIndividualEtiqueta)
        {
            using (var sessao = new GDATransaction())
            {
                bool pedidoDeReposicao = PedidoDAO.Instance.IsPedidoReposicao(
                    sessao,
                    produtoImpressaoIndividualEtiqueta.IdPedido.ToString());

                return pedidoDeReposicao ? "Red" : string.Empty;
            }
        }
    }
}