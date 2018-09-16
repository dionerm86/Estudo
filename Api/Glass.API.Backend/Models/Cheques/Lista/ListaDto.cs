// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Cheques.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um cheque para a tela de listagem.
    /// </summary>
    [DataContract(Name = "Cheque")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="cheque">A model de cheque.</param>
        internal ListaDto(Data.Model.Cheques cheque)
        {
            this.Id = (int)cheque.IdCheque;
            this.Referencia = cheque.Referencia;
            this.Loja = new IdNomeDto
            {
                Id = (int)cheque.IdLoja,
                Nome = cheque.NomeLoja,
            };

            this.Cliente = new IdNomeDto
            {
                Id = (int)cheque.IdCliente,
                Nome = cheque.NomeCliente,
            };

            this.Fornecedor = new IdNomeDto
            {
                Id = (int)cheque.IdFornecedor,
                Nome = cheque.NomeFornecedor,
            };

            this.NumeroCheque = cheque.Num;
            this.DigitoNumeroCheque = cheque.DigitoNum;
            this.Banco = cheque.Banco;
            this.Agencia = cheque.Agencia;
            this.Conta = cheque.Conta;
            this.Titular = cheque.Titular;
            this.CpfCnpj = cheque.CpfCnpj;
            this.ValorRecebido = cheque.ValorRecebido;
            this.DataVencimento = cheque.DataVenc;
            this.DataVencimentoOriginal = cheque.DataVencOriginal;
            this.Observacao = cheque.Obs;
            this.Situacao = cheque.DescrSituacao;

            this.Permissoes = new PermissoesDto
            {
                AlterarDadosCheque = cheque.EditarAgenciaConta,
                AlterarDataVencimento = cheque.AlterarDataVenc,
                CancelarReapresentacao = cheque.CancelarReapresentadoVisible,
                CancelarDevolucao = cheque.ExibirCancelarDevolucao,
                CancelarProtesto = cheque.ExibirDesmarcarProtestado,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.Cheque, cheque.IdCheque, null),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define a referência do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("referencia")]
        public string Referencia { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("loja")]
        public IdNomeDto Loja { get; set; }

        /// <summary>
        /// Obtém ou define o cliente do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define o fornecedor do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("fornecedor")]
        public IdNomeDto Fornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o número do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroCheque")]
        public int NumeroCheque { get; set; }

        /// <summary>
        /// Obtém ou define o dígito do número do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("digitoNumeroCheque")]
        public string DigitoNumeroCheque { get; set; }

        /// <summary>
        /// Obtém ou define o banco do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("banco")]
        public string Banco { get; set; }

        /// <summary>
        /// Obtém ou define a agência do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("agencia")]
        public string Agencia { get; set; }

        /// <summary>
        /// Obtém ou define a conta do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("conta")]
        public string Conta { get; set; }

        /// <summary>
        /// Obtém ou define o titular do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("titular")]
        public string Titular { get; set; }

        /// <summary>
        /// Obtém ou define o cpf/cnpj do titular do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("cpfCnpj")]
        public string CpfCnpj { get; set; }

        /// <summary>
        /// Obtém ou define o valor recebido do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("valorRecebido")]
        public string ValorRecebido { get; set; }

        /// <summary>
        /// Obtém ou define a data de vencimento do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("dataVencimento")]
        public DateTime? DataVencimento { get; set; }

        /// <summary>
        /// Obtém ou define a data de vencimento original do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("dataVencimentoOriginal")]
        public DateTime? DataVencimentoOriginal { get; set; }

        /// <summary>
        /// Obtém ou define a observação do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define a situação do cheque.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas na tela.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
