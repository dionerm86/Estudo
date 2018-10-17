// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.Turnos.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um turno.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o nome do turno.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome
        {
            get { return this.ObterValor(c => c.Nome); }
            set { this.AdicionarValor(c => c.Nome, value); }
        }

        /// <summary>
        /// Obtém ou define a sequência do turno.
        /// </summary>
        [DataMember]
        [JsonProperty("sequencia")]
        public Data.Model.TurnoSequencia Sequencia
        {
            get { return this.ObterValor(c => c.Sequencia); }
            set { this.AdicionarValor(c => c.Sequencia, value); }
        }

        /// <summary>
        /// Obtém ou define o início do turno.
        /// </summary>
        [DataMember]
        [JsonProperty("inicio")]
        public string Inicio
        {
            get { return this.ObterValor(c => c.Inicio); }
            set { this.AdicionarValor(c => c.Inicio, value); }
        }

        /// <summary>
        /// Obtém ou define o término do turno.
        /// </summary>
        [DataMember]
        [JsonProperty("termino")]
        public string Termino
        {
            get { return this.ObterValor(c => c.Termino); }
            set { this.AdicionarValor(c => c.Termino, value); }
        }
    }
}
