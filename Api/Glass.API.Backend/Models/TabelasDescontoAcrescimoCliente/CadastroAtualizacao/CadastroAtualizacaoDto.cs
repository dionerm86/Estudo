// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.TabelasDescontoAcrescimoCliente.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de uma tabela de desconto/acréscimo de cliente.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define a descrição da tabela de desconto/acréscimo.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome
        {
            get { return this.ObterValor(c => c.Nome); }
            set { this.AdicionarValor(c => c.Nome, value); }
        }
    }
}
