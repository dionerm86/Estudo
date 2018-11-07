// <copyright file="CadastroAtualizacaoDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1.CadastroAtualizacao;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Projetos.V1.MedidasProjeto.CadastroAtualizacao
{
    /// <summary>
    /// Classe que encapsula os dados de cadastro ou atualização de uma medida de projeto.
    /// </summary>
    [DataContract(Name = "CadastroAtualizacao")]
    public class CadastroAtualizacaoDto : BaseCadastroAtualizacaoDto<CadastroAtualizacaoDto>
    {
        /// <summary>
        /// Obtém ou define a descrição da medida de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome
        {
            get { return this.ObterValor(c => c.Nome); }
            set { this.AdicionarValor(c => c.Nome, value); }
        }

        /// <summary>
        /// Obtém ou define um valor padrão para a medida.
        /// </summary>
        [DataMember]
        [JsonProperty("valorPadrao")]
        public int ValorPadrao
        {
            get { return this.ObterValor(c => c.ValorPadrao); }
            set { this.AdicionarValor(c => c.ValorPadrao, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se a medida será exibida apenas em cálculos de projeto de medida exata.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirApenasEmCalculosDeMedidaExata")]
        public bool ExibirApenasEmCalculosDeMedidaExata
        {
            get { return this.ObterValor(c => c.ExibirApenasEmCalculosDeMedidaExata); }
            set { this.AdicionarValor(c => c.ExibirApenasEmCalculosDeMedidaExata, value); }
        }

        /// <summary>
        /// Obtém ou define um valor que indica se a medida será exibida apenas em cálculos de projeto de ferragem e alumínio apenas.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirApenasEmCalculosDeFerragensEAluminios")]
        public bool ExibirApenasEmCalculosDeFerragensEAluminios
        {
            get { return this.ObterValor(c => c.ExibirApenasEmCalculosDeFerragensEAluminios); }
            set { this.AdicionarValor(c => c.ExibirApenasEmCalculosDeFerragensEAluminios, value); }
        }

        /// <summary>
        /// Obtém ou define o grupo de medida de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("idGrupoMedidaProjeto")]
        public int? IdGrupoMedidaProjeto
        {
            get { return this.ObterValor(c => c.IdGrupoMedidaProjeto); }
            set { this.AdicionarValor(c => c.IdGrupoMedidaProjeto, value); }
        }
    }
}
