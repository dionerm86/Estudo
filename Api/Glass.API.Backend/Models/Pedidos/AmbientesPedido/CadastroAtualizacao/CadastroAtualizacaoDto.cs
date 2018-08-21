// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.AmbientesPedido.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados para cadastro ou atualização de ambiente.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto
    {
        /// <summary>
        /// Obtém ou define o nome do ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Obtém ou define os dados do produto mão-de-obra, se existir.
        /// </summary>
        [DataMember]
        [JsonProperty("produtoMaoDeObra")]
        public ProdutoMaoDeObraDto ProdutoMaoDeObra { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define o acréscimo aplicado ao ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("acrescimo")]
        public AcrescimoDescontoDto Acrescimo { get; set; }

        /// <summary>
        /// Obtém ou define o desconto aplicado ao ambiente.
        /// </summary>
        [DataMember]
        [JsonProperty("desconto")]
        public AcrescimoDescontoDto Desconto { get; set; }
    }
}
