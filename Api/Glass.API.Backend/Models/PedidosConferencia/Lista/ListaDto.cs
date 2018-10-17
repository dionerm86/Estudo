// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas;
using Glass.Configuracoes;
using Glass.Data.DAL;
using Glass.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.PedidosConferencia.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de um pedido em conferência para a tela de listagem.
    /// </summary>
    [DataContract(Name = "PedidoConferencia")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="pedidoConferencia">A model de pedido em conferência.</param>
        internal ListaDto(PedidoEspelho pedidoConferencia)
        {
            this.Id = (int)pedidoConferencia.IdPedido;
            this.PedidoGeradoParceiro = pedidoConferencia.GeradoParceiro;
            this.PedidoImportado = pedidoConferencia.Importado;
            this.Situacao = pedidoConferencia.DescrSituacao;
            this.Cliente = new IdNomeDto
            {
                Id = (int)pedidoConferencia.IdCli,
                Nome = BibliotecaTexto.GetThreeFirstWords(pedidoConferencia.NomeCli),
            };

            this.NomeLoja = pedidoConferencia.NomeLoja;
            this.NomeConferente = pedidoConferencia.ResponsavelConferecia;
            this.TotalPedidoComercial = pedidoConferencia.TotalPedido;
            this.TotalPedidoConferencia = pedidoConferencia.Total;
            this.DataCadastroConferencia = pedidoConferencia.DataEspelho;
            this.DataFinalizacaoConferencia = pedidoConferencia.DataConf;
            this.TotalM2 = (decimal)pedidoConferencia.TotM;
            this.QuantidadePecas = (int)pedidoConferencia.QtdePecas;
            this.Peso = (decimal)pedidoConferencia.Peso;
            this.DataEntregaPedidoComercial = pedidoConferencia.DataEntrega;
            this.DataEntregaOriginalPedidoComercial = pedidoConferencia.DataEntregaOriginal;
            this.FastDelivery = pedidoConferencia.FastDelivery;
            this.DataEntregaFabrica = pedidoConferencia.DataFabrica;
            this.SituacaoCnc = new IdNomeDto
            {
                Id = pedidoConferencia.SituacaoCnc,
                Nome = pedidoConferencia.DescrSituacaoCnc,
            };

            this.PedidoConferido = pedidoConferencia.PedidoConferido;
            this.ComprasGeradas = !string.IsNullOrEmpty(pedidoConferencia.CompraGerada) ? pedidoConferencia.CompraGerada.Split(',').Select(f => f.StrParaInt()) : new List<int>();
            this.CorLinhaTabela = pedidoConferencia.CorLinhaLista.ToString();

            this.Permissoes = new PermissoesDto
            {
                Editar = pedidoConferencia.EditVisible,
                Cancelar = pedidoConferencia.CancelarVisible,
                Reabrir = pedidoConferencia.ExibirReabrir,
                Imprimir = pedidoConferencia.Situacao != (int)PedidoEspelho.SituacaoPedido.Processando,
                ImprimirMemoriaCalculo = pedidoConferencia.ExibirRelatorioCalculo,
                UsarControleReposicao = pedidoConferencia.Situacao != (int)PedidoEspelho.SituacaoPedido.Processando && pedidoConferencia.UsarControleReposicao,
                AnexarArquivos = pedidoConferencia.Situacao != (int)PedidoEspelho.SituacaoPedido.Processando,
                ImprimirProjeto = pedidoConferencia.Situacao != (int)PedidoEspelho.SituacaoPedido.Processando,
                AssociarImagemAsPecas = pedidoConferencia.Situacao != (int)PedidoEspelho.SituacaoPedido.Processando,
                ImprimirProdutosAComprar = pedidoConferencia.Situacao != (int)PedidoEspelho.SituacaoPedido.Processando,
                ExibirSituacaoCnc = pedidoConferencia.ExibirSituacaoCnc,
                ExibirSituacaoCncConferencia = pedidoConferencia.ExibirSituacaoCncConferencia,
                ExibirConferirPedido = pedidoConferencia.ConferirPedidoVisible,
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.PedidoEspelho, pedidoConferencia.IdPedido, null),
                PedidoImportadoPodeGerarArquivo = !pedidoConferencia.Importado || pedidoConferencia.PedidoConferido,
            };
        }

        /// <summary>
        /// Obtém ou define o identificador do pedido em conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido foi gerado por algum parceiro.
        /// </summary>
        [DataMember]
        [JsonProperty("pedidoGeradoParceiro")]
        public bool PedidoGeradoParceiro { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido é de importação.
        /// </summary>
        [DataMember]
        [JsonProperty("pedidoImportado")]
        public bool PedidoImportado { get; set; }

        /// <summary>
        /// Obtém ou define a situação do pedido em conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public string Situacao { get; set; }

        /// <summary>
        /// Obtém ou define o cliente do pedido em conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("cliente")]
        public IdNomeDto Cliente { get; set; }

        /// <summary>
        /// Obtém ou define o nome da loja do pedido em conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("nomeLoja")]
        public string NomeLoja { get; set; }

        /// <summary>
        /// Obtém ou define o conferente do pedido em conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("nomeConferente")]
        public string NomeConferente { get; set; }

        /// <summary>
        /// Obtém ou define o total do pedido comercial.
        /// </summary>
        [DataMember]
        [JsonProperty("totalPedidoComercial")]
        public decimal TotalPedidoComercial { get; set; }

        /// <summary>
        /// Obtém ou define o total do pedido em conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("totalPedidoConferencia")]
        public decimal TotalPedidoConferencia { get; set; }

        /// <summary>
        /// Obtém ou define a data de cadastro da conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("dataCadastroConferencia")]
        public DateTime? DataCadastroConferencia { get; set; }

        /// <summary>
        /// Obtém ou define a data de finalização do pedido em conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("dataFinalizacaoConferencia")]
        public DateTime? DataFinalizacaoConferencia { get; set; }

        /// <summary>
        /// Obtém ou define o total de m² do pedido em conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("totalM2")]
        public decimal TotalM2 { get; set; }

        /// <summary>
        /// Obtém ou define a quantidade de peças de vidro do pedido em conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("quantidadePecas")]
        public int QuantidadePecas { get; set; }

        /// <summary>
        /// Obtém ou define o peso total do pedido em conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("peso")]
        public decimal Peso { get; set; }

        /// <summary>
        /// Obtém ou define a data de entrega do pedido comercial.
        /// </summary>
        [DataMember]
        [JsonProperty("dataEntregaPedidoComercial")]
        public DateTime? DataEntregaPedidoComercial { get; set; }

        /// <summary>
        /// Obtém ou define a data de entrega original do pedido comercial.
        /// </summary>
        [DataMember]
        [JsonProperty("dataEntregaOriginalPedidoComercial")]
        public DateTime? DataEntregaOriginalPedidoComercial { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se pedido em conferência é fast delivery.
        /// </summary>
        [DataMember]
        [JsonProperty("fastDelivery")]
        public bool FastDelivery { get; set; }

        /// <summary>
        /// Obtém ou define a data de entrega da fábrica do pedido em conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("dataEntregaFabrica")]
        public DateTime? DataEntregaFabrica { get; set; }

        /// <summary>
        /// Obtém ou define a situação CNC do pedido em conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("situacaoCnc")]
        public IdNomeDto SituacaoCnc { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o pedido importado foi conferido.
        /// </summary>
        [DataMember]
        [JsonProperty("pedidoConferido")]
        public bool PedidoConferido { get; set; }

        /// <summary>
        /// Obtém ou define as compras geradas desse pedido em conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("comprasGeradas")]
        public IEnumerable<int> ComprasGeradas { get; set; }

        /// <summary>
        /// Obtém ou define a cor da linha da lista de pedidos em conferência.
        /// </summary>
        [DataMember]
        [JsonProperty("corLinhaTabela")]
        public string CorLinhaTabela { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas na nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
