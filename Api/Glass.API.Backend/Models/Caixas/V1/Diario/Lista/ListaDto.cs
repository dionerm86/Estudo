// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Caixas.V1.Diario.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de uma movimentação do caixa diário para a tela de listagem.
    /// </summary>
    [DataContract(Name = "CaixaDiario")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="caixaDiario">A model de caixa diário.</param>
        internal ListaDto(Data.Model.CaixaDiario caixaDiario)
        {
            this.Id = (int)caixaDiario.IdCaixaDiario;
            this.Referencia = caixaDiario.Referencia;
            this.Cliente = new IdNomeDto
            {
                Id = (int?)caixaDiario.IdCliente,
                Nome = caixaDiario.NomeCliente,
            };

            this.Fornecedor = new IdNomeDto
            {
                Id = (int?)caixaDiario.IdFornec,
                Nome = caixaDiario.NomeFornecedor,
            };

            this.UsuarioCadastro = caixaDiario.DescrUsuCad;
            this.DataCadastro = caixaDiario.DataCad;
            this.PlanoDeConta = caixaDiario.DescrPlanoConta;
            this.Valor = caixaDiario.Valor;
            this.Juros = caixaDiario.Juros;
            this.Saldo = caixaDiario.Saldo;
            this.CorLinha = caixaDiario.TipoMov == (int)Data.Model.CaixaDiario.TipoMovimentacaoEnum.Saida ? "Red" : "Black";
        }

        /// <summary>
        /// Obtém ou define o identificador da movimentação do caixa diário.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define a referência da movimentação do caixa diário.
        /// </summary>
        [DataMember]
        [JsonProperty("referencia")]
        public string Referencia { get; set; }

        /// <summary>
        /// Obtém ou define o cliente da movimentação do caixa diário.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define o fornecedor da movimentação do caixa diário.
        /// </summary>
        [DataMember]
        [JsonProperty("fornecedor")]
        public IdNomeDto Fornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o usuário que realizou a movimentação do caixa diário.
        /// </summary>
        [DataMember]
        [JsonProperty("usuarioCadastro")]
        public string UsuarioCadastro { get; set; }

        /// <summary>
        /// Obtém ou define a data de cadastro da movimentação do caixa diário.
        /// </summary>
        [DataMember]
        [JsonProperty("dataCadastro")]
        public DateTime DataCadastro { get; set; }

        /// <summary>
        /// Obtém ou define o plano de conta da movimentação do caixa diário.
        /// </summary>
        [DataMember]
        [JsonProperty("planoDeConta")]
        public string PlanoDeConta { get; set; }

        /// <summary>
        /// Obtém ou define o valor da movimentação do caixa diário.
        /// </summary>
        [DataMember]
        [JsonProperty("valor")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Obtém ou define os juros da movimentação do caixa diário.
        /// </summary>
        [DataMember]
        [JsonProperty("juros")]
        public decimal Juros { get; set; }

        /// <summary>
        /// Obtém ou define o saldo da movimentação do caixa diário.
        /// </summary>
        [DataMember]
        [JsonProperty("saldo")]
        public decimal Saldo { get; set; }

        /// <summary>
        /// Obtém ou define a cor da linha da movimentação do caixa diário.
        /// </summary>
        [DataMember]
        [JsonProperty("corLinha")]
        public string CorLinha { get; set; }
    }
}
