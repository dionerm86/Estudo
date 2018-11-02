// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Projetos.V1.GruposProjeto.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um grupo de projeto.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o nome do grupo de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome
        {
            get { return this.ObterValor(c => c.Nome); }
            set { this.AdicionarValor(c => c.Nome, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o grupo é de box padrão.
        /// </summary>
        [DataMember]
        [JsonProperty("boxPadrao")]
        public bool BoxPadrao
        {
            get { return this.ObterValor(c => c.BoxPadrao); }
            set { this.AdicionarValor(c => c.BoxPadrao, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o grupo é de esquadria.
        /// </summary>
        [DataMember]
        [JsonProperty("esquadria")]
        public bool Esquadria
        {
            get { return this.ObterValor(c => c.Esquadria); }
            set { this.AdicionarValor(c => c.Esquadria, value); }
        }

        /// <summary>
        /// Obtém ou define a situação grupo de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public Situacao Situacao
        {
            get { return this.ObterValor(c => c.Situacao); }
            set { this.AdicionarValor(c => c.Situacao, value); }
        }
    }
}
