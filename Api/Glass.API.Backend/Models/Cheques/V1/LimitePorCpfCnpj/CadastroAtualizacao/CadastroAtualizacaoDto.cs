// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cheques.V1.LimitePorCpfCnpj.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de atualização do limite de cheques por cpf/cnpj.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o cpf/cnpj do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("cpfCnpj")]
        public string CpfCnpj
        {
            get { return this.ObterValor(c => c.CpfCnpj); }
            set { this.AdicionarValor(c => c.CpfCnpj, value); }
        }

        /// <summary>
        /// Obtém ou define o limite do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("limite")]
        public decimal Limite
        {
            get { return this.ObterValor(c => c.Limite); }
            set { this.AdicionarValor(c => c.Limite, value); }
        }

        /// <summary>
        /// Obtém ou define a observação do cheque.
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