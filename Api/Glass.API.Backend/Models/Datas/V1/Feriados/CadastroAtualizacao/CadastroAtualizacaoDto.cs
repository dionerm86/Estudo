// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Datas.V1.Feriados.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um feriado.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define a descrição do feriado.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao
        {
            get { return this.ObterValor(c => c.Descricao); }
            set { this.AdicionarValor(c => c.Descricao, value); }
        }

        /// <summary>
        /// Obtém ou define o dia do feriado.
        /// </summary>
        [DataMember]
        [JsonProperty("dia")]
        public int Dia
        {
            get { return this.ObterValor(c => c.Dia); }
            set { this.AdicionarValor(c => c.Dia, value); }
        }

        /// <summary>
        /// Obtém ou define o mês do feriado.
        /// </summary>
        [DataMember]
        [JsonProperty("mes")]
        public int Mes
        {
            get { return this.ObterValor(c => c.Mes); }
            set { this.AdicionarValor(c => c.Mes, value); }
        }
    }
}
