// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Projetos.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um projeto para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Projeto")]
    public class ListaDto : IdDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="projeto">A model de projeto.</param>
        internal ListaDto(Data.Model.Projeto projeto)
        {
            this.Id = (int)projeto.IdProjeto;
            this.Cliente = new IdNomeDto
            {
                Id = (int?)projeto.IdCliente,
                Nome = projeto.NomeCliente,
            };

            this.Loja = projeto.NomeLoja;
            this.Funcionario = projeto.NomeFunc;
            this.Total = projeto.Total;
            this.DataCadastro = projeto.DataCad;
            this.Situacao = projeto.DescrSituacao;
            this.NumeroItensProjeto = projeto.NumeroItensProjeto;

            this.Permissoes = new PermissoesDto
            {
                Editar = projeto.EditVisible,
                Excluir = projeto.DeleteVisible,
            };
        }

        /// <summary>
        /// Obtém ou define o cliente do projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define a loja do projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("loja")]
        public string Loja { get; set; }

        /// <summary>
        /// Obtém ou define o funcionário do projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("funcionario")]
        public string Funcionario { get; set; }

        /// <summary>
        /// Obtém ou define o total do projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtém ou define a data de cadastro do projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("dataCadastro")]
        public DateTime DataCadastro { get; set; }

        /// <summary>
        /// Obtém ou define a situação do projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de itens no projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroItensProjeto")]
        public int NumeroItensProjeto { get; set; }

        /// <summary>
        /// Obtém ou define as permissões do projeto.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
