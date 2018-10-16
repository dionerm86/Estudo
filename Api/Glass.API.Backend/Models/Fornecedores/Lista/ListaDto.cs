// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.Data.DAL;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Fornecedores.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um fornecedor para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Fornecedor")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="fornecedor">A model de fornecedor.</param>
        internal ListaDto(Global.Negocios.Entidades.FornecedorPesquisa fornecedor)
        {
            this.Id = fornecedor.IdFornec;
            this.NomeFantasia = fornecedor.Nomefantasia;
            this.CpfCnpj = fornecedor.CpfCnpj;
            this.RgInscricaoEstadual = fornecedor.RgInscEst;
            this.DataUltimaCompra = fornecedor.Dtultcompra;
            this.Situacao = fornecedor.Situacao.ToString();
            this.TelefoneContato = fornecedor.Telcont;
            this.Credito = fornecedor.Credito;
            this.Vendedor = new VendedorDto
            {
                Nome = fornecedor.Vendedor,
                Celular = fornecedor.Telcelvend,
            };

            this.Permissoes = new PermissoesDto()
            {
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(Data.Model.LogAlteracao.TabelaAlteracao.Fornecedor, (uint)fornecedor.IdFornec, null),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do fornecedor.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o nome fantasia do fornecedor.
        /// </summary>
        [DataMember]
        [JsonProperty("nomeFantasia")]
        public string NomeFantasia { get; set; }

        /// <summary>
        /// Obtém ou define o CPF/CNPJ do fornecedor.
        /// </summary>
        [DataMember]
        [JsonProperty("cpfCnpj")]
        public string CpfCnpj { get; set; }

        /// <summary>
        /// Obtém ou define o RG/Inscrição estadual do fornecedor.
        /// </summary>
        [DataMember]
        [JsonProperty("rgInscricaoEstadual")]
        public string RgInscricaoEstadual { get; set; }

        /// <summary>
        /// Obtém ou define a data de úiltima compra feita deste fornecedor.
        /// </summary>
        [DataMember]
        [JsonProperty("dataUltimaCompra")]
        public DateTime? DataUltimaCompra { get; set; }

        /// <summary>
        /// Obtém ou define a situação do fornecedor.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define o telefone de contato do fornecedor.
        /// </summary>
        [DataMember]
        [JsonProperty("telefoneContato")]
        public string TelefoneContato { get; set; }

        /// <summary>
        /// Obtém ou define o crédito do fornecedor.
        /// </summary>
        [DataMember]
        [JsonProperty("credito")]
        public decimal Credito { get; set; }

        /// <summary>
        /// Obtém ou define dados do vendedor associado ao fornecedor.
        /// </summary>
        [DataMember]
        [JsonProperty("vendedor")]
        public VendedorDto Vendedor { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas na tela.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
