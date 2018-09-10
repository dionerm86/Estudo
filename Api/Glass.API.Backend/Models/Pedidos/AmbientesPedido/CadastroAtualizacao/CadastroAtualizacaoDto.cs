// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Genericas.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.AmbientesPedido.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados para cadastro ou atualização de ambiente.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto<T> : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto<T>>
        where T : BaseProdutoMaoDeObraDto<T>
    {
        /// <summary>
        /// Obtém ou define o nome do ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome
        {
            get { return this.ObterValor(c => c.Nome); }
            set { this.AdicionarValor(c => c.Nome, value); }
        }

        /// <summary>
        /// Obtém ou define os dados do produto mão-de-obra, se existir.
        /// </summary>
        [DataMember]
        [JsonProperty("produtoMaoDeObra")]
        public T ProdutoMaoDeObra
        {
            get { return this.ObterValor(c => c.ProdutoMaoDeObra); }
            set { this.AdicionarValor(c => c.ProdutoMaoDeObra, value); }
        }

        /// <summary>
        /// Obtém ou define a descrição do ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao
        {
            get { return this.ObterValor(c => c.Descricao); }
            set { this.AdicionarValor(c => c.Descricao, value); }
        }

        /// <summary>
        /// Obtém ou define o acréscimo aplicado ao ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("acrescimo")]
        public AcrescimoDescontoDto Acrescimo
        {
            get { return this.ObterValor(c => c.Acrescimo); }
            set { this.AdicionarValor(c => c.Acrescimo, value); }
        }

        /// <summary>
        /// Obtém ou define o desconto aplicado ao ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("desconto")]
        public AcrescimoDescontoDto Desconto
        {
            get { return this.ObterValor(c => c.Desconto); }
            set { this.AdicionarValor(c => c.Desconto, value); }
        }
    }
}
