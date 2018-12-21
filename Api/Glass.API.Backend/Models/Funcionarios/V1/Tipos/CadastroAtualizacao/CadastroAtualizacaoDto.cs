// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.V1.Tipos.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um tipo de funcionário.
    /// </summary>
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define a descrição do tipo de funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao
        {
            get { return this.ObterValor(c => c.Descricao); }
            set { this.AdicionarValor(c => c.Descricao, value); }
        }
    }
}