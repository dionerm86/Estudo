// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Carregamentos.V1.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um carregamento.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o identificador do motorista.
        /// </summary>
        [DataMember]
        [JsonProperty("idMotorista")]
        public int IdMotorista
        {
            get { return this.ObterValor(c => c.IdMotorista); }
            set { this.AdicionarValor(c => c.IdMotorista, value); }
        }

        /// <summary>
        /// Obtém ou define a placa do veículo.
        /// </summary>
        [DataMember]
        [JsonProperty("placa")]
        public string Placa
        {
            get { return this.ObterValor(c => c.Placa); }
            set { this.AdicionarValor(c => c.Placa, value); }
        }

        /// <summary>
        /// Obtém ou define a data de previsão de saída do carregamento.
        /// </summary>
        [DataMember]
        [JsonProperty("dataPrevisaoSaida")]
        public DateTime DataPrevisaoSaida
        {
            get { return this.ObterValor(c => c.DataPrevisaoSaida); }
            set { this.AdicionarValor(c => c.DataPrevisaoSaida, value); }
        }
    }
}
