// <copyright file="CoresDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula dados das cores da regra de natureza de operação.
    /// </summary>
    [DataContract(Name = "Cores")]
    public class CoresDto : BaseCadastroAtualizacaoDto<CoresDto>
    {
        /// <summary>
        /// Obtém ou define o cor do vidro da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("vidro")]
        public int? Vidro
        {
            get { return this.ObterValor(c => c.Vidro); }
            set { this.AdicionarValor(c => c.Vidro, value); }
        }

        /// <summary>
        /// Obtém ou define o cor da ferragem da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("ferragem")]
        public int? Ferragem
        {
            get { return this.ObterValor(c => c.Ferragem); }
            set { this.AdicionarValor(c => c.Ferragem, value); }
        }

        /// <summary>
        /// Obtém ou define o cor do alumínio da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("aluminio")]
        public int? Aluminio
        {
            get { return this.ObterValor(c => c.Aluminio); }
            set { this.AdicionarValor(c => c.Aluminio, value); }
        }
    }
}
