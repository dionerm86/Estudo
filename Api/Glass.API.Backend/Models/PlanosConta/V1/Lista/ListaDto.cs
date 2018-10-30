// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.PlanosConta.V1.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de planos de conta.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="planoConta">O plano de conta que será retornado.</param>
        public ListaDto(Financeiro.Negocios.Entidades.PlanoContasPesquisa planoConta)
        {
            this.Id = planoConta.IdConta;
            this.Nome = planoConta.Descricao;
            this.Codigo = planoConta.IdContaGrupo;
            this.ExibirDre = planoConta.ExibirDre;
            this.GrupoConta = new IdNomeDto
            {
                Id = planoConta.IdGrupo,
                Nome = planoConta.Grupo,
            };

            this.Situacao = new IdNomeDto
            {
                Id = (int)planoConta.Situacao,
                Nome = Colosoft.Translator.Translate(planoConta.Situacao).Format(),
            };

            this.Permissoes = new PermissoesDto
            {
                Excluir = !this.VerificarPlanoDeContaInternoSistema(planoConta.IdGrupo),
                EditarApenasExibirDre = this.VerificarPlanoDeContaInternoSistema(planoConta.IdGrupo),
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.PlanoContas, (uint)planoConta.IdConta, null),
            };
        }

        /// <summary>
        /// Obtém ou define o código interno do plano de conta.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public int Codigo { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o plano de conta será exibido no DRE.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirDre")]
        public bool ExibirDre { get; set; }

        /// <summary>
        /// Obtém ou define o grupo do plano de conta.
        /// </summary>
        [DataMember]
        [JsonProperty("grupoConta")]
        public IdNomeDto GrupoConta { get; set; }

        /// <summary>
        /// Obtém ou define a situação do plano de conta.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public IdNomeDto Situacao { get; set; }

        /// <summary>
        /// Obtém ou define as permissões do plano de conta.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        private bool VerificarPlanoDeContaInternoSistema(int idGrupoConta)
        {
            return Glass.Data.Helper.UtilsPlanoConta.GetGruposSistema.Split(',')
                .Select(f => int.Parse(f))
                .Contains(idGrupoConta);
        }
    }
}
