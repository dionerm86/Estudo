// <copyright file="BaseProdutoMaoDeObraDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.AmbientesPedido.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de produto mão-de-obra do ambiente.
    /// </summary>
    /// <typeparam name="T">O tipo da classe que representa o produto mão-de-obra.</typeparam>
    public abstract class BaseProdutoMaoDeObraDto<T> : BaseCadastroAtualizacaoDto<T>
        where T : BaseProdutoMaoDeObraDto<T>
    {
        /// <summary>
        /// Obtém ou define o identificador do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int? Id
        {
            get { return this.ObterValor(c => c.Id); }
            set { this.AdicionarValor(c => c.Id, value); }
        }

        /// <summary>
        /// Obtém ou define o código interno do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoInterno")]
        public string CodigoInterno
        {
            get { return this.ObterValor(c => c.CodigoInterno); }
            set { this.AdicionarValor(c => c.CodigoInterno, value); }
        }

        /// <summary>
        /// Obtém ou define a quantidade de produtos.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidade")]
        public decimal? Quantidade
        {
            get { return this.ObterValor(c => c.Quantidade); }
            set { this.AdicionarValor(c => c.Quantidade, value); }
        }

        /// <summary>
        /// Obtém ou define a altura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("altura")]
        public decimal? Altura
        {
            get { return this.ObterValor(c => c.Altura); }
            set { this.AdicionarValor(c => c.Altura, value); }
        }

        /// <summary>
        /// Obtém ou define a largura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("largura")]
        public int? Largura
        {
            get { return this.ObterValor(c => c.Largura); }
            set { this.AdicionarValor(c => c.Largura, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o ambiente é redondo.
        /// </summary>
        [DataMember]
        [JsonProperty("redondo")]
        public bool Redondo
        {
            get { return this.ObterValor(c => c.Redondo); }
            set { this.AdicionarValor(c => c.Redondo, value); }
        }
    }
}
