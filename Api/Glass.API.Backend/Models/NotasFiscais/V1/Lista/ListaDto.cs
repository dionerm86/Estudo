// <copyright file="ListaDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Glass.API.Backend.Models.Genericas.V1;
using Glass.Configuracoes;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.API.Backend.Models.NotasFiscais.V1.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de uma nota fiscal para a tela de listagem.
    /// </summary>
    [DataContract(Name = "NotaFiscal")]
    public class ListaDto
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ListaDto"/>.
        /// </summary>
        /// <param name="notaFiscal">A model de nota fiscal.</param>
        internal ListaDto(NotaFiscal notaFiscal)
        {
            this.Id = (int)notaFiscal.IdNf;
            this.NumeroNfe = (int)notaFiscal.NumeroNFe;
            this.Serie = notaFiscal.Serie;
            this.Modelo = notaFiscal.Modelo;
            this.CodigoCfop = notaFiscal.CodCfop;
            this.TipoDocumento = new IdNomeDto
            {
                Id = notaFiscal.TipoDocumento,
                Nome = notaFiscal.TipoDocumentoString,
            };

            this.UsuarioCadastro = notaFiscal.DescrUsuCad;
            this.NomeEmitente = notaFiscal.NomeEmitente;
            this.NomeDestinatario = notaFiscal.NomeDestRem;
            this.TotalNota = notaFiscal.TotalNota;
            this.DataEmissao = notaFiscal.DataEmissao;
            this.DataEntradaSaida = notaFiscal.DataSaidaEnt;
            this.Situacao = new IdNomeDto
            {
                Id = (int)notaFiscal.Situacao,
                Nome = notaFiscal.SituacaoString,
            };

            this.CorSituacao = notaFiscal.CorSituacao;
            this.CorLinhaTabela = notaFiscal.CorLinhaGrid;
            this.IdsPedido = !string.IsNullOrEmpty(notaFiscal.IdsPedido) ? notaFiscal.IdsPedido.Split(',').Select(f => f.StrParaInt()) : new List<int>();
            this.IdsCompra = !string.IsNullOrEmpty(notaFiscal.IdCompras) ? notaFiscal.IdCompras.Split(',').Select(f => f.StrParaInt()) : new List<int>();
            this.CentroCustoCompleto = notaFiscal.CentroCustoCompleto;
            this.BaseDeCalculoIcms = notaFiscal.BcIcms;
            this.BaseDeCalculoIcmsSt = notaFiscal.BcIcmsSt;
            this.ValorIcms = notaFiscal.Valoricms;
            this.ValorIcmsSt = notaFiscal.ValorIcmsSt;
            this.ValorIpi = notaFiscal.ValorIpi;
            this.Consumidor = notaFiscal.Consumidor;

            this.Permissoes = new PermissoesDto
            {
                Editar = NotaFiscalDAO.Instance.PodeEditar(null, notaFiscal.IdNf),
                CancelarNotasFiscaisDeTerceiros = notaFiscal.TerceirosEmAbertoVisible,
                ExibirLogDeEventos = notaFiscal.Situacao != (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros,
                ImprimirDanfe = notaFiscal.PrintDanfeVisible,
                ImprimirDanfeTerceiros = notaFiscal.Situacao == (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros,
                ConsultarSituacaoLote = notaFiscal.ConsSitLoteVisible,
                ConsultarSituacaoNotaFiscal = notaFiscal.ConsSitVisible,
                BaixarXml = notaFiscal.BaixarXmlVisible,
                BaixarArquivoInutilizacao = notaFiscal.ExibirSalvarInutilizacao,
                AnexarXmlTerceiros = notaFiscal.AnexarXMLTercVisible,
                BaixarXmlTerceiros = notaFiscal.BaixarXMLTercVisible,
                ExibirProcessosReferenciados = notaFiscal.ExibirDocRef,
                ExibirInformacoesAdicionais = notaFiscal.ExibirLinkInfoAdic,
                EmitirNotaFiscalFsda = notaFiscal.EmitirNfFsVisible,
                ExibirBoleto = notaFiscal.ExibirBoleto,
                ReenviarEmail = notaFiscal.ReenviarEmailXmlVisible,
                ReenviarEmailCancelamento = notaFiscal.ReenviarEmailXmlCancelamentoVisible,
                ExibirRentabilidade = RentabilidadeConfig.ExibirRentabilidadeNotaFiscal && notaFiscal.TipoDocumento == (int)NotaFiscal.TipoDoc.Saída,
                Reabrir = notaFiscal.ExibirReabrir,
                GerarNotaFiscalComplementar = notaFiscal.GerarNFComplVisible,
                ExibirCartaCorrecao = notaFiscal.CartaCorrecaoVisible,
                ExibirPedidos = notaFiscal.ExibirPedidosVisible,
                ExibirCompras = notaFiscal.ExibirComprasVisible,
                ExibirCentroCusto = notaFiscal.ExibirCentroCusto,
                SepararValores = notaFiscal.SepararValoresVisible,
                CancelarSeparacaoDeValores = notaFiscal.CancelarSeparacaoValoresVisible,
                EmitirNfce = notaFiscal.EmitirNFCeVisible,
                ExibirLogEstoque = notaFiscal.Situacao == (int)NotaFiscal.SituacaoEnum.Autorizada || notaFiscal.Situacao == (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros,
                PossuiCartaCorrecaoRegistrada = NotaFiscalDAO.Instance.ExisteCartaCorrecaoRegistrada(null, notaFiscal.IdNf),
                LogAlteracoes = LogAlteracaoDAO.Instance.TemRegistro(LogAlteracao.TabelaAlteracao.NotaFiscal, notaFiscal.IdNf, null),
            };
        }

        /// <summary>
        /// Obtém ou define o identificador da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Obtém ou define o número da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("numeroNfe")]
        public int NumeroNfe { get; set; }

        /// <summary>
        /// Obtém ou define a séria da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("serie")]
        public string Serie { get; set; }

        /// <summary>
        /// Obtém ou define o modelo da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("modelo")]
        public string Modelo { get; set; }

        /// <summary>
        /// Obtém ou define o código do CFOP da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("codigoCfop")]
        public string CodigoCfop { get; set; }

        /// <summary>
        /// Obtém ou define o tipo do documento da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("tipoDocumento")]
        public IdNomeDto TipoDocumento { get; set; }

        /// <summary>
        /// Obtém ou define o usuário que cadastrou a nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("usuarioCadastro")]
        public string UsuarioCadastro { get; set; }

        /// <summary>
        /// Obtém ou define o emitente da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("nomeEmitente")]
        public string NomeEmitente { get; set; }

        /// <summary>
        /// Obtém ou define o destinatário da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("nomeDestinatario")]
        public string NomeDestinatario { get; set; }

        /// <summary>
        /// Obtém ou define o total da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("totalNota")]
        public decimal TotalNota { get; set; }

        /// <summary>
        /// Obtém ou define a data de emissão da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("dataEmissao")]
        public DateTime? DataEmissao { get; set; }

        /// <summary>
        /// Obtém ou define a data de entrada/saída da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("dataEntradaSaida")]
        public DateTime? DataEntradaSaida { get; set; }

        /// <summary>
        /// Obtém ou define a situação da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("situacao")]
        public IdNomeDto Situacao { get; set; }

        /// <summary>
        /// Obtém ou define a cor do campo de situação da lista da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("corSituacao")]
        public System.Drawing.Color CorSituacao { get; set; }

        /// <summary>
        /// Obtém ou define a cor da linha da lista da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("corLinhaTabela")]
        public string CorLinhaTabela { get; set; }

        /// <summary>
        /// Obtém ou define uma lista com os pedidos que geraram a nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("idsPedido")]
        public IEnumerable<int> IdsPedido { get; set; }

        /// <summary>
        /// Obtém ou define uma lista com as compras que geraram a nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("idsCompra")]
        public IEnumerable<int> IdsCompra { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o centro de custo da nota fiscal foi totalmente informado.
        /// </summary>
        [DataMember]
        [JsonProperty("centroCustoCompleto")]
        public bool CentroCustoCompleto { get; set; }

        /// <summary>
        /// Obtém ou define a base de cálculo do icms da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("baseDeCalculoIcms")]
        public decimal BaseDeCalculoIcms { get; set; }

        /// <summary>
        /// Obtém ou define a base de cálculo do icms st da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("baseDeCalculoIcmsSt")]
        public decimal BaseDeCalculoIcmsSt { get; set; }

        /// <summary>
        /// Obtém ou define o valor do icms da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("valorIcms")]
        public decimal ValorIcms { get; set; }

        /// <summary>
        /// Obtém ou define o valor do icms st da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("valorIcmsSt")]
        public decimal ValorIcmsSt { get; set; }

        /// <summary>
        /// Obtém ou define o valor do ipi da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("valorIpi")]
        public decimal ValorIpi { get; set; }

        /// <summary>
        /// Obtém ou define se a nota fiscal é para consumidor final (NFC-e)
        /// </summary>
        [DataMember]
        [JsonProperty("consumidor")]
        public bool Consumidor { get; set; }

        /// <summary>
        /// Obtém ou define a lista de permissões concedidas na nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("permissoes")]
        public PermissoesDto Permissoes { get; set; }
    }
}
