// <copyright file="CidadeDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Genericas.V1
{
    /// <summary>
    /// Classe que encapsula os dados de cidades para o endereço.
    /// </summary>
    [DataContract(Name = "Cidade")]
    public class CidadeDto : BaseCadastroAtualizacaoDto<CidadeDto>
    {
        /// <summary>
        /// Obtém ou define o nome da cidade.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id
        {
            get { return this.ObterValor(c => c.Id); }
            set { this.AdicionarValor(c => c.Id, value); }
        }

        /// <summary>
        /// Obtém ou define o nome da cidade.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome
        {
            get { return this.ObterValor(c => c.Nome); }
            set { this.AdicionarValor(c => c.Nome, value); }
        }

        /// <summary>
        /// Obtém ou define a UF da cidade.
        /// </summary>
        [DataMember]
        [JsonProperty("uf")]
        public string Uf
        {
            get { return this.ObterValor(c => c.Uf); }
            set { this.AdicionarValor(c => c.Uf, value); }
        }
    }
}
