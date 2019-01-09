// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasPagar.V1.Pagas.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de uma conta paga.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o identificador do plano de contas da conta paga.
        /// </summary>
        [DataMember]
        [JsonProperty("idPlanoConta")]
        public int? IdPlanoConta
        {
            get { return this.ObterValor(c => c.IdPlanoConta); }
            set { this.AdicionarValor(c => c.IdPlanoConta, value); }
        }

        /// <summary>
        /// Obtém ou define a observação da conta paga.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao
        {
            get { return this.ObterValor(c => c.Observacao); }
            set { this.AdicionarValor(c => c.Observacao, value); }
        }
    }
}