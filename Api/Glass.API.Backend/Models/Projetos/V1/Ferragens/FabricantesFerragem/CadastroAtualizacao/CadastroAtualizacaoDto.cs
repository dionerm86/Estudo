// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Projetos.V1.Ferragens.FabricantesFerragem.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um fabricante de ferragem.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o nome do fabricante de ferragem.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome
        {
            get { return this.ObterValor(c => c.Nome); }
            set { this.AdicionarValor(c => c.Nome, value); }
        }

        /// <summary>
        /// Obtém ou define o site do fabricante de ferragem.
        /// </summary>
        [DataMember]
        [JsonProperty("site")]
        public string Site
        {
            get { return this.ObterValor(c => c.Site); }
            set { this.AdicionarValor(c => c.Site, value); }
        }
    }
}
