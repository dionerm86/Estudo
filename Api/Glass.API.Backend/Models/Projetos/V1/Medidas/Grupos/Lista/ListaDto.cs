// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Linq;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.API.Backend.Models.Projetos.V1.Medidas.Grupos.Lista
{
    /// <summary>
    /// Classe que encapsula os dados dos grupos de medida de projeto.
    /// </summary>
    [DataContract(Name = "GrupoMedidasProjeto")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="grupoMedidaProjeto">A model de grupo de medida de projeto.</param>
        internal ListaDto(Data.Model.GrupoMedidaProjeto grupoMedidaProjeto)
        {
            this.Id = (int)grupoMedidaProjeto.IdGrupoMedProj;
            this.Descricao = grupoMedidaProjeto.Descricao;

            var podeEditarExcluir = !System.Enum.GetNames(typeof(GrupoMedidaProjeto.TipoMedida))
                .Any(f => f == Conversoes.ConverteValor<GrupoMedidaProjeto.TipoMedida>(this.Id).ToString());

            this.Permissoes = new PermissoesDto
            {
                Editar = podeEditarExcluir,
                Excluir = podeEditarExcluir,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.GrupoMedidaProjeto, (uint)this.Id, null),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do grupo de medida de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define a descrição do grupo de medida de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// Obtém ou define as permissoes para a listagem de grupos de medida de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}