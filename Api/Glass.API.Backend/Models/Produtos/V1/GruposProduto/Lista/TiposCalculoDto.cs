// <copyright file="TiposCalculoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Produtos.V1.GruposProduto.Lista
{
    /// <summary>
    /// Classe que encapsula os tipso de cálculo do grupo/subgrupo de produto.
    /// </summary>
    [DataContract(Name = "TiposCalculo")]
    public class TiposCalculoDto
    {
        /// <summary>
        /// Obtém ou define o tipo de cálculo de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("pedido")]
        public IdNomeDto Pedido { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de cálculo de nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("notaFiscal")]
        public IdNomeDto NotaFiscal { get; set; }
    }
}
