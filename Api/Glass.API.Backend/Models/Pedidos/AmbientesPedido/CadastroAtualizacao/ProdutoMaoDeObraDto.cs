// <copyright file="ProdutoMaoDeObraDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.AmbientesPedido.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de produto mão-de-obra do ambiente.
    /// </summary>
    public class ProdutoMaoDeObraDto : BaseProdutoMaoDeObraDto<ProdutoMaoDeObraDto>
    {
        /// <summary>
        /// Obtém ou define o identificador do processo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idProcesso")]
        public int? IdProcesso
        {
            get { return this.ObterValor(c => c.IdProcesso); }
            set { this.AdicionarValor(c => c.IdProcesso, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador da aplicação do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("idAplicacao")]
        public int? IdAplicacao
        {
            get { return this.ObterValor(c => c.IdAplicacao); }
            set { this.AdicionarValor(c => c.IdAplicacao, value); }
        }
    }
}
