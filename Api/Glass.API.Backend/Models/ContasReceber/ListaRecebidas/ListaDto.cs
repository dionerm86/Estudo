// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.ContasReceber.ListaRecebidas
{
    /// <summary>
    /// Classe que encapsula os dados de uma conta recebida para a tela de listagem.
    /// </summary>
    [DataContract(Name = "ContaRecebida")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="contaRecebida">A model de conta receber.</param>
        internal ListaDto(Data.Model.ContasReceber contaRecebida)
        {
            this.Id = (int)contaRecebida.IdContaR;
            this.IdLiberarPedido = (int?)contaRecebida.IdLiberarPedido;
            this.IdPedido = (int?)contaRecebida.IdPedido;
            this.IdAcertoParcial = (int?)contaRecebida.IdAcertoParcial;
            this.IdSinal = (int?)contaRecebida.IdSinal;
            this.IdEncontroContas = (int?)contaRecebida.IdEncontroContas;
            this.IdObra = (int?)contaRecebida.IdObra;
            this.IdAcerto = (int?)contaRecebida.IdAcerto;
            this.IdComissao = contaRecebida.IdComissao;
            this.Referecia = contaRecebida.Referencia;
            this.NumeroParcela = contaRecebida.NumParc;
            this.NumeroMaximoParcelas = contaRecebida.NumParcMax;
            this.Cliente = new IdNomeDto
            {
                Id = (int)contaRecebida.IdCliente,
                Nome = contaRecebida.NomeCli,
            };

            this.FormaPagamento = contaRecebida.DescrFormaPagto;
            this.ValorVencimento = contaRecebida.ValorVec;
            this.DataVencimento = contaRecebida.DataVec;
            this.ValorRecebimento = contaRecebida.ValorRec;
            this.DataRecebimento = contaRecebida.DataRec;
            this.RecebidaPor = contaRecebida.NomeFunc;
            this.NumeroNfe = contaRecebida.NumeroNFe;
            this.Localizacao = contaRecebida.DestinoRec;
            this.NumeroArquivoRemessaCnab = contaRecebida.NumeroArquivoRemessaCnab;
            this.Observacao = contaRecebida.Obs;
            this.ObservacaoDescontoAcrescimo = contaRecebida.ObsDescAcresc;
            this.DetalhamentoComissao = contaRecebida.DescrComissao;
            this.DescricaoContaContabil = contaRecebida.DescricaoContaContabil;
            this.CorLinha = this.ObterCorLinha(contaRecebida);

            this.Permissoes = new PermissoesDto
            {
                Editar = !contaRecebida.IsParcelaCartao,
                Cancelar = contaRecebida.DeleteVisible,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.ContasReceber, contaRecebida.IdContaR, null),
                LogCancelamento = LogCancelamentoDAO.Instance.TemRegistro(LogCancelamento.TabelaCancelamento.ContasReceber, contaRecebida.IdContaR),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador da conta recebida.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da liberação de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idLiberarPedido")]
        public int? IdLiberarPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do acerto parcial.
        /// </summary>
        [DataMember]
        [JsonProperty("idAcertoParcial")]
        public int? IdAcertoParcial { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do sinal.
        /// </summary>
        [DataMember]
        [JsonProperty("idSinal")]
        public int? IdSinal { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do encontro de contas.
        /// </summary>
        [DataMember]
        [JsonProperty("idEncontroContas")]
        public int? IdEncontroContas { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da obra.
        /// </summary>
        [DataMember]
        [JsonProperty("idObra")]
        public int? IdObra { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do acerto.
        /// </summary>
        [DataMember]
        [JsonProperty("idAcerto")]
        public int? IdAcerto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da comissão.
        /// </summary>
        [DataMember]
        [JsonProperty("idComissao")]
        public int? IdComissao { get; set; }

        /// <summary>
        /// Obtém ou define a referência da conta recebida.
        /// </summary>
        [DataMember]
        [JsonProperty("referencia")]
        public string Referecia { get; set; }

        /// <summary>
        /// Obtém ou define o número (posição) desta parcela.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroParcela")]
        public int NumeroParcela { get; set; }

        /// <summary>
        /// Obtém ou define o número máximo de parcelas que esta conta recebida faz parte.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroMaximoParcelas")]
        public int NumeroMaximoParcelas { get; set; }

        /// <summary>
        /// Obtém ou define o cliente da conta recebida.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento da conta recebida.
        /// </summary>
        [DataMember]
        [JsonProperty("formaPagamento")]
        public string FormaPagamento { get; set; }

        /// <summary>
        /// Obtém ou define o valor da conta recebida.
        /// </summary>
        [DataMember]
        [JsonProperty("valorVencimento")]
        public decimal ValorVencimento { get; set; }

        /// <summary>
        /// Obtém ou define a data de vencimento da conta recebida.
        /// </summary>
        [DataMember]
        [JsonProperty("dataVencimento")]
        public DateTime DataVencimento { get; set; }

        /// <summary>
        /// Obtém ou define o valor recebido da conta.
        /// </summary>
        [DataMember]
        [JsonProperty("valorRecebimento")]
        public decimal ValorRecebimento { get; set; }

        /// <summary>
        /// Obtém ou define a data de recebimento da conta.
        /// </summary>
        [DataMember]
        [JsonProperty("dataRecebimento")]
        public DateTime? DataRecebimento { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica por quem a conta foi recebida.
        /// </summary>
        [DataMember]
        [JsonProperty("recebidaPor")]
        public string RecebidaPor { get; set; }

        /// <summary>
        /// Obtém ou define o número da NF-e da conta foi recebida.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroNfe")]
        public string NumeroNfe { get; set; }

        /// <summary>
        /// Obtém ou define a localização da conta recebida.
        /// </summary>
        [DataMember]
        [JsonProperty("localizacao")]
        public string Localizacao { get; set; }

        /// <summary>
        /// Obtém ou define o número do arquivo de remessa da conta recebida.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroArquivoRemessaCnab")]
        public int? NumeroArquivoRemessaCnab { get; set; }

        /// <summary>
        /// Obtém ou define a observação da conta recebida.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define a observação do desconto/acréscimo da conta recebida.
        /// </summary>
        [DataMember]
        [JsonProperty("observacaoDescontoAcrescimo")]
        public string ObservacaoDescontoAcrescimo { get; set; }

        /// <summary>
        /// Obtém ou define o detalhamento da comissão da conta recebida.
        /// </summary>
        [DataMember]
        [JsonProperty("detalhamentoComissao")]
        public string DetalhamentoComissao { get; set; }

        /// <summary>
        /// Obtém ou define a descrição contábil da conta recebida.
        /// </summary>
        [DataMember]
        [JsonProperty("descricaoContaContabil")]
        public string DescricaoContaContabil { get; set; }

        /// <summary>
        /// Obtém ou define a a cor da linha da tabela.
        /// </summary>
        [DataMember]
        [JsonProperty("corLinha")]
        public string CorLinha { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas na conta recebida.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }

        private string ObterCorLinha(Data.Model.ContasReceber contaRecebida)
        {
            if (!contaRecebida.Recebida)
            {
                return "Green";
            }

            if (contaRecebida.Protestado)
            {
                return System.Drawing.Color.FromArgb(225, 200, 0).ToString();
            }

            if (contaRecebida.IdArquivoRemessa > 0)
            {
                return "Blue";
            }

            return "Black";
        }
    }
}
