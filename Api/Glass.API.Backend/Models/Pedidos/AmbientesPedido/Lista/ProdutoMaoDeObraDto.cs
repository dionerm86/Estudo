// <copyright file="ProdutoMaoDeObraDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Pedidos.AmbientesPedido.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.AmbientesPedido.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de produto mão-de-obra do ambiente.
    /// </summary>
    public class ProdutoMaoDeObraDto : BaseProdutoMaoDeObraDto<ProdutoMaoDeObraDto>
    {
        /// <summary>
        /// Obtém ou define o processo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("processo")]
        public IdCodigoDto Processo
        {
            get { return this.ObterValor(c => c.Processo); }
            set { this.AdicionarValor(c => c.Processo, value); }
        }

        /// <summary>
        /// Obtém ou define a aplicação do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("aplicacao")]
        public IdCodigoDto Aplicacao
        {
            get { return this.ObterValor(c => c.Aplicacao); }
            set { this.AdicionarValor(c => c.Aplicacao, value); }
        }
    }
}
