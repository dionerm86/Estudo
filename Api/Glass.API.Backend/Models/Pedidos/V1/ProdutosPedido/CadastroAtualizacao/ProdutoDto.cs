// <copyright file="ProdutoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.ProdutosPedido.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de produto.
    /// </summary>
    [DataContract(Name = "Produto")]
    public class ProdutoDto : BaseCadastroAtualizacaoDto<ProdutoDto>
    {
        /// <summary>
        /// Obtém ou define o identificador do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id
        {
            get { return this.ObterValor(c => c.Id); }
            set { this.AdicionarValor(c => c.Id, value); }
        }

        /// <summary>
        /// Obtém ou define a espessura do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("espessura")]
        public decimal Espessura
        {
            get { return this.ObterValor(c => c.Espessura); }
            set { this.AdicionarValor(c => c.Espessura, value); }
        }
    }
}
