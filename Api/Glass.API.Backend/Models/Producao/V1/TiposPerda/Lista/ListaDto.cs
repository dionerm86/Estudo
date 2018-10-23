// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Producao.V1.TiposPerda.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de tipos de perda.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="tipoPerda">O tipo de perda que será retornado.</param>
        public ListaDto(PCP.Negocios.Entidades.TipoPerdaPesquisa tipoPerda)
        {
            this.Id = tipoPerda.IdTipoPerda;
            this.Nome = tipoPerda.Descricao;
            this.ExibirNoPainelDeProducao = tipoPerda.ExibirPainelProducao;
            this.Situacao = new IdNomeDto()
            {
                Id = (int)tipoPerda.Situacao,
                Nome = Colosoft.Translator.Translate(tipoPerda.Situacao).Format(),
            };

            this.Setor = new IdNomeDto()
            {
                Id = tipoPerda.IdSetor,
                Nome = tipoPerda.Setor,
            };

            this.Permissoes = new PermissoesDto
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(
                    Data.Model.LogAlteracao.TabelaAlteracao.TipoPerda,
                    (uint)tipoPerda.IdTipoPerda,
                    null),
            };
        }

        /// <summary>
        /// Obtém ou define um valor que indica se peças com este tipo de perda será exibido no painel de produção.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirNoPainelDeProducao")]
        public bool ExibirNoPainelDeProducao { get; set; }

        /// <summary>
        /// Obtém ou define a situação do tipo de perda.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public IdNomeDto Situacao { get; set; }

        /// <summary>
        /// Obtém ou define o setor do tipo de perda.
        /// </summary>
        [DataMember]
        [JsonProperty("setor")]
        public IdNomeDto Setor { get; set; }

        /// <summary>
        /// Obtém ou define as permissões para o tipo de perda.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
