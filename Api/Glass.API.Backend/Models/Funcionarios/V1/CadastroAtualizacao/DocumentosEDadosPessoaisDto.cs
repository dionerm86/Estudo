// <copyright file="DocumentosEDadosPessoaisDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe com a implementação dos documentos para a busca de detalhe de funcionário.
    /// </summary>
    [DataContract(Name = "DocumentosEDadosPessoais")]
    public class DocumentosEDadosPessoaisDto : Comuns.DocumentosEDadosPessoaisDto<DocumentosEDadosPessoaisDto>
    {
        /// <summary>
        /// Obtém ou define a foto do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("foto")]
        public string Foto
        {
            get { return this.ObterValor(c => c.Foto); }
            set { this.AdicionarValor(c => c.Foto, value); }
        }
    }
}
