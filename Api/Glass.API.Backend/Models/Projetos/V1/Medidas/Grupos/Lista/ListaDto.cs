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
            this.Nome = grupoMedidaProjeto.Descricao;

            this.Permissoes = new PermissoesDto
            {
                Editar = this.PodeEditarExcluir(),
                Excluir = this.PodeEditarExcluir(),
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
        [JsonProperty("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Obtém ou define as permissões para a listagem de grupos de medida de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o item da lista poder ser editado ou excluido.
        /// </summary>
        /// <returns>Resultado da validação que indica se o item atual pode ser editado/excluido.</returns>
        private bool PodeEditarExcluir()
        {
            return !System.Enum.GetNames(typeof(GrupoMedidaProjeto.TipoMedida))
                   .Any(f => f == Conversoes.ConverteValor<GrupoMedidaProjeto.TipoMedida>(this.Id).ToString());
        }
    }
}