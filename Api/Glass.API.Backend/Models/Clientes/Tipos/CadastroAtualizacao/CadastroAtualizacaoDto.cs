// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Clientes.Tipos.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um tipo de cliente.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define a descrição da cor do vidro.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao
        {
            get { return this.ObterValor(c => c.Descricao); }
            set { this.AdicionarValor(c => c.Descricao, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o tipo cliente cobra área mínima.
        /// </summary>
        [DataMember]
        [JsonProperty("cobrarAreaMinima")]
        public bool CobrarAreaMinima
        {
            get { return this.ObterValor(c => c.CobrarAreaMinima); }
            set { this.AdicionarValor(c => c.CobrarAreaMinima, value); }
        }
    }
}
