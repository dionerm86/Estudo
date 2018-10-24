// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.PlanosConta.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um plano de conta.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o nome do plano de conta.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome
        {
            get { return this.ObterValor(c => c.Nome); }
            set { this.AdicionarValor(c => c.Nome, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o plano de conta será exibido no DRE.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirDre")]
        public bool ExibirDre
        {
            get { return this.ObterValor(c => c.ExibirDre); }
            set { this.AdicionarValor(c => c.ExibirDre, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador do grupo do plano de conta.
        /// </summary>
        [DataMember]
        [JsonProperty("idGrupoConta")]
        public int IdGrupoConta
        {
            get { return this.ObterValor(c => c.IdGrupoConta); }
            set { this.AdicionarValor(c => c.IdGrupoConta, value); }
        }

        /// <summary>
        /// Obtém ou define a situação do plano de conta.
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
