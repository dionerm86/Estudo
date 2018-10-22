// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.TiposPerda.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de um tipo de perda.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define o nome do tipo de perda.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome
        {
            get { return this.ObterValor(c => c.Nome); }
            set { this.AdicionarValor(c => c.Nome, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se peças com este tipo de perda será exibido no painel de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirNoPainelProducao")]
        public string ExibirNoPainelProducao
        {
            get { return this.ObterValor(c => c.ExibirNoPainelProducao); }
            set { this.AdicionarValor(c => c.ExibirNoPainelProducao, value); }
        }

        /// <summary>
        /// Obtém ou define a situação do tipo de perda.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao
        {
            get { return this.ObterValor(c => c.Situacao); }
            set { this.AdicionarValor(c => c.Situacao, value); }
        }

        /// <summary>
        /// Obtém ou define o identificador do setos do tipo de perda.
        /// </summary>
        [DataMember]
        [JsonProperty("idSetor")]
        public int? IdSetor
        {
            get { return this.ObterValor(c => c.IdSetor); }
            set { this.AdicionarValor(c => c.IdSetor, value); }
        }
    }
}
