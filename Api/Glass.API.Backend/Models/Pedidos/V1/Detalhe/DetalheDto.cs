// <copyright file="DetalheDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Data.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.Pedidos.V1.Detalhe
{
    /// <summary>
    /// Classe que encapsula os dados de um pedido para a tela de detalhes.
    /// </summary>
    [DataContract(Name = "Pedido")]
    public class DetalheDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="DetalheDto"/>.
        /// </summary>
        /// <param name="pedido">A model de pedidos.</param>
        internal DetalheDto(Data.Model.Pedido pedido)
        {
            this.Id = (int)pedido.IdPedido;
            this.Cliente = new ClienteDto
            {
                Id = (int)pedido.IdCli,
                Nome = pedido.NomeCli,
                Revenda = pedido.CliRevenda,
                ExigePagamentoAntecipado = pedido.ClientePagaAntecipado,
                PercentualSinalMinimo = pedido.PercSinalMinCliente ?? 0,
                Telefone = pedido.RptTelContCli,
                Endereco = pedido.EnderecoCompletoCliente,
                Observacao = pedido.ObsCliente,
            };

            this.PercentualComissao = pedido.PercentualComissao;
            this.DataPedido = pedido.DataPedido;
            this.FastDelivery = new FastDeliveryDto
            {
                Aplicado = pedido.FastDelivery,
                Taxa = pedido.TaxaFastDelivery,
            };

            this.CodigoPedidoCliente = pedido.CodCliente;
            this.IdOrcamento = (int?)pedido.IdOrcamento;
            this.DeveTransferir = pedido.DeveTransferir;
            this.TipoVenda = !pedido.TipoVenda.HasValue
                ? null
                : new IdNomeDto
                {
                    Id = pedido.TipoVenda.Value,
                    Nome = pedido.DescrTipoVenda,
                };

            this.Vendedor = new IdNomeDto
            {
                Id = (int)pedido.IdFunc,
                Nome = pedido.NomeFunc,
            };

            this.FuncionarioComprador = !pedido.IdFuncVenda.HasValue
                ? null
                : new IdNomeDto
                {
                    Id = (int)pedido.IdFuncVenda.Value,
                    Nome = pedido.NomeFuncVenda,
                };

            this.IdPedidoRevenda = pedido.IdPedidoRevenda;
            this.GerarPedidoCorte = pedido.GerarPedidoProducaoCorte;
            this.Entrega = new EntregaDto
            {
                Tipo = !pedido.TipoEntrega.HasValue
                    ? null
                    : new IdNomeDto
                    {
                        Id = pedido.TipoEntrega.Value,
                        Nome = pedido.DescrTipoEntrega,
                    },
                Data = pedido.DataEntrega,
                Valor = pedido.ValorEntrega,
            };

            this.TextoSinal = pedido.ConfirmouRecebeuSinal;
            this.Sinal = !pedido.IdSinal.HasValue
                ? null
                : new SinalDto
                {
                    Id = (int)pedido.IdSinal.Value,
                    Valor = pedido.ValorEntrada,
                };

            this.Obra = !pedido.IdObra.HasValue
                ? null
                : new ObraDto
                {
                    Id = (int)pedido.IdObra.Value,
                    IdCliente = ObraDAO.Instance.ObtemIdCliente(null, (int)pedido.IdObra),
                    Saldo = pedido.SaldoObra,
                    TotalPedidosEmAberto = pedido.TotalPedidosAbertosObra,
                    Descricao = pedido.DescrObra,
                };

            var detalhesParcelas = new List<DetalheParcelaDto>();

            if (pedido.DatasParcelas != null && pedido.ValoresParcelas != null)
            {
                for (int i = 0; i < pedido.DatasParcelas.Length; i++)
                {
                    detalhesParcelas.Add(new DetalheParcelaDto
                    {
                        Data = pedido.DatasParcelas[i],
                        Valor = pedido.ValoresParcelas[i],
                    });
                }
            }

            this.FormaPagamento = new FormaPagamentoDto
            {
                Id = (int)pedido.IdFormaPagto.GetValueOrDefault(),
                Nome = pedido.PagtoParcela,
                IdTipoCartao = (int?)pedido.IdTipoCartao,
                Parcelas = new ParcelasDto
                {
                    Id = (int?)pedido.IdParcela,
                    NumeroParcelas = pedido.NumParc,
                    Dias = pedido.IdParcela > 0 ? ParcelasDAO.Instance.ObterDiasParcelas((int)pedido.IdParcela) : null,
                    ParcelaAVista = ParcelasDAO.Instance.ObterParcelaAVista(null, (int)pedido.IdParcela.GetValueOrDefault()),
                    Detalhes = detalhesParcelas,
                },
            };

            this.Desconto = new AcrescimoDescontoDto
            {
                Tipo = pedido.TipoDesconto,
                Valor = pedido.Desconto,
            };

            this.Acrescimo = new AcrescimoDescontoDto
            {
                Tipo = pedido.TipoAcrescimo,
                Valor = pedido.Acrescimo,
            };

            this.Total = pedido.Total;
            this.Transportador = !pedido.IdTransportador.HasValue
                ? null
                : new IdNomeDto
                {
                    Id = (int)pedido.IdTransportador.Value,
                    Nome = pedido.NomeTransportador,
                };

            this.Comissionado = !pedido.IdComissionado.HasValue
                ? null
                : new ComissionadoDto
                {
                    Id = (int)pedido.IdComissionado.Value,
                    Nome = pedido.NomeComissionado,
                    Comissao = new PercentualValorDto
                    {
                        Percentual = pedido.PercComissao,
                        Valor = pedido.ValorComissao,
                    },
                };

            this.Medidor = !pedido.IdMedidor.HasValue
                ? null
                : new IdNomeDto
                {
                    Id = (int)pedido.IdMedidor.Value,
                    Nome = pedido.NomeMedidor,
                };

            this.Observacao = pedido.Obs;
            this.ObservacaoLiberacao = pedido.ObsLiberacao;
            this.Situacao = new IdNomeDto
            {
                Id = (int)pedido.Situacao,
                Nome = pedido.DescrSituacaoPedido,
            };

            this.Icms = pedido.AliquotaIcms == 0 && !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIcmsPedido
                ? null
                : new ImpostoDto
                {
                    Aliquota = pedido.AliquotaIcms,
                    Valor = pedido.ValorIcms,
                };

            this.Ipi = pedido.AliquotaIpi == 0 && !Glass.Configuracoes.PedidoConfig.Impostos.CalcularIpiPedido
                ? null
                : new ImpostoDto
                {
                    Aliquota = pedido.AliquotaIpi,
                    Valor = pedido.ValorIpi,
                };

            this.Importado = pedido.Importado;
            this.Tipo = new IdNomeDto
            {
                Id = pedido.TipoPedido,
                Nome = pedido.DescricaoTipoPedido,
            };

            this.Loja = new IdNomeDto
            {
                Id = (int)pedido.IdLoja,
                Nome = pedido.NomeLoja,
            };

            this.EnderecoObra = new EnderecoDto
            {
                Logradouro = pedido.EnderecoObra,
                Bairro = pedido.BairroObra,
                Cep = pedido.CepObra,
                Cidade = new CidadeDto
                {
                    Nome = pedido.CidadeObra,
                },
            };

            this.TotalBruto = pedido.TotalBruto;
            this.Rentabilidade = pedido.PercentualRentabilidade == 0 || pedido.RentabilidadeFinanceira == 0
                ? null
                : new PercentualValorDto
                {
                    Percentual = (double)pedido.PercentualRentabilidade,
                    Valor = pedido.RentabilidadeFinanceira,
                };

            this.Permissoes = new PermissoesDto
            {
                ColocarEmConferencia = pedido.ConferenciaVisible,
                AlterarCliente = pedido.ClienteEnabled,
                AlterarTipoVenda = !(pedido.IdSinal > 0) || Glass.Configuracoes.PedidoConfig.LiberarPedido,
                AlterarVendedor = pedido.SelVendEnabled,
                AlterarDesconto = pedido.DescontoEnabled,
                PodeEditar = pedido.EditVisible,
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define os dados de cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public ClienteDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define o percentual de comissão do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("percentualComissao")]
        public double PercentualComissao { get; set; }

        /// <summary>
        /// Obtém ou define a data de emissão do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("dataPedido")]
        public DateTime DataPedido { get; set; }

        /// <summary>
        /// Obtém ou define os dados de "fast delivery".
        /// </summary>
        [DataMember]
        [JsonProperty("fastDelivery")]
        public FastDeliveryDto FastDelivery { get; set; }

        /// <summary>
        /// Obtém ou define o código de pedido do cliente.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoPedidoCliente")]
        public string CodigoPedidoCliente { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do orçamento.
        /// </summary>
        [DataMember]
        [JsonProperty("idOrcamento")]
        public int? IdOrcamento { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido "deve transferir".
        /// </summary>
        [DataMember]
        [JsonProperty("deveTransferir")]
        public bool DeveTransferir { get; set; }

        /// <summary>
        /// Obtém ou define os dados de venda do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoVenda")]
        public IdNomeDto TipoVenda { get; set; }

        /// <summary>
        /// Obtém ou define os dados do vendedor.
        /// </summary>
        [DataMember]
        [JsonProperty("vendedor")]
        public IdNomeDto Vendedor { get; set; }

        /// <summary>
        /// Obtém ou define os dados do funcionário comprador do pedido, se existir.
        /// </summary>
        [DataMember]
        [JsonProperty("funcionarioComprador")]
        public IdNomeDto FuncionarioComprador { get; set; }

        /// <summary>
        /// Obtém ou define o identificador do pedido original de revenda.
        /// </summary>
        [DataMember]
        [JsonProperty("idPedidoRevenda")]
        public int? IdPedidoRevenda { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido gera um pedido de corte para produção.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarPedidoCorte")]
        public bool GerarPedidoCorte { get; set; }

        /// <summary>
        /// Obtém ou define um texto padrão para exibição de dados do sinal do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("textoSinal")]
        public string TextoSinal { get; set; }

        /// <summary>
        /// Obtém ou define os dados de entrega do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("entrega")]
        public EntregaDto Entrega { get; set; }

        /// <summary>
        /// Obtém ou define os dados de sinal do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("sinal")]
        public SinalDto Sinal { get; set; }

        /// <summary>
        /// Obtém ou define os dados de obra do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("obra")]
        public ObraDto Obra { get; set; }

        /// <summary>
        /// Obtém ou define os dados da forma de pagamento do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("formaPagamento")]
        public FormaPagamentoDto FormaPagamento { get; set; }

        /// <summary>
        /// Obtém ou define dados de desconto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("desconto")]
        public AcrescimoDescontoDto Desconto { get; set; }

        /// <summary>
        /// Obtém ou define dados de acréscimo do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("acrescimo")]
        public AcrescimoDescontoDto Acrescimo { get; set; }

        /// <summary>
        /// Obtém ou define o total do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Obtém ou define os dados do transportador do pedido, se existir.
        /// </summary>
        [DataMember]
        [JsonProperty("transportador")]
        public IdNomeDto Transportador { get; set; }

        /// <summary>
        /// Obtém ou define os dados do comissionado do pedido, se existir.
        /// </summary>
        [DataMember]
        [JsonProperty("comissionado")]
        public ComissionadoDto Comissionado { get; set; }

        /// <summary>
        /// Obtém ou define os dados do medidor do pedido, se existir.
        /// </summary>
        [DataMember]
        [JsonProperty("medidor")]
        public IdNomeDto Medidor { get; set; }

        /// <summary>
        /// Obtém ou define a observação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("observacao")]
        public string Observacao { get; set; }

        /// <summary>
        /// Obtém ou define a observação de liberação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("observacaoLiberacao")]
        public string ObservacaoLiberacao { get; set; }

        /// <summary>
        /// Obtém ou define a situação do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public IdNomeDto Situacao { get; set; }

        /// <summary>
        /// Obtém ou define os dados do ICMS.
        /// </summary>
        [DataMember]
        [JsonProperty("icms")]
        public ImpostoDto Icms { get; set; }

        /// <summary>
        /// Obtém ou define os dados do IPI.
        /// </summary>
        [DataMember]
        [JsonProperty("ipi")]
        public ImpostoDto Ipi { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido foi importado de outro sistema WebGlass.
        /// </summary>
        [DataMember]
        [JsonProperty("importado")]
        public bool Importado { get; set; }

        /// <summary>
        /// Obtém ou define o tipo de pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("tipo")]
        public IdNomeDto Tipo { get; set; }

        /// <summary>
        /// Obtém ou define a loja do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("loja")]
        public IdNomeDto Loja { get; set; }

        /// <summary>
        /// Obtém ou define o endereço do local da obra.
        /// </summary>
        [DataMember]
        [JsonProperty("enderecoObra")]
        public EnderecoDto EnderecoObra { get; set; }

        /// <summary>
        /// Obtém ou define o total bruto do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("totalBruto")]
        public decimal TotalBruto { get; set; }

        /// <summary>
        /// Obtém ou define os dados de rentabilidade do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("rentabilidade")]
        public PercentualValorDto Rentabilidade { get; set; }

        /// <summary>
        /// Obtém ou define os dados de permissões do pedido.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
