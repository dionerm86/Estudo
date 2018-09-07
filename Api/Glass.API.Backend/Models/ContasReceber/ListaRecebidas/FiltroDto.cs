// <copyright file="FiltroDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Helper.ContasReceber;
using Glass.API.Backend.Models.Genericas;
using Newtonsoft.Json;
using System;

namespace Glass.API.Backend.Models.ContasReceber.ListaRecebidas
{
    /// <summary>
    /// Classe com os itens para o filtro de lista de contas recebidas.
    /// </summary>
    public class FiltroDto : PaginacaoDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="FiltroDto"/>.
        /// </summary>
        public FiltroDto()
            : base(item => new TraducaoOrdenacaoListaContasRecebidas(item.Ordenacao))
        {
        }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [JsonProperty("idPedido")]
        public int? IdPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da liberação de pedido.
        /// </summary>
        [JsonProperty("idLiberarPedido")]
        public int? IdLiberarPedido { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do acerto.
        /// </summary>
        [JsonProperty("idAcerto")]
        public int? IdAcerto { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do acerto parcial.
        /// </summary>
        [JsonProperty("idAcertoParcial")]
        public int? IdAcertoParcial { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da troca/devolução.
        /// </summary>
        [JsonProperty("IdTrocaDevolucao")]
        public int? IdTrocaDevolucao { get; set; }

        /// <summary>
        /// Obtém ou define o número da NFe.
        /// </summary>
        [JsonProperty("numeroNfe ")]
        public int? NumeroNfe { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do Sinal.
        /// </summary>
        [JsonProperty("idSinal")]
        public int? IdSinal { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do CT-e.
        /// </summary>
        [JsonProperty("idCte")]
        public int? IdCte { get; set; }

        /// <summary>
        /// Obtém ou define a data de início do vencimento da conta recebida.
        /// </summary>
        [JsonProperty("periodoVencimentoInicio")]
        public DateTime? PeriodoVencimentoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final do vencimento da conta recebida.
        /// </summary>
        [JsonProperty("periodoVencimentoFim")]
        public DateTime? PeriodoVencimentoFim { get; set; }

        /// <summary>
        /// Obtém ou define a data de início do recebimento da conta recebida.
        /// </summary>
        [JsonProperty("periodoRecebimentoInicio")]
        public DateTime? PeriodoRecebimentoInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final do recebimento da conta recebida.
        /// </summary>
        [JsonProperty("periodoRecebimentoFim")]
        public DateTime? PeriodoRecebimentoFim { get; set; }

        /// <summary>
        /// Obtém ou define a data de início do cadastro da conta recebida.
        /// </summary>
        [JsonProperty("periodoCadastroInicio")]
        public DateTime? PeriodoCadastroInicio { get; set; }

        /// <summary>
        /// Obtém ou define a data final do cadastro da conta recebida.
        /// </summary>
        [JsonProperty("periodoCadastroFim")]
        public DateTime? PeriodoCadastroFim { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do usuário que recebeu a conta.
        /// </summary>
        [JsonProperty("recebidaPor")]
        public int? RecebidaPor { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja da conta recebida.
        /// </summary>
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do vendedor do pedido da conta recebida.
        /// </summary>
        [JsonProperty("idVendedor")]
        public int? IdVendedor { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do cliente da conta recebida.
        /// </summary>
        [JsonProperty("idCliente")]
        public int? IdCliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome do cliente da conta recebida.
        /// </summary>
        [JsonProperty("nomeCliente")]
        public string NomeCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do vendedor associado ao cliente da conta recebida.
        /// </summary>
        [JsonProperty("idVendedorAssociadoCliente")]
        public int? IdVendedorAssociadoCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do vendedor associado à obra da conta recebida.
        /// </summary>
        [JsonProperty("idVendedorObra")]
        public int? IdVendedorObra { get; set; }

        /// <summary>
        /// Obtém ou define as formas de pagamento que serão filtradas por conta recebida.
        /// </summary>
        [JsonProperty("formasPagamento")]
        public int[] FormasPagamento { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de entrega do pedido da conta recebida.
        /// </summary>
        [JsonProperty("tipoEntrega")]
        public int? TipoEntrega { get; set; }

        /// <summary>
        /// Obtém ou define o valor recebido inicial para filtro da conta recebida.
        /// </summary>
        [JsonProperty("valorRecebidoInicio")]
        public decimal? ValorRecebidoInicio { get; set; }

        /// <summary>
        /// Obtém ou define o valor recebido final para filtro da conta recebida.
        /// </summary>
        [JsonProperty("valorRecebidoFim")]
        public decimal? ValorRecebidoFim { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da rota do cliente da conta recebida.
        /// </summary>
        [JsonProperty("idRota")]
        public int? IdRota { get; set; }

        /// <summary>
        /// Obtém ou define os tipos contábeis da conta recebida.
        /// </summary>
        [JsonProperty("tiposContabeis")]
        public int[] TiposContabeis { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do comissionado do pedido da conta recebida.
        /// </summary>
        [JsonProperty("idComissionado")]
        public int? IdComissionado { get; set; }

        /// <summary>
        /// Obtém ou define a observação da conta recebida.
        /// </summary>
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define o número de autorização do cartão usado ao receber a conta.
        /// </summary>
        [JsonProperty("numeroAutorizacaoCartao")]
        public string NumeroAutorizacaoCartao { get; set; }

        /// <summary>
        /// Obtém ou define o número do arquivo de remessa da conta recebida.
        /// </summary>
        [JsonProperty("numeroArquivoRemessa")]
        public int? NumeroArquivoRemessa { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscadas ou não contas recebidas por arquivo de remessa.
        /// </summary>
        [JsonProperty("buscaArquivoRemessa")]
        public int? BuscaArquivoRemessa { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da comissão da conta recebida.
        /// </summary>
        [JsonProperty("idComissao")]
        public int? IdComissao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscadas ou não contas com notas fiscais geradas.
        /// </summary>
        [JsonProperty("buscaNotaFiscal")]
        public int[] BuscaNotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscadas ou não contas a receber.
        /// </summary>
        [JsonProperty("buscarContasAReceber")]
        public bool BuscarContasAReceber { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscadas ou não contas renegociadas.
        /// </summary>
        [JsonProperty("buscarContasRenegociadas")]
        public bool BuscarContasRenegociadas { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscadas ou não contas de obra.
        /// </summary>
        [JsonProperty("buscarContasDeObra")]
        public bool BuscarContasDeObra { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscadas ou não contas protestadas.
        /// </summary>
        [JsonProperty("buscarContasProtestadas")]
        public bool BuscarContasProtestadas { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se serão buscadas ou não contas vinculadas.
        /// </summary>
        [JsonProperty("buscarContasVinculadas")]
        public bool BuscarContasVinculadas { get; set; }

        /// <summary>
        /// Obtém ou define a ordenação a ser usada na listagem.
        /// </summary>
        [JsonProperty("ordenacaoFiltro")]
        public int OrdenacaoFiltro { get; set; }
    }
}
