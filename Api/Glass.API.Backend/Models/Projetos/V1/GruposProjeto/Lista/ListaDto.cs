// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Projetos.V1.GruposProjeto.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de grupos de projeto.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="grupo">O grupo de projeto que será retornado.</param>
        public ListaDto(GrupoModelo grupo)
        {
            this.Id = (int)grupo.IdGrupoModelo;
            this.Nome = grupo.Descricao;
            this.BoxPadrao = grupo.BoxPadrao;
            this.Esquadria = grupo.Esquadria;
            this.Situacao = new IdNomeDto
            {
                Id = (int)grupo.Situacao,
                Nome = Colosoft.Translator.Translate(grupo.Situacao).Format(),
            };

            this.Permissoes = new PermissoesDto
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.GrupoModelo, grupo.IdGrupoModelo, null),
            };
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o grupo é de box padrão.
        /// </summary>
        [DataMember]
        [JsonProperty("boxPadrao")]
        public bool BoxPadrao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o grupo é de esquadria.
        /// </summary>
        [DataMember]
        [JsonProperty("esquadria")]
        public bool Esquadria { get; set; }

        /// <summary>
        /// Obtém ou define a situação do grupo de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public IdNomeDto Situacao { get; set; }

        /// <summary>
        /// Obtém ou define as permissões do grupo de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
