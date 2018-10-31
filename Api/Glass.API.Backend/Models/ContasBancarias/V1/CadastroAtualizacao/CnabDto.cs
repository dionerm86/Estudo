// <copyright file="CnabDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasBancarias.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula dados do CNAB.
    /// </summary>
    [DataContract(Name = "Cnab")]
    public class CnabDto : BaseCadastroAtualizacaoDto<CnabDto>
    {
        /// <summary>
        /// Obtém ou define o código do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoCliente")]
        public string CodigoCliente
        {
            get { return this.ObterValor(c => c.CodigoCliente); }
            set { this.AdicionarValor(c => c.CodigoCliente, value); }
        }

        /// <summary>
        /// Obtém ou define o posto.
        /// </summary>
        [DataMember]
        [JsonProperty("posto")]
        public int? Posto
        {
            get { return this.ObterValor(c => c.Posto); }
            set { this.AdicionarValor(c => c.Posto, value); }
        }
    }
}
