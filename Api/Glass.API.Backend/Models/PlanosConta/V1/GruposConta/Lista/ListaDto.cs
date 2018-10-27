// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.PlanosConta.V1.GruposConta.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de grupos de conta.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="grupoConta">O grupo de conta que será retornado.</param>
        public ListaDto(Financeiro.Negocios.Entidades.GrupoContaPesquisa grupoConta)
        {
            this.Id = grupoConta.IdGrupo;
            this.Nome = grupoConta.Descricao;
            this.ExibirPontoEquilibrio = grupoConta.PontoEquilibrio;
            this.CategoriaConta = new IdNomeDto
            {
                Id = grupoConta.IdCategoriaConta,
                Nome = grupoConta.Categoria,
            };

            this.Situacao = new IdNomeDto
            {
                Id = (int)grupoConta.Situacao,
                Nome = Colosoft.Translator.Translate(grupoConta.Situacao).Format(),
            };

            this.Permissoes = new PermissoesDto
            {
                Excluir = !this.VerificarGrupoDeContaInternoSistema(grupoConta.IdGrupo),
                EditarApenasExibirPontoEquilibrio = this.VerificarGrupoDeContaInternoSistema(grupoConta.IdGrupo),
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.GrupoConta, (uint)grupoConta.IdGrupo, null),
            };
        }

        /// <summary>
        /// Obtém ou define um valor que indica se o grupo de conta será exibido no ponto de equilíbrio.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirPontoEquilibrio")]
        public bool ExibirPontoEquilibrio { get; set; }

        /// <summary>
        /// Obtém ou define a categoria do grupo de conta.
        /// </summary>
        [DataMember]
        [JsonProperty("categoriaConta")]
        public IdNomeDto CategoriaConta { get; set; }

        /// <summary>
        /// Obtém ou define a situação do grupo de conta.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public IdNomeDto Situacao { get; set; }

        /// <summary>
        /// Obtém ou define as permissões do grupo de conta.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        private bool VerificarGrupoDeContaInternoSistema(int idGrupoConta)
        {
            return Glass.Data.Helper.UtilsPlanoConta.GetGruposSistema.Split(',')
                .Select(f => int.Parse(f))
                .Contains(idGrupoConta);
        }
    }
}
