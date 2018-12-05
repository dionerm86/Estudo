// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Global.Negocios.Entidades;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Funcionarios.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um funcionário para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Funcionario")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// /// <param name="funcionario">A model de funcionario.</param>
        internal ListaDto(FuncionarioPesquisa funcionario)
        {
            this.Id = funcionario.IdFunc;
            this.Nome = funcionario.Nome;
            this.Loja = funcionario.Loja;
            this.TipoFuncionario = funcionario.TipoFuncionario;
            this.Documentos = new DocumentosDto
            {
                Rg = funcionario.Rg,
                Cpf = funcionario.Cpf,
            };

            this.Contatos = new ContatosDto
            {
                TelefoneResidencial = funcionario.TelRes,
                TelefoneCelular = funcionario.TelCel,
            };

            var permissao = !funcionario.AdminSync || UserInfo.GetUserInfo.IsAdminSync;
            this.Permissoes = new PermissoesDto
            {
                Apagar = permissao,
                Editar = permissao,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(Data.Model.LogAlteracao.TabelaAlteracao.Funcionario, (uint)funcionario.IdFunc, null),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do funcionario.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define a loja do funcionario.
        /// </summary>
        [DataMember]
        [JsonProperty("nome")]
        public string Nome { get; set; }

        /// <summary>
        /// Obtém ou define a loja do funcionario.
        /// </summary>
        [DataMember]
        [JsonProperty("loja")]
        public string Loja { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do funcionario.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoFuncionario")]
        public string TipoFuncionario { get; set; }

        /// <summary>
        /// Obtém ou define o documentos do funcionario.
        /// </summary>
        [DataMember]
        [JsonProperty("documentos")]
        public DocumentosDto Documentos { get; set; }

        /// <summary>
        /// Obtém ou define o contatos do funcionario.
        /// </summary>
        [DataMember]
        [JsonProperty("contatos")]
        public ContatosDto Contatos { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas ao funcionário.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
