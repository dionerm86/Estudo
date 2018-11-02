// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Projetos.V1.MedidasProjeto.Lista
{
    /// <summary>
    /// Classe que encapsula um item para a lista de medidas de projeto.
    /// </summary>
    [DataContract(Name = "Lista")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="medida">A medida de projeto que será retornada.</param>
        public ListaDto(MedidaProjeto medida)
        {
            this.Id = (int)medida.IdMedidaProjeto;
            this.Nome = medida.Descricao;
            this.ValorPadrao = medida.ValorPadrao;
            this.ExibirApenasEmCalculosDeMedidaExata = medida.ExibirMedidaExata;
            this.ExibirApenasEmCalculosDeFerragensEAluminios = medida.ExibirApenasFerragensAluminios;
            this.GrupoMedidaProjeto = new IdNomeDto
            {
                Id = (int)medida.IdGrupoMedProj,
                Nome = medida.DescrGrupo,
            };

            this.Permissoes = new PermissoesDto
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.MedidaProjeto, medida.IdMedidaProjeto, null),
            };
        }

        /// <summary>
        /// Obtém ou define um valor padrão para a medida.
        /// </summary>
        [DataMember]
        [JsonProperty("valorPadrao")]
        public int ValorPadrao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a medida será exibida apenas em cálculos de projeto de medida exata.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirApenasEmCalculosDeMedidaExata")]
        public bool ExibirApenasEmCalculosDeMedidaExata { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a medida será exibida apenas em cálculos de projeto de ferragem e alumínio apenas.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirApenasEmCalculosDeFerragensEAluminios")]
        public bool ExibirApenasEmCalculosDeFerragensEAluminios { get; set; }

        /// <summary>
        /// Obtém ou define o grupo de medida de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("grupoMedidaProjeto")]
        public IdNomeDto GrupoMedidaProjeto { get; set; }

        /// <summary>
        /// Obtém ou define as permissões da medida de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
