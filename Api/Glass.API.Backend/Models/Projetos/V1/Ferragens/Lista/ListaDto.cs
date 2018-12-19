// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.Helper;
using Glass.Projeto.Negocios.Entidades;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Projetos.V1.Ferragens.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de uma ferragem para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Ferragem")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="ferragem">A model de ferragem.</param>
        internal ListaDto(FerragemPesquisa ferragem)
        {
            this.Id = ferragem.IdFerragem;
            this.Nome = ferragem.Nome;
            this.Fabricante = ferragem.NomeFabricante;
            this.Situacao = Colosoft.Translator.Translate(ferragem.Situacao).Format();
            this.DataAlteracao = ferragem.DataAlteracao == DateTime.MinValue ? null : (DateTime?)ferragem.DataAlteracao;

            this.Permissoes = new PermissoesDto
            {
                Excluir = UserInfo.GetUserInfo.IsAdminSync,
                AlterarSituacao = UserInfo.GetUserInfo.IsAdminSync,
                LogAlteracoes = Glass.Data.DAL.LogAlteracaoDAO.Instance.TemRegistro(Glass.Data.Model.LogAlteracao.TabelaAlteracao.Ferragem, (uint)this.Id, null),
            };
        }

        /// <summary>
        /// Obtém ou define o código do modelo de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("fabricante")]
        public string Fabricante { get; set; }

        /// <summary>
        /// Obtém ou define a situação do modelo de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a data da última alteração da ferragem.
        /// </summary>
        [DataMember]
        [JsonProperty("dataAlteracao")]
        public DateTime? DataAlteracao { get; set; }

        /// <summary>
        /// Obtém ou define as permissões do modelo de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
