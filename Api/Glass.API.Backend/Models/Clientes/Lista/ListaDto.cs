// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Global.Negocios.Entidades;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Clientes.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um cliente para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Cliente")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="cliente">A model de clientes.</param>
        internal ListaDto(ClientePesquisa cliente)
        {
            this.Id = cliente.IdCli;
            this.NomeCliente = cliente.Nome;
            this.CpfCnpj = cliente.CpfCnpj;
            this.EnderecoCompleto = cliente.EnderecoCompleto;
            this.TelefoneContato = cliente.Telefone;
            this.Celular = cliente.TelCel;
            this.Atendente = new IdNomeDto
            {
                Id = (int)cliente.IdFunc.GetValueOrDefault(),
                Nome = cliente.NomeFunc,
            };

            this.Email = cliente.Email;
            this.DataUltimaCompra = cliente.DtUltCompra;
            this.TotalComprado = cliente.TotalComprado;
            this.Situacao = cliente.Situacao.ToString();
            this.Permissoes = new PermissoesDto
            {
                AlterarSituacao = Config.PossuiPermissao(Config.FuncaoMenuCadastro.AtivarInativarCliente) && cliente.Situacao != Data.Model.SituacaoCliente.Cancelado,
                CadastrarDescontoTabela = Config.PossuiPermissao(Config.FuncaoMenuCadastro.DescontoAcrescimoProdutoCliente) && cliente.IdTabelaDesconto.GetValueOrDefault() == 0,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(Data.Model.LogAlteracao.TabelaAlteracao.Cliente, (uint)cliente.IdCli, null),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("nomeCliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define o cpf/cnpj do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("cpfCnpj")]
        public string CpfCnpj { get; set; }

        /// <summary>
        /// Obtém ou define o endereço completo do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("enderecoCompleto")]
        public string EnderecoCompleto { get; set; }

        /// <summary>
        /// Obtém ou define o telefone de contato do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("telefoneContato")]
        public string TelefoneContato { get; set; }

        /// <summary>
        /// Obtém ou define o celular do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("celular")]
        public string Celular { get; set; }

        /// <summary>
        /// Obtém ou define os dados básicos do atendente.
        /// </summary>
        [DataMember]
        [JsonProperty("atendente")]
        public IdNomeDto Atendente { get; set; }

        /// <summary>
        /// Obtém ou define o email do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Obtém ou define a data da última compra do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("dataUltimaCompra")]
        public DateTime? DataUltimaCompra { get; set; }

        /// <summary>
        /// Obtém ou define o total comprado do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("totalComprado")]
        public decimal TotalComprado { get; set; }

        /// <summary>
        /// Obtém ou define a situação do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas ao cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
