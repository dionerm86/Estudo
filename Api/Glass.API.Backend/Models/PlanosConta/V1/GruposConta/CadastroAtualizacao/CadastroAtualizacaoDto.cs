// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.PlanosConta.V1.GruposConta.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um grupo de conta.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o nome do grupo de conta.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome
        {
            get { return this.ObterValor(c => c.Nome); }
            set { this.AdicionarValor(c => c.Nome, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o grupo de conta será exibido no ponto de equilíbrio.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirPontoEquilibrio")]
        public bool ExibirPontoEquilibrio
        {
            get { return this.ObterValor(c => c.ExibirPontoEquilibrio); }
            set { this.AdicionarValor(c => c.ExibirPontoEquilibrio, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador da categoria de conta.
        /// </summary>
        [DataMember]
        [JsonProperty("idCategoriaConta")]
        public int? IdCategoriaConta
        {
            get { return this.ObterValor(c => c.IdCategoriaConta); }
            set { this.AdicionarValor(c => c.IdCategoriaConta, value); }
        }

        /// <summary>
        /// Obtém ou define a situação da categoria de conta.
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
