// <copyright file="ClienteDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.API.Backend.Models.Parcelas.Filtro;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Glass.API.Backend.Models.Clientes.Filtro
{
    /// <summary>
    /// Classe que encapsula a resposta para o controle de clientes.
    /// </summary>
    [DataContract(Name = "Cliente")]
    public class ClienteDto : ClienteBaseDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ClienteDto"/>.
        /// </summary>
        /// <param name="cliente">A model de clientes.</param>
        /// <param name="tipoValidacao">O tipo de validação informado ao serviço (usado para definir quais informações serão disponibilizadas).</param>
        public ClienteDto(Cliente cliente, string tipoValidacao)
        {
            this.Id = cliente.IdCli;
            this.Nome = cliente.Nome;
            this.Revenda = cliente.Revenda;
            this.Observacoes = this.ObterObservacoesCliente(cliente, tipoValidacao);
            this.Credito = PedidoConfig.TelaCadastro.ExibirCreditoClienteAoBuscar
                ? cliente.Credito
                : (decimal?)null;

            this.Rota = new RotaDto
            {
                Data = RotaDAO.Instance.GetDataRota((uint)cliente.IdCli, DateTime.Now),
                EntregaBalcao = cliente.IdRota > 0 ? (bool?)RotaDAO.Instance.ObterEntregaBalcao(cliente.IdRota.Value) : null,
                Entrega = PedidoConfig.TelaCadastro.MarcarPedidosRotaComoEntrega
                    && RotaClienteDAO.Instance.IsClienteAssociado((uint)cliente.IdCli),
            };

            this.PagamentoAntecipado = cliente.PagamentoAntesProducao;
            this.PercentualMinimoSinal = cliente.PercSinalMinimo;
            this.IdVendedor = this.ObterIdVendedorCliente(cliente);
            this.Comissionado = !cliente.IdComissionado.HasValue
                ? null
                : new ComissionadoDto
                {
                    Id = cliente.IdComissionado.Value,
                    Nome = ComissionadoDAO.Instance.GetNome((uint)cliente.IdComissionado.Value),
                    Percentual = ComissionadoDAO.Instance.ObtemPercentual((uint)cliente.IdComissionado.Value),
                };

            this.PercentualComissao = cliente.PercentualComissao;
            this.IdLoja = this.ObterIdLojaCliente(cliente);
            this.IdTransportador = cliente.IdTransportador;
            this.EnderecoEntrega = this.ObterEnderecoEntrega(cliente);

            this.TipoVenda = cliente.TipoPagto == null || ParcelasDAO.Instance.ObterNumParcelas(cliente.TipoPagto.Value) == 0 ? Data.Model.Pedido.TipoVendaPedido.AVista : Data.Model.Pedido.TipoVendaPedido.APrazo;
            this.IdFormaPagamento = cliente.IdFormaPagto;

            if (cliente.TipoPagto.HasValue)
            {
                var parcelaPadrao = ParcelasDAO.Instance.GetElementByPrimaryKey(cliente.TipoPagto.Value);

                this.Parcela = new ParcelaDto
                {
                    Id = cliente.TipoPagto.Value,
                    Nome = parcelaPadrao.Descricao,
                    NumeroParcelas = parcelaPadrao.NumParcelas,
                    Dias = parcelaPadrao.NumeroDias,
                };
            }
        }

        /// <summary>
        /// Obtém ou define as observações do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("observacoes")]
        public string Observacoes { get; set; }

        /// <summary>
        /// Obtém ou define o crédito disponível ao cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("credito")]
        public decimal? Credito { get; set; }

        /// <summary>
        /// Obtém ou define os dados de rota do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("rota")]
        public RotaDto Rota { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o cliente precisa realizar pagamentos antecipados.
        /// </summary>
        [DataMember]
        [JsonProperty("pagamentoAntecipado")]
        public bool PagamentoAntecipado { get; set; }

        /// <summary>
        /// Obtém ou define o percentual mínimo de pagamento de sinal do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("percentualMinimoSinal")]
        public double? PercentualMinimoSinal { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do vendedor associado ao cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("idVendedor")]
        public int? IdVendedor { get; set; }

        /// <summary>
        /// Obtém ou define os dados de comissionado do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("comissionado")]
        public ComissionadoDto Comissionado { get; set; }

        /// <summary>
        /// Obtém ou define o percentual de comissão associado ao cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("percentualComissao")]
        public double? PercentualComissao { get; set; }

        /// <summary>
        /// Obtém ou define o identificador da loja do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("idLoja")]
        public int? IdLoja { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do transportador do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("idTransportador")]
        public int? IdTransportador { get; set; }

        /// <summary>
        /// Obtém ou define o endereço de entrega do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("enderecoEntrega")]
        public EnderecoDto EnderecoEntrega { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de venda padrão do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoVenda")]
        public Data.Model.Pedido.TipoVendaPedido TipoVenda { get; set; }

        /// <summary>
        /// Obtém ou define a forma de pagamento padrão do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("idFormaPagamento")]
        public int? IdFormaPagamento { get; set; }

        /// <summary>
        /// Obtém ou define o dto da parcela padrão do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("parcela")]
        public ParcelaDto Parcela { get; set; }

        private string ObterObservacoesCliente(Cliente cliente, string tipoValidacao)
        {
            var observacoes = new StringBuilder();

            switch ((tipoValidacao ?? string.Empty).ToLowerInvariant())
            {
                case "pedido":
                    this.IncluirObservacoesPedido(cliente, observacoes);
                    break;
            }

            return observacoes.ToString();
        }

        private void IncluirObservacoesPedido(Cliente cliente, StringBuilder observacoes)
        {
            observacoes.Append(ClienteDAO.Instance.ObterObsPedido((uint)cliente.IdCli));

            if (!FinanceiroConfig.PermitirFinalizacaoPedidoPeloFinanceiro
                && cliente.BloquearPedidoContaVencida
                && ContasReceberDAO.Instance.ClientePossuiContasVencidas((uint)cliente.IdCli))
            {
                observacoes.AppendLine("Cliente bloqueado. Motivo: Contas a receber em atraso.");
            }
        }

        private int? ObterIdVendedorCliente(Cliente cliente)
        {
            var idVendedor = PedidoConfig.DadosPedido.BuscarVendedorEmitirPedido
                ? cliente.IdFunc
                : null;

            return idVendedor.HasValue && FuncionarioDAO.Instance.GetVendedores().Any(f => f.IdFunc == idVendedor.Value)
                ? idVendedor
                : null;
        }

        private int? ObterIdLojaCliente(Cliente cliente)
        {
            return cliente.IdLoja > 0
                && (Geral.ConsiderarLojaClientePedidoFluxoSistema
                    || (PedidoConfig.ExibirOpcaoDeveTransferir
                        && cliente.IdFunc > 0
                        && FuncionarioDAO.Instance.ObtemIdLoja((uint)cliente.IdFunc.Value) != cliente.IdLoja))
                ? cliente.IdLoja
                : null;
        }

        private EnderecoDto ObterEnderecoEntrega(Cliente cliente)
        {
            if (cliente.IdCidadeEntrega.HasValue)
            {
                return new EnderecoDto
                {
                    Logradouro = cliente.EnderecoEntrega,
                    Bairro = cliente.BairroEntrega,
                    Cep = cliente.CepEntrega,
                    Cidade = new CidadeDto
                    {
                        Id = cliente.IdCidadeEntrega.Value,
                        Nome = cliente.CidadeEntrega,
                        Uf = cliente.UfEntrega,
                    },
                };
            }

            if (cliente.IdCidade.HasValue)
            {
                return new EnderecoDto
                {
                    Logradouro = cliente.Endereco,
                    Bairro = cliente.Bairro,
                    Cep = cliente.Cep,
                    Cidade = new CidadeDto
                    {
                        Id = cliente.IdCidade.Value,
                        Nome = cliente.Cidade,
                        Uf = cliente.Uf,
                    },
                };
            }

            return null;
        }
    }
}
