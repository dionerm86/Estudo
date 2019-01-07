// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Glass.API.Backend.Models.Genericas.V1;

namespace Glass.API.Backend.Models.Produtos.V1.PrecosTabelaCliente.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de preço de tabela por cliente para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Produto")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="produto">A model de produtos.</param>
        internal ListaDto(Produto produto)
        {
            this.Produto = produto.CodInterno;
            this.Grupo = produto.DescrGrupo;
            this.Subgrupo = produto.DescrSubgrupo;
            this.Beneficiamentos = produto.DescricaoProdutoBeneficiamento;
            this.TipoValorTabela = produto.TituloValorTabelaUtilizado;
            this.ValorOriginal = produto.ValorOriginalUtilizado;
            this.ValorTabela = produto.ValorTabelaUtilizado;
            this.FatorDescontoAcrescimo = (decimal)produto.PercDescAcrescimo;
            this.Altura = produto.Altura;
            this.Largura = produto.Largura;
        }

        /// <summary>
        /// Obtém ou define a descrição do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("produto")]
        public string Produto { get; set; }

        /// <summary>
        /// Obtém ou define o grupo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("grupo")]
        public string Grupo { get; set; }

        /// <summary>
        /// Obtém ou define o subgrupo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("subgrupo")]
        public string Subgrupo { get; set; }

        /// <summary>
        /// Obtém ou define os beneficiamentos associados ao produto.
        /// </summary>
        [DataMember]
        [JsonProperty("beneficiamentos")]
        public string Beneficiamentos { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de valor de tabela.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoValorTabela")]
        public string TipoValorTabela { get; set; }

        /// <summary>
        /// Obtém ou define o valor original utilizado.
        /// </summary>
        [DataMember]
        [JsonProperty("valorOriginal")]
        public decimal ValorOriginal { get; set; }

        /// <summary>
        /// Obtém ou define o valor de tabela utilizado.
        /// </summary>
        [DataMember]
        [JsonProperty("valorTabela")]
        public decimal ValorTabela { get; set; }

        /// <summary>
        /// Obtém ou define o valor de desconto/acrescimo.
        /// </summary>
        [DataMember]
        [JsonProperty("fatorDescontoAcrescimo")]
        public decimal FatorDescontoAcrescimo { get; set; }

        /// <summary>
        /// Obtém ou define a altura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public int? Altura { get; set; }

        /// <summary>
        /// Obtém ou define a largura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public int? Largura { get; set; }
    }
}