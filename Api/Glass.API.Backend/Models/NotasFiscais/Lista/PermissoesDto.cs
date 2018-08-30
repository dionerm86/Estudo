// <copyright file="PermissoesDto.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Glass.API.Backend.Models.NotasFiscais.Lista
{
    /// <summary>
    /// Classe que encapsula os dados de permissão da nota fiscal.
    /// </summary>
    [DataContract(Name = "Permissoes")]
    public class PermissoesDto
    {
        /// <summary>
        /// Obtém ou define um valor que indica se a nota fiscal poderá ser editada.
        /// </summary>
        [DataMember]
        [JsonProperty("editar")]
        public bool Editar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido cancelar notas fiscais de terceiros.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelarNotasFiscaisDeTerceiros")]
        public bool CancelarNotasFiscaisDeTerceiros { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se o log de eventos será exibido.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirLogDeEventos")]
        public bool ExibirLogDeEventos { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido imprimir o DANFE.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimirDanfe")]
        public bool ImprimirDanfe { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido imprimir o DANFE de terceiros.
        /// </summary>
        [DataMember]
        [JsonProperty("imprimirDanfeTerceiros")]
        public bool ImprimirDanfeTerceiros { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido consultar a situação do lote de emissão.
        /// </summary>
        [DataMember]
        [JsonProperty("consultarSituacaoLote")]
        public bool ConsultarSituacaoLote { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será permitido consultar a situação da emissão da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("consultarSituacaoNotaFiscal")]
        public bool ConsultarSituacaoNotaFiscal { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será exibida a opção de baixar xml da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("baixarXml")]
        public bool BaixarXml { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será exibida a opção de baixar xml da inutilização de numeração de nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("baixarArquivoInutilizacao")]
        public bool BaixarArquivoInutilizacao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será exibida a opção de anexar xml em notas fiscais de terceiros.
        /// </summary>
        [DataMember]
        [JsonProperty("anexarXmlTerceiros")]
        public bool AnexarXmlTerceiros { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será exibida a opção de baixar xml da nota fiscal de terceiros.
        /// </summary>
        [DataMember]
        [JsonProperty("baixarXmlTerceiros")]
        public bool BaixarXmlTerceiros { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será exibida a opção de gerenciar processos referenciados da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirProcessosReferenciados")]
        public bool ExibirProcessosReferenciados { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será exibida a opção de visualizar as informações adicionais da nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirInformacoesAdicionais")]
        public bool ExibirInformacoesAdicionais { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será exibida a opção de emitir nota fiscal FS-DA.
        /// </summary>
        [DataMember]
        [JsonProperty("emitirNotaFiscalFsda")]
        public bool EmitirNotaFiscalFsda { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá exibir o boleto.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirBoleto")]
        public bool ExibirBoleto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá exibir a opção de reenviar email.
        /// </summary>
        [DataMember]
        [JsonProperty("reenviarEmail")]
        public bool ReenviarEmail { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá exibir dados de rentabilidade.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirRentabilidade")]
        public bool ExibirRentabilidade { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá exibir a opção de reabrir a nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("reabrir")]
        public bool Reabrir { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá exibir a opção de gerar nota fiscal complementar.
        /// </summary>
        [DataMember]
        [JsonProperty("gerarNotaFiscalComplementar")]
        public bool GerarNotaFiscalComplementar { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá exibir a opção de carta de correção.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirCartaCorrecao")]
        public bool ExibirCartaCorrecao { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá exibir os pedidos que compõem a nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirPedidos")]
        public bool ExibirPedidos { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá exibir as compras que compõem a nota fiscal.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirCompras")]
        public bool ExibirCompras { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá exibir a opção de centro de custo.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirCentroCusto")]
        public bool ExibirCentroCusto { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá exibir a opção de separação de valores.
        /// </summary>
        [DataMember]
        [JsonProperty("separarValores")]
        public bool SepararValores { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá exibir a opção de cancelar a separação de valores.
        /// </summary>
        [DataMember]
        [JsonProperty("cancelarSeparacaoDeValores")]
        public bool CancelarSeparacaoDeValores { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se deverá exibir a opção de emitir NFC-e.
        /// </summary>
        [DataMember]
        [JsonProperty("emitirNfce")]
        public bool EmitirNfce { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se será exibido o log de estoque.
        /// </summary>
        [DataMember]
        [JsonProperty("exibirLogEstoque")]
        public bool ExibirLogEstoque { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a nota fiscal tem carta de correção registrada.
        /// </summary>
        [DataMember]
        [JsonProperty("possuiCartaCorrecaoRegistrada")]
        public bool PossuiCartaCorrecaoRegistrada { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a nota fiscal possui log de alterações.
        /// </summary>
        [DataMember]
        [JsonProperty("logAlteracoes")]
        public bool LogAlteracoes { get; set; }
    }
}
