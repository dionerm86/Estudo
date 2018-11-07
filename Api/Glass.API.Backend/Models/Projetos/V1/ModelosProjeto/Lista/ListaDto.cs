// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Projetos.V1.ModelosProjeto.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um modelo de projeto para a tela de listagem.
    /// </summary>
    [DataContract(Name = "ModeloProjeto")]
    public class ListaDto : IdNomeDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="modeloProjeto">A model de modelo de projeto.</param>
        internal ListaDto(Data.Model.ProjetoModelo modeloProjeto)
        {
            this.Id = (int)modeloProjeto.IdProjetoModelo;
            this.Nome = modeloProjeto.Descricao;
            this.Codigo = modeloProjeto.Codigo;
            this.Situacao = modeloProjeto.DescrSituacao;

            this.Permissoes = new PermissoesDto
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(Data.Model.LogAlteracao.TabelaAlteracao.ProjetoModelo, modeloProjeto.IdProjetoModelo, null),
            };
        }

        /// <summary>
        /// Obtém ou define o código do modelo de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        /// <summary>
        /// Obtém ou define a situação do modelo de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define as permissões do modelo de projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
