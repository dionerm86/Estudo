// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.Geral.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de uma movimentação do caixa geral para a tela de listagem.
    /// </summary>
    [DataContract(Name = "CaixaGeral")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="caixaGeral">A model de caixa geral.</param>
        internal ListaDto(Data.Model.CaixaGeral caixaGeral)
        {
            this.Id = (int)caixaGeral.IdCaixaGeral;
            this.Referencia = caixaGeral.Referencia;
            this.Cliente = new IdNomeDto
            {
                Id = (int?)caixaGeral.IdCliente,
                Nome = caixaGeral.NomeCliente,
            };

            this.Fornecedor = new IdNomeDto
            {
                Id = (int?)caixaGeral.IdFornec,
                Nome = caixaGeral.NomeFornecedor,
            };

            this.DataMovimentacao = caixaGeral.DataMov;
            this.Loja = caixaGeral.NomeLoja;
            this.PlanoDeConta = caixaGeral.DescrPlanoConta;
            this.ContaBanco = caixaGeral.IdContaBanco > 0 ? ContaBancoDAO.Instance.GetDescricao(caixaGeral.IdContaBanco.Value) : string.Empty;
            this.Observacao = caixaGeral.Obs;
            this.Valor = caixaGeral.ValorMov;
            this.Juros = caixaGeral.Juros;
            this.Saldo = caixaGeral.Saldo;
            this.CorLinha = caixaGeral.TipoMov == (int)Data.Model.CaixaDiario.TipoMovimentacaoEnum.Saida && caixaGeral.IdCaixaGeral > 0 ? "Red" : "Black";
        }

        /// <summary>
        /// Obtém ou define o identificador da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define a referência da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("referencia")]
        public string Referencia { get; set; }

        /// <summary>
        /// Obtém ou define o cliente da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define o fornecedor da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("fornecedor")]
        public IdNomeDto Fornecedor { get; set; }

        /// <summary>
        /// Obtém ou define a data de de movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("dataMovimentacao")]
        public DateTime DataMovimentacao { get; set; }

        /// <summary>
        /// Obtém ou define a loja que foi realizada a movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("loja")]
        public string Loja { get; set; }

        /// <summary>
        /// Obtém ou define o plano de conta da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("planoDeConta")]
        public string PlanoDeConta { get; set; }

        /// <summary>
        /// Obtém ou define a descrição da conta bancária da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("contaBanco")]
        public string ContaBanco { get; set; }

        /// <summary>
        /// Obtém ou define a observação da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define o valor da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Obtém ou define os juros da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("juros")]
        public decimal Juros { get; set; }

        /// <summary>
        /// Obtém ou define o saldo da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("saldo")]
        public decimal Saldo { get; set; }

        /// <summary>
        /// Obtém ou define a cor da linha da movimentação.
        /// </summary>
        [DataMember]
        [JsonProperty("corLinha")]
        public string CorLinha { get; set; }
    }
}
