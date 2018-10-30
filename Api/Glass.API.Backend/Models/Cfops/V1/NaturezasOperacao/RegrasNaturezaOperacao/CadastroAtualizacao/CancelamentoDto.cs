// <copyright file="CancelamentoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cfops.V1.NaturezasOperacao.RegrasNaturezaOperacao.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula dados do cancelamento da regra de natureza de operação.
    /// </summary>
    [DataContract(Name = "Cancelamento")]
    public class CancelamentoDto : BaseCadastroAtualizacaoDto<CancelamentoDto>
    {
        /// <summary>
        /// Obtém ou define o motivo do cancelamento da regra de natureza de operação.
        /// </summary>
        [DataMember]
        [JsonProperty("motivo")]
        public string Motivo { get; set; }
    }
}
