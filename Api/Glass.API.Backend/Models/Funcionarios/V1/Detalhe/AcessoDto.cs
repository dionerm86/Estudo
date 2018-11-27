// <copyright file="AcessoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.Detalhe
{
    /// <summary>
    /// Classe que encapsula os dados de acesso do funcionário.
    /// </summary>
    [DataContract(Name = "Acesso")]
    public class AcessoDto : BaseCadastroAtualizacaoDto<AcessoDto>
    {
        /// <summary>
        /// Obtém ou define o login de acesso do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("login")]
        public string Login
        {
            get { return this.ObterValor(c => c.Login); }
            set { this.AdicionarValor(c => c.Login, value); }
        }

        /// <summary>
        /// Obtém ou define a senha de acesso do funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("senha")]
        public string Senha
        {
            get { return this.ObterValor(c => c.Senha); }
            set { this.AdicionarValor(c => c.Senha, value); }
        }
    }
}
