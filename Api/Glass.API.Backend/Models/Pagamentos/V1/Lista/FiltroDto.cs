// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.Pagamentos;
using Glass.API.Backend.Models.Genericas.V1;
using Newtonsoft.Json;
using System;

namespace Glass.API.Backend.Models.Pagamentos.V1.Lista
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de pagamentos.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaPagamentos(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do pagamento.
        /// </summary>
        [JsonProperty("id")]
        public int? Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da compra.
        /// </summary>
        [JsonProperty("idCompra")]
        public int? IdCompra { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do custo fixo.
        /// </summary>
        [JsonProperty("idCustoFixo")]
        public int? IdCustoFixo { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do imposto/serviço.
        /// </summary>
        [JsonProperty("idImpostoServico")]
        public int? IdImpostoServico { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do fornecedor.
        /// </summary>
        [JsonProperty("idFornecedor")]
        public int? IdFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o nome do fornecedor.
        /// </summary>
        [JsonProperty("nomeFornecedor")]
        public string NomeFornecedor { get; set; }

        /// <summary>
        /// Obtém ou define o período inicial de cadastro do pagamento.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define o período final de cadastro do pagamento.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define o inicio da faixa de valores dos pagamentos.
        /// </summary>
        [JsonProperty("valorInicial")]
        public decimal? ValorInicial { get; set; }

        /// <summary>
        /// Obtém ou define o fim da faixa de valores dos pagamentos.
        /// </summary>
        [JsonProperty("valorFinal")]
        public decimal? ValorFinal { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da situação do pagamento.
        /// </summary>
        [JsonProperty("situacao")]
        public int? Situacao { get; set; }

        /// <summary>
        /// Obtém ou define o número da nota fiscal referente ao pagamento.
        /// </summary>
        [JsonProperty("numeroNotaFiscal")]
        public int? NumeroNotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define a observação do pagamento.
        /// </summary>
        [JsonProperty("observacao")]
        public string Observacao { get; set; }
    }
}