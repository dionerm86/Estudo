// <copyright file="DadoRealECalculadoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.ProdutosPedido.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de altura do produto.
    /// </summary>
    [DataContract(Name = "Altura")]
    public class DadoRealECalculadoDto : BaseCadastroAtualizacaoDto<DadoRealECalculadoDto>
    {
        /// <summary>
        /// Obtém ou define a altura real do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("real")]
        public double Real
        {
            get { return this.ObterValor(c => c.Real); }
            set { this.AdicionarValor(c => c.Real, value); }
        }

        /// <summary>
        /// Obtém ou define a altura para cálculo do produto.
        /// </summary>
        [DataMember]
        [JsonProperty("paraCalculo")]
        public double ParaCalculo
        {
            get { return this.ObterValor(c => c.ParaCalculo); }
            set { this.AdicionarValor(c => c.ParaCalculo, value); }
        }
    }
}
